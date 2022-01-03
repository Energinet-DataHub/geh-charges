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

using System.Collections.Generic;
using System.Linq;

namespace GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands.Validation
{
    public class ValidationError
    {
        public ValidationError(
            ValidationRuleIdentifier validationRuleIdentifier,
            params ValidationErrorMessageParameter[] validationErrorMessageParameters)
        {
            ValidationRuleIdentifier = validationRuleIdentifier;
            ValidationErrorMessageParameters = validationErrorMessageParameters.ToList();
        }

        /// <summary>
        /// Identifier of the current rule
        /// </summary>
        public ValidationRuleIdentifier ValidationRuleIdentifier { get; }

        /// <summary>
        /// Parameters for errorMessage
        /// </summary>
        public List<ValidationErrorMessageParameter> ValidationErrorMessageParameters { get; }
    }
}