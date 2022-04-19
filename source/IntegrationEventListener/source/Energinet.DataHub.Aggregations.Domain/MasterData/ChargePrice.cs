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
using NodaTime;

namespace Energinet.DataHub.Aggregations.Domain.MasterData
{
    internal class ChargePrice : IMasterDataObject
    {
        public Instant FromDate { get; set; }

        public Instant ToDate { get; set; }

        public Guid RowId { get; set; }

        public T ShallowCopy<T>()
            where T : IMasterDataObject
        {
            return (T)MemberwiseClone();
        }
    }
}
