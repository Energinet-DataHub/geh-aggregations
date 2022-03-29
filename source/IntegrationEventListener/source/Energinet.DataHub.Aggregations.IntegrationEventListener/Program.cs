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

using System.IO;
using Azure.Messaging.EventHubs.Producer;
using Energinet.DataHub.Aggregations.Application;
using Energinet.DataHub.Aggregations.Application.Interfaces;
using Energinet.DataHub.Aggregations.Common;
using Energinet.DataHub.Aggregations.Configuration;
using Energinet.DataHub.Aggregations.Domain;
using Energinet.DataHub.Aggregations.Infrastructure;
using Energinet.DataHub.Aggregations.Infrastructure.Messaging.Registration;
using Energinet.DataHub.Aggregations.Infrastructure.Repository;
using Energinet.DataHub.Aggregations.Infrastructure.Serialization;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Energinet.DataHub.Aggregations
{
    public static class Program
    {
        public static void Main()
        {
            var host = new HostBuilder().ConfigureAppConfiguration(configurationBuilder =>
                {
                    configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
                    configurationBuilder.AddJsonFile("local.settings.json", true, true);
                    configurationBuilder.AddEnvironmentVariables();
                })
                .ConfigureFunctionsWorkerDefaults();

            var buildHost = host.ConfigureServices((context, services) =>
            {
                using var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
                telemetryConfiguration.InstrumentationKey = context.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
                var logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces)
                    .CreateLogger();

                services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(logger));
                services.AddSingleton<IJsonSerializer, JsonSerializer>();
                services.AddSingleton<EventDataHelper>();

                services.AddSingleton<IMasterDataRepository>(x =>
                    new MasterDataRepository(context.Configuration["DATABASE_MASTERDATA_CONNECTIONSTRING"]));

                services.AddSingleton<IEventToMasterDataTransformer, EventToMasterDataTransformer>();

                services.ConfigureProtobufReception();
                ConsumptionMeteringPointCreatedHandlerConfiguration.ConfigureServices(services);
                MeteringPointConnectedHandlerConfiguration.ConfigureServices(services);
                EnergySupplierChangedHandlerConfiguration.ConfigureServices(services);
            }).Build();

            buildHost.Run();
        }
    }
}
