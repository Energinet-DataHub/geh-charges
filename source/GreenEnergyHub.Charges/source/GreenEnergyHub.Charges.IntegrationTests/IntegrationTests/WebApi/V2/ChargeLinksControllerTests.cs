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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Energinet.Charges.Contracts.ChargeLink;
using FluentAssertions;
using GreenEnergyHub.Charges.IntegrationTests.Fixtures.WebApi;
using GreenEnergyHub.Charges.IntegrationTests.TestHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.IntegrationTests.IntegrationTests.WebApi.V2
{
    [IntegrationTest]
    [Collection(nameof(ChargesWebApiCollectionFixture))]
    public class ChargeLinksControllerTests :
        WebApiTestBase<ChargesWebApiFixture>,
        IClassFixture<ChargesWebApiFixture>,
        IClassFixture<WebApiFactory>,
        IAsyncLifetime
    {
        private const string BaseUrl = "/v2/ChargeLinks?meteringPointId=";
        private const string KnownMeteringPointId = SeededData.MeteringPoints.Mp571313180000000005.Id;
        private readonly HttpClient _client;
        private readonly BackendAuthenticationClient _authenticationClient;

        public ChargeLinksControllerTests(
            ChargesWebApiFixture chargesWebApiFixture,
            WebApiFactory factory,
            ITestOutputHelper testOutputHelper)
            : base(chargesWebApiFixture, testOutputHelper)
        {
            _client = factory.CreateClient();
            _authenticationClient = new BackendAuthenticationClient(
                chargesWebApiFixture.AuthorizationConfiguration.BackendAppScope,
                chargesWebApiFixture.AuthorizationConfiguration.ClientCredentialsSettings,
                chargesWebApiFixture.AuthorizationConfiguration.B2cTenantId);
        }

        public async Task InitializeAsync()
        {
            var authenticationResult = await _authenticationClient.GetAuthenticationTokenAsync();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authenticationResult.AccessToken}");
        }

        public Task DisposeAsync()
        {
            _client.Dispose();
            return Task.CompletedTask;
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
            var actual = Deserialize<List<ChargeLinkV2Dto>>(jsonString);

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

        private static T Deserialize<T>(string jsonSerialized)
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() },
            };
            return JsonSerializer.Deserialize<T>(jsonSerialized, jsonSerializerOptions)!;
        }
    }
}