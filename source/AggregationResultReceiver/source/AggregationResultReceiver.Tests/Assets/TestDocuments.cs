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
using Microsoft.Extensions.FileProviders;

namespace Energinet.DataHub.Aggregations.AggregationResultReceiver.Tests.Assets
{
    public class TestDocuments
    {
        private readonly EmbeddedFileProvider _fileProvider;

        public TestDocuments()
        {
            _fileProvider = new EmbeddedFileProvider(GetType().Assembly);
        }

        public string JobCompletedEvent => GetDocumentAsString("job_completed_event.json");

        private Stream GetDocumentStream(string documentName)
        {
            var rootNamespace = GetType().Namespace;
            return GetType().Assembly.GetManifestResourceStream($"{rootNamespace}.{documentName}");
        }

        private string GetDocumentAsString(string documentName)
        {
            var stream = GetDocumentStream(documentName);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
