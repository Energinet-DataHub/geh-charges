# Publishing Azure functions

As a developer, you can publish functions to Azure using Azure Functions Core Tools.

Please see prerequisites section at the end of this document.

Following Terraform deploy to an Azure Resource Group you can publish functions:

Azure login:

```PowerShell
az login
```

## Azure Function Publish

### Automated

Script to automated Azure function publish is located  [here](../../source/GreenEnergyHub.Charges/publish-functions-from-localhost.cmd)

## Manually

```PowerShell
func azure functionapp publish {Azure function name}
```

Azure function naming: `azfun-[name]-[project]-[organisation]-[environment]`, eg. `azfun-message-receiver-charges-endk-[environment]`

## Prerequisites

Install Azure Functions Core Tools as [documented here](run-integration-tests-local.md)

Create a `local.settings.json` as a copy of `local.settings.sample.json` file for each Azure function to ensure trouble-free publish.
