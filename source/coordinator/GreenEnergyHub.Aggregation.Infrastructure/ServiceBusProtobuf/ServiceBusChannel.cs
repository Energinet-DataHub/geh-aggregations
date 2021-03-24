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
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using GreenEnergyHub.Messaging.Transport;

namespace GreenEnergyHub.Aggregation.Infrastructure.ServiceBusProtobuf
{
   public class ServiceBusChannel : Channel
    {
        private readonly string _connectionString;
        private readonly string _topic;

        public ServiceBusChannel(string connectionString, string topic)
        {
            _connectionString = connectionString;
            _topic = topic;
        }

        protected override async Task WriteAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            // create a Service Bus client
            await using var client = new ServiceBusClient(_connectionString);

            // create a sender for the queue
            var sender = client.CreateSender(_topic);

            // create a message that we can send
            var message = new ServiceBusMessage(new BinaryData(data));

            // send the message
            await sender.SendMessageAsync(message, cancellationToken);
        }
    }
}
