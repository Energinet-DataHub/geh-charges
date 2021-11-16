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
using AutoFixture.Xunit2;
using GreenEnergyHub.Charges.Application;
using GreenEnergyHub.Charges.Application.ChargeLinks.CreateDefaultChargeLinkReplier;
using GreenEnergyHub.Charges.Application.ChargeLinks.Handlers;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeLinkCommandAcceptedEvents;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeLinkCommands;
using GreenEnergyHub.Charges.Domain.Dtos.DefaultChargeLinksDataAvailableNotifiedEvents;
using GreenEnergyHub.TestHelpers;
using Moq;
using NodaTime;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Application.ChargeLinks.Handlers
{
    [UnitTest]
    public class ChargeLinkEventReplyHandlerTests
    {
        private const string MeteringPointId = "first";

        [Theory]
        [InlineAutoDomainData]
        public async Task HandleAsync_WhenCalledWithReplyToSetInMessageMetaDataContext_ReplyWithDefaultChargeLinkSucceededDto(
            [Frozen] Mock<IMessageMetaDataContext> messageMetaDataContext,
            [Frozen] Mock<ICorrelationContext> correlationContext,
            [Frozen] Mock<ICreateDefaultChargeLinksReplier> defaultChargeLinkClient,
            string replyTo,
            string correlationId,
            CreateDefaultChargeLinksReplierHandler sut)
        {
            // Arrange
            messageMetaDataContext.Setup(m => m.IsReplyToSet()).Returns(true);
            messageMetaDataContext.Setup(m => m.ReplyTo).Returns(replyTo);
            correlationContext.Setup(c => c.Id).Returns(correlationId);

            var command = new DefaultChargeLinksDataAvailableNotifierEvent(SystemClock.Instance.GetCurrentInstant(), MeteringPointId);

            // Act
            await sut.HandleAsync(command).ConfigureAwait(false);

            // Assert
            defaultChargeLinkClient.Verify(
                x => x.ReplyWithSucceededAsync(MeteringPointId, true, replyTo));
        }
    }
}
