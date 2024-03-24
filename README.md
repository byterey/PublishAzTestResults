# Powershell Gallery
[Powershell gallery: Import-JUnitToAzTestPlan](https://www.powershellgallery.com/packages/Import-JUnitToAzTestPlan)

Latest release version: `1.1.5`

### Current supported test file
- JUnit

### Execute the Import Command
```r
Import-JUnitToAzTestPlan -Token $(System.AccessToken) -ProjectUrl "https://dev.azure.com/yourorganization/yourproject" -TestPlanID 1 -TestSuiteID 11 -TestConfiguration "Windows 10" -ExecutionReport "path/to/your/junit-results.xml"
```

###  Task for Azure Pipeline
```yaml r
- task:using PowerShell@2
    inputs:
      targetType: 'inline'
      script: 'Import-JUnitToAzTestPlan -Token $(System.AccessToken) -ProjectUrl "https://dev.azure.com/yourorganization/yourproject" -TestPlanID 1 -TestSuiteID 11 -TestConfiguration $(TC) -ExecutionReport "path/to/your/junit-results.xml"'
      pwsh: true
```
