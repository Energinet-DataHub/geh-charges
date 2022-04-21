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

using FluentAssertions;
using GreenEnergyHub.Charges.Domain.Charges;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation.BusinessValidation.ValidationRules;
using GreenEnergyHub.Charges.Tests.Builders.Command;
using GreenEnergyHub.TestHelpers;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Domain.Dtos.ChargeCommands.Validation.BusinessValidation.ValidationRules
{
    [UnitTest]
    public class ChargeResolutionCanNotBeUpdatedRuleTests
    {
        [Theory]
        [InlineAutoDomainData]
        public void IsValid_WhenResolutionIsTheSame_ShouldReturnTrue(
            ChargeOperationDtoBuilder builder,
            Charge charge)
        {
            var chargeOperationDto = builder.WithResolution(charge.Resolution).Build();
            var sut = new ChargeResolutionCanNotBeUpdatedRule(charge, chargeOperationDto);
            sut.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineAutoDomainData]
        public void IsValid_WhenResolutionIsNotTheSame_ShouldReturnFalse(
            ChargeOperationDtoBuilder builder,
            Charge charge)
        {
            var chargeOperationDto = builder
                .WithResolution(charge.Resolution == Resolution.P1D ? Resolution.P1M : Resolution.PT1H)
                .Build();
            var sut = new ChargeResolutionCanNotBeUpdatedRule(charge, chargeOperationDto);
            sut.IsValid.Should().BeFalse();
        }
    }
}