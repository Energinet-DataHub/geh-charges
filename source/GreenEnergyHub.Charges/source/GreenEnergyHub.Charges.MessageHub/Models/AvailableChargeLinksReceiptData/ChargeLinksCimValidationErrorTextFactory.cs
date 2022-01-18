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
using GreenEnergyHub.Charges.Domain.Dtos.ChargeLinksCommands;
using GreenEnergyHub.Charges.Domain.Dtos.Validation;
using GreenEnergyHub.Charges.Infrastructure.Core.Cim.ValidationErrors;
using GreenEnergyHub.Charges.MessageHub.Models.AvailableData;
using GreenEnergyHub.Charges.MessageHub.Models.Shared;
using Microsoft.Extensions.Logging;

namespace GreenEnergyHub.Charges.MessageHub.Models.AvailableChargeLinksReceiptData
{
    public class ChargeLinksCimValidationErrorTextFactory : ICimValidationErrorTextFactory<ChargeLinksCommand>
    {
        private readonly ICimValidationErrorTextProvider _cimValidationErrorTextProvider;
        private readonly ILogger _logger;

        public ChargeLinksCimValidationErrorTextFactory(
            ICimValidationErrorTextProvider cimValidationErrorTextProvider,
            ILoggerFactory loggerFactory)
        {
            _cimValidationErrorTextProvider = cimValidationErrorTextProvider;
            _logger = loggerFactory.CreateLogger(nameof(ChargeLinksCimValidationErrorTextFactory));
        }

        public string Create(ValidationError validationError, ChargeLinksCommand command)
        {
            return GetMergedErrorMessage(validationError, command);
        }

        private string GetMergedErrorMessage(ValidationError validationError, ChargeLinksCommand chargeLinksCommand)
        {
            var errorTextTemplate = _cimValidationErrorTextProvider
                .GetCimValidationErrorText(validationError.ValidationRuleIdentifier);

            return MergeErrorText(errorTextTemplate, chargeLinksCommand, validationError.TriggeredBy);
        }

        private string MergeErrorText(
            string errorTextTemplate,
            ChargeLinksCommand chargeLinksCommand,
            string? triggeredBy)
        {
            var tokens = CimValidationErrorTextTokenMatcher.GetTokens(errorTextTemplate);

            var mergedErrorText = errorTextTemplate;

            foreach (var token in tokens)
            {
                var data = GetDataForToken(token, chargeLinksCommand, triggeredBy);
                mergedErrorText = mergedErrorText.Replace("{{" + token + "}}", data);
            }

            return mergedErrorText;
        }

        private string GetDataForToken(
            CimValidationErrorTextToken token,
            ChargeLinksCommand chargeLinksCommand,
            string? triggeredBy)
        {
            // Please keep sorted by CimValidationErrorTextToken
            return token switch
            {
                CimValidationErrorTextToken.ChargeLinkStartDate =>
                    GetChargeLinkStartDate(chargeLinksCommand, triggeredBy),
                CimValidationErrorTextToken.DocumentSenderProvidedChargeId =>
                    GetDocumentSenderProvidedChargeId(chargeLinksCommand, triggeredBy),
                CimValidationErrorTextToken.MeteringPointId =>
                    chargeLinksCommand.MeteringPointId,
                _ => string.Empty,
            };
        }

        private string GetChargeLinkStartDate(ChargeLinksCommand chargeLinksCommand, string? triggeredBy)
        {
            try
            {
                return chargeLinksCommand.ChargeLinks
                    .Single(p => p.SenderProvidedChargeId == triggeredBy)
                    .StartDateTime.ToString();
            }
            catch (Exception e)
            {
                return LogErrorAndReturnErrorMessage(triggeredBy, nameof(ChargeLinkDto.StartDateTime), e);
            }
        }

        private string GetDocumentSenderProvidedChargeId(ChargeLinksCommand chargeLinksCommand, string? triggeredBy)
        {
            try
            {
                return chargeLinksCommand.ChargeLinks
                    .Single(p => p.SenderProvidedChargeId == triggeredBy)
                    .SenderProvidedChargeId;
            }
            catch (Exception e)
            {
                return LogErrorAndReturnErrorMessage(triggeredBy, nameof(ChargeLinkDto.SenderProvidedChargeId), e);
            }
        }

        private string LogErrorAndReturnErrorMessage(string? triggeredBy, string elementNotFound, Exception e)
        {
            var errorMessage = $"{elementNotFound} not found by operationId: {triggeredBy}";
            _logger.LogError(e, errorMessage);

            return CimValidationErrorTextTemplateMessages.Unknown;
        }
    }
}
