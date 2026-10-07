#requires -Version 5.1
$ErrorActionPreference = "Stop"

Write-Host "=== SOFIA Windows bootstrap ==="

function Ensure-WingetPackage {
    param(
        [Parameter(Mandatory=$true)][string]$Id,
        [string]$Name = $Id,
        [switch]$Optional
    )

    try {
        $found = winget list --id $Id --exact --accept-source-agreements 2>$null | Out-String
        if ($LASTEXITCODE -eq 0 -and $found -match [regex]::Escape($Id)) {
            Write-Host "[OK] $Name already installed"
            return
        }

        Write-Host "[INSTALL] $Name"
        winget install --id $Id --exact --silent --accept-package-agreements --accept-source-agreements
        if ($LASTEXITCODE -ne 0) { throw "winget returned $LASTEXITCODE" }
    }
    catch {
        if ($Optional) {
            Write-Warning "Optional package failed: $Name ($Id): $($_.Exception.Message)"
        } else {
            throw
        }
    }
}

if (-not (Get-Command winget -ErrorAction SilentlyContinue)) {
    throw "winget is required. Update/install App Installer from Microsoft Store."
}

New-Item -ItemType Directory -Force C:\Dev | Out-Null

Ensure-WingetPackage -Id "Git.Git" -Name "Git"
Ensure-WingetPackage -Id "GitHub.GitLFS" -Name "Git LFS"
Ensure-WingetPackage -Id "GitHub.cli" -Name "GitHub CLI"
Ensure-WingetPackage -Id "OpenJS.NodeJS.LTS" -Name "Node.js LTS"
Ensure-WingetPackage -Id "Unity.UnityHub" -Name "Unity Hub"
Ensure-WingetPackage -Id "Microsoft.VisualStudioCode" -Name "Visual Studio Code"
Ensure-WingetPackage -Id "Python.Python.3.13" -Name "Python 3.13"
Ensure-WingetPackage -Id "Gyan.FFmpeg" -Name "FFmpeg"
Ensure-WingetPackage -Id "7zip.7zip" -Name "7-Zip"
Ensure-WingetPackage -Id "BlenderFoundation.Blender" -Name "Blender" -Optional
Ensure-WingetPackage -Id "KDE.Krita" -Name "Krita" -Optional
Ensure-WingetPackage -Id "Cockos.REAPER" -Name "REAPER" -Optional

Write-Host ""
Write-Host "Base packages processed."
Write-Host "Close and reopen PowerShell before continuing so PATH is refreshed."
Write-Host "Then follow docs/05_technical/hermes_pc_bootstrap.md"
