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

using GreenEnergyHub.Charges.Domain.Charges;

namespace GreenEnergyHub.Charges.Infrastructure.ChargeBundle.Cim
{
    public static class ResolutionMapper
    {
        private const string CimPT15M = "PT15M";
        private const string CimPT1H = "PT1H";
        private const string CimP1D = "P1D";
        private const string CimP1M = "P1M";

        public static Resolution Map(string value)
        {
            return value switch
            {
                CimPT15M => Resolution.PT15M,
                CimPT1H => Resolution.PT1H,
                CimP1D => Resolution.P1D,
                CimP1M => Resolution.P1M,
                _ => Resolution.Unknown,
            };
        }
    }
}
