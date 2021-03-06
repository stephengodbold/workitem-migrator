#requires -version 2.0
param(
    [Parameter(Mandatory=$true)]
    [string]
    $siteName,
    
    [Parameter(Mandatory=$true)]
    [string]
    $physicalPath,
    
    [Parameter(Mandatory=$true)]
    [string]
    $sourcePath
)


$script:ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
Import-Module WebAdministration

function Set-AppPool {
    param(
        [Parameter(Mandatory=$true)]
        $siteName
    )
    
    $iisPath =  Join-Path "IIS:\AppPools\" $siteName
    
    if (-not (Test-Path $iisPath)) {
        New-WebAppPool $siteName
    
        #set credentials
        $credential = Get-Credential
        $specificUserValue = 3
        
        Set-ItemProperty $iisPath -name processModel.username -value $credential.username
        Set-ItemProperty $iisPath -name processModel.password -value $credential.GetNetworkCredential().Password
        Set-ItemProperty $iisPath -name processModel.identityType -value $specificUserValue
    }
    
    Set-ItemProperty $iisPath -Name managedRuntimeVersion -value v4.0
    Set-ItemProperty $iisPath -Name enable32BitAppOnWin64 -value $true
}

function Set-Site {
    
    param(
        [Parameter(Mandatory=$true)]
        [string]
        $siteName,
        
        [Parameter(Mandatory=$true)]
        [string]
        $physicalPath
    )

    $iisPath =  Join-Path "IIS:\Sites\" $siteName
    
    if (Test-Path($iisPath)) {
        Write-Warning 'Site already exists. Please verify configuration manually'
        return
    }
    
    New-Website `
        -Name $siteName `
        -ApplicationPool $siteName `
        -PhysicalPath $physicalPath `
        -HostHeader $siteName
}

function Set-AppOnline {
    param(
        [Parameter(Mandatory=$true)]
        [string]
        $targetPath
    )
    
    $offlinePagePath = Join-Path $targetPath 'appOffline.html'
    Remove-Item $offlinePagePath
}

function Set-SiteContent {
    param( 
        [Parameter(Mandatory=$true)]
        [string]
        $sourcePath,
        
        [Parameter(Mandatory=$true)]
        [string]
        $targetPath
    )
   
    $sourceWildcard = Join-Path $sourcePath '*'
    $targetWildcard = Join-Path $targetPath '*'
    
    Remove-Item $targetWildcard -Exclude 'appOffline.html' -Recurse
    Copy-Item  $sourceWildcard $targetPath -Recurse
}

function Add-Extensions {
    param(
        [Parameter(Mandatory=$true)]
        [string]
        $sourcePath,
        
        [Parameter(Mandatory=$true)]
        [string]
        $targetPath
    )
    
    $baseTypePath = Join-Path $sourcePath 'WorkItemMigrator.Migration.dll'
    $extensionItemsPattern = Join-Path $sourcePath '*.Migration.dll'
    $extensionsPath = Join-Path $targetPath 'Extensions'
    
    if (-not(Test-Path $extensionsPath)) {
        New-Item $extensionsPath -ItemType Directory
    }
    
    Copy-Item $extensionItemsPattern $extensionsPath -Exclude $baseTypePath
}

Set-AppPool $siteName
Set-Site $siteName $physicalPath
Set-SiteContent $sourcePath $physicalPath
Set-AppOnline $physicalPath
Add-Extensions $sourcePath $physicalPath