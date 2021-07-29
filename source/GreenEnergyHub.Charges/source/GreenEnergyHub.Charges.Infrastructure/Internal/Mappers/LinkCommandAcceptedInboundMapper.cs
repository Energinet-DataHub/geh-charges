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
using System.Diagnostics.CodeAnalysis;
using GreenEnergyHub.Charges.Domain.ChangeOfCharges.Transaction;
using GreenEnergyHub.Charges.Domain.ChargeLinks;
using GreenEnergyHub.Charges.Domain.MarketDocument;
using GreenEnergyHub.Charges.Infrastructure.Internal.ChargeLinkCommandAccepted;
using GreenEnergyHub.Messaging.Protobuf;
using GreenEnergyHub.Messaging.Transport;
using NodaTime;

namespace GreenEnergyHub.Charges.Infrastructure.Internal.Mappers
{
    public class LinkCommandAcceptedInboundMapper : ProtobufInboundMapper<ChargeLinkCommandAcceptedContract>
    {
        protected override IInboundMessage Convert([NotNull]ChargeLinkCommandAcceptedContract chargeLinkCommandAcceptedContract)
        {
            return new ChargeLinkCommandAcceptedEvent(chargeLinkCommandAcceptedContract.CorrelationId)
            {
                Document = MapDocument(chargeLinkCommandAcceptedContract.Document),
                ChargeLink = MapChargeLink(chargeLinkCommandAcceptedContract.ChargeLink),
            };
        }

        private static Document MapDocument(DocumentContract document)
        {
            return new Document
            {
                Id = document.Id,
                RequestDate = Instant.FromUnixTimeSeconds(document.RequestDate.Seconds),
                Type = (DocumentType)document.Type,
                CreatedDateTime = Instant.FromUnixTimeSeconds(document.CreatedDateTime.Seconds),
                Sender = MapMarketParticipant(document.Sender),
                Recipient = MapMarketParticipant(document.Recipient),
                IndustryClassification = (IndustryClassification)document.IndustryClassification,
                BusinessReasonCode = (BusinessReasonCode)document.BusinessReasonCode,
            };
        }

        private static MarketParticipant MapMarketParticipant(MarketParticipantContract marketParticipant)
        {
            return new MarketParticipant
            {
                Id = marketParticipant.Id,
                BusinessProcessRole = (MarketParticipantRole)marketParticipant.BusinessProcessRole,
            };
        }

        private static ChargeLink MapChargeLink(ChargeLinkContract link)
        {
            return new ChargeLink
            {
                Id = link.Id,
                MeteringPointId = link.MeteringPointId,
                StartDateTime = Instant.FromUnixTimeSeconds(link.StartDateTime.Seconds),
                EndDateTime = Instant.FromUnixTimeSeconds(link.EndDateTime.Seconds),
                ChargeId = link.ChargeId,
                Factor = link.Factor,
                ChargeOwner = link.ChargeOwner,
                ChargeType = (ChargeType)link.ChargeType,
            };
        }
    }
}
