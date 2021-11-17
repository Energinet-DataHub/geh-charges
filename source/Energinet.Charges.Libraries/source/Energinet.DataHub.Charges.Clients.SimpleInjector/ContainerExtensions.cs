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
using Azure.Messaging.ServiceBus;
using Energinet.DataHub.Charges.Libraries.DefaultChargeLink;
using Energinet.DataHub.Charges.Libraries.Models;
using Energinet.DataHub.Charges.Libraries.Providers;
using SimpleInjector;

namespace Energinet.DataHub.Charges.Libraries.Clients.SimpleInjector
{
    public static class ContainerExtensions
    {
        /// <summary>
        /// SimpleInjector extension for registering Energinet.DataHub.Charges.Clients NuGet package.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="serviceBusClient">ServiceBusClient required to connect to the shared service bus namespace</param>
        /// <param name="serviceBusRequestSenderConfiguration"></param>
        public static void AddDefaultChargeLinkClient(
            this Container container,
            [DisallowNull] ServiceBusClient serviceBusClient,
            [DisallowNull] IServiceBusRequestSenderConfiguration serviceBusRequestSenderConfiguration)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            if (serviceBusClient == null)
                throw new ArgumentNullException(nameof(serviceBusClient));

            if (serviceBusRequestSenderConfiguration == null)
                throw new ArgumentNullException(nameof(serviceBusRequestSenderConfiguration));

            container.RegisterSingleton<IServiceBusRequestSenderProvider>(() =>
                new ServiceBusRequestSenderProvider(serviceBusClient, serviceBusRequestSenderConfiguration));

            container.Register<IDefaultChargeLinkClient, DefaultChargeLinkClient>(Lifestyle.Scoped);

            container.Register<IDefaultChargeLinkReplyReader, DefaultChargeLinkReplyReader>(Lifestyle.Scoped);
        }
    }
}
