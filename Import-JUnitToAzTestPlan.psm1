<#
 .Synopsis
  Published the testrun results into Azure DevOps TestPlan.

 .Description
  Published the testrun results into Azure DevOps TestPlan.

 .Parameter Token
  The personal access token (PAT) from devops project

 .Parameter AzDevOpsProjectUrl
  The azure devops project url. example: https://dev.azure.com/{organization}/{project}

 .Parameter TestPlan
  The Testplan ID where the test result will be published

 .Parameter TestSuite
  The TestSuite ID where the test result will be published

 .Parameter ExecutionReport
  The JUnit execution report (TEST-*.xml)

 .Parameter TestConfiguration
  The Testconfiguration on which the test result will be map in the Test Suite.

 .Example
   # Description: This command imports JUnit test results into Azure Test Plans. Replace `<PAT>` with your Personal Access Token (PAT), `{organization}` and `{project}` with your Azure DevOps organization and project names respectively, `<TestPlan ID>` and `<Testsuite ID>` with the respective IDs of your test plan and test suite, `"Windows 10"` with the desired test configuration, and `"TEST-example.xml"` with the filename of the JUnit XML test report you wish to import.
   Import-JUnitToAzTestPlan -Token <PAT> -AzDevOpsProjectUrl "https://dev.azure.com/{organization}/{project}" -TestPlanID <TestPlan ID> -testSuite <Testsuite ID> -TestConfiguration "Windows 10" -ExecutionReport "TEST-example.xml"

 .Example
   # Description: This command using parameter alias
   Import=JUnitToAzTestPlan -T <PAT> -Url "https://dev.azure.com/{organization}/{project}" -TP <TestPlan ID> -TS <Testsuite ID> -TS "Windows 10" -E "TEST-example.xml"

 .Example
    # Description
   Import-JUnitToAzTestPlan -Token $Token -ProjectUrl "https://dev.azure.com/{organization}/{project}" -TestPlanID 1 -TestSuiteID 2 -TestConfiguration "Windows 10" -ExecutionReport ".\TEST-example.xml"
 #>
 #>
 #>
 #>
 #>
#>
# Function to run the test
function Import-JUnitToAzTestPlan {
    param (
        [Parameter(Mandatory=$true)]
        [Alias("T")]
        [string] $Token,
        [Parameter(Mandatory=$true)]
        [Alias("Url")]
        [string] $ProjectUrl,
        [Parameter(Mandatory=$true)]
        [Alias("TP")]
        [int] $TestPlanID,
        [Parameter(Mandatory=$true)]
        [Alias("TS")]
        [int] $TestSuiteID,
        [Parameter(Mandatory=$true)]
        [Alias("E")]
        [string] $ExecutionReport,
        [Parameter(Mandatory=$true)]
        [Alias("TC")]
        [string] $TestConfiguration
    )

    $TestPlanObj = Get-AzDoTestPlan -Token $B64Pat -Uri $ProjectUrl -TestPlanID $TestPlanID -TestSuiteID $TestSuiteID

    Import-AzDoTestRunJUnit -FilePath $ExecutionReport -Uri $ProjectUrl -Token $Token -TestPlan $TestPlanObj -TestConfiguration $TestConfiguration
}