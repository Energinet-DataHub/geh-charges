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
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation.InputValidation.ValidationRules;
using GreenEnergyHub.Charges.Domain.MarketParticipants;
using GreenEnergyHub.Charges.TestCore.Attributes;
using GreenEnergyHub.TestHelpers;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Domain.Dtos.ChargeCommands.Validation.InputValidation.ValidationRules
{
    [UnitTest]
    public class DocumentTypeMustBeRequestUpdateChargeInformationTests
    {
        [Theory]
        [InlineAutoMoqData(DocumentType.Unknown, false)]
        [InlineAutoMoqData(DocumentType.RequestUpdateChargeInformation, true)]
        public void DocumentTypeMustBeRequestUpdateChargeInformation_Test(
            DocumentType documentType,
            bool expected,
            [NotNull] ChargeCommand command)
        {
            command.Document.Type = documentType;
            var sut = new DocumentTypeMustBeRequestUpdateChargeInformationRule(command);
            Assert.Equal(expected, sut.IsValid);
        }

        [Theory]
        [InlineAutoDomainData]
        public void ValidationRuleIdentifier_ShouldBe_EqualTo([NotNull] ChargeCommand command)
        {
            var sut = new DocumentTypeMustBeRequestUpdateChargeInformationRule(command);
            sut.ValidationRuleIdentifier.Should().Be(ValidationRuleIdentifier.DocumentTypeMustBeRequestUpdateChargeInformation);
        }
    }
}