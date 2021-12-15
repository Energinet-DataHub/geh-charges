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

using System.Threading.Tasks;
using Energinet.DataHub.Core.Messaging.Transport.SchemaValidation;
using Energinet.DataHub.Core.Schemas;
using Energinet.DataHub.Core.SchemaValidation;
using Energinet.DataHub.Core.SchemaValidation.Extensions;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeLinksCommands;

namespace GreenEnergyHub.Charges.Infrastructure.ChargeLinkBundle.Cim
{
    public class ChargeLinkCommandDeserializer : SchemaValidatingMessageDeserializer<ChargeLinksCommand>
    {
        private readonly ChargeLinkCommandConverter _chargeLinkCommandConverter;

        public ChargeLinkCommandDeserializer(ChargeLinkCommandConverter chargeLinkCommandConverter)
            : base(Schemas.CimXml.StructureRequestChangeBillingMasterData)
        {
            _chargeLinkCommandConverter = chargeLinkCommandConverter;
        }

        protected override async Task<ChargeLinksCommand> ConvertAsync(SchemaValidatingReader reader)
        {
            var xmlReader = await reader.AsXmlReaderAsync().ConfigureAwait(false);

            var command = await _chargeLinkCommandConverter
                .ConvertAsync(xmlReader)
                .ConfigureAwait(false);

            return (ChargeLinksCommand)command;
        }
    }
}
