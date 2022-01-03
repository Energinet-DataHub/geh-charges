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

using Energinet.DataHub.Core.Messaging.Protobuf;
using Google.Protobuf;
using GreenEnergyHub.Charges.Core.DateTime;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeLinksDataAvailableNotifiedEvents;
using GreenEnergyHub.Charges.Domain.Dtos.DefaultChargeLinksDataAvailableNotifiedEvents;

namespace GreenEnergyHub.Charges.Infrastructure.Contracts.Public.DefaultChargeLinksCreated
{
    public class DefaultChargeLinksCreatedOutboundMapper
        : ProtobufOutboundMapper<ChargeLinksDataAvailableNotifiedEvent>
    {
        protected override IMessage Convert(ChargeLinksDataAvailableNotifiedEvent chargeLinksDataAvailableNotifiedEvent)
        {
            return new
                Infrastructure.Internal.DefaultChargeLinksCreated.DefaultChargeLinksCreated
                {
                    MeteringPointId = chargeLinksDataAvailableNotifiedEvent.MeteringPointId,
                    PublishedTime = chargeLinksDataAvailableNotifiedEvent.PublishedTime.ToTimestamp().TruncateToSeconds(),
                };
        }
    }
}