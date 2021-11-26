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

namespace Energinet.DataHub.Charges.Clients.Common
{
    /// <summary>
    /// Contains names of settings used by the package.
    /// </summary>
    internal static class EnvironmentSettingNames
    {
        public const string AppInsightsInstrumentationKey = "APPINSIGHTS_INSTRUMENTATIONKEY";
        public const string AzureWebJobsStorage = "AzureWebJobsStorage";
        public const string LocalTimezoneName = "LOCAL_TIMEZONENAME";
        public const string Currency = "CURRENCY";

        public const string IntegrationEventSenderConnectionString = "INTEGRATIONEVENT_SENDER_CONNECTION_STRING";
        public const string IntegrationEventListenerConnectionString = "INTEGRATIONEVENT_LISTENER_CONNECTION_STRING";

        public const string CreateLinkRequestQueueName = "CREATE_LINK_REQUEST_QUEUE_NAME";
        public const string CreateLinkReplyQueueName = "CREATE_LINK_REPLY_QUEUE_NAME";
    }
}