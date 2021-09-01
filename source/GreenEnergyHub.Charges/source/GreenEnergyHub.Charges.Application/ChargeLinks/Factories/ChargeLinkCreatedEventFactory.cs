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
using GreenEnergyHub.Charges.Core.DateTime;
using GreenEnergyHub.Charges.Domain.ChargeLinks.Events.Integration;
using GreenEnergyHub.Charges.Domain.ChargeLinks.Events.Local;

namespace GreenEnergyHub.Charges.Application.ChargeLinks.Factories
{
    public class ChargeLinkCreatedEventFactory : IChargeLinkCreatedEventFactory
    {
        public ChargeLinkCreatedEvent CreateEvent([NotNull] ChargeLinkCommandAcceptedEvent command)
        {
            return new ChargeLinkCreatedEvent(
                command.ChargeLinkCommand.ChargeLink.Id,
                command.ChargeLinkCommand.ChargeLink.MeteringPointId,
                command.ChargeLinkCommand.ChargeLink.ChargeId,
                command.ChargeLinkCommand.ChargeLink.ChargeType,
                command.ChargeLinkCommand.ChargeLink.ChargeOwner,
                new ChargeLinkPeriod(
                    command.ChargeLinkCommand.ChargeLink.StartDateTime,
                    command.ChargeLinkCommand.ChargeLink.EndDateTime.TimeOrEndDefault(),
                    command.ChargeLinkCommand.ChargeLink.Factor));
        }
    }
}
