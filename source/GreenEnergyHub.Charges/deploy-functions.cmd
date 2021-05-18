@echo off

echo =======================================================================================
echo Deploy Azure functions to your private sandbox
echo .
echo Assuming: Domain=charges, Environment=s
echo *** Make sure that you created all local.settings.json settings files ***
echo *** Deployments are executed in separate windows in order to execute in parallel ***
echo =======================================================================================

setlocal

set /p project=Enter organization used with Terraform (perhaps your initials?): 
set /p doBuild=Build solution ([y]/n)?
set /p deployMessageReceiver=Deploy message receiver ([y]/n)?
set /p deployCommandReceiver=Deploy command receiver ([y]/n)?
set /p deployConfirmationSender=Deploy confirmation sender ([y]/n)?
set /p deployRejectionSender=Deploy rejection sender ([y]/n)?

IF /I not "%doBuild%" == "n" (
    rem Clean is necessary if e.g. a function project name has changed because otherwise both assemblies will be picked up by deployment
    dotnet clean GreenEnergyHub.Charges.sln -c Release
    dotnet build GreenEnergyHub.Charges.sln -c Release
)

rem All (but the last) deployments are opened in separate windows in order to execute in parallel

IF /I not "%deployMessageReceiver%" == "n" (
    pushd source\GreenEnergyHub.Charges.MessageReceiver\bin\Release\netcoreapp3.1
    start "Deploy: Message Receiver" cmd /c "func azure functionapp publish azfun-message-receiver-charges-%project%-s & pause"
    popd
)

IF /I not "%deployCommandReceiver%" == "n" (
    pushd source\GreenEnergyHub.Charges.ChargeCommandReceiver\bin\Release\netcoreapp3.1
    start "Deploy: Charge Command Receiver" cmd /c "func azure functionapp publish azfun-charge-command-receiver-charges-%project%-s & pause"
    popd
)

IF /I not "%deployConfirmationSender%" == "n" (
    pushd source\GreenEnergyHub.Charges.ChargeConfirmationSender\bin\Release\netcoreapp3.1
    start "Deploy: Charge Confirmation Sender" cmd /c "func azure functionapp publish azfun-charge-confirmation-sender-charges-%project%-s & pause"
    popd
)

IF /I not "%deployRejectionSender%" == "n" (
    pushd source\GreenEnergyHub.Charges.ChargeRejectionSender\bin\Release\netcoreapp3.1
	echo Deploy: Charge Rejection Sender
    func azure functionapp publish azfun-charge-rejection-sender-charges-%project%-s
	popd
)

endlocal
