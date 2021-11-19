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
using System.IO;
using System.Reflection;
using Energinet.DataHub.Aggregations.AggregationResultReceiver.Domain;
using Energinet.DataHub.Aggregations.AggregationResultReceiver.Domain.Enums;
using Newtonsoft.Json;

namespace Energinet.DataHub.Aggregations.AggregationResultReceiver.Tests.Helpers
{
    public class TestDataGenerator
    {
        public List<DataResult> GetResultsParameterForMapToCimXml(List<string> list, Grouping grouping)
        {
            var dataResultList = new List<DataResult>();

            foreach (var file in list)
            {
                var resultDataList = new List<ResultData>();
                resultDataList.AddRange(JsonMultipleContentReader(EmbeddedResourceAssetReader(file)));
                dataResultList.Add(new DataResult()
                {
                    ResultDataCollection = resultDataList,
                    Grouping = grouping,
                });
            }

            return dataResultList;
        }

        public string EmbeddedResourceAssetReader(string fileName)
        {
            var ns = "Energinet.DataHub.Aggregations.AggregationResultReceiver";
            var resource = $"{ns}.Tests.Assets.{fileName}";
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
