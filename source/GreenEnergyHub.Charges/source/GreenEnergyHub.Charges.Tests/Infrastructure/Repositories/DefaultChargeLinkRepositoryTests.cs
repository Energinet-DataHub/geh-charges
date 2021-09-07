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
using System.Threading.Tasks;
using FluentAssertions;
using GreenEnergyHub.Charges.Domain.Charges;
using GreenEnergyHub.Charges.Domain.MarketDocument;
using GreenEnergyHub.Charges.Domain.MeteringPoints;
using GreenEnergyHub.Charges.Infrastructure.Context;
using GreenEnergyHub.Charges.Infrastructure.Context.Model;
using GreenEnergyHub.Charges.Infrastructure.Repositories;
using GreenEnergyHub.Charges.TestCore.Squadron;
using Microsoft.EntityFrameworkCore;
using Squadron;
using Xunit;
using Xunit.Categories;
using Charge = GreenEnergyHub.Charges.Infrastructure.Context.Model.Charge;
using ChargeOperation = GreenEnergyHub.Charges.Infrastructure.Context.Model.ChargeOperation;
using DefaultChargeLink = GreenEnergyHub.Charges.Infrastructure.Context.Model.DefaultChargeLink;
using MarketParticipant = GreenEnergyHub.Charges.Infrastructure.Context.Model.MarketParticipant;

namespace GreenEnergyHub.Charges.Tests.Infrastructure.Repositories
{
    /// <summary>
    /// Tests <see cref="DefaultChargeLinkRepository"/> using an SQLite in-memory database.
    /// </summary>
    [UnitTest]
    public class DefaultChargeLinkRepositoryTests : IClassFixture<SqlServerResource<SqlServerOptions>>
    {
        private readonly SqlServerResource<SqlServerOptions> _resource;

        public DefaultChargeLinkRepositoryTests(SqlServerResource<SqlServerOptions> resource)
        {
            _resource = resource;
        }

        [Fact]
        public async Task GetDefaultChargeLinks_WhenCalledWithMeteringPointType_ReturnsDefaultCharges()
        {
            await using var chargesDatabaseContext = await SquadronContextFactory
                .GetDatabaseContextAsync(_resource)
                .ConfigureAwait(false);

            // Arrange
            var sut = new DefaultChargeLinkRepository(chargesDatabaseContext);

            // Act
            var actual = await
                sut.GetAsync(
                    MeteringPointType.Consumption).ConfigureAwait(false);

            // Assert
            var actualDefaultChargeLinkSettings =
                actual as Charges.Domain.Charges.DefaultChargeLink[] ?? actual.ToArray();

            actualDefaultChargeLinkSettings.Should().NotBeNullOrEmpty();
        }
    }
}
