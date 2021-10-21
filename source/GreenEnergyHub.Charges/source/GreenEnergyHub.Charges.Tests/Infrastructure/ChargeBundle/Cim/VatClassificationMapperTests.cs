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

using GreenEnergyHub.Charges.Domain.Charges;
using GreenEnergyHub.Charges.Infrastructure.ChargeBundle.Cim;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.Charges.Tests.Infrastructure.ChargeBundle.Cim
{
    public class VatClassificationMapperTests
    {
        [UnitTest]
        public class ResolutionMapperTests
        {
            [Theory]
            [InlineData("D01", VatClassification.NoVat)]
            [InlineData("D02", VatClassification.Vat25)]
            [InlineData("", VatClassification.Unknown)]
            [InlineData("DoesNotExist", VatClassification.Unknown)]
            [InlineData(null, VatClassification.Unknown)]
            public void Map_WhenGivenInput_MapsToCorrectEnum(string vatClassification, VatClassification expected)
            {
                var actual = VatClassificationMapper.Map(vatClassification);
                Assert.Equal(expected, actual);
            }
        }
    }
}
