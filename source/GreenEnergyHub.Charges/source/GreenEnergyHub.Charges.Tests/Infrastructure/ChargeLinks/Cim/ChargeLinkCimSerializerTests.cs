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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using GreenEnergyHub.Charges.Application;
using GreenEnergyHub.Charges.Domain.ChargeLinks;
using GreenEnergyHub.Charges.Domain.Charges;
using GreenEnergyHub.Charges.Domain.MarketParticipants;
using GreenEnergyHub.Charges.Infrastructure.ChargeLinks.Cim;
using GreenEnergyHub.Charges.TestCore;
using GreenEnergyHub.TestHelpers;
using Moq;
using NodaTime;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Infrastructure.ChargeLinks.Cim
{
    [UnitTest]
    public class ChargeLinkCimSerializerTests
    {
        private const int NoOfLinksInBundle = 10;

        [Theory]
        [InlineAutoDomainData]
        public async Task SerializeAsync_WhenCalled_StreamHasSerializedResult(
            [NotNull] [Frozen] Mock<IHubSenderConfiguration> hubSenderConfiguration,
            [NotNull] [Frozen] Mock<IClock> clock,
            [NotNull] ChargeLinkCimSerializer sut)
        {
            // Arrange
            SetupMocks(hubSenderConfiguration, clock);
            await using var stream = new MemoryStream();

            var expected =
                GetExpectedValue("GreenEnergyHub.Charges.Tests.TestFiles.ExpectedOutputChargeLinkCimSerializer.xml");

            var chargeLinks = GetChargeLinks();

            // Act
            await sut.SerializeToStreamAsync(chargeLinks, stream);

            // Assert
            var actual = GetStreamAsStringWithReplacedGuids(stream);

            Assert.Equal(expected, actual);
        }

        [Theory(Skip = "Manually run test to save the generated file to disk")]
        [InlineAutoDomainData]
        public async Task SerializeAsync_WhenCalled_SaveSerializedStream(
            [NotNull] [Frozen] Mock<IHubSenderConfiguration> hubSenderConfiguration,
            [NotNull] [Frozen] Mock<IClock> clock,
            [NotNull] ChargeLinkCimSerializer sut)
        {
            SetupMocks(hubSenderConfiguration, clock);

            var chargeLinks = GetChargeLinks();

            await using var stream = new MemoryStream();

            await sut.SerializeToStreamAsync(chargeLinks, stream);

            await using var fileStream = File.Create("C:\\Temp\\Test" + Guid.NewGuid() + ".txt");

            await stream.CopyToAsync(fileStream);
        }

        private void SetupMocks(
            Mock<IHubSenderConfiguration> hubSenderConfiguration,
            Mock<IClock> clock)
        {
            hubSenderConfiguration.Setup(
                    h => h.GetSenderMarketParticipant())
                .Returns(new MarketParticipant
                {
                    Id = "5790001330552", BusinessProcessRole = MarketParticipantRole.MeteringPointAdministrator,
                });

            var currentTime = Instant.FromUtc(2021, 10, 12, 13, 37, 43).PlusNanoseconds(4);
            clock.Setup(
                    c => c.GetCurrentInstant())
                .Returns(currentTime);
        }

        private List<ChargeLinkTransmissionDto> GetChargeLinks()
        {
            var chargeLinks = new List<ChargeLinkTransmissionDto>();

            for (var i = 1; i <= NoOfLinksInBundle; i++)
            {
                chargeLinks.Add(GetChargeLink(i));
            }

            return chargeLinks;
        }

        private ChargeLinkTransmissionDto GetChargeLink(int no)
        {
            var validTo = no % 2 == 0 ?
                Instant.FromUtc(9999, 12, 31, 23, 59, 59) :
                Instant.FromUtc(2021, 4, 30, 22, 0, 0);

            return new ChargeLinkTransmissionDto(
                "TestRecipient1111",
                MarketParticipantRole.GridAccessProvider,
                BusinessReasonCode.UpdateMasterDataSettlement,
                "Charge" + no,
                ChargeType.Tariff,
                "MeteringPoint" + no,
                "Owner" + no,
                no,
                Instant.FromUtc(2020, 12, 31, 23, 0, 0),
                validTo);
        }

        private static string GetStreamAsStringWithReplacedGuids(Stream stream)
        {
            var text = GetStreamAsString(stream);
            return ReplaceGuids(text);
        }

        private static string GetExpectedValue(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = EmbeddedStreamHelper.GetInputStream(assembly, path);
            return GetStreamAsStringWithReplacedGuids(stream);
        }

        private static string GetStreamAsString(Stream stream)
        {
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private static string ReplaceGuids(string input)
        {
            // The following regex will watch guids regardless of case
            var result = Regex.Replace(
                input,
                @"[0-9A-Fa-f]{8}-([0-9A-Fa-f]{4}-){3}[0-9A-Fa-f]{12}",
                Guid.Empty.ToString());
            return result;
        }
    }
}
