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
using System.Threading.Tasks;
using GreenEnergyHub.Charges.Domain.ChargeLinks;
using GreenEnergyHub.Charges.Infrastructure.Context;

namespace GreenEnergyHub.Charges.Infrastructure.Repositories
{
    public class ChargeLinkRepository : IChargeLinkRepository
    {
        private readonly IChargesDatabaseContext _context;

        public ChargeLinkRepository(IChargesDatabaseContext context)
        {
            _context = context;
        }

        public async Task StoreAsync(IReadOnlyCollection<ChargeLink> chargeLink)
        {
            await _context.ChargeLinks.AddRangeAsync(chargeLink);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}