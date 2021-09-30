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
using System.IO;
using System.Text.RegularExpressions;
using GreenEnergyHub.Charges.FunctionHost.Common;
using GreenEnergyHub.FunctionApp.TestCommon;
using GreenEnergyHub.FunctionApp.TestCommon.FunctionAppHost;
using Squadron;
using Squadron.AzureCloud;

namespace GreenEnergyHub.Charges.IntegrationTests.Fixtures
{
    public class ChargesFunctionAppServiceBusOptions : AzureCloudServiceBusOptions
    {
        public const string PostOfficeTopicKey = "postOffice";

        public const string PostOfficeTopicSubscriptionName = "defaultSubscription";

        public override void Configure(ServiceBusOptionsBuilder builder)
        {
            builder.SetConfigResolver(ConfigurationResolver);

            var localSettingsSnapshot = new FunctionAppHostConfigurationBuilder().BuildLocalSettingsConfiguration();
            var domainEventListenerConnectionString = localSettingsSnapshot.GetValue(EnvironmentSettingNames.DomainEventSenderConnectionString);

            // Example value: 'Endpoint=sb://sbn-charges-xdas-s.servicebus.windows.net/;'
            var namespaceMatchPattern = @"Endpoint=sb://(.*?).servicebus.windows.net/";
            var match = Regex.Match(domainEventListenerConnectionString, namespaceMatchPattern, RegexOptions.IgnoreCase);
            var domainEventListenerNamespace = match.Groups[1].Value;

            builder
                .Namespace(domainEventListenerNamespace)
                .AddTopic(PostOfficeTopicKey)
                .AddSubscription(PostOfficeTopicSubscriptionName);
        }

        private AzureResourceConfiguration ConfigurationResolver()
        {
            // TODO: We should probably find another solution.
            ConfigureEnvironmentVariables();

            var secret = Environment.GetEnvironmentVariable("CLIENT_SECRET") ?? string.Empty;
            var clientId = Environment.GetEnvironmentVariable("CLIENT_ID") ?? string.Empty;
            var tenantId = Environment.GetEnvironmentVariable("TENANT_ID") ?? string.Empty;
            var defaultLocation = Environment.GetEnvironmentVariable("DEFAULT_LOCATION") ?? string.Empty;
            var resourceGroup = Environment.GetEnvironmentVariable("RESOURCE_GROUP_NAME") ?? string.Empty;
            var subscriptionId = Environment.GetEnvironmentVariable("SUBSCRIPTION_ID") ?? string.Empty;

            if (string.IsNullOrWhiteSpace(secret)
                || string.IsNullOrWhiteSpace(clientId)
                || string.IsNullOrWhiteSpace(tenantId)
                || string.IsNullOrWhiteSpace(defaultLocation)
                || string.IsNullOrWhiteSpace(resourceGroup)
                || string.IsNullOrWhiteSpace(subscriptionId))
            {
                var errormessage =
                    $"{nameof(ChargesFunctionAppServiceBusOptions)} Missing configuration: " +
                    $"Length of {nameof(secret)}: {secret.Length}," +
                    $"Length of {nameof(clientId)}: {clientId.Length}," +
                    $"Length of {nameof(tenantId)}: {tenantId.Length}," +
                    $"Length of {nameof(subscriptionId)}: {subscriptionId.Length}," +
                    $"Default location: {defaultLocation}, {resourceGroup}";

                throw new InvalidOperationException(errormessage);
            }

            var azureResourceConfiguration = new AzureResourceConfiguration
            {
                Credentials = new AzureCredentials
                {
                    Secret = secret,
                    ClientId = clientId,
                    TenantId = tenantId,
                },
                DefaultLocation = defaultLocation,
                ResourceGroup = resourceGroup,
                SubscriptionId = subscriptionId,
            };

            return azureResourceConfiguration;
        }

        /// <summary>
        /// EnvironmentVariables are not automatically loaded when running xUnit integrationstests.
        /// This method follows the suggested workaround mentioned here:
        /// https://github.com/Azure/azure-functions-host/issues/6953
        /// </summary>
        private static void ConfigureEnvironmentVariables()
        {
            var path = Path.GetDirectoryName(typeof(ChargesFunctionAppServiceBusOptions).Assembly.Location);
            var settingsFile = Path.Join(path, "integrationtest.local.settings.json");
            if (!File.Exists(settingsFile))
                return;

            var json = File.ReadAllText(settingsFile);
            var parsed = Newtonsoft.Json.Linq.JObject.Parse(json).Value<Newtonsoft.Json.Linq.JObject>("Values");

            foreach (var item in parsed!)
            {
                Environment.SetEnvironmentVariable(item.Key, item.Value?.ToString());
            }
        }
    }
}
