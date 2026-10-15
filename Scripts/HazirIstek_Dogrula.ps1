$ErrorActionPreference = 'Stop'
$taskTestDir = Join-Path $env:TEMP ('AsyModbus_HazirIstek_' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $taskTestDir | Out-Null
Copy-Item -LiteralPath (Join-Path $PSScriptRoot '../BusinessLayer/bin/Debug/BusinessLayer.dll') -Destination $taskTestDir
& 'C:/Windows/Microsoft.NET/Framework64/v4.0.30319/csc.exe' /nologo /out:"$taskTestDir/HazirIstekTest.exe" /reference:"$taskTestDir/BusinessLayer.dll" /reference:System.Data.dll (Join-Path $PSScriptRoot 'HazirIstek_Dogrula.cs')
if ($LASTEXITCODE -ne 0) { throw 'Test derlenemedi.' }
& (Join-Path $taskTestDir 'HazirIstekTest.exe')
if ($LASTEXITCODE -ne 0) { throw 'Hazir istek testleri basarisiz.' }
