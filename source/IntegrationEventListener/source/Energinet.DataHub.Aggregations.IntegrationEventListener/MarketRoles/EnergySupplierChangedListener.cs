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
using System.Threading.Tasks;
using Energinet.DataHub.Aggregations.Application.Interfaces;
using Energinet.DataHub.Aggregations.Common;
using Energinet.DataHub.Aggregations.Infrastructure.Messaging;
using Energinet.DataHub.MarketRoles.IntegrationEventContracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Energinet.DataHub.Aggregations.MarketRoles
{
    public class EnergySupplierChangedListener
    {
        private readonly MessageExtractor<EnergySupplierChanged> _messageExtractor;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly EventDataHelper _eventDataHelper;

        public EnergySupplierChangedListener(MessageExtractor<EnergySupplierChanged> messageExtractor, IEventDispatcher eventDispatcher, EventDataHelper eventDataHelper)
        {
            _messageExtractor = messageExtractor;
            _eventDispatcher = eventDispatcher;
            _eventDataHelper = eventDataHelper;
        }

        [Function("EnergySupplierChangedListener")]
        public async Task RunAsync(
            [ServiceBusTrigger(
                "%ENERGY_SUPPLIER_CHANGED_TOPIC_NAME%",
                "%ENERGY_SUPPLIER_CHANGED_SUBSCRIPTION_NAME%",
                Connection = "INTEGRATION_EVENT_LISTENER_CONNECTION_STRING")] byte[] data,
            FunctionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var eventName = _eventDataHelper.GetEventName(context);
            var request = await _messageExtractor.ExtractAsync(data).ConfigureAwait(false);
            var eventHubMetaData = new Dictionary<string, string>()
            {
                { "EventId", request.Transaction.MRID },
                { "EventName", eventName },
            };

            await _eventDispatcher.DispatchAsync(request, eventHubMetaData).ConfigureAwait(false);
        }
    }
}