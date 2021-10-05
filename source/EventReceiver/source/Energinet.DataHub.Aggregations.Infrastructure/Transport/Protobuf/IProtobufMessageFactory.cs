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

using Google.Protobuf;

namespace Energinet.DataHub.Aggregations.Infrastructure.Transport.Protobuf
{
    /// <summary>
    /// Factory for generating protobuf messages
    /// </summary>
    public interface IProtobufMessageFactory
    {
        /// <summary>
        /// Create message from binary payload
        /// </summary>
        /// <param name="payload">Payload</param>
        /// <param name="messageTypeName">Name of protobuf message type</param>
        /// <returns><see cref="IMessage"/></returns>
        IMessage CreateMessageFrom(byte[] payload, string messageTypeName);
    }
}