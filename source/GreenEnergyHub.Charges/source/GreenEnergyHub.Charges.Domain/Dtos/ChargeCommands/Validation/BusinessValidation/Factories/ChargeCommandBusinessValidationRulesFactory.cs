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
using System.Threading.Tasks;
using GreenEnergyHub.Charges.Core.DateTime;
using GreenEnergyHub.Charges.Domain.Charges;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation.BusinessValidation.ValidationRules;
using GreenEnergyHub.Charges.Domain.Dtos.Validation;
using GreenEnergyHub.Charges.Domain.MarketParticipants;
using NodaTime;

namespace GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation.BusinessValidation.Factories
{
    public class ChargeCommandBusinessValidationRulesFactory : IBusinessValidationRulesFactory<ChargeCommand>
    {
        private readonly IRulesConfigurationRepository _rulesConfigurationRepository;
        private readonly IChargeRepository _chargeRepository;
        private readonly IMarketParticipantRepository _marketParticipantRepository;
        private readonly IZonedDateTimeService _zonedDateTimeService;
        private readonly IClock _clock;

        public ChargeCommandBusinessValidationRulesFactory(
            IRulesConfigurationRepository rulesConfigurationRepository,
            IChargeRepository chargeRepository,
            IMarketParticipantRepository marketParticipantRepository,
            IZonedDateTimeService zonedDateTimeService,
            IClock clock)
        {
            _rulesConfigurationRepository = rulesConfigurationRepository;
            _chargeRepository = chargeRepository;
            _marketParticipantRepository = marketParticipantRepository;
            _zonedDateTimeService = zonedDateTimeService;
            _clock = clock;
        }

        public async Task<IValidationRuleSet> CreateRulesAsync(ChargeCommand chargeCommand)
        {
            if (chargeCommand == null) throw new ArgumentNullException(nameof(chargeCommand));

            var senderId = chargeCommand.Document.Sender.Id;
            var sender = await _marketParticipantRepository.GetOrNullAsync(senderId).ConfigureAwait(false);
            var configuration = await _rulesConfigurationRepository.GetConfigurationAsync().ConfigureAwait(false);

            var existingCharge = await GetChargeOrNullAsync(chargeCommand).ConfigureAwait(false);
            var rules = GetMandatoryRules(chargeCommand, configuration, sender);

            if (existingCharge == null)
                return ValidationRuleSet.FromRules(rules);

            if (chargeCommand.ChargeOperation.Type == ChargeType.Tariff)
                AddTariffOnlyRules(rules, chargeCommand, existingCharge);

            AddUpdateRules(rules, chargeCommand, existingCharge);

            return ValidationRuleSet.FromRules(rules);
        }

        private static void AddTariffOnlyRules(
            List<IValidationRule> rules,
            ChargeCommand command,
            Charge existingCharge)
        {
            rules.Add(new ChangingTariffTaxValueNotAllowedRule(command, existingCharge));
        }

        private List<IValidationRule> GetMandatoryRules(
            ChargeCommand chargeCommand,
            RulesConfiguration configuration,
            MarketParticipant? sender)
        {
            var rules = new List<IValidationRule>
            {
                new StartDateValidationRule(
                    chargeCommand,
                    configuration.StartDateValidationRuleConfiguration,
                    _zonedDateTimeService,
                    _clock),
                new CommandSenderMustBeAnExistingMarketParticipantRule(sender),
            };

            return rules;
        }

        private void AddUpdateRules(List<IValidationRule> rules, ChargeCommand chargeCommand, Charge existingCharge)
        {
            var updateRules = new List<IValidationRule>
            {
                new UpdateChargeMustHaveEffectiveDateBeforeOrOnStopDateRule(existingCharge, chargeCommand),
            };

            rules.AddRange(updateRules);
        }

        private Task<Charge?> GetChargeOrNullAsync(ChargeCommand command)
        {
            var chargeIdentifier = new ChargeIdentifier(
                command.ChargeOperation.ChargeId,
                command.ChargeOperation.ChargeOwner,
                command.ChargeOperation.Type);

            return _chargeRepository.GetOrNullAsync(chargeIdentifier);
        }
    }
}
