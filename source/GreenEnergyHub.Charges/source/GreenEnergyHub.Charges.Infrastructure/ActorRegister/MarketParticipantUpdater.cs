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
using GreenEnergyHub.Charges.Domain.MarketParticipants;
using GreenEnergyHub.Charges.Infrastructure.ActorRegister.Persistence.Actors;

namespace GreenEnergyHub.Charges.Infrastructure.ActorRegister
{
    public static class MarketParticipantUpdater
    {
        public static void Update(
            MarketParticipant marketParticipant,
            Actor actor,
            MarketParticipantRole businessProcessRole)
        {
            if (marketParticipant.MarketParticipantId != actor.IdentificationNumber)
            {
                throw new InvalidOperationException(
                    $"Actor with identification number '{actor.IdentificationNumber}'"
                    + ", seems to have an updated identification number, which is not a valid business operation.");
            }

            UpdateIsActive(marketParticipant, actor);
            UpdateRole(marketParticipant, businessProcessRole);
        }

        private static void UpdateIsActive(MarketParticipant marketParticipant, Actor actor)
        {
            if (marketParticipant.IsActive == actor.Active) return;
            marketParticipant.IsActive = actor.Active;
        }

        private static void UpdateRole(MarketParticipant marketParticipant, MarketParticipantRole businessProcessRole)
        {
            if (marketParticipant.BusinessProcessRole == businessProcessRole) return;
            marketParticipant.UpdateBusinessProcessRole(businessProcessRole);
        }
    }
}