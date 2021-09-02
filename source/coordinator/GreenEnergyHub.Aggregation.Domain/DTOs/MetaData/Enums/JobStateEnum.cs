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

using System.ComponentModel;

namespace GreenEnergyHub.Aggregation.Domain.DTOs.MetaData.Enums
{
    public enum JobStateEnum
    {
        [Description("Creating cluster")]
        ClusterStartup = 1,
        [Description("Cluster created")]
        ClusterCreated = 2,
        [Description("Cluster Warming up")]
        ClusterWarmingUp = 3,
        [Description("Cluster failed to start")]
        ClusterFailed = 4,
        [Description("Calculation running")]
        Calculating = 5,
        [Description("Calculation completed")]
        Completed = 6,
        [Description("Calculation failed")]
        CompletedWithFail = 7,
        [Description("Created")]
        Created = 8,
    }
}
