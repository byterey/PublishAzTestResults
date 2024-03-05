#Powershell Gallery
https://www.powershellgallery.com/packages/Import-JUnitToAzTestPlan

Latest release version: `1.1.3`

### Current supported test file
- JUnit

### Execute the Import Command
```r
Import-JUnitToAzTestPlan -Token $(System.AccessToken) -ProjectUrl "https://dev.azure.com/yourorganization/yourproject" -TestPlanID 1 -TestSuiteID 11 -TestConfiguration "Windows 10" -ExecutionReport "path/to/your/junit-results.xml"
```

