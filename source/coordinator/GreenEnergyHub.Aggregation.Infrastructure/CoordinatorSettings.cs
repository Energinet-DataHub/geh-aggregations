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

namespace GreenEnergyHub.Aggregation.Infrastructure
{
    public class CoordinatorSettings
    {
        public const string ClusterName = "Aggregation Autoscaling";

        public const string ClusterAggregationJobName = "Aggregation Job";

        public const string ClusterWholesaleJobName = "Wholesale Job";

        public const string AdjustedFlexConsumptionName = "AdjustedFlexConsumption";

        public const string HourlyProductionName = "HourlyProduction";

        public const string HourlyConsumptionName = "HourlyConsumption";

        public const string AdjustedHourlyProductionName = "AdjustedHourlyProduction";

        public const string FlexConsumptionName = "FlexConsumption";

        public const string ExchangeName = "Exchange";

        public const string ExchangeNeighbourName = "ExchangeNeighbour";

        public string ConnectionStringDatabricks { get; set;  }

        public string HostKey { get; set; }

        public string TokenDatabricks { get; set; }

        public Uri ResultUrl { get; set; }

        public Uri SnapshotUrl { get; set; }

        public string InputStorageAccountName { get; set; }

        public string InputStorageAccountKey { get; set; }

        public string InputStorageContainerName { get; set; }

        public string InputPath { get; set; }

        public string GridLossSysCorPath { get; set; }

        public string PersistLocation { get; set; }

        public string TelemetryInstrumentationKey { get; set; }

        public string PythonFile { get; set; }

        public int ClusterTimeoutMinutes { get; set; }

        public string CosmosAccountEndpoint { get; set; }

        public string CosmosAccountKey { get; set; }

        public object CosmosDatabase { get; set; }
    }
}
