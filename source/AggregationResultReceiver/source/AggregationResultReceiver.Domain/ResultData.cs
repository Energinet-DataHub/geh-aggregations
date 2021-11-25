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

using Newtonsoft.Json;

namespace Energinet.DataHub.Aggregations.AggregationResultReceiver.Domain
{
#pragma warning disable SA1313
#pragma warning disable CA1801
    public record ResultData(
        string JobId,
        string SnapshotId,
        string ResultId,
        string ResultName,
        string GridArea,
        string InGridArea,
        string OutGridArea,
        string BalanceResponsibleId,
        string EnergySupplierId,
        string StartDateTime,
        string EndDateTime,
        string Resolution,
        decimal SumQuantity,
        string Quality,
        string MeteringPointType,
        string SettlementMethod);
#pragma warning restore SA1313
#pragma warning restore CA1801
}
