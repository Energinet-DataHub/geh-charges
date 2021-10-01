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
using Energinet.DataHub.MeteringPoints.IntegrationEventContracts;
using GreenEnergyHub.Charges.Application.MeteringPoints.Handlers;
using GreenEnergyHub.Charges.Domain.MeteringPointCreatedEvents;
using GreenEnergyHub.Charges.Infrastructure.Correlation;
using GreenEnergyHub.Charges.Infrastructure.Messaging;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace GreenEnergyHub.Charges.FunctionHost.MeteringPoint
{
    public class ConsumptionMeteringPointCreatedReceiverEndpoint
    {
        /// <summary>
        /// The name of the function.
        /// Function name affects the URL and thus possibly dependent infrastructure.
        /// </summary>
        public const string FunctionName = nameof(ConsumptionMeteringPointCreatedReceiverEndpoint);
        private readonly MessageExtractor<ConsumptionMeteringPointCreated> _messageExtractor;
        private readonly IConsumptionMeteringPointCreatedEventHandler _consumptionMeteringPointCreatedEventHandler;
        private readonly ILogger _log;

        public ConsumptionMeteringPointCreatedReceiverEndpoint(
            MessageExtractor<ConsumptionMeteringPointCreated> messageExtractor,
            IConsumptionMeteringPointCreatedEventHandler consumptionMeteringPointCreatedEventHandler,
            [NotNull] ILoggerFactory loggerFactory)
        {
            _messageExtractor = messageExtractor;
            _consumptionMeteringPointCreatedEventHandler = consumptionMeteringPointCreatedEventHandler;
            _log = loggerFactory.CreateLogger(nameof(ConsumptionMeteringPointCreatedReceiverEndpoint));
        }

        [Function(FunctionName)]
        public async Task RunAsync(
            [ServiceBusTrigger(
                "%CONSUMPTION_METERING_POINT_CREATED_TOPIC_NAME%",
                "%CONSUMPTION_METERING_POINT_CREATED_SUBSCRIPTION_NAME%",
                Connection = "INTEGRATIONEVENT_LISTENER_CONNECTION_STRING")]
            [NotNull] byte[] message)
        {
            var meteringPointCreatedEvent = (ConsumptionMeteringPointCreatedEvent)await _messageExtractor.ExtractAsync(message).ConfigureAwait(false);

            await _consumptionMeteringPointCreatedEventHandler.HandleAsync(meteringPointCreatedEvent).ConfigureAwait(false);

            _log.LogInformation("Received metering point created event '{@MeteringPointId}'", meteringPointCreatedEvent.MeteringPointId);
        }
    }
}
