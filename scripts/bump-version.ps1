param(
    [ValidateSet('major', 'minor', 'patch')]
    [string]$Part = 'patch'
)

$root = Split-Path -Parent $PSScriptRoot
$versionFile = Join-Path $root 'VERSION'

$current = (Get-Content $versionFile -Raw).Trim()
$segments = $current.Split('.')
[int]$major, [int]$minor, [int]$patch = $segments[0], $segments[1], $segments[2]

switch ($Part) {
    'major' { $major++; $minor = 0; $patch = 0 }
    'minor' { $minor++; $patch = 0 }
    'patch' { $patch++ }
}

$next = "$major.$minor.$patch"
Set-Content -Path $versionFile -Value $next -NoNewline
Add-Content -Path $versionFile -Value ""

Write-Output "Version bumped: $current -> $next"
