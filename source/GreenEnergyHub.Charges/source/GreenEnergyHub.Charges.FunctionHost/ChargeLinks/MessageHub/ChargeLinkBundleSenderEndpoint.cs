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

using System.Threading.Tasks;
using Energinet.DataHub.MessageHub.Client.Peek;
using GreenEnergyHub.Charges.Application.ChargeLinks.MessageHub;
using GreenEnergyHub.Charges.FunctionHost.Common;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace GreenEnergyHub.Charges.FunctionHost.ChargeLinks.MessageHub
{
    /// <summary>
    /// Trigger on request from MessageHub to create a bundle
    /// and create bundle and send response to MessageHub.
    /// This is the RSM-031 CIM XML 'NotifyBillingMasterData'.
    /// </summary>
    public class ChargeLinkBundleSenderEndpoint
    {
        private const string FunctionName = nameof(ChargeLinkBundleSenderEndpoint);
        private readonly IChargeLinkBundleSender _chargeLinkBundleSender;
        private readonly ILogger _log;
        private readonly IRequestBundleParser _requestBundleParser;

        public ChargeLinkBundleSenderEndpoint(
            IChargeLinkBundleSender chargeLinkBundleSender,
            ILoggerFactory loggerFactory,
            IRequestBundleParser requestBundleParser)
        {
            _chargeLinkBundleSender = chargeLinkBundleSender;
            _requestBundleParser = requestBundleParser;
            _log = loggerFactory.CreateLogger(nameof(ChargeLinkBundleSenderEndpoint));
        }

        [Function(FunctionName)]
        public async Task RunAsync(
            [ServiceBusTrigger(
                "%" + EnvironmentSettingNames.MessageHubRequestQueue + "%",
                Connection = EnvironmentSettingNames.DataHubListenerConnectionString,
                IsSessionsEnabled = true)]
            byte[] data,
            FunctionContext functionContext)
        {
            _log.LogInformation("Function {FunctionName} started to process a request with size {Size}", FunctionName, data.Length);

            var request = _requestBundleParser.Parse(data);
            await _chargeLinkBundleSender.SendAsync(request).ConfigureAwait(false);
        }
    }
}
