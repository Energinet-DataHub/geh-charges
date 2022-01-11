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
using FluentAssertions;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation;
using GreenEnergyHub.Charges.Infrastructure.Core.Cim;
using GreenEnergyHub.Charges.MessageHub.Models.AvailableChargeReceiptData;
using GreenEnergyHub.Charges.TestCore.Attributes;
using Xunit;

namespace GreenEnergyHub.Charges.Tests.MessageHub.Models.AvailableChargeReceiptData
{
    public class CimValidationErrorCodeFactoryTests
    {
        [Theory]
        [InlineAutoMoqData(ValidationRuleIdentifier.MaximumPrice, ReasonCode.E90)]
        [InlineAutoMoqData(ValidationRuleIdentifier.ResolutionFeeValidation, ReasonCode.D23)]
        [InlineAutoMoqData(ValidationRuleIdentifier.ResolutionSubscriptionValidation, ReasonCode.D23)]
        [InlineAutoMoqData(ValidationRuleIdentifier.ResolutionTariffValidation, ReasonCode.D23)]
        [InlineAutoMoqData(ValidationRuleIdentifier.StartDateValidation, ReasonCode.E17)]
        [InlineAutoMoqData(ValidationRuleIdentifier.VatClassificationValidation, ReasonCode.E86)]
        [InlineAutoMoqData(ValidationRuleIdentifier.ChargeIdLengthValidation, ReasonCode.E86)]
        [InlineAutoMoqData(ValidationRuleIdentifier.ChargeIdRequiredValidation, ReasonCode.E0H)]
        [InlineAutoMoqData(ValidationRuleIdentifier.ChargeOperationIdRequired, ReasonCode.E0H)]
        [InlineAutoMoqData(ValidationRuleIdentifier.UpdateNotYetSupported, ReasonCode.D13)]
        [InlineAutoMoqData(ValidationRuleIdentifier.ChargeDescriptionHasMaximumLength, ReasonCode.E86)]
        [InlineAutoMoqData(ValidationRuleIdentifier.ChargeNameHasMaximumLength, ReasonCode.E86)]
        [InlineAutoMoqData(ValidationRuleIdentifier.ChargeOwnerIsRequiredValidation, ReasonCode.E0H)]
        [InlineAutoMoqData(ValidationRuleIdentifier.ChargeTypeIsKnownValidation, ReasonCode.E86)]
        [InlineAutoMoqData(ValidationRuleIdentifier.ChargeTypeTariffPriceCount, ReasonCode.E87)]
        [InlineAutoMoqData(ValidationRuleIdentifier.FeeMustHaveSinglePrice, ReasonCode.E87)]
        [InlineAutoMoqData(ValidationRuleIdentifier.RecipientIsMandatoryTypeValidation, ReasonCode.D02)]
        [InlineAutoMoqData(ValidationRuleIdentifier.SenderIsMandatoryTypeValidation, ReasonCode.D02)]
        [InlineAutoMoqData(ValidationRuleIdentifier.StartDateTimeRequiredValidation, ReasonCode.E0H)]
        [InlineAutoMoqData(ValidationRuleIdentifier.SubscriptionMustHaveSinglePrice, ReasonCode.E87)]
        [InlineAutoMoqData(ValidationRuleIdentifier.ChangingTariffTaxValueNotAllowed, ReasonCode.D14)]
        [InlineAutoMoqData(ValidationRuleIdentifier.ChangingTariffVatValueNotAllowed, ReasonCode.D14)]
        [InlineAutoMoqData(ValidationRuleIdentifier.ChargePriceMaximumDigitsAndDecimals, ReasonCode.E86)]
        [InlineAutoMoqData(ValidationRuleIdentifier.BusinessReasonCodeMustBeUpdateChargeInformation, ReasonCode.D02)]
        [InlineAutoMoqData(ValidationRuleIdentifier.CommandSenderMustBeAnExistingMarketParticipant, ReasonCode.D02)]
        [InlineAutoMoqData(ValidationRuleIdentifier.DocumentTypeMustBeRequestUpdateChargeInformation, ReasonCode.D02)]
        public void Create_ReturnsExpectedCode(ValidationRuleIdentifier identifier, ReasonCode expected, CimValidationErrorCodeFactory sut)
        {
            var actual = sut.Create(identifier);
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineAutoMoqData]
        public void Create_HandlesAllIdentifiers(CimValidationErrorCodeFactory sut)
        {
            foreach (var value in Enum.GetValues<ValidationRuleIdentifier>())
            {
                // Assert that create does not throw (ensures that we are mapping all enum values)
                sut.Create(value);
            }
        }
    }
}