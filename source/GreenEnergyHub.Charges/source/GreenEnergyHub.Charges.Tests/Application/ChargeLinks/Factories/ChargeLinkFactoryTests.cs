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
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using GreenEnergyHub.Charges.Application.ChangeOfCharges.Repositories;
using GreenEnergyHub.Charges.Application.ChargeLinks.Factories;
using GreenEnergyHub.Charges.Application.Charges.Repositories;
using GreenEnergyHub.Charges.Domain.ChargeLinks.Events.Local;
using GreenEnergyHub.Charges.Domain.Charges;
using GreenEnergyHub.Charges.Domain.MeteringPoints;
using GreenEnergyHub.TestHelpers;
using Moq;
using Xunit;

namespace GreenEnergyHub.Charges.Tests.Application.ChargeLinks.Factories
{
    public class ChargeLinkFactoryTests
    {
        [Theory]
        [InlineAutoDomainData]
        public async Task CreateAsync_WhenCalled_ShouldCreateChargeLinkCorrectly(
            [NotNull] ChargeLinkCommandReceivedEvent expectedEvent,
            [NotNull] Charge expectedCharge,
            [NotNull] MeteringPoint expectedMeteringPoint,
            [NotNull] [Frozen] Mock<IChargeRepository> chargeRepository,
            [NotNull] [Frozen] Mock<IMeteringPointRepository> meteringPointRepository,
            [NotNull] ChargeLinkFactory sut)
        {
            // Arrange
            expectedEvent.SetCorrelationId(Guid.NewGuid().ToString("N"));
            expectedCharge.RowId = 11;
            expectedMeteringPoint.RowId = 22;

            chargeRepository
                .Setup(x => x.GetChargeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ChargeType>()))
                .ReturnsAsync(expectedCharge);

            meteringPointRepository
                .Setup(x => x.GetMeteringPointAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedMeteringPoint);

            // Act
            var actual = await sut.CreateAsync(expectedEvent).ConfigureAwait(false);

            // Assert
            actual.ChargeId
                .Should().Be(expectedCharge.RowId);
            actual.MeteringPointId
                .Should().Be(expectedMeteringPoint.RowId);
            actual.PeriodDetails.First().StartDateTime
                .Should().Be(expectedEvent.ChargeLinkCommand.ChargeLink.StartDateTime);
            actual.PeriodDetails.First().EndDateTime
                .Should().Be(expectedEvent.ChargeLinkCommand.ChargeLink.EndDateTime);
            actual.PeriodDetails.First().Factor
                .Should().Be(expectedEvent.ChargeLinkCommand.ChargeLink.Factor);
            actual.Operations.First().CorrelationId
                .Should().Be(expectedEvent.CorrelationId);
            actual.Operations.First().Id
                .Should().Be(expectedEvent.ChargeLinkCommand.ChargeLink.OperationId);
        }

        [Theory]
        [InlineAutoDomainData]
        public async Task CreateAsync_WhenCalledWithNull_ShouldThrow(
            [NotNull] ChargeLinkFactory sut)
        {
            await Assert
                .ThrowsAsync<ArgumentNullException>(async () => await sut.CreateAsync(null!)
                    .ConfigureAwait(false))
                .ConfigureAwait(false);
        }
    }
}
