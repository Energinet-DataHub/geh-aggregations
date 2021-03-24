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
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using GreenEnergyHub.Aggregation.Domain;
using GreenEnergyHub.Aggregation.Infrastructure.Contracts;
using GreenEnergyHub.Messaging.Protobuf;

namespace GreenEnergyHub.Aggregation.Infrastructure.ServiceBusProtobuf
{
    public class AggregationResultMessageToDtoMapper : ProtobufOutboundMapper<AggregatedMeteredDataTimeSeries>
    {
        protected override IMessage Convert(AggregatedMeteredDataTimeSeries obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return new Document()
            {
                Content = System.Text.Json.JsonSerializer.Serialize(obj),

                // TODO use noda time
                EffectuationDate = Timestamp.FromDateTime(DateTime.UtcNow),
                Recipient = $"khs {DateTime.Now:HHmm dd MMMM}",
                Type = "KHS doc",
                Version = "1",
            };
        }
    }
}
