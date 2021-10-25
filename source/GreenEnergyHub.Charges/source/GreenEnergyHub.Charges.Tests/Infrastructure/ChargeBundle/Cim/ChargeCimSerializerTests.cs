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
using GreenEnergyHub.Charges.Domain.AvailableChargeData;
using GreenEnergyHub.Charges.Domain.Charges;
using GreenEnergyHub.Charges.Domain.MarketParticipants;
using GreenEnergyHub.Charges.Infrastructure.ChargeBundle.Cim;
using GreenEnergyHub.Charges.Infrastructure.Cim;
using GreenEnergyHub.Charges.Infrastructure.Configuration;
using GreenEnergyHub.Charges.TestCore;
using GreenEnergyHub.Iso8601;
using GreenEnergyHub.TestHelpers;
using Moq;
using NodaTime;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Infrastructure.ChargeBundle.Cim
{
    [UnitTest]
    public class ChargeCimSerializerTests
    {
        private const int NoOfChargesInBundle = 10;
        private const string CimTestId = "00000000000000000000000000000000";

/*        [Theory]
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

            var chargeLinks = GetChargeLinks(clock.Object);

            // Act
            await sut.SerializeToStreamAsync(chargeLinks, stream);

            // Assert
            var text = GetStreamAsStringWithReplacedGuids(stream);
            var actual = RemoveCarriageReturn(text);

            Assert.Equal(expected, actual);
        }*/

        [Theory/*(Skip = "Manually run test to save the generated file to disk")*/]
        [InlineAutoDomainData]
        public async Task SerializeAsync_WhenCalled_SaveSerializedStream(
            [NotNull] [Frozen] Mock<IHubSenderConfiguration> hubSenderConfiguration,
            [NotNull] [Frozen] Mock<IClock> clock,
            [NotNull] [Frozen] Mock<IIso8601Durations> iso8601Durations,
            [NotNull] [Frozen] Mock<ICimIdProvider> cimIdProvider,
            [NotNull] ChargeCimSerializer sut)
        {
            SetupMocks(hubSenderConfiguration, clock, iso8601Durations, cimIdProvider);

            var charges = GetCharges(clock.Object);

            await using var stream = new MemoryStream();

            await sut.SerializeToStreamAsync(charges, stream);

            await using var fileStream = File.Create("C:\\Temp\\TestChargeBundle" + Guid.NewGuid() + ".xml");

            await stream.CopyToAsync(fileStream);
        }

        private void SetupMocks(
            Mock<IHubSenderConfiguration> hubSenderConfiguration,
            Mock<IClock> clock,
            Mock<IIso8601Durations> iso8601Durations,
            Mock<ICimIdProvider> cimIdProvider)
        {
            hubSenderConfiguration.Setup(
                    h => h.GetSenderMarketParticipant())
                .Returns(new MarketParticipant
                {
                    Id = "5790001330552", BusinessProcessRole = MarketParticipantRole.MeteringPointAdministrator,
                });

            var currentTime = Instant.FromUtc(2021, 10, 22, 15, 30, 41).PlusNanoseconds(4);
            clock.Setup(
                    c => c.GetCurrentInstant())
                .Returns(currentTime);

            iso8601Durations.Setup(
                    i => i.GetTimeFixedToDuration(
                        It.IsAny<Instant>(),
                        It.IsAny<string>(),
                        It.IsAny<int>()))
                .Returns(Instant.FromUtc(2100, 3, 31, 22, 0, 0));

            cimIdProvider.Setup(
                    c => c.GetUniqueId())
                .Returns(CimTestId);
        }

        private List<AvailableChargeData> GetCharges(IClock clock)
        {
            var chargeLinks = new List<AvailableChargeData>();

            for (var i = 1; i <= NoOfChargesInBundle; i++)
            {
                chargeLinks.Add(GetCharge(i, clock));
            }

            return chargeLinks;
        }

        private AvailableChargeData GetCharge(int no, IClock clock)
        {
            var validTo = no % 2 == 0 ?
                Instant.FromUtc(9999, 12, 31, 23, 59, 59) :
                Instant.FromUtc(2021, 4, 30, 22, 0, 0);

            return new AvailableChargeData(
                "Recipient" + no,
                MarketParticipantRole.GridAccessProvider,
                BusinessReasonCode.UpdateChargeInformation,
                "ChargeId" + no,
                "Owner" + no,
                GetChargeType(no),
                "Name" + no,
                "Description" + no,
                Instant.FromUtc(2020, 12, 31, 23, 0, 0),
                validTo,
                VatClassification.NoVat,
                true,
                false,
                GetResolution(no),
                GetPoints(GetNoOfPoints(no)),
                clock.GetCurrentInstant(),
                Guid.NewGuid());
        }

        private static ChargeType GetChargeType(int no)
        {
            return (no % 3) switch
            {
                0 => ChargeType.Subscription,
                1 => ChargeType.Fee,
                _ => ChargeType.Tariff,
            };
        }

        private static Resolution GetResolution(int no)
        {
            return (no % 3) switch
            {
                0 => Resolution.P1M,
                1 => Resolution.P1M,
                _ => Resolution.PT1H,
            };
        }

        private static int GetNoOfPoints(int no)
        {
            return (no % 3) switch
            {
                0 => 1,
                1 => 1,
                _ => 24,
            };
        }

        private static List<AvailableChargeDataPoint> GetPoints(int noOfPoints)
        {
            var points = new List<AvailableChargeDataPoint>();

            for (int i = 1; i <= noOfPoints; i++)
            {
                points.Add(new AvailableChargeDataPoint(i, i));
            }

            return points;
        }

        private static string GetStreamAsStringWithReplacedGuids(Stream stream)
        {
            var text = GetStreamAsString(stream);
            return ReplaceGuids(text);
        }

        private static string GetExpectedValue(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var xmlStream = EmbeddedStreamHelper.GetInputStream(assembly, path);
            var text = GetStreamAsStringWithReplacedGuids(xmlStream);
            text = RemoveLicense(text);
            return RemoveCarriageReturn(text);
        }

        private static string GetStreamAsString(Stream stream)
        {
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private static string ReplaceGuids(string input)
        {
            // The following regex will match guids regardless of case
            var result = Regex.Replace(
                input,
                @"[0-9A-Fa-f]{8}-([0-9A-Fa-f]{4}-){3}[0-9A-Fa-f]{12}",
                Guid.Empty.ToString());
            return result;
        }

        private static string RemoveLicense(string input)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = EmbeddedStreamHelper.GetInputStream(assembly, "GreenEnergyHub.Charges.Tests.TestFiles.License.txt");
            var license = GetStreamAsString(stream);
            return input.Replace(license, string.Empty);
        }

        private static string RemoveCarriageReturn(string input)
        {
            return input.Replace("\r", string.Empty);
        }
    }
}
