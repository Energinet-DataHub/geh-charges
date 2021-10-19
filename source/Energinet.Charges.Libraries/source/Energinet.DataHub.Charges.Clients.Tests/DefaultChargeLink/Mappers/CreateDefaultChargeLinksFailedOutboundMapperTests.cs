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
using AutoFixture;
using Energinet.DataHub.Charges.Libraries.Models;
using Energinet.DataHub.Charges.Libraries.Protobuf;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace Energinet.DataHub.Charges.Clients.CreateDefaultChargeLink.Tests.DefaultChargeLink.Mappers
{
    [UnitTest]
    public class CreateDefaultChargeLinksFailedOutboundMapperTests
    {
        private readonly Fixture _fixture;

        public CreateDefaultChargeLinksFailedOutboundMapperTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void Convert_WhenCalled_ShouldMapToProtobufWithCorrectValues()
        {
            // Arrange
            var createDefaultChargeLinksFailedDto = _fixture.Create<CreateDefaultChargeLinksFailedDto>();

            // Act
            var actual = CreateDefaultChargeLinksFailedOutboundMapper.Convert(createDefaultChargeLinksFailedDto);

            // Assert
            actual.MeteringPointId.Should().Be(createDefaultChargeLinksFailedDto.MeteringPointId);
            ((int)actual.ErrorCode).Should().Be((int)createDefaultChargeLinksFailedDto.ErrorCode);
        }

        [Fact]
        public void Convert_WhenCalledWithNull_ShouldThrow()
        {
            Assert.Throws<NullReferenceException>(() => CreateDefaultChargeLinksFailedOutboundMapper.Convert(null!));
        }
    }
}
