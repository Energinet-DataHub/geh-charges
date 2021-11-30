# Energinet.DataHub.Charges.Clients Release notes

## Version 1.0.14

Updated NuGet dependencies and removed deprecated FxCopAnalyzers

## Version 1.0.13

Removed `IDefaultChargeLinkReplyReader`

## Version 1.0.12

Updated a number of NuGet dependencies.

## Version 1.0.11

Updated the documentation to refer to `Energinet.DataHub.Charges.Clients.SimpleInjector` NuGet package for registration purposes.

Namespaces adjusted to reflect NuGet package id: `Energinet.DataHub.Charges.Clients`

## Version 1.0.10

Integration tests re-enabled in Energinet GitHub pipeline.

## Version 1.0.9

Protobuf contracts common for both Energinet.Charges.Libraries and elsewhere in the Charges domain are now located in one common place to avoid duplication.

All references to Charges.Clients from elsewhere in Charges is removed.

## Version 1.0.8

Missing release notes added

## Version 1.0.7

Introduced `DefaultChargeLinkClientServiceBusRequestSenderProvider` and `DefaultChargeLinkMessagesClientServiceBusRequestSenderProvider`
which now are used when registering their respective dependent client.

## Version 1.0.6

Fixes links to documentation and release notes on [nuget.org](https://www.nuget.org/packages/Energinet.DataHub.Charges.Clients/).

Improves library documentation.

## Version 1.0.5

Renames `IDefaultChargeLinkMessagesRequestClient` to `IDefaultChargeLinkMessagesClient`.

Updates dependency package versions.

## Version 1.0.4

The failed and succeeded methods included in public interface.

## Version 1.0.3

Add functionality to reply when requested to create messages.

## Version 1.0.2

Provide ReplyTo queuename as Service Bus message ApplicationProperty.

## Version 1.0.1

`ReadAsync(byte[] data)` with delegate functionality.

The `ReadAsync(byte[] data)` in conjunction with the `OnSuccess` and `OnFailure` delegates provides the functionality
to deserialize Protobuf data and the flexibility to handle the received data as desired by the receiving domain.

## Version 1.0.0

Initial package release.