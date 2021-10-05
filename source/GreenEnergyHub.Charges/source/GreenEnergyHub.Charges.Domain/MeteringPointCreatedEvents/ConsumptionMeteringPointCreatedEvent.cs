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

using GreenEnergyHub.Charges.Domain.Messages.Events;
using GreenEnergyHub.Charges.Domain.MeteringPoints;
using GreenEnergyHub.Messaging.MessageTypes.Common;

namespace GreenEnergyHub.Charges.Domain.MeteringPointCreatedEvents
{
    public class ConsumptionMeteringPointCreatedEvent : InboundIntegrationEvent
    {
        public ConsumptionMeteringPointCreatedEvent(
            string meteringPointId,
            string gsrnNumber,
            string gridAreaCode,
            SettlementMethod settlementMethod,
            NetSettlementGroup netSettlementGroup,
            string effectiveDate)
            : base(Transaction.NewTransaction())
        {
            MeteringPointId = meteringPointId;
            GsrnNumber = gsrnNumber;
            GridAreaCode = gridAreaCode;
            SettlementMethod = settlementMethod;
            NetSettlementGroup = netSettlementGroup;
            EffectiveDate = effectiveDate;
        }

        public string MeteringPointId { get; }

        public string GsrnNumber { get; }

        public string GridAreaCode { get; }

        public SettlementMethod SettlementMethod { get; }

        public NetSettlementGroup NetSettlementGroup { get; }

        public string EffectiveDate { get; }
    }
}
