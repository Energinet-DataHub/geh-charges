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
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using GreenEnergyHub.Charges.Domain.AvailableChargeLinkReceiptData;
using GreenEnergyHub.Charges.Domain.AvailableChargeReceiptData;
using GreenEnergyHub.Charges.Domain.AvailableData;
using GreenEnergyHub.Charges.Domain.MarketParticipants;
using GreenEnergyHub.Charges.Infrastructure.ChargeReceiptBundle.Cim;
using GreenEnergyHub.Charges.Infrastructure.Cim;
using GreenEnergyHub.Charges.Infrastructure.Configuration;
using GreenEnergyHub.Charges.TestCore;
using GreenEnergyHub.TestHelpers;
using Moq;
using NodaTime;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Infrastructure.ChargeReceiptBundle.Cim
{
    [UnitTest]
    public class ChargeReceiptCimSerializerTests
    {
        private const int NoOfReceiptsInBundle = 10;
        private const string CimTestId = "00000000000000000000000000000000";
        private const string RecipientId = "TestRecipient2222";

        [Theory]
        [InlineAutoDomainData(ReceiptStatus.Confirmed, "GreenEnergyHub.Charges.Tests.TestFiles.ExpectedOutputChargeReceiptCimSerializerConfirmation.blob")]
        [InlineAutoDomainData(ReceiptStatus.Rejected, "GreenEnergyHub.Charges.Tests.TestFiles.ExpectedOutputChargeReceiptCimSerializerRejection.blob")]
        public async Task SerializeAsync_WhenCalled_StreamHasSerializedResult(
            ReceiptStatus receiptStatus,
            string expectedFilePath,
            [Frozen] Mock<IHubSenderConfiguration> hubSenderConfiguration,
            [Frozen] Mock<IClock> clock,
            [Frozen] Mock<ICimIdProvider> cimIdProvider,
            ChargeReceiptCimSerializer sut)
        {
            // Arrange
            SetupMocks(hubSenderConfiguration, clock, cimIdProvider);
            await using var stream = new MemoryStream();

            var expected = EmbeddedStreamHelper.GetEmbeddedStreamAsString(
                Assembly.GetExecutingAssembly(),
                expectedFilePath);

            var receipts = GetReceipts(receiptStatus, clock.Object);

            // Act
            await sut.SerializeToStreamAsync(
                receipts,
                stream,
                BusinessReasonCode.UpdateMasterDataSettlement,
                RecipientId,
                MarketParticipantRole.GridAccessProvider);

            // Assert
            var actual = stream.AsString();

            Assert.Equal(expected, actual);
        }

        [Theory(Skip = "Manually run test to save the generated file to disk")]
        [InlineAutoDomainData]
        public async Task SerializeAsync_WhenCalled_SaveSerializedStream(
            [Frozen] Mock<IHubSenderConfiguration> hubSenderConfiguration,
            [Frozen] Mock<IClock> clock,
            [Frozen] Mock<ICimIdProvider> cimIdProvider,
            ChargeReceiptCimSerializer sut)
        {
            SetupMocks(hubSenderConfiguration, clock, cimIdProvider);

            var receipts = GetReceipts(ReceiptStatus.Rejected, clock.Object);

            await using var stream = new MemoryStream();

            await sut.SerializeToStreamAsync(
                receipts,
                stream,
                BusinessReasonCode.UpdateMasterDataSettlement,
                RecipientId,
                MarketParticipantRole.GridAccessProvider);

            await using var fileStream = File.Create("C:\\Temp\\TestChargeReceiptBundle" + Guid.NewGuid() + ".xml");

            await stream.CopyToAsync(fileStream);
        }

        private void SetupMocks(
            Mock<IHubSenderConfiguration> hubSenderConfiguration,
            Mock<IClock> clock,
            Mock<ICimIdProvider> cimIdProvider)
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

            cimIdProvider.Setup(
                    c => c.GetUniqueId())
                .Returns(CimTestId);
        }

        private List<AvailableChargeReceiptData> GetReceipts(ReceiptStatus receiptStatus, IClock clock)
        {
            var receipts = new List<AvailableChargeReceiptData>();

            for (var i = 1; i <= NoOfReceiptsInBundle; i++)
            {
                receipts.Add(GetReceipt(i, receiptStatus, clock));
            }

            return receipts;
        }

        private AvailableChargeReceiptData GetReceipt(int no, ReceiptStatus receiptStatus, IClock clock)
        {
            return new AvailableChargeReceiptData(
                RecipientId,
                MarketParticipantRole.GridAccessProvider,
                BusinessReasonCode.UpdateChargeInformation,
                clock.GetCurrentInstant(),
                Guid.NewGuid(),
                receiptStatus,
                "OriginalOperationId" + no,
                GetReasonCodes(no));
        }

        private List<AvailableChargeReceiptDataReasonCode> GetReasonCodes(int no)
        {
            var reasonCodes = new List<AvailableChargeReceiptDataReasonCode>();
            var noOfReasons = (no % 3) + 1;

            for (var i = 1; i <= noOfReasons; i++)
            {
                var text = string.Empty;
                if (i % 2 == 0)
                {
                    text = "Text" + no + "_" + i;
                }

                reasonCodes.Add(new AvailableChargeReceiptDataReasonCode(
                    ReasonCode.IncorrectChargeInformation,
                    text));
            }

            return reasonCodes;
        }
    }
}
