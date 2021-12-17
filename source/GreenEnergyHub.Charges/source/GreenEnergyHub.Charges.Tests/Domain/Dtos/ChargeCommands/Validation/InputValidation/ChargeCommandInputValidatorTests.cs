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

using System.Diagnostics.CodeAnalysis;
using AutoFixture.Xunit2;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation.InputValidation;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Domain.Dtos.ChargeCommands.Validation.InputValidation
{
    [UnitTest]
    public class ChargeCommandInputValidatorTests
    {
        [Theory]
        [InlineAutoData]
        public void Validate_WhenValidatingChargeCommand_ReturnsChargeCommandValidationResult(
            [NotNull] InputValidationRulesFactory inputValidationRulesFactory,
            [NotNull] ChargeCommand chargeCommand)
        {
            // Arrange
            var sut = new ChargeCommandInputValidator(inputValidationRulesFactory);

            // Act
            var result = sut.Validate(chargeCommand);

            // Assert
            Assert.IsType<ChargeCommandValidationResult>(result);
        }
    }
}