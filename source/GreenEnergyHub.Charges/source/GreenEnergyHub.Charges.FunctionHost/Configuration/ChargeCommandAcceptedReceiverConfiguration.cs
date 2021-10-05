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

using GreenEnergyHub.Charges.Application.Charges.Acknowledgement;
using GreenEnergyHub.Charges.Application.Charges.Factories;
using GreenEnergyHub.Charges.Application.Charges.Handlers;
using GreenEnergyHub.Charges.Domain.ChargeCommandAcceptedEvents;
using GreenEnergyHub.Charges.Infrastructure.Integration.ChargeCreated;
using GreenEnergyHub.Charges.Infrastructure.Internal.ChargeCommandAccepted;
using GreenEnergyHub.Charges.Infrastructure.Messaging;
using GreenEnergyHub.Charges.Infrastructure.Messaging.Registration;
using GreenEnergyHub.Messaging.Protobuf;
using Microsoft.Extensions.DependencyInjection;

namespace GreenEnergyHub.Charges.FunctionHost.Configuration
{
    internal static class ChargeCommandAcceptedReceiverConfiguration
    {
        internal static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.ReceiveProtobufMessage<ChargeCommandAcceptedContract>(
                configuration => configuration.WithParser(() => ChargeCommandAcceptedContract.Parser));

            serviceCollection.AddScoped<MessageExtractor<ChargeCommandAcceptedEvent>>();
            serviceCollection.AddScoped<IChargeCreatedEventFactory, ChargeCreatedEventFactory>();
            serviceCollection.AddScoped<IChargePricesUpdatedEventFactory, ChargePricesUpdatedEventFactory>();
            serviceCollection.AddScoped<IChargePublisher, ChargePublisher>();
            serviceCollection.AddScoped<IChargePricesUpdatedPublisher, ChargePricesUpdatedPublisher>();
            serviceCollection.AddScoped<IChargeCommandAcceptedEventHandler, ChargeCommandAcceptedEventHandler>();

            serviceCollection.SendProtobuf<Infrastructure.Integration.ChargeCreated.ChargeCreated>();
            serviceCollection.AddMessagingProtobuf().AddMessageDispatcher<GreenEnergyHub.Charges.Domain.Charges.Acknowledgements.ChargeCreatedEvent>(
                EnvironmentHelper.GetEnv("INTEGRATIONEVENT_SENDER_CONNECTION_STRING"),
                EnvironmentHelper.GetEnv("CHARGE_CREATED_TOPIC_NAME"));

            serviceCollection.AddMessagingProtobuf().AddMessageDispatcher<ChargePricesUpdatedEvent>(
                EnvironmentHelper.GetEnv("INTEGRATIONEVENT_SENDER_CONNECTION_STRING"),
                EnvironmentHelper.GetEnv("CHARGE_PRICES_UPDATED_TOPIC_NAME"));
        }
    }
}
