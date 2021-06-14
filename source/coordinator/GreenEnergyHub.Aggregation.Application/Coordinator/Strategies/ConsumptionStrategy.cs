﻿// Copyright 2020 Energinet DataHub A/S
//
// Licensed under the Apache License, Version 2.0 (the "License2");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using GreenEnergyHub.Aggregation.Application.Services;
using GreenEnergyHub.Aggregation.Domain.DTOs;
using GreenEnergyHub.Aggregation.Domain.ResultMessages;
using GreenEnergyHub.Aggregation.Domain.Types;
using GreenEnergyHub.Aggregation.Infrastructure;
using GreenEnergyHub.Aggregation.Infrastructure.ServiceBusProtobuf;
using GreenEnergyHub.Messaging.Transport;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace GreenEnergyHub.Aggregation.Application.Coordinator.Strategies
{
    public class ConsumptionStrategy : BaseStrategy<ConsumptionDto>, IDispatchStrategy
    {
        private readonly IDistributionListService _distributionListService;
        private readonly IGLNService _glnService;

        public ConsumptionStrategy(
            IDistributionListService distributionListService,
            IGLNService glnService,
            ILogger<ConsumptionDto> logger,
            PostOfficeDispatcher messageDispatcher,
            IJsonSerializer jsonSerializer)
            : base(logger, messageDispatcher, jsonSerializer)
        {
            _distributionListService = distributionListService;
            _glnService = glnService;
        }

        public string FriendlyNameInstance => "hourly_consumption_df";

        public override IEnumerable<IOutboundMessage> PrepareMessages(
            IEnumerable<ConsumptionDto> aggregationResultList,
            ProcessType processType,
            Instant timeIntervalStart,
            Instant timeIntervalEnd)
        {
            return (from energySupplier in aggregationResultList.GroupBy(hc => hc.EnergySupplierMarketParticipantmRID)
                    from gridArea in energySupplier.GroupBy(e => e.MeteringGridAreaDomainmRID)
                    let first = gridArea.First()
                    select new AggregatedConsumptionResultMessage
                    {
                        MeteringGridAreaDomainmRID = first.MeteringGridAreaDomainmRID,
                        BalanceResponsiblePartyMarketParticipantmRID = first.BalanceResponsiblePartyMarketParticipantmRID,
                        BalanceSupplierPartyMarketParticipantmRID = first.EnergySupplierMarketParticipantmRID,
                        AggregationType = CoordinatorSettings.HourlyConsumptionName,
                        MarketEvaluationPointType = MarketEvaluationPointType.Consumption,
                        SettlementMethod = SettlementMethodType.NonProfiled,
                        ProcessType = Enum.GetName(typeof(ProcessType), processType),
                        Quantities = gridArea.Select(e => e.SumQuantity).ToArray(),
                        TimeIntervalStart = timeIntervalStart,
                        TimeIntervalEnd = timeIntervalEnd,
                        ReceiverMarketParticipantmRID = _distributionListService.GetDistributionItem(first.MeteringGridAreaDomainmRID),
                        SenderMarketParticipantmRID = _glnService.GetSenderGln(),
                        AggregatedQuality = first.AggregatedQuality,
                    }).Cast<IOutboundMessage>()
                .ToList();
        }
    }
}