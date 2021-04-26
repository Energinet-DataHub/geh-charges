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
using System.Threading.Tasks;
using GreenEnergyHub.Charges.Domain.ChangeOfCharges.Transaction;

namespace GreenEnergyHub.Charges.Application.Validation.InputValidation
{
    /// <summary>
    /// Contract defining the input validator for change of charges messages.
    /// </summary>
    public interface IChangeOfChargeTransactionInputValidator
    {
        /// <summary>
        /// Input validates a <see cref="ChargeCommand"/>.
        /// </summary>
        /// <param name="chargeCommand">The message to validate.</param>
        /// <returns>The validation result.</returns>
        Task<ChargeCommandValidationResult> ValidateAsync([NotNull] ChargeCommand chargeCommand);
    }
}
