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

using System.Net;
using System.Threading.Tasks;
using Energinet.DataHub.Core.FunctionApp.TestCommon;
using FluentAssertions;
using GreenEnergyHub.Charges.IntegrationTests.Fixtures;
using GreenEnergyHub.Charges.IntegrationTests.TestHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.IntegrationTests.DomainTests
{
    /// <summary>
    /// Proof-of-concept on integration testing a function.
    /// </summary>
    [IntegrationTest]
    public class HealthStatusTests
    {
        [Collection(nameof(ChargesFunctionAppCollectionFixture))]
        public class Run : FunctionAppTestBase<ChargesFunctionAppFixture>
        {
            private readonly HttpRequestGenerator _httpRequestGenerator;

            public Run(ChargesFunctionAppFixture fixture, ITestOutputHelper testOutputHelper)
                : base(fixture, testOutputHelper)
            {
                TestDataGenerator.GenerateDataForIntegrationTests(Fixture);
                _httpRequestGenerator = new HttpRequestGenerator(fixture);
            }

            [Fact]
            public async Task When_RequestingHealthStatus_Then_ReturnStatusOK()
            {
                // Arrange
                var result = await _httpRequestGenerator.CreateHttpGetRequestAsync("api/HealthStatus");

                // Act
                var actualResponse = await Fixture.HostManager.HttpClient.SendAsync(result.Request);

                // Assert
                actualResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            }

            [Fact]
            public async Task When_RequestingUnknownEndpoint_Then_ReturnStatusNotFound()
            {
                // Arrange
                var result = await _httpRequestGenerator.CreateHttpGetRequestAsync("api/unknown");

                // Act
                var actualResponse = await Fixture.HostManager.HttpClient.SendAsync(result.Request);

                // Assert
                actualResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }
    }
}
