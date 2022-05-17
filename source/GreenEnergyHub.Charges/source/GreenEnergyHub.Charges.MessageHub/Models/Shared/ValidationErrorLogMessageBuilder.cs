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
using System.Text;
using GreenEnergyHub.Charges.Domain.Dtos.SharedDtos;
using GreenEnergyHub.Charges.Domain.Dtos.Validation;

namespace GreenEnergyHub.Charges.MessageHub.Models.Shared
{
    public static class ValidationErrorLogMessageBuilder
    {
        public static string BuildErrorMessage(DocumentDto documentDto, IEnumerable<ValidationError> validationErrors)
        {
            var sb = new StringBuilder();
            sb.Append($"document Id {documentDto.Id} with Type {documentDto.Type} from GLN {documentDto.Sender.Id}:\r\n");

            foreach (var validationError in validationErrors)
            {
                sb.Append($"- {nameof(ValidationRuleIdentifier)}: {validationError.ValidationRuleIdentifier}\r\n");
            }

            return sb.ToString();
        }
    }
}