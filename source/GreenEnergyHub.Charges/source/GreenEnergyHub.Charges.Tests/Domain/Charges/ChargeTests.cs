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
using FluentAssertions;
using FluentAssertions.Equivalency.Steps;
using GreenEnergyHub.Charges.Domain.Charges;
using GreenEnergyHub.Charges.TestCore;
using GreenEnergyHub.Charges.TestCore.Attributes;
using GreenEnergyHub.Charges.Tests.Builders.Command;
using GreenEnergyHub.TestHelpers;
using Microsoft.Identity.Client.Extensions.Msal;
using NodaTime;
using NuGet.Frameworks;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Domain.Charges
{
    [UnitTest]
    public class ChargeTests
    {
        [Fact]
        public void UpdateCharge_NewPeriodInsideSingleExistingPeriod_SetsNewEndDateForExistingPeriodAndInsertsNewPeriod()
        {
            // Arrange
            var existingPeriod = new ChargePeriodBuilder()
                .WithName("ExistingPeriod")
                .WithStartDateTime(InstantHelper.GetTodayAtMidnightUtc())
                .Build();

            var sut = new ChargeBuilder()
                .WithPeriods(new List<ChargePeriod> { existingPeriod })
                .Build();

            var newPeriod = new ChargePeriodBuilder()
                .WithName("NewPeriod")
                .WithStartDateTime(InstantHelper.GetTomorrowAtMidnightUtc())
                .Build();

            // Act
            sut.Update(newPeriod);

            // Assert
            var actualTimeline = sut.Periods.OrderBy(p => p.StartDateTime).ToList();
            var actualFirstPeriod = actualTimeline[0];
            var actualSecondPeriod = actualTimeline[1];

            actualTimeline.Count.Should().Be(2);
            actualFirstPeriod.Name.Should().Be("ExistingPeriod");
            actualFirstPeriod.StartDateTime.Should().Be(InstantHelper.GetTodayAtMidnightUtc());
            actualFirstPeriod.EndDateTime.Should().Be(InstantHelper.GetTomorrowAtMidnightUtc());
            actualSecondPeriod.Name.Should().Be("NewPeriod");
            actualSecondPeriod.StartDateTime.Should().Be(InstantHelper.GetTomorrowAtMidnightUtc());
            actualSecondPeriod.EndDateTime.Should().Be(InstantHelper.GetEndDefault());
        }

        [Fact]
        public void UpdateCharge_NewPeriodStartsInsideFirstOfThreeExistingPeriods_SetsNewEndDateTimeForFirstExistingPeriod_AndRemovesSecondAndThird_AndInsertsNewPeriod()
        {
            // Arrange
            var sut = new ChargeBuilder()
                .WithPeriods(CreateThreeExistingPeriods())
                .Build();

            var newPeriod = new ChargePeriodBuilder()
                .WithName("NewPeriod")
                .WithStartDateTime(InstantHelper.GetTodayAtMidnightUtc())
                .Build();

            // Act
            sut.Update(newPeriod);

            // Assert
            var actualTimeline = sut.Periods.OrderBy(p => p.StartDateTime).ToList();
            var actualFirstPeriod = actualTimeline[0];
            var actualSecondPeriod = actualTimeline[1];

            actualTimeline.Count.Should().Be(2);
            actualFirstPeriod.Name.Should().Be("FirstPeriod");
            actualFirstPeriod.StartDateTime.Should().Be(InstantHelper.GetYesterdayAtMidnightUtc());
            actualFirstPeriod.EndDateTime.Should().Be(InstantHelper.GetTodayAtMidnightUtc());
            actualSecondPeriod.Name.Should().Be("NewPeriod");
            actualSecondPeriod.StartDateTime.Should().Be(InstantHelper.GetTodayAtMidnightUtc());
            actualSecondPeriod.EndDateTime.Should().Be(InstantHelper.GetEndDefault());
        }

        [Fact]
        public void UpdateCharge_NewPeriodStartEqualsSecondOfThreeExistingPeriods_NewPeriodOverwritesSecondAndThird()
        {
            // Arrange
            var sut = new ChargeBuilder()
                .WithPeriods(CreateThreeExistingPeriods())
                .Build();

            var newPeriod = new ChargePeriodBuilder()
                .WithName("NewPeriod")
                .WithStartDateTime(InstantHelper.GetTomorrowAtMidnightUtc())
                .Build();

            // Act
            sut.Update(newPeriod);

            // Assert
            var actualTimeline = sut.Periods.OrderBy(p => p.StartDateTime).ToList();
            var actualFirstPeriod = actualTimeline[0];
            var actualSecondPeriod = actualTimeline[1];

            actualTimeline.Count.Should().Be(2);
            actualFirstPeriod.Name.Should().Be("FirstPeriod");
            actualFirstPeriod.StartDateTime.Should().Be(InstantHelper.GetYesterdayAtMidnightUtc());
            actualFirstPeriod.EndDateTime.Should().Be(InstantHelper.GetTomorrowAtMidnightUtc());
            actualSecondPeriod.Name.Should().Be("NewPeriod");
            actualSecondPeriod.StartDateTime.Should().Be(InstantHelper.GetTomorrowAtMidnightUtc());
            actualSecondPeriod.EndDateTime.Should().Be(InstantHelper.GetEndDefault());
        }

        [Fact]
        public void UpdateCharge_NewPeriodStartsBeforeExistingPeriod_NewPeriodOverwritesExisting()
        {
            // Arrange
            var existingPeriod = new ChargePeriodBuilder()
                .WithName("ExistingPeriod")
                .WithStartDateTime(InstantHelper.GetTodayAtMidnightUtc())
                .Build();

            var sut = new ChargeBuilder()
                .WithPeriods(new List<ChargePeriod> { existingPeriod })
                .Build();

            var newPeriod = new ChargePeriodBuilder()
                .WithName("NewPeriod")
                .WithStartDateTime(InstantHelper.GetYesterdayAtMidnightUtc())
                .Build();

            // Act
            sut.Update(newPeriod);

            // Assert
            var actualTimeline = sut.Periods.OrderBy(p => p.StartDateTime).ToList();
            var actualFirstPeriod = actualTimeline[0];

            actualTimeline.Count.Should().Be(1);
            actualFirstPeriod.Name.Should().Be("NewPeriod");
            actualFirstPeriod.StartDateTime.Should().Be(InstantHelper.GetYesterdayAtMidnightUtc());
            actualFirstPeriod.EndDateTime.Should().Be(InstantHelper.GetEndDefault());
        }

        [Theory]
        [InlineAutoMoqData]
        public void UpdateCharge_WhenChargePeriodIsNull_ThrowsArgumentNullException(Charge sut)
        {
            ChargePeriod? chargePeriod = null;

            Assert.Throws<ArgumentNullException>(() => sut.Update(chargePeriod!));
        }

        [Fact]
        public void UpdateCharge_WhenEndDateIsBound_ThenThrowException()
        {
            // Arrange
            var existingPeriod = new ChargePeriodBuilder()
                .WithName("ExistingPeriod")
                .WithStartDateTime(InstantHelper.GetTodayAtMidnightUtc())
                .Build();

            var sut = new ChargeBuilder()
                .WithPeriods(new List<ChargePeriod> { existingPeriod })
                .Build();

            var newPeriod = new ChargePeriodBuilder()
                .WithStartDateTime(InstantHelper.GetYesterdayAtMidnightUtc())
                .WithEndDateTime(InstantHelper.GetTomorrowAtMidnightUtc())
                .Build();

            // Act
            Assert.Throws<InvalidOperationException>(() => sut.Update(newPeriod));
        }

        [Fact]
        public void StopCharge_WhenSingleExistingChargePeriod_SetNewEndDate()
        {
            // Arrange
            var today = InstantHelper.GetTodayAtMidnightUtc();
            var dayAfterTomorrow = InstantHelper.GetTodayPlusDaysAtMidnightUtc(2);
            var existingPeriod = new ChargePeriodBuilder()
                .WithStartDateTime(today)
                .WithEndDateTime(InstantHelper.GetEndDefault())
                .Build();

            var sut = new ChargeBuilder().WithPeriods(new List<ChargePeriod> { existingPeriod }).Build();

            // Act
            sut.Stop(dayAfterTomorrow);

            // Assert
            var actual = sut.Periods.OrderByDescending(p => p.StartDateTime).First();
            actual.EndDateTime.Should().BeEquivalentTo(dayAfterTomorrow);
        }

        [Fact]
        public void StopCharge_WhenThreeExistingPeriods_ThenRemoveAllAfterStop()
        {
            // Arrange
            var sut = new ChargeBuilder().WithPeriods(CreateThreeExistingPeriods()).Build();
            var stopDate = InstantHelper.GetTomorrowAtMidnightUtc();

            // Act
            sut.Stop(stopDate);

            // Assert
            sut.Periods.Count.Should().Be(1);
            sut.Periods.OrderByDescending(p => p.StartDateTime).First().EndDateTime.Should().Be(stopDate);
        }

        [Fact]
        public void StopCharge_WhenNoPeriodsExist_ThenThrowException()
        {
            // Arrange
            var sut = new ChargeBuilder().Build();
            var stopDate = InstantHelper.GetTomorrowAtMidnightUtc();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => sut.Stop(stopDate));
        }

        [Fact]
        public void StopCharge_WhenStopExistAfterStopDate_ThenNewStopReplaceOldStop()
        {
            // Arrange
            var period = new ChargePeriodBuilder()
                .WithStartDateTime(InstantHelper.GetTodayAtMidnightUtc())
                .WithEndDateTime(InstantHelper.GetTodayPlusDaysAtMidnightUtc(10))
                .Build();
            var periods = new List<ChargePeriod>() { period };
            var sut = new ChargeBuilder().WithPeriods(periods).Build();
            var stopDate = InstantHelper.GetTomorrowAtMidnightUtc();

            // Act
            sut.Stop(stopDate);

            // Assert
            var actual = sut.Periods.OrderByDescending(p => p.StartDateTime).First();
            actual.EndDateTime.Should().Be(stopDate);
        }

        [Fact]
        public void StopCharge_WhenStopExistBeforeStopDate_ThenThrowException()
        {
            // Arrange
            var period = new ChargePeriodBuilder()
                .WithStartDateTime(InstantHelper.GetTodayAtMidnightUtc())
                .WithEndDateTime(InstantHelper.GetTodayPlusDaysAtMidnightUtc(5))
                .Build();
            var periods = new List<ChargePeriod>() { period };
            var sut = new ChargeBuilder().WithPeriods(periods).Build();
            var stopDate = InstantHelper.GetTodayPlusDaysAtMidnightUtc(10);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => sut.Stop(stopDate));
        }

        [Fact]
        public void StopCharge_WhenChargeStartDateAfterStopDate_ThenThrowException()
        {
            // Arrange
            var period = new ChargePeriodBuilder()
                .WithStartDateTime(InstantHelper.GetTodayPlusDaysAtMidnightUtc(5))
                .WithEndDateTime(InstantHelper.GetEndDefault())
                .Build();
            var periods = new List<ChargePeriod>() { period };
            var sut = new ChargeBuilder().WithPeriods(periods).Build();
            var stopDate = InstantHelper.GetTodayPlusDaysAtMidnightUtc(2);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => sut.Stop(stopDate));
        }

        [Fact]
        public void StopCharge_WhenNewEndDateIsNot_ThenThrowException()
        {
            // Arrange
            var sut = new ChargeBuilder().Build();
            Instant? stopDate = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => sut.Stop(stopDate));
        }

        [Fact]
        public void CancelStop_WhenStopPeriodExists_ThenReplaceEndDateWithDefaultEndDate()
        {
            // Arrange
            var sut = new ChargeBuilder().WithPeriods(new List<ChargePeriod>
            {
                new ChargePeriodBuilder()
                    .WithStartDateTime(InstantHelper.GetYesterdayAtMidnightUtc())
                    .WithEndDateTime(InstantHelper.GetTodayAtMidnightUtc())
                    .Build(),
                new ChargePeriodBuilder()
                    .WithStartDateTime(InstantHelper.GetTodayAtMidnightUtc())
                    .WithEndDateTime(InstantHelper.GetTomorrowAtMidnightUtc())
                    .Build(),
            }).Build();

            // Act
            sut.CancelStop();

            // Assert
            var actual = sut.Periods.OrderByDescending(p => p.StartDateTime).First();

            actual.EndDateTime.Should().BeEquivalentTo(InstantHelper.GetEndDefault());
        }

        [Fact]
        public void CancelStop_WhenNoExistingPeriods_ThenThrowException()
        {
            // Arrange
            var sut = new ChargeBuilder().Build();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(sut.CancelStop);
        }

        private static List<ChargePeriod> CreateThreeExistingPeriods()
        {
            return new List<ChargePeriod>
            {
                new ChargePeriodBuilder()
                    .WithName("FirstPeriod")
                    .WithStartDateTime(InstantHelper.GetYesterdayAtMidnightUtc())
                    .WithEndDateTime(InstantHelper.GetTomorrowAtMidnightUtc())
                    .Build(),
                new ChargePeriodBuilder()
                    .WithName("SecondPeriod")
                    .WithStartDateTime(InstantHelper.GetTomorrowAtMidnightUtc())
                    .WithEndDateTime(InstantHelper.GetTodayPlusDaysAtMidnightUtc(2))
                    .Build(),
                new ChargePeriodBuilder()
                    .WithName("ThirdPeriod")
                    .WithStartDateTime(InstantHelper.GetTodayPlusDaysAtMidnightUtc(2))
                    .WithEndDateTime(InstantHelper.GetEndDefault())
                    .Build(),
            };
        }
    }
}
