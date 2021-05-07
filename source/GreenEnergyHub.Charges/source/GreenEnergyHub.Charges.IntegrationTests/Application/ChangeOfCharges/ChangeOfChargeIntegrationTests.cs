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
using System.Text;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using GreenEnergyHub.Charges.Application;
using GreenEnergyHub.Charges.Application.ChangeOfCharges;
using GreenEnergyHub.Charges.ChargeCommandReceiver;
using GreenEnergyHub.Charges.Domain.Events.Local;
using GreenEnergyHub.Charges.Infrastructure.Messaging;
using GreenEnergyHub.Charges.IntegrationTests.TestHelpers;
using GreenEnergyHub.Charges.MessageReceiver;
using GreenEnergyHub.Charges.TestCore;
using GreenEnergyHub.Json;
using GreenEnergyHub.TestHelpers.Traits;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace GreenEnergyHub.Charges.IntegrationTests.Application.ChangeOfCharges
{
    [Trait(TraitNames.Category, TraitValues.IntegrationTest)]
    public class ChangeOfChargesMessageHandlerTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ChargeHttpTrigger _chargeHttpTrigger;
        private readonly ChargeCommandEndpoint _chargeCommandEndpoint;
        private string _subscriptionName;
        private string _commandReceivedTopicName;
        private string _commandAcceptedTopicName;
        private string _commandRejectedTopicName;
        private string _commandReceivedConnectionString;
        private string _commandAcceptedConnectionString;
        private string _commandRejectedConnectionString;

        public ChangeOfChargesMessageHandlerTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            TestConfigurationHelper.ConfigureEnvironmentVariablesFromLocalSettings();
            var messageReceiverHost = TestConfigurationHelper.SetupHost(new MessageReceiver.Startup());
            var chargeCommandReceiverHost = TestConfigurationHelper.SetupHost(new ChargeCommandReceiver.Startup());

            _chargeHttpTrigger = new ChargeHttpTrigger(
                messageReceiverHost.Services.GetRequiredService<IJsonSerializer>(),
                messageReceiverHost.Services.GetRequiredService<IChangeOfChargesMessageHandler>(),
                messageReceiverHost.Services.GetRequiredService<ICorrelationContext>());

            _chargeCommandEndpoint = new ChargeCommandEndpoint(
                chargeCommandReceiverHost.Services.GetRequiredService<IJsonSerializer>(),
                chargeCommandReceiverHost.Services.GetRequiredService<IChargeCommandHandler>(),
                chargeCommandReceiverHost.Services.GetRequiredService<ICorrelationContext>());

            _subscriptionName = Environment.GetEnvironmentVariable("COMMAND_INTEGRATIONTEST_SUBSCRIPTION_NAME") !;
            _commandReceivedTopicName = Environment.GetEnvironmentVariable("COMMAND_RECEIVED_TOPIC_NAME") !;
            _commandAcceptedTopicName = Environment.GetEnvironmentVariable("COMMAND_ACCEPTED_TOPIC_NAME") !;
            _commandRejectedTopicName = Environment.GetEnvironmentVariable("COMMAND_REJECTED_TOPIC_NAME") !;
            _commandReceivedConnectionString = Environment.GetEnvironmentVariable("COMMAND_RECEIVED_LISTENER_CONNECTION_STRING") !;
            _commandAcceptedConnectionString = Environment.GetEnvironmentVariable("COMMAND_ACCEPTED_LISTENER_CONNECTION_STRING") !;
            _commandRejectedConnectionString = Environment.GetEnvironmentVariable("COMMAND_REJECTED_LISTENER_CONNECTION_STRING") !;
        }

        [Theory]
        [InlineAutoMoqData("TestFiles\\ValidChargeAddition.json")]
        [InlineAutoMoqData("TestFiles\\ValidChargeUpdate.json")]
        public async Task Test_ChargeCommand_is_Accepted(
            string testFilePath,
            [NotNull] [Frozen] Mock<ILogger> logger,
            [NotNull] ExecutionContext executionContext,
            [NotNull] IClock clock)
        {
            // arrange
            var req = CreateHttpRequest(testFilePath, clock);

            // act
            var messageReceiverResult = await RunMessageReceiver(logger, executionContext, req).ConfigureAwait(false);
            var commandReceivedMessage = GetMessageFromServiceBus(_commandReceivedConnectionString, _commandReceivedTopicName, _subscriptionName);
            _testOutputHelper.WriteLine($"Message to be handled by ChargeCommandEndpoint: {commandReceivedMessage.Body.Length}");

            await _chargeCommandEndpoint.RunAsync(commandReceivedMessage.Body, logger.Object).ConfigureAwait(false);

            // var commandAcceptedMessage = GetMessageFromServiceBus(commandAcceptedConnectionString, commandAcceptedTopicName, subscriptionName);
            var commandRejectedMessage = GetMessageFromServiceBus(_commandRejectedConnectionString, _commandRejectedTopicName, _subscriptionName);
            _testOutputHelper.WriteLine($"Message accepted by ChargeCommandEndpoint: {commandRejectedMessage.Body.Length}");

            // assert
            Assert.Equal(200, messageReceiverResult!.StatusCode!.Value);
            Assert.Equal(nameof(ChargeCommandReceivedEvent), commandReceivedMessage.Label);
            Assert.Equal(nameof(ChargeCommandRejectedEvent), commandRejectedMessage.Label);
            Assert.True(commandRejectedMessage.Body.Length > 0);
        }

        [Theory]
        [InlineAutoMoqData("TestFiles\\InvalidChargeAddition.json")]
        [InlineAutoMoqData("TestFiles\\InvalidChargeUpdate.json")]
        public async Task Test_ChargeCommand_is_Rejected(
            string testFilePath,
            [NotNull] [Frozen] Mock<ILogger> logger,
            [NotNull] ExecutionContext executionContext,
            [NotNull] IClock clock)
        {
            // arrange
            var req = CreateHttpRequest(testFilePath, clock);

            // act
            var messageReceiverResult = await RunMessageReceiver(logger, executionContext, req).ConfigureAwait(false);
            var commandReceivedMessage = GetMessageFromServiceBus(_commandReceivedConnectionString, _commandReceivedTopicName, _subscriptionName);
            _testOutputHelper.WriteLine($"Message to be handled by ChargeCommandEndpoint: {commandReceivedMessage.Body.Length}");

            await _chargeCommandEndpoint.RunAsync(commandReceivedMessage.Body, logger.Object).ConfigureAwait(false);

            var commandRejectedMessage = GetMessageFromServiceBus(_commandRejectedConnectionString, _commandRejectedTopicName, _subscriptionName);
            _testOutputHelper.WriteLine($"Message accepted by ChargeCommandEndpoint: {commandRejectedMessage.Body.Length}");

            // assert
            Assert.Equal(200, messageReceiverResult!.StatusCode!.Value);
            Assert.Equal(nameof(ChargeCommandReceivedEvent), commandReceivedMessage.Label);
            Assert.Equal(nameof(ChargeCommandRejectedEvent), commandRejectedMessage.Label);
            Assert.True(commandRejectedMessage.Body.Length > 0);
        }

        private async Task<OkObjectResult> RunMessageReceiver(Mock<ILogger> logger, ExecutionContext executionContext, DefaultHttpRequest req)
        {
            return (OkObjectResult)await _chargeHttpTrigger.RunAsync(req, executionContext, logger.Object).ConfigureAwait(false);
        }

        private Message GetMessageFromServiceBus(
            string serviceBusConnectionString,
            string serviceBusTopic,
            string serviceBusSubscription)
        {
            Message receivedMessage = null!;

            var subscriptionClient = GetSubscriptionClient(serviceBusConnectionString, serviceBusTopic, serviceBusSubscription);

            subscriptionClient.RegisterMessageHandler(
                async (message, token) =>
                {
                    var messageJson = Encoding.UTF8.GetString(message.Body);
                    receivedMessage = message;

                    if (messageJson.Length > 0)
                    {
                        _testOutputHelper.WriteLine($"Message received with body: {message.Body.Length}");
                        await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);

                        var inflightMessageHandlerTasksWaitTimeout = new TimeSpan(0, 0, 0, 0, 1);
                        await subscriptionClient.UnregisterMessageHandlerAsync(inflightMessageHandlerTasksWaitTimeout).ConfigureAwait(false);
                    }
                },
                new MessageHandlerOptions(async args =>
                    {
                        await Task.Run(() => _testOutputHelper.WriteLine(args.Exception.ToString())).ConfigureAwait(false);
                    })
                    { MaxConcurrentCalls = 1, AutoComplete = false });

            var count = 0;
            while (receivedMessage == null)
            {
                ++count;
                //_testOutputHelper.WriteLine("still running: " + ++count);
            }

            return receivedMessage;
        }

        private static DefaultHttpRequest CreateHttpRequest(string testFile, IClock clock)
        {
            var stream = TestDataHelper.GetInputStream(testFile, clock);
            var defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Body = stream;
            var req = new DefaultHttpRequest(defaultHttpContext);
            return req;
        }

        private static SubscriptionClient GetSubscriptionClient(
            string serviceBusConnectionString,
            string topicPath,
            string subscriptionName)
        {
            var subscriptionClient = new SubscriptionClient(serviceBusConnectionString, topicPath, subscriptionName);
            return subscriptionClient;
        }
    }
}
