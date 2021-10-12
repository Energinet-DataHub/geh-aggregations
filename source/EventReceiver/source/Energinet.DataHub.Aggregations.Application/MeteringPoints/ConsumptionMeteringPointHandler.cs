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

using System.Threading;
using System.Threading.Tasks;
using Energinet.DataHub.Aggregations.Application.Interfaces;
using MediatR;

namespace Energinet.DataHub.Aggregations.Application.MeteringPoints
{
    public class ConsumptionMeteringPointHandler : IRequestHandler<ConsumptionMeteringPointCreatedCommand>
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IJsonSerializer _jsonSerializer;

        public ConsumptionMeteringPointHandler(IEventDispatcher eventDispatcher, IJsonSerializer jsonSerializer)
        {
            _eventDispatcher = eventDispatcher;
            _jsonSerializer = jsonSerializer;
        }

        public async Task<Unit> Handle(ConsumptionMeteringPointCreatedCommand request, CancellationToken cancellationToken)
        {
            var serializedMessage = _jsonSerializer.Serialize(request);
            await _eventDispatcher.DispatchAsync(serializedMessage, cancellationToken).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}
