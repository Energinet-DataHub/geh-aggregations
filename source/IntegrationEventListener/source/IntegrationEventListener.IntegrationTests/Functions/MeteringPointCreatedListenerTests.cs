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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Energinet.DataHub.Aggregations.IntegrationEventListener.IntegrationTests.Assets;
using Energinet.DataHub.Aggregations.IntegrationEventListener.IntegrationTests.Common;
using Energinet.DataHub.Aggregations.IntegrationEventListener.IntegrationTests.Fixtures;
using Energinet.DataHub.Aggregations.MeteringPoints;
using Energinet.DataHub.Core.FunctionApp.TestCommon;
using FluentAssertions;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace Energinet.DataHub.Aggregations.IntegrationEventListener.IntegrationTests.Functions
{
    [Collection(nameof(AggregationsFunctionAppCollectionFixture))]
    public class MeteringPointCreatedIntegrationTests : FunctionAppTestBase<AggregationsFunctionAppFixture>, IAsyncLifetime
    {
        private static readonly Random MyRandom = new Random();

        public MeteringPointCreatedIntegrationTests(AggregationsFunctionAppFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public Task InitializeAsync()
        {
             return Task.CompletedTask;
        }

        [Fact]
        public async Task When_Receiving_MeteringPointCreatedEvent_Then_EventIsProcessed()
        {
            // Arrange
            var meteringPointId = RandomString(20);
            var effectiveDate = SystemClock.Instance.GetCurrentInstant();
            var message = TestMessages.CreateMpCreatedMessage(meteringPointId, effectiveDate.ToDateTimeUtc());

            //Act
            await Fixture.MPCreatedTopic.SenderClient.SendMessageAsync(message)
                .ConfigureAwait(false);

            await FunctionAsserts.AssertHasExecutedAsync(
                Fixture.HostManager, nameof(MeteringPointCreatedListener)).ConfigureAwait(false);
            var mp = await Fixture.MeteringPointRepository.GetByIdAndDateAsync(meteringPointId, effectiveDate).ConfigureAwait(false);
            Fixture.HostManager.ClearHostLog();
        }

        private static string RandomString(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[MyRandom.Next(s.Length)]).ToArray());
        }
    }
}