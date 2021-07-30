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
using GreenEnergyHub.Charges.Domain.ChargeLinks;
using GreenEnergyHub.Charges.Infrastructure.Internal.ChargeLinkCommandAccepted;
using GreenEnergyHub.Charges.Infrastructure.Internal.Mappers;
using GreenEnergyHub.Charges.TestCore;
using NodaTime;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Infrastructure.Internal.Mappers
{
    [UnitTest]
    public class LinkCommandAcceptedOutboundMapperTests
    {
        [Theory]
        [InlineAutoMoqData]
        public void Convert_WhenCalled_ShouldMapToProtobufWithCorrectValues(
            [NotNull] ChargeLinkCommandAcceptedEvent chargeLinkCommandAcceptedEvent,
            [NotNull] LinkCommandAcceptedOutboundMapper sut)
        {
            UpdateInstantsToValidTimes(chargeLinkCommandAcceptedEvent);
            var result = (ChargeLinkCommandAcceptedContract)sut.Convert(chargeLinkCommandAcceptedEvent);
            AssertExtensions.ContractIsEquivalent(result, chargeLinkCommandAcceptedEvent);
        }

        [Theory]
        [InlineAutoMoqData]
        public void Convert_WhenCalledWithNull_ShouldThrow([NotNull]LinkCommandAcceptedOutboundMapper sut)
        {
            Assert.Throws<InvalidOperationException>(() => sut.Convert(null!));
        }

        private static void UpdateInstantsToValidTimes([NotNull] ChargeLinkCommand chargeLinkCommand)
        {
            chargeLinkCommand.Document.RequestDate = Instant.FromUtc(2021, 7, 21, 11, 42, 25);
            chargeLinkCommand.Document.CreatedDateTime = Instant.FromUtc(2021, 7, 21, 12, 14, 43);
            chargeLinkCommand.ChargeLink.StartDateTime = Instant.FromUtc(2021, 8, 31, 22, 0);
            chargeLinkCommand.ChargeLink.EndDateTime = Instant.FromUtc(2021, 9, 30, 22, 0);
        }
    }
}
