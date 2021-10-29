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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GreenEnergyHub.Charges.Domain.Charges;
using GreenEnergyHub.Charges.Domain.MarketParticipants;
using GreenEnergyHub.Charges.Domain.SharedDtos;
using GreenEnergyHub.Charges.Infrastructure.Context;
using GreenEnergyHub.Charges.Infrastructure.Repositories;
using GreenEnergyHub.Charges.TestCore.Attributes;
using GreenEnergyHub.Charges.TestCore.Database;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Xunit;
using Xunit.Categories;
using Charge = GreenEnergyHub.Charges.Domain.Charges.Charge;
using MarketParticipant = GreenEnergyHub.Charges.Infrastructure.Context.Model.MarketParticipant;

namespace GreenEnergyHub.Charges.Tests.Infrastructure.Repositories
{
    /// <summary>
    /// Tests <see cref="ChargeRepository"/> using a database created with squadron.
    /// </summary>
    [UnitTest]
    public class ChargeRepositoryTests : IClassFixture<ChargesDatabaseFixture>
    {
        private const string MarketParticipantOwner = "MarketParticipantId";

        private readonly ChargesDatabaseManager _databaseManager;

        public ChargeRepositoryTests(ChargesDatabaseFixture fixture)
        {
            _databaseManager = fixture.DatabaseManager;
        }

        [Fact]
        public async Task GetChargeAsync_WhenChargeIsCreated_ThenSuccessReturnedAsync()
        {
            // Arrange
            await using var chargesDatabaseWriteContext = _databaseManager.CreateDbContext();
            var charge = GetValidCharge();
            await SeedDatabaseAsync(chargesDatabaseWriteContext).ConfigureAwait(false);
            var sut = new ChargeRepository(chargesDatabaseWriteContext);

            // Act
            await sut.StoreChargeAsync(charge, MarketParticipantOwner, DateTime.Now).ConfigureAwait(false);

            // Assert
            await using var chargesDatabaseReadContext = _databaseManager.CreateDbContext();

            var actual = await chargesDatabaseReadContext.Charges
                .Include(x => x.ChargePrices)
                .Include(x => x.ChargePeriodDetails)
                .SingleOrDefaultAsync(x =>
                    x.Id == charge.Id &&
                    x.SenderProvidedChargeId == charge.SenderProvidedChargeId &&
                    x.MarketParticipant.MarketParticipantId == charge.Owner &&
                    x.ChargeType == (int)charge.Type)
                .ConfigureAwait(false);

            var list = chargesDatabaseReadContext.Charges.Include(x => x.MarketParticipant).ToList();

            list.Should().NotBeEmpty();
            actual.Should().NotBeNull();
            actual.ChargePrices.Should().NotBeNullOrEmpty();
            actual.ChargePeriodDetails.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task CheckIfChargeExistsAsync_WhenChargeIsCreated_ThenSuccessReturnedAsync()
        {
            await using var chargesDatabaseWriteContext = _databaseManager.CreateDbContext();

            // Arrange
            var charge = GetValidCharge();
            await SeedDatabaseAsync(chargesDatabaseWriteContext).ConfigureAwait(false);
            var sut = new ChargeRepository(chargesDatabaseWriteContext);

            // Act
            await sut.StoreChargeAsync(charge, MarketParticipantOwner, DateTime.Now).ConfigureAwait(false);

            // Assert
            await using var chargesDatabaseReadContext = _databaseManager.CreateDbContext();
            var actual = chargesDatabaseReadContext.Charges.Any(x => x.SenderProvidedChargeId == charge.SenderProvidedChargeId &&
                                                                     x.MarketParticipant.MarketParticipantId == charge.Owner &&
                                                                     x.ChargeType == (int)charge.Type);

            actual.Should().BeTrue();
        }

        [Fact]
        public async Task CheckIfChargeExistsByCorrelationIdAsync_WhenChargeIsCreated_ThenSuccessReturnedAsync()
        {
            await using var chargesDatabaseWriteContext = _databaseManager.CreateDbContext();

            // Arrange
            var charge = GetValidCharge();
            await SeedDatabaseAsync(chargesDatabaseWriteContext).ConfigureAwait(false);
            var sut = new ChargeRepository(chargesDatabaseWriteContext);

            // Act
            await sut.StoreChargeAsync(charge, MarketParticipantOwner, DateTime.Now).ConfigureAwait(false);

            // Assert
            await using var chargesDatabaseReadContext = _databaseManager.CreateDbContext();
            var actual = chargesDatabaseReadContext
                .Charges
                .Any(x => x.ChargeOperation.CorrelationId == charge.CorrelationId);

            actual.Should().BeTrue();
        }

        [Theory]
        [InlineAutoMoqData]
        public async Task StoreChargeAsync_WhenChargeIsNull_ThrowsArgumentNullException(
            [NotNull] ChargeRepository sut)
        {
            // Arrange
            Charge? charge = null;

            // Act / Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                    () => sut.StoreChargeAsync(charge!, MarketParticipantOwner, DateTime.Now))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task GetChargeAsync_WithId_ThenSuccessReturnedAsync()
        {
            await using var chargesDatabaseContext = _databaseManager.CreateDbContext();

            // Arrange
            var sut = new ChargeRepository(chargesDatabaseContext);
            var charge = GetValidCharge();
            await sut.StoreChargeAsync(charge, MarketParticipantOwner, DateTime.Now).ConfigureAwait(false);
            await using var chargesDatabaseReadContext = _databaseManager.CreateDbContext();
            var createdCharge = chargesDatabaseReadContext.
                Charges.First(x => x.SenderProvidedChargeId == charge.SenderProvidedChargeId &&
                                                                     x.MarketParticipant.MarketParticipantId == charge.Owner &&
                                                                     x.ChargeType == (int)charge.Type);
            // Act
            var actual = await sut.GetChargeAsync(createdCharge.Id).ConfigureAwait(false);

            // Assert
            actual.Should().NotBeNull();
        }

        private static Charge GetValidCharge()
        {
            var charge = new Charge(
                Guid.NewGuid(),
                "ChargeOperationId",
                "SenderProvidedId",
                "Name",
                "description",
                MarketParticipantOwner,
                "CorrelationId",
                SystemClock.Instance.GetCurrentInstant(),
                Instant.FromUtc(9999, 12, 31, 23, 59, 59),
                ChargeType.Fee,
                VatClassification.Unknown,
                Resolution.P1D,
                true,
                false,
                new List<Point>
                {
                    new Point { Position = 0, Time = SystemClock.Instance.GetCurrentInstant(), Price = 200m },
                });

            return charge;
        }

        private static async Task SeedDatabaseAsync(ChargesDatabaseContext context)
        {
            var marketParticipant = await context.MarketParticipants.SingleOrDefaultAsync(x => x.MarketParticipantId == MarketParticipantOwner)
                .ConfigureAwait(false);

            if (marketParticipant == null)
            {
                context.MarketParticipants.Add(
                                new MarketParticipant { Name = "Name", Role = 1, MarketParticipantId = MarketParticipantOwner });
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
