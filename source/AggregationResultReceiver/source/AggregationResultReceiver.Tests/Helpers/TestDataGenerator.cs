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

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Energinet.DataHub.ResultReceiver.Domain;
using Newtonsoft.Json;

namespace Energinet.DataHub.AggregationResultReceiver.Tests.Helpers
{
    public class TestDataGenerator
    {
        public List<ResultData> GetResultsParameterForMapToCimXml()
        {
            var list = new List<string>()
            {
                "result_mock_flex_consumption_per_grid_area.json",
                "result_mock_hourly_consumption_per_grid_area.json",
                "result_mock_net_exchange_per_grid_area.json",
                "result_mock_production_per_grid_area.json",
                "result_mock_total_consumption.json",
            };
            var resultDataList = new List<ResultData>();

            foreach (var file in list)
            {
                resultDataList.AddRange(JsonMultipleContentReader(EmbeddedResourceAssetReader(file)));
            }

            return resultDataList;
        }

        public string EmbeddedResourceAssetReader(string fileName)
        {
            var resource = $"Energinet.DataHub.AggregationResultReceiver.Tests.Assets.{fileName}";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
            if (stream == null) return string.Empty;
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public List<ResultData> JsonMultipleContentReader(string jsonContent)
        {
            var resultDataArray = new List<ResultData>();
            JsonTextReader reader = new JsonTextReader(new StringReader(jsonContent));
            reader.SupportMultipleContent = true;
            JsonSerializer serializer = new JsonSerializer();
            while (true)
            {
                if (!reader.Read())
                {
                    break;
                }

                ResultData resultData = serializer.Deserialize<ResultData>(reader);

                resultDataArray.Add(resultData);
            }

            return resultDataArray;
        }
    }
}
