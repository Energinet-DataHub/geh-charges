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

using GreenEnergyHub.Charges.Domain.MeteringPoint;
using GreenEnergyHub.Messaging.MessageTypes.Common;
using NodaTime;

namespace GreenEnergyHub.Charges.Domain.Events.Integration
{
    public class CreateChargeLinksCommandReceivedEvent : InboundIntegrationEvent
    {
        public CreateChargeLinksCommandReceivedEvent(
            string meteringPointId,
            MeteringPointType meteringPointType,
            Instant startDateTime)
            : base(Transaction.NewTransaction())
        {
            MeteringPointId = meteringPointId;
            MeteringPointType = meteringPointType;
            StartDateTime = startDateTime;
        }

        public string MeteringPointId { get; }

        public MeteringPointType MeteringPointType { get; }

        public Instant StartDateTime { get; }
    }
}
