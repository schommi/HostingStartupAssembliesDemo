function InjectFragment($target, $name, $fragment) {
    if($target.$name) {
        # already injected
        return $false
    }

    $target | Add-Member -Name $name -Value (Convertfrom-Json $fragment) -MemberType NoteProperty
    return $true
}

# Copy assembly and .deps.json
# Make sure to build before running this script
Copy-Item ..\..\..\..\DemoHostingStartupAssembly\bin\Debug\net8.0\DemoHostingStartupAssembly.* -Destination . -Force

# Adjust web application .deps.json
$assemblyNameWithVersion = "DemoHostingStartupAssembly/1.0.0"
$depsJsonFile = ".\WebApplication.deps.json"
$json = Get-Content -Path $depsJsonFile | ConvertFrom-Json

$hasInjectedIntoTarget = InjectFragment $json.targets.'.NETCoreApp,Version=v8.0' $assemblyNameWithVersion @"
{
    "runtime": {
	    "DemoHostingStartupAssembly.dll": {
	    }
    }
}
"@

$hasInjectedIntoLibraries = InjectFragment $json.libraries $assemblyNameWithVersion @"
{
    "type": "library",
    "serviceable": false,
    "sha512": ""
}	
"@


if ($hasInjectedIntoTarget -Or $hasInjectedIntoLibraries) {
    Set-Content -Path $depsJsonFile -Value ($json | ConvertTo-Json -Depth 5)
}

# Set environment variable
$env:ASPNETCORE_HOSTINGSTARTUPASSEMBLIES = "DemoHostingStartupAssembly"

# Run application
.\WebApplication.exe
