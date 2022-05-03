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

using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using GreenEnergyHub.Charges.Application.Charges.Handlers;
using GreenEnergyHub.Charges.Application.Messaging;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommandAcceptedEvents;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommandReceivedEvents;
using GreenEnergyHub.Charges.Domain.MarketParticipants;
using GreenEnergyHub.TestHelpers;
using Moq;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Application.Charges.Handlers
{
    [UnitTest]
    public class ChargeHandlerTests
    {
        [Theory]
        [InlineAutoDomainData]
        public async Task HandleAsync_WhenBusinessReasonCodeIsUpdateChargePrices_SendConfirmation(
            ChargeCommandReceivedEvent chargeCommandReceivedEvent,
            ChargeCommandAcceptedEvent chargeCommandAcceptedEvent,
            [Frozen] Mock<IMessageDispatcher<ChargeCommandAcceptedEvent>> messageDispatcherMock,
            [Frozen] Mock<IChargeCommandAcceptedEventFactory> chargeCommandAcceptedEventFactoryMock,
            ChargeHandler sut)
        {
            // Arrange
            chargeCommandReceivedEvent.Command.Document.BusinessReasonCode = BusinessReasonCode.UpdateChargePrices;
            chargeCommandAcceptedEventFactoryMock
                .Setup(x => x.CreateEvent(chargeCommandReceivedEvent.Command))
                .Returns(chargeCommandAcceptedEvent);

            // Act
            await sut.HandleAsync(chargeCommandReceivedEvent);

            // Assert
            messageDispatcherMock.Verify(
                x => x.DispatchAsync(
                    It.IsAny<ChargeCommandAcceptedEvent>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [InlineAutoDomainData]
        public async Task HandleAsync_WhenBusinessReasonCodeIsUpdateChargeInformation_ActivateHandler(
            ChargeCommandReceivedEvent chargeCommandReceivedEvent,
            [Frozen] Mock<IChargeCommandReceivedEventHandler> chargeCommandReceivedEventHandlerMock,
            ChargeHandler sut)
        {
            // Arrange
            chargeCommandReceivedEvent.Command.Document.BusinessReasonCode = BusinessReasonCode.UpdateChargeInformation;

            // Act
            await sut.HandleAsync(chargeCommandReceivedEvent);

            // Assert
            chargeCommandReceivedEventHandlerMock.Verify(
                x => x.HandleAsync(
                    chargeCommandReceivedEvent),
                Times.Once);
        }
    }
}