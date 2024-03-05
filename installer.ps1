$env:vs_workspace
$sdk_dll = "AzDoTestPlanSDK.dll"
$import_dll = "PublishAzDoTestResults.dll"
$module_name = "Import-JUnitToAzTestPlan"
$NuGetApiKey = "$($env:NuGetApiKey)"
$azdosdk_fullpath = "$($env:vs_workspace)\aztestplan\bin\Debug\netstandard2.0\$($sdk_dll)"
$import_azdo_full_path = "$($env:vs_workspace)\aztestplan\bin\Debug\netstandard2.0\$($import_dll)"

$psm_file = "$($env:vs_workspace)\aztestplan\$($module_name).psm1"
$psd_file = "$($env:vs_workspace)\aztestplan\$($module_name).psd1"


# Destination path (where you want to copy the file)
$destinationPath = "$($env:workspace)\psmodules\Import-JUnitToAzTestPlan"

# Copy the file
Copy-Item -Path $import_azdo_full_path -Destination $destinationPath
Copy-Item -Path $azdosdk_fullpath -Destination $destinationPath
Copy-Item -Path $psm_file -Destination $destinationPath
Copy-Item -Path $psd_file -Destination $destinationPath


#Publish-Module -Path "C:\workspace\psmodules\Import-JUnitToAzTestPlan\" -NuGetApiKey $($env:NuGetApiKey)


