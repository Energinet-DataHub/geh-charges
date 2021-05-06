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
using GreenEnergyHub.Charges.Domain.ChangeOfCharges.Transaction;
using GreenEnergyHub.Charges.Domain.Common;
using MarketParticipant = GreenEnergyHub.Charges.Domain.Common.MarketParticipant;

namespace GreenEnergyHub.Charges.Application
{
    public static class ChargeCommandNullChecker
    {
        public static void ThrowExceptionIfRequiredPropertyIsNull(ChargeCommand chargeCommand)
        {
            if (chargeCommand == null) throw new ArgumentNullException(nameof(chargeCommand));
            CheckCharge(chargeCommand.Charge);
            CheckDocument(chargeCommand.Document);
            CheckEvent(chargeCommand.ChargeEvent);
        }

        private static void CheckCharge(ChargeDto chargeDto)
        {
            if (chargeDto == null) throw new ArgumentNullException(nameof(chargeDto));

            if (string.IsNullOrWhiteSpace(chargeDto.Owner)) throw new ArgumentException(chargeDto.Owner);
            if (string.IsNullOrWhiteSpace(chargeDto.Id)) throw new ArgumentException(chargeDto.Id);
            if (string.IsNullOrWhiteSpace(chargeDto.Name)) throw new ArgumentException(chargeDto.Name);
            if (string.IsNullOrWhiteSpace(chargeDto.Description)) throw new ArgumentException(chargeDto.Description);
        }

        private static void CheckEvent(ChargeEvent chargeEvent)
        {
            if (chargeEvent == null) throw new ArgumentNullException(nameof(chargeEvent));
            if (string.IsNullOrWhiteSpace(chargeEvent.Id)) throw new ArgumentException(chargeEvent.Id);
            if (string.IsNullOrWhiteSpace(chargeEvent.LastUpdatedBy)) throw new ArgumentException(chargeEvent.LastUpdatedBy);
        }

        private static void CheckDocument(Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (string.IsNullOrWhiteSpace(document.Id)) throw new ArgumentException(document.Id);
            if (string.IsNullOrWhiteSpace(document.CorrelationId)) throw new ArgumentException(document.CorrelationId);
            if (string.IsNullOrWhiteSpace(document.Type)) throw new ArgumentException(document.Id);
            CheckMarketDocumentMarketParticipant(document.Recipient);
            CheckMarketDocumentMarketParticipant(document.Sender);
        }

        private static void CheckMarketDocumentMarketParticipant(MarketParticipant marketParticipant)
        {
            if (marketParticipant == null) throw new ArgumentNullException(nameof(marketParticipant));
            if (string.IsNullOrWhiteSpace(marketParticipant.MRid)) throw new ArgumentException(marketParticipant.MRid);
        }
    }
}
