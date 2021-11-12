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
using System.Threading.Tasks;
using Energinet.Charges.Contracts;
using Google.Protobuf;
using GreenEnergyHub.Charges.Application.ChargeLinks.CreateDefaultChargeLinkReplier;
using GreenEnergyHub.Charges.Collective.Models;
using GreenEnergyHub.Charges.Infrastructure.ServiceBusReplySenderProvider;

namespace GreenEnergyHub.Charges.Infrastructure.CreateDefaultChargeLinkReplier
{
    public sealed class CreateDefaultChargeLinksReplier : ICreateDefaultChargeLinksReplier
    {
        private readonly IServiceBusReplySenderProvider _serviceBusReplySenderProvider;

        public CreateDefaultChargeLinksReplier([NotNull] IServiceBusReplySenderProvider serviceBusReplySenderProvider)
        {
            _serviceBusReplySenderProvider = serviceBusReplySenderProvider;
        }

        public async Task ReplyWithSucceededAsync(
            [NotNull] string meteringPointId,
            bool didCreateChargeLinks,
            [NotNull] string replyTo,
            [NotNull] string correlationId)
        {
            ValidateParametersOrThrow(meteringPointId, replyTo, correlationId);

            var createDefaultChargeLinksReplySucceeded = new CreateDefaultChargeLinksReply
            {
                MeteringPointId = meteringPointId,
                CreateDefaultChargeLinksSucceeded =
                    new CreateDefaultChargeLinksReply.Types.CreateDefaultChargeLinksSucceeded
                {
                    DidCreateChargeLinks = didCreateChargeLinks,
                },
            };

            await SendReplyAsync(createDefaultChargeLinksReplySucceeded, replyTo, correlationId);
        }

        public async Task ReplyWithFailedAsync(
            [NotNull] string meteringPointId,
            ErrorCode errorCode,
            [NotNull] string replyTo,
            [NotNull] string correlationId)
        {
            ValidateParametersOrThrow(meteringPointId, replyTo, correlationId);

            var createDefaultChargeLinksReplyFailed = new CreateDefaultChargeLinksReply
            {
                MeteringPointId = meteringPointId,
                CreateDefaultChargeLinksFailed =
                    new CreateDefaultChargeLinksReply.Types.CreateDefaultChargeLinksFailed
                    {
                        ErrorCode =
                            (CreateDefaultChargeLinksReply.Types.CreateDefaultChargeLinksFailed.Types.ErrorCode)errorCode,
                    },
            };

            await SendReplyAsync(createDefaultChargeLinksReplyFailed, replyTo, correlationId);
        }

        private async Task SendReplyAsync(
            CreateDefaultChargeLinksReply createDefaultChargeLinks,
            string replyTo,
            string correlationId)
        {
            var sender = _serviceBusReplySenderProvider.GetInstance(replyTo);

            await sender.SendReplyAsync(createDefaultChargeLinks.ToByteArray(), correlationId)
                .ConfigureAwait(false);
        }

        private static void ValidateParametersOrThrow(string meteringPointId, string replyTo, string correlationId)
        {
            if (meteringPointId == null)
                throw new ArgumentNullException(nameof(meteringPointId));

            if (replyTo == null)
                throw new ArgumentNullException(nameof(replyTo));

            if (correlationId == null)
                throw new ArgumentNullException(nameof(correlationId));
        }
    }
}
