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
using Energinet.DataHub.Aggregations.Application.IntegrationEvents.MeteringPoints;
using Energinet.DataHub.Aggregations.Domain;
using Energinet.DataHub.MeteringPoints.IntegrationEventContracts;
using GreenEnergyHub.Messaging.Protobuf;
using GreenEnergyHub.Messaging.Transport;

namespace Energinet.DataHub.Aggregations.Infrastructure.Mappers
{
    public class ConsumptionMeteringPointCreatedMapper : ProtobufInboundMapper<ConsumptionMeteringPointCreated>
    {
        protected override IInboundMessage Convert(ConsumptionMeteringPointCreated obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return new ConsumptionMeteringPointCreatedEvent(
                obj.MeteringPointId,
                MeteringPointType.Consumption,
                obj.GridAreaCode,
                ProtobufToDomainTypeParser.ParseSettlementMethod(obj.SettlementMethod),
                ProtobufToDomainTypeParser.ParseMeteringMethod(obj.MeteringMethod),
                ProtobufToDomainTypeParser.ParseMeterReadingPeriodicity(obj.MeterReadingPeriodicity),
                ProtobufToDomainTypeParser.ParseConnectionState(obj.ConnectionState),
                ProtobufToDomainTypeParser.ParseProduct(obj.Product),
                ProtobufToDomainTypeParser.ParseUnitType(obj.UnitType),
                ProtobufToDomainTypeParser.ParseEffectiveDate(obj.EffectiveDate));
        }
    }
}