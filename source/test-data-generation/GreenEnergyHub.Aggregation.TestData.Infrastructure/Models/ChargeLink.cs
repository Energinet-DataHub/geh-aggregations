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
using Newtonsoft.Json;

namespace GreenEnergyHub.Aggregation.TestData.Infrastructure.Models
{
    public class ChargeLink : IStoragebleObject
    {
        [JsonProperty(PropertyName = "id")]
        public string Id => Guid.NewGuid().ToString();

        [JsonProperty(PropertyName = "charge_id")]
        public string ChargeId { get; set; }

        [JsonProperty(PropertyName = "metering_point_id")]
        public string MeteringPointId { get; set; }

        [JsonProperty(PropertyName = "from_date")]
        public string FromDate { get; set; }

        [JsonProperty(PropertyName = "to_date")]
        public string ToDate { get; set; }
    }
}