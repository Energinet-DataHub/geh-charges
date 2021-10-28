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
/*
using System;
using EntityFrameworkCore.SqlServer.NodaTime.Extensions;
using GreenEnergyHub.Charges.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GreenEnergyHub.Charges.IntegrationTests.TestHelpers
{
    public class DbContextRegistrator
    {
        public DbContextRegistrator()
        {
            FunctionHostConfigurationHelper.ConfigureEnvironmentVariables();
            var connectionString = Environment.GetEnvironmentVariable(EnvironmentSettingNames.ChargeDbConnectionString) ?? string.Empty;

            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddDbContext<ChargesDatabaseContext>(
                    options => options.UseSqlServer(connectionString!, options => options.UseNodaTime()),
                    ServiceLifetime.Transient);

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public ServiceProvider ServiceProvider { get; private set; }
    }
}
*/
