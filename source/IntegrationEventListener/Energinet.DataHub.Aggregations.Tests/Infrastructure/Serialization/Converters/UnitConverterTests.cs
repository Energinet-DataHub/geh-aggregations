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
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Energinet.DataHub.Aggregations.Domain;
using Energinet.DataHub.Aggregations.Infrastructure.Serialization.Converters;
using Energinet.DataHub.Aggregations.Tests.Attributes;
using Xunit;
using Xunit.Categories;

namespace Energinet.DataHub.Aggregations.Tests.Infrastructure.Serialization.Converters
{
    [UnitTest]
    public static class UnitConverterTests
    {
        [Theory]
        [InlineAutoMoqData(@"""KWH""", Unit.Kwh)]
        public static void Read_ValidStrings_ReturnsCorrectType(
            string json,
            Unit expected,
            [NotNull] JsonSerializerOptions options,
            UnitConverter sut)
        {
            // Arrange
            options.Converters.Add(sut);

            // Act
            var actual = JsonSerializer.Deserialize<Unit>(json, options);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void Read_UnknownString_ThrowsException()
        {
            // Arrange
            const string json = @"""Unknown""";
            var options = new JsonSerializerOptions();
            var sut = new UnitConverter();
            options.Converters.Add(sut);

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() => JsonSerializer.Deserialize<Unit>(json, options));
        }

        [Theory]
        [InlineAutoMoqData(@"""KWH""", Unit.Kwh)]
        public static void Write_ValidValue_ReturnsCorrectString(
            string expected,
            Unit unit,
            [NotNull] JsonSerializerOptions options,
            UnitConverter sut)
        {
            // Arrange
            options.Converters.Add(sut);

            // Act
            var actual = JsonSerializer.Serialize(unit, options);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void Write_UnknownValue_ThrowsException()
        {
            // Arrange
            const Unit quantityUnit = (Unit)999;
            var options = new JsonSerializerOptions();
            var sut = new UnitConverter();
            options.Converters.Add(sut);

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() => JsonSerializer.Serialize(quantityUnit, options));
        }
    }
}
