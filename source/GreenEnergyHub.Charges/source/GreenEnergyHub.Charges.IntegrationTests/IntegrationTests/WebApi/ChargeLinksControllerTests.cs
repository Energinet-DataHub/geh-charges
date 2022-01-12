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
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Energinet.Charges.Contracts.ChargeLink;
using FluentAssertions;
using GreenEnergyHub.Charges.IntegrationTests.Fixtures;
using GreenEnergyHub.Charges.IntegrationTests.Fixtures.Database;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.IntegrationTests.IntegrationTests.WebApi
{
    [IntegrationTest]
    public class ChargeLinksControllerTests : WebApiHost, IClassFixture<ChargesDatabaseFixture>
    {
        private const string BaseUrl = "/ChargeLinks/GetAsync?meteringPointId=";
        private const string KnownMeteringPointId = "571313180000000005";
        private readonly HttpClient _client;

        public ChargeLinksControllerTests(WebApiFactory factory, ChargesDatabaseFixture chargesDatabaseFixture)
            : base(chargesDatabaseFixture)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAsync_WhenMeteringPointIdHasChargeLinks_ReturnsOkAndCorrectContentType()
        {
            // Act
            var response = await _client.GetAsync($"{BaseUrl}{KnownMeteringPointId}");

            // Assert
            var contentType = response.Content.Headers.ContentType!.ToString();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            contentType.Should().Be("application/json; charset=utf-8");
        }

        [Fact]
        public async Task GetAsync_WhenMeteringPointIdHasChargeLinks_ReturnsOrderedChargeLinks()
        {
            // Act
            var response = await _client.GetAsync($"{BaseUrl}{KnownMeteringPointId}");

            // Assert
            var jsonString = await response.Content.ReadAsStringAsync();
            var actual = JsonSerializer.Deserialize<List<ChargeLinkDto>>(
                jsonString,
                GetJsonSerializerOptions());

            actual.Should().BeInAscendingOrder(c => c.ChargeType)
                .And.ThenBeInAscendingOrder(c => c.ChargeId)
                .And.ThenBeInDescendingOrder(c => c.StartDate);
        }

        [Fact]
        public async Task GetAsync_WhenMeteringPointIdDoesNotExist_ReturnsNotFound()
        {
            var response = await _client.GetAsync($"{BaseUrl}{Guid.NewGuid()}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetAsync_WhenNoMeteringPointIdInput_ReturnsBadRequest()
        {
            var missingMeteringPointId = string.Empty;
            var response = await _client.GetAsync($"{BaseUrl}{missingMeteringPointId}");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() },
            };
        }
    }
}
