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

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using GreenEnergyHub.Charges.Domain.ChargeCommands;
using GreenEnergyHub.Charges.Domain.Charges;
using GreenEnergyHub.TestHelpers;
using GreenEnergyHub.TestHelpers.FluentAssertionsExtensions;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Application.Charges.Factories
{
    [UnitTest]
    public class ChargeCommandFactoryTests
    {
        [Theory]
        [InlineAutoDomainData]
        public async Task CreateFromChargeAsync_ChargeCommand_HasNoNullsOrEmptyCollections(
            [NotNull] Charge charge,
            [NotNull] ChargeCommandFactory sut)
        {
            // Act
            var actual = await sut.CreateFromChargeAsync(charge).ConfigureAwait(false);

            // Assert
            actual.Should().NotContainNullsOrEmptyEnumerables();
        }
    }
}
