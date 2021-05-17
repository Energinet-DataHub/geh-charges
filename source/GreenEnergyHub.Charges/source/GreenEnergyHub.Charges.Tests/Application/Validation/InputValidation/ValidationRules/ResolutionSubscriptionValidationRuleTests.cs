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
using FluentAssertions;
using GreenEnergyHub.Charges.Application.Validation.InputValidation.ValidationRules;
using GreenEnergyHub.Charges.Domain.ChangeOfCharges.Transaction;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Application.Validation.InputValidation.ValidationRules
{
    [UnitTest]
    public class ResolutionSubscriptionValidationRuleTests
    {
        [Theory]
        [InlineAutoMoqData(Resolution.Unknown, true)]
        [InlineAutoMoqData(Resolution.P1D, true)]
        [InlineAutoMoqData(Resolution.P1M, true)]
        [InlineAutoMoqData(Resolution.PT1H, true)]
        [InlineAutoMoqData(Resolution.PT15M, true)]
        public void ResolutionTariffValidationRule_WithTariffType_Test(
            Resolution resolution,
            bool expected,
            [NotNull] ChargeCommand command)
        {
            command.ChargeOperation.Type = ChargeType.Tariff;
            command.ChargeOperation.Resolution = resolution;
            var sut = new ResolutionSubscriptionValidationRule(command);
            sut.IsValid.Should().Be(expected);
        }

        [Theory]
        [InlineAutoMoqData(Resolution.Unknown, false)]
        [InlineAutoMoqData(Resolution.P1D, false)]
        [InlineAutoMoqData(Resolution.P1M, true)]
        [InlineAutoMoqData(Resolution.PT1H, false)]
        [InlineAutoMoqData(Resolution.PT15M, false)]
        public void ResolutionTariffValidationRule_WithSubscriptionType_Test(
            Resolution resolution,
            bool expected,
            [NotNull] ChargeCommand command)
        {
            command.ChargeOperation.Type = ChargeType.Subscription;
            command.ChargeOperation.Resolution = resolution;
            var sut = new ResolutionSubscriptionValidationRule(command);
            sut.IsValid.Should().Be(expected);
        }

        [Theory]
        [InlineAutoMoqData(Resolution.Unknown, false)]
        [InlineAutoMoqData(Resolution.P1D, true)]
        [InlineAutoMoqData(Resolution.P1M, false)]
        [InlineAutoMoqData(Resolution.PT1H, false)]
        [InlineAutoMoqData(Resolution.PT15M, false)]
        public void ResolutionTariffValidationRule_WithFeeType_Test(
            Resolution resolution,
            bool expected,
            [NotNull] ChargeCommand command)
        {
            command.ChargeOperation.Type = ChargeType.Fee;
            command.ChargeOperation.Resolution = resolution;
            var sut = new ResolutionSubscriptionValidationRule(command);
            sut.IsValid.Should().Be(expected);
        }
    }
}
