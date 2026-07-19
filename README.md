# PrintixAutomation
CloudPrintApi,Server Automation, WebAutomation, DesktopAutomation &amp; Android Mobile Automation
Required Installations to run this project: 

1. install visual studio 2022
2. install MSTEST "Test Adapter"  "TestFrameWork" "Microsoft.NET.Test.Sdk"
3. Install coverlet.collector
4. Cautpure workflow for Mobile has to be there "KofaxQATest"
5. Network "Herlev_Dev" should be added to network
6. active printers must be present
7. authentication to sign on with azure otherewise it will not allow you to create workflows.


Documentation:
https://kofaxinc.sharepoint.com/:o:/r/sites/products/controlsuitecommunity/_layouts/15/doc2.aspx?sourcedoc=%7Bf833aa4d-8bbb-4f88-be23-f1cf5b5d7c6d%7D&action=edit&wd=target(Printix%20QA%20Notes.one%7C67be761b-3e41-4698-9bcb-2688f83a131b%2FTesting%20compliance%20for%20PDF%20documents%7Cdc984392-1fdb-c447-8637-62f60f5ed282%2F)&wdorigin=NavigationUrl

CloudPrintApi: https://printix.github.io/

Admin Manual : https://manuals.printix.net/administrator/
User Manual: https://manuals.printix.net/user/

how to run the test ?
setx SESSION_ENV "devenv"         or 
setx SESSION_ENV "testenv"
dotnet test --filter TestCategory=smoke
dotnet test --filter TestCategory="smoke"  --logger "trx;LogFileName=Printix_server_Automation_smoke_results.trx"  --results-directory "Server_Automation\Reports"
dotnet test --filter TestCategory="regression"  --logger "trx;LogFileName=Printix_server_Automation_Regression_results.trx"  --results-directory "Server_Automation\Reports"
dotnet test --filter TestCategory="release"  --logger "trx;LogFileName=Printix_server_Automation_Release_results.trx"  --results-directory "Server_Automation\Reports"
dotnet test --filter TestCategory="cloudprint"  --logger "trx;LogFileName=Printix_Server_Automation_CloudPrintApi_results.trx"  --results-directory "Server_Automation\Reports"