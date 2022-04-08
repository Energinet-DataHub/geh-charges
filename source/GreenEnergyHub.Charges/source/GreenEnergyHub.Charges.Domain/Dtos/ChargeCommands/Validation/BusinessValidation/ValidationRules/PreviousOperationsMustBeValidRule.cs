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

using GreenEnergyHub.Charges.Domain.Dtos.Validation;

namespace GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation.BusinessValidation.ValidationRules
{
    public class PreviousOperationsMustBeValidRule : IValidationRuleWithExtendedData
    {
        private readonly ChargeOperationDto _chargeOperationDto;

        public PreviousOperationsMustBeValidRule(string triggeredBy, ChargeOperationDto chargeOperationDto)
        {
            _chargeOperationDto = chargeOperationDto;
            TriggeredBy = triggeredBy;
        }

        public bool IsValid => string.IsNullOrEmpty(TriggeredBy);

        public ValidationRuleIdentifier ValidationRuleIdentifier =>
            ValidationRuleIdentifier.SubsequentBundleOperationsFail;

        public string OperationId => _chargeOperationDto.Id;

        /// <summary>
        /// This property will tell which previous failed operation that triggered this rule,
        /// it contains the Id of the previous failed operation.
        /// </summary>
        public string TriggeredBy { get; }
    }
}