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
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreenEnergyHub.Charges.Infrastructure.MarketParticipantRegistry.Persistence.Actors
{
    public class ActorEntityConfiguration : IEntityTypeConfiguration<Actor>
    {
        public void Configure(EntityTypeBuilder<Actor> builder)
        {
            builder.ToView("Actor");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(a => a.IdentificationNumber);
            builder.Property(a => a.IdentificationType);
            builder.Property(a => a.Active);

            builder
                .Property(a => a.Roles)
                .HasField("_roles")
                .HasColumnType("nvarchar")
                .HasConversion(
                    v => string.Join(",", v.Select(r => ((int)r).ToString())),
                    v => v.Split(',', StringSplitOptions.None).Select(r => Enum.Parse<Role>(r, true)).ToList());
        }
    }
}
