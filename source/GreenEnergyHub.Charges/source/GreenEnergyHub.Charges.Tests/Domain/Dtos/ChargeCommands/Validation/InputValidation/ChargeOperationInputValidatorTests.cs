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

using AutoFixture.Xunit2;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation.InputValidation.Factories;
using GreenEnergyHub.Charges.Domain.Dtos.Validation;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Domain.Dtos.ChargeCommands.Validation.InputValidation
{
    [UnitTest]
    public class ChargeOperationInputValidatorTests
    {
        [Theory]
        [InlineAutoData]
        public void Validate_WhenValidatingChargeCommandWithChargeInformation_ReturnsChargeCommandValidationResult(
            ChargeInformationInputValidationRulesFactory chargeInformationInputValidationRulesFactory,
            ChargeInformationDto chargeInformationDto)
        {
            // Arrange
            var sut = new InputValidator<ChargeInformationDto>(chargeInformationInputValidationRulesFactory);

            // Act
            var result = sut.Validate(chargeInformationDto);

            // Assert
            Assert.IsType<ValidationResult>(result);
        }

        [Theory]
        [InlineAutoData]
        public void Validate_WhenValidatingChargeCommandWithPrices_ReturnsChargeCommandValidationResult(
            ChargePriceInputValidationRulesFactory chargePriceInputValidationRulesFactory,
            ChargePriceDto chargePriceDto)
        {
            // Arrange
            var sut = new InputValidator<ChargePriceDto>(chargePriceInputValidationRulesFactory);

            // Act
            var result = sut.Validate(chargePriceDto);

            // Assert
            Assert.IsType<ValidationResult>(result);
        }
    }
}
