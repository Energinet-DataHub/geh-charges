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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using GreenEnergyHub.Charges.Application;
using GreenEnergyHub.Charges.Application.ChargeLinks.Factories;
using GreenEnergyHub.Charges.Application.ChargeLinks.Handlers;
using GreenEnergyHub.Charges.Application.Charges.Repositories;
using GreenEnergyHub.Charges.Core.DateTime;
using GreenEnergyHub.Charges.Domain.ChargeLinks.Command;
using GreenEnergyHub.Charges.Domain.ChargeLinks.Events.Integration;
using GreenEnergyHub.Charges.Domain.ChargeLinks.Events.Local;
using GreenEnergyHub.Charges.Domain.Charges;
using GreenEnergyHub.Charges.Domain.MeteringPoints;
using GreenEnergyHub.TestHelpers;
using Moq;
using NodaTime;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Application.ChargeLinks
{
    [UnitTest]
    public class CreateLinkCommandEventHandlerTests
    {
        [Theory]
        [InlineAutoDomainData]
        public async Task HandleAsync_WhenCalled_UsesFactoryToCreateEventAndDispatchesIt(
            [Frozen] [NotNull] Mock<IDefaultChargeLinkRepository> defaultChargeLinkRepository,
            [Frozen] [NotNull] Mock<IChargeLinkCommandFactory> chargeLinkCommandFactory,
            [Frozen] [NotNull] Mock<IMessageDispatcher<ChargeLinkCommandReceivedEvent>> dispatcher,
            [NotNull] string correlationId,
            [NotNull] ChargeLinkCommand chargeLinkCommand,
            [NotNull] CreateLinkCommandEventHandler sut)
        {
            // Arrange
            chargeLinkCommand.ChargeLink.EndDateTime = null;
            var createLinkCommandEvent = new CreateLinkCommandEvent(
                correlationId,
                MeteringPointType.Consumption,
                SystemClock.Instance.GetCurrentInstant());

            var defaultChargeLink = new DefaultChargeLink(
                SystemClock.Instance.GetCurrentInstant(),
                ((Instant?)null).TimeOrEndDefault(),
                1,
                MeteringPointType.Consumption);

            defaultChargeLinkRepository.Setup(
                    f => f.GetAsync(
                        It.IsAny<MeteringPointType>()))
                .ReturnsAsync(new List<DefaultChargeLink> { defaultChargeLink });

            chargeLinkCommandFactory.Setup(
                    f => f.CreateAsync(
                        createLinkCommandEvent,
                        defaultChargeLink,
                        correlationId))
                .ReturnsAsync(chargeLinkCommand);

            // Act
            await sut.HandleAsync(createLinkCommandEvent, correlationId).ConfigureAwait(false);

            // Assert
            dispatcher.Verify(
                x => x.DispatchAsync(
                    It.IsAny<ChargeLinkCommandReceivedEvent>(),
                    It.IsAny<CancellationToken>()));
        }
    }
}
