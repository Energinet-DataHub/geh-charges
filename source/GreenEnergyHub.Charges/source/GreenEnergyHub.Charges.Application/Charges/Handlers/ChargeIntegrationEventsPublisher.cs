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
using System.Threading.Tasks;
using GreenEnergyHub.Charges.Application.Charges.Acknowledgement;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommandAcceptedEvents;

namespace GreenEnergyHub.Charges.Application.Charges.Handlers
{
    public class ChargeIntegrationEventsPublisher : IChargeIntegrationEventsPublisher
    {
        private readonly IChargePublisher _chargePublisher;
        private readonly IChargePricesUpdatedPublisher _chargePricesUpdatedPublisher;

        public ChargeIntegrationEventsPublisher(
            IChargePublisher chargePublisher,
            IChargePricesUpdatedPublisher chargePricesUpdatedPublisher)
        {
            _chargePublisher = chargePublisher;
            _chargePricesUpdatedPublisher = chargePricesUpdatedPublisher;
        }

        public async Task PublishAsync(ChargeCommandAcceptedEvent chargeCommandAcceptedEvent)
        {
            if (chargeCommandAcceptedEvent == null) throw new ArgumentNullException(nameof(chargeCommandAcceptedEvent));

            await _chargePublisher
                .PublishChargeCreatedAsync(chargeCommandAcceptedEvent)
                .ConfigureAwait(false);

            if (chargeCommandAcceptedEvent.Command.ChargeOperation.Points.Any())
            {
                await _chargePricesUpdatedPublisher
                    .PublishChargePricesAsync(chargeCommandAcceptedEvent)
                    .ConfigureAwait(false);
            }
        }
    }
}