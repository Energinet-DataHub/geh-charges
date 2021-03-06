rem Utility script to set up full private Azure environment directly from localhost

rem Deploy infrastructure using TerraForm
pushd build\infrastructure
call deploy-infrastructure-from-localhost.cmd
popd

rem Build and deploy Azure Functions and update database
pushd source\GreenEnergyHub.Charges
call publish-functions-from-localhost.cmd
popd
