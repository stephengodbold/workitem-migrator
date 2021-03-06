#requires -version 2.0

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

function Set-LocalRedirect {
    param(
        [Parameter(Mandatory=$true)]
        [string]
        $siteName
    )
    
    $hostsPath = Join-Path $env:WinDir 'System32\drivers\etc\hosts'
    $redirectContent = '127.0.0.1    workitemmigrator #created by set-localenvironment'
    
    $hostsContent = Get-Content $hostsPath
    
    
    if (-not ($hostsContent -contains $redirectContent)) {
        $hostsContent += $redirectContent
        Set-Content -Path $hostsPath -Value $hostsContent
    }
}

$siteName = 'WorkItemMigrator'
$physicalPath = 'C:\dev\Administration\WorkItemMigrator\WorkItemMigrator'

Set-AppPool $siteName
Set-Site $siteName $physicalPath
Set-LocalRedirect $siteName