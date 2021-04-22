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
using System.Text.Json;
using GreenEnergyHub.Aggregation.Application.Coordinator.HourlyConsumption;
using GreenEnergyHub.Aggregation.Application.GLN;
using GreenEnergyHub.Aggregation.Domain;
using GreenEnergyHub.Aggregation.Domain.Types;
using GreenEnergyHub.Messaging.Transport;

namespace GreenEnergyHub.Aggregation.Application.Coordinator.Handlers
{
    public class HourlyConsumptionHandler : IAggregationHandler
    {
        private readonly IGLNService _glnService;

        public HourlyConsumptionHandler(IGLNService glnService)
        {
            _glnService = glnService;
        }

        public IEnumerable<IOutboundMessage> PrepareMessages(
            List<string> result,
            ProcessType processType,
            string timeIntervalStart,
            string timeIntervalEnd)
        {
            var list = result.Select(json => JsonSerializer.Deserialize<Domain.DTOs.HourlyConsumption>(json)).ToList();

            return (from energySupplier in list.GroupBy(hc => hc.EnergySupplierMarketParticipantMRID)
                    from gridArea in energySupplier.GroupBy(e => e.MeteringGridAreaDomainMRID)
                    let first = gridArea.First()
                    select new AggregatedMeteredDataTimeSeries(CoordinatorSettings.HourlyConsumptionName)
                    {
                        MeteringGridAreaDomainMRid = first.MeteringGridAreaDomainMRID,
                        BalanceResponsiblePartyMarketParticipantMRid = first.BalanceResponsiblePartyMarketParticipantMRID,
                        BalanceSupplierPartyMarketParticipantMRid = first.EnergySupplierMarketParticipantMRID,
                        MarketEvaluationPointType = MarketEvaluationPointType.Consumption,
                        SettlementMethod = SettlementMethodType.NonProfiled,
                        ProcessType = Enum.GetName(typeof(ProcessType), processType),
                        Quantities = gridArea.Select(e => e.SumQuantity).ToArray(),
                        TimeIntervalStart = timeIntervalStart,
                        TimeIntervalEnd = timeIntervalEnd,
                        ReceiverMarketParticipantMRid = _glnService.GetGlnFromSupplierId(first.EnergySupplierMarketParticipantMRID),
                        SenderMarketParticipantMRid = _glnService.GetSenderGln(),
                    }).Cast<IOutboundMessage>()
                .ToList();
        }
    }
}
