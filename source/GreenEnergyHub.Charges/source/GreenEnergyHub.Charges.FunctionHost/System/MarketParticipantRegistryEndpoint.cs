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
using System.Net;
using System.Threading.Tasks;
using GreenEnergyHub.Charges.Infrastructure.MarketParticipantRegistry;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace GreenEnergyHub.Charges.FunctionHost.System
{
    public class MarketParticipantRegistryEndpoint
    {
        private readonly IMarketParticipantRegistrySynchronizer _marketParticipantRegistrySynchronizer;

        public MarketParticipantRegistryEndpoint(IMarketParticipantRegistrySynchronizer marketParticipantRegistrySynchronizer)
        {
            _marketParticipantRegistrySynchronizer = marketParticipantRegistrySynchronizer;
        }

        /// <summary>
        /// Endpoint to trigger synchronization of market participant registry into charges domain.
        /// </summary>
        [Function("SynchronizeFromMarketParticipantRegistry")]
        public async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)]
            HttpRequestData req)
        {
            var response = req.CreateResponse();
            string statusMessage;

            try
            {
                await _marketParticipantRegistrySynchronizer.SynchronizeAsync().ConfigureAwait(false);
                statusMessage = "Synchronization from market participant registry succeeded.";
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                statusMessage = e.ToString();
            }

            await response.WriteStringAsync(statusMessage).ConfigureAwait(false);
            return response;
        }
    }
}