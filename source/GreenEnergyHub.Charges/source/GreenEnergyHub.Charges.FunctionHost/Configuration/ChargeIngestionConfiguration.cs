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

using GreenEnergyHub.Charges.Application.Charges.Handlers;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommandReceivedEvents;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands;
using GreenEnergyHub.Charges.FunctionHost.Common;
using GreenEnergyHub.Charges.Infrastructure.ChargeBundle.Cim;
using GreenEnergyHub.Charges.Infrastructure.Internal.ChargeCommandReceived;
using GreenEnergyHub.Charges.Infrastructure.Messaging;
using GreenEnergyHub.Charges.Infrastructure.Messaging.Registration;
using GreenEnergyHub.Charges.Infrastructure.Messaging.Serialization;
using SimpleInjector;

namespace GreenEnergyHub.Charges.FunctionHost.Configuration
{
    internal static class ChargeIngestionConfiguration
    {
        internal static void ConfigureServices(Container container)
        {
            container.Register<ChargeCommandConverter>(Lifestyle.Scoped);
            container.Register<MessageExtractor<ChargeCommand>>(Lifestyle.Scoped);
            container.Register<MessageDeserializer<ChargeCommand>, ChargeCommandDeserializer>(Lifestyle.Scoped);

            container.Register<IChargesMessageHandler, ChargesMessageHandler>(Lifestyle.Scoped);
            container.Register<IChargeCommandHandler, ChargeCommandHandler>(Lifestyle.Scoped);

            container.SendProtobufMessage<ChargeCommandReceivedContract>();
            container.AddMessagingProtobuf().AddMessageDispatcher<ChargeCommandReceivedEvent>(
                    EnvironmentHelper.GetEnv(EnvironmentSettingNames.DomainEventSenderConnectionString),
                    EnvironmentHelper.GetEnv(EnvironmentSettingNames.CommandReceivedTopicName));
        }
    }
}
