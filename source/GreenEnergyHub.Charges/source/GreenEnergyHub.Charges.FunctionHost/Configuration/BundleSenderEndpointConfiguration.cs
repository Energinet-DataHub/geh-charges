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

using Energinet.DataHub.MessageHub.Model.Peek;
using GreenEnergyHub.Charges.Application.MessageHub;
using GreenEnergyHub.Charges.Domain.AvailableChargeData;
using GreenEnergyHub.Charges.Domain.AvailableChargeLinkReceiptData;
using GreenEnergyHub.Charges.Domain.AvailableChargeLinksData;
using GreenEnergyHub.Charges.Infrastructure.ChargeBundle;
using GreenEnergyHub.Charges.Infrastructure.ChargeBundle.Cim;
using GreenEnergyHub.Charges.Infrastructure.ChargeLinkBundle;
using GreenEnergyHub.Charges.Infrastructure.ChargeLinkBundle.Cim;
using GreenEnergyHub.Charges.Infrastructure.ChargeLinkReceiptBundle;
using GreenEnergyHub.Charges.Infrastructure.ChargeLinkReceiptBundle.Cim;
using GreenEnergyHub.Charges.Infrastructure.Cim;
using GreenEnergyHub.Charges.Infrastructure.MessageHub;
using Microsoft.Extensions.DependencyInjection;

namespace GreenEnergyHub.Charges.FunctionHost.Configuration
{
    internal static class BundleSenderEndpointConfiguration
    {
        internal static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Common for all bundles
            serviceCollection.AddScoped<IBundleCreatorProvider, BundleCreatorProvider>();
            serviceCollection.AddScoped<IBundleSender, BundleSender>();
            serviceCollection.AddScoped<IBundleReplier, BundleReplier>();
            serviceCollection.AddScoped<IRequestBundleParser, RequestBundleParser>();

            // Charge bundles
            serviceCollection.AddScoped<IBundleCreator, BundleCreator<AvailableChargeData>>();
            serviceCollection.AddScoped<ICimSerializer<AvailableChargeData>, ChargeCimSerializer>();

            // Charge link bundles
            serviceCollection.AddScoped<IBundleCreator, ChargeLinkBundleCreator>();
            serviceCollection.AddScoped<ICimSerializer<AvailableChargeLinksData>, ChargeLinkCimSerializer>();
            serviceCollection.AddScoped<IBundleCreator, ChargeLinkConfirmationBundleCreator>();
            serviceCollection.AddScoped<ICimSerializer<AvailableChargeLinkReceiptData>, ChargeLinkReceiptCimSerializer>();
        }
    }
}
