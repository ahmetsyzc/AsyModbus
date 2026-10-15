# Yalnizca SQL kuyruk testleri. RoleKontrol cagrilmaz.
# Her senaryo transaction icinde calisir ve sonunda geri alinir.
# SQL Server IDENTITY sayaci rollback ile geri donmez; testler ID boslugu olusturabilir.
$ErrorActionPreference = 'Stop'
[xml]$taskConfig = Get-Content (Join-Path $PSScriptRoot '../AsyModbus/Web.config')
$taskConnectionString = ($taskConfig.configuration.connectionStrings.add |
    Where-Object { $_.name -eq 'SqlBaglanti' }).connectionString
$taskConnection = New-Object System.Data.SqlClient.SqlConnection($taskConnectionString)
$taskTransaction = $null

function Get-TaskTable([string]$sql) {
    $taskCommand = $taskConnection.CreateCommand()
    $taskCommand.CommandText = $sql
    $taskCommand.CommandTimeout = 10
    $taskCommand.Transaction = $script:taskTransaction
    try {
        $taskReader = $taskCommand.ExecuteReader()
        try {
            $taskTable = New-Object System.Data.DataTable
            $taskTable.Load($taskReader)
            return ,$taskTable
        } finally { $taskReader.Dispose() }
    } finally { $taskCommand.Dispose() }
}

function Get-TaskClaim {
    return ,(Get-TaskTable 'EXEC dbo.SP_MakineKomutlar_SAHIPLEN @guncelleyen_ip=N''127.0.0.1''')
}

function Invoke-TaskSql([string]$sql) {
    $taskCommand = $taskConnection.CreateCommand()
    $taskCommand.CommandText = $sql
    $taskCommand.CommandTimeout = 10
    $taskCommand.Transaction = $script:taskTransaction
    try { return $taskCommand.ExecuteNonQuery() } finally { $taskCommand.Dispose() }
}

function Assert-Task([bool]$condition, [string]$message) {
    if (-not $condition) { throw $message }
}

function Assert-TaskSqlError([string]$sql, [int]$number) {
    $taskExpectedError = $false
    try { $null = Invoke-TaskSql $sql }
    catch {
        $taskError = $_.Exception
        while ($taskError -and $taskError -isnot [System.Data.SqlClient.SqlException]) {
            $taskError = $taskError.InnerException
        }
        if (-not $taskError -or $taskError.Number -ne $number) { throw }
        $taskExpectedError = $true
    }
    Assert-Task $taskExpectedError "Beklenen SQL hata kodu gelmedi: $number"
}

function Test-Task([string]$name, [scriptblock]$test) {
    $script:taskTransaction = $taskConnection.BeginTransaction()
    try { & $test; Write-Output "GECTI: $name" }
    finally {
        try { $script:taskTransaction.Rollback() } finally {
            $script:taskTransaction.Dispose()
            $script:taskTransaction = $null
        }
    }
}

try {
    $taskConnection.Open()
    $taskBefore = Get-TaskTable 'SELECT COUNT(*) AS adet FROM dbo.MakineKomutlar'
    Assert-Task ($taskBefore.Rows[0].adet -eq 0) 'Testler yalnizca bos komut kuyrugunda calistirilir.'
    $taskSetup = Get-TaskTable @'
SELECT (SELECT TOP (1) M.id FROM dbo.Makineler M JOIN dbo.RoleCihazlar R ON R.id=M.rolecihazlar_id WHERE M.aktif_mi=1 AND R.aktif_mi=1 AND M.role_kanal_no BETWEEN 1 AND R.kanal_sayisi ORDER BY M.id) AS makine_id,
       (SELECT P.id FROM dbo.Parametreler P JOIN dbo.ParametreGruplari G ON G.id=P.parametre_grup_id
        WHERE G.kod=N'ISLEM_TURU' AND P.kod=N'DURDURMA' AND P.aktif_mi=1) AS tur_id,
       (SELECT P.id FROM dbo.Parametreler P JOIN dbo.ParametreGruplari G ON G.id=P.parametre_grup_id
        WHERE G.kod=N'MAKINE_KOMUT_DURUM' AND P.kod=N'BASARILI' AND P.aktif_mi=1) AS basarili_id,
       (SELECT P.id FROM dbo.Parametreler P JOIN dbo.ParametreGruplari G ON G.id=P.parametre_grup_id
        WHERE G.kod=N'MAKINE_KOMUT_DURUM' AND P.kod=N'BASARISIZ' AND P.aktif_mi=1) AS basarisiz_id,
       (SELECT P.id FROM dbo.Parametreler P JOIN dbo.ParametreGruplari G ON G.id=P.parametre_grup_id
        WHERE G.kod=N'MAKINE_KOMUT_DURUM' AND P.kod=N'BEKLIYOR' AND P.aktif_mi=1) AS bekliyor_id
'@
    $taskMachineId = [int]$taskSetup.Rows[0].makine_id
    $taskTypeId = [int]$taskSetup.Rows[0].tur_id
    $taskSuccessId = [int]$taskSetup.Rows[0].basarili_id
    $taskFailureId = [int]$taskSetup.Rows[0].basarisiz_id
    $taskWaitingId = [int]$taskSetup.Rows[0].bekliyor_id
    Assert-Task ($taskMachineId -gt 0 -and $taskTypeId -gt 0) 'Aktif test makinesi veya DURDURMA parametresi yok.'
    $taskHedef = Get-TaskTable "SELECT R.id,R.ip,R.port,M.role_kanal_no FROM dbo.Makineler M JOIN dbo.RoleCihazlar R ON R.id=M.rolecihazlar_id WHERE M.id=$taskMachineId"
    $taskRow = $taskHedef.Rows[0]
    $taskOn = (New-Object System.UriBuilder('http', [string]$taskRow.ip, [int]$taskRow.port, (([int]$taskRow.role_kanal_no-1)*2).ToString('D2'))).Uri.AbsoluteUri.Replace("'", "''")
    $taskOff = (New-Object System.UriBuilder('http', [string]$taskRow.ip, [int]$taskRow.port, (([int]$taskRow.role_kanal_no-1)*2+1).ToString('D2'))).Uri.AbsoluteUri.Replace("'", "''")
    $taskIp = ([string]$taskRow.ip).Replace("'", "''")
    $taskHazir = ", @tetikleme_url=N'$taskOn', @kapatma_url=N'$taskOff', @hedef_rolecihaz_id=$($taskRow.id), @hedef_ip=N'$taskIp', @hedef_port=$($taskRow.port), @tetikleme_suresi_ms=2000"
    $taskInsertSql = "EXEC dbo.SP_MakineKomutlar_EKLE @makineler_id=$taskMachineId, @islem_tur_parametre_id=$taskTypeId, @detay=N'Geri alinan SQL testi', @ekleyen_ip=N'SQL_TEST'$taskHazir;"

    Test-Task 'Ekle / Doldur / Liste / BEKLIYOR ve row count' {
        $taskAffected = Invoke-TaskSql $taskInsertSql
        Assert-Task ($taskAffected -eq 1) 'Ekle row count 1 olmali.'
        $taskRows = Get-TaskTable 'EXEC dbo.SP_MakineKomutlar_BEKLEYEN_GETIR'
        Assert-Task ($taskRows.Rows.Count -eq 1 -and $taskRows.Rows[0].deneme_sayisi -eq 0) 'Ilk kayit BEKLIYOR / 0 olmali.'
        $taskId = [int]$taskRows.Rows[0].id
        $taskFilled = Get-TaskTable "EXEC dbo.SP_MakineKomutlar_DOLDUR @id=$taskId"
        Assert-Task ($taskFilled.Rows.Count -eq 1 -and $taskFilled.Rows[0].durum_parametre_id -eq $taskWaitingId) 'Doldur sonucu yanlis.'
        $taskList = Get-TaskTable 'EXEC dbo.SP_MakineKomutlar_TUMUNU_GETIR'
        Assert-Task ($taskList.Rows.Count -eq 1 -and $taskList.Rows[0].durum_kod -eq 'BEKLIYOR') 'Liste sonucu yanlis.'
        $taskActive = Get-TaskTable "EXEC dbo.SP_MakineKomutlar_AKTIF_KOMUT_VAR_MI @makineler_id=$taskMachineId, @islem_tur_parametre_id=$taskTypeId"
        Assert-Task ($taskActive.Rows[0][0] -eq 1) 'Aktif komut bulunmali.'
    }
    Test-Task 'BEKLIYOR durumunda cift talep engeli' {
        $null = Invoke-TaskSql $taskInsertSql
        Assert-TaskSqlError $taskInsertSql 51104
    }
    Test-Task 'Atomik sahiplenme / ikinci sahiplenme bos / BASARILI' {
        $null = Invoke-TaskSql $taskInsertSql
        $taskClaim = Get-TaskClaim
        Assert-Task ($taskClaim.Rows.Count -eq 1 -and $taskClaim.Rows[0].deneme_sayisi -eq 1) 'Komut bir kere sahiplenilmeli.'
        $taskId = [int]$taskClaim.Rows[0].id
        $taskSecond = Get-TaskClaim
        Assert-Task ($taskSecond.Rows.Count -eq 0) 'Ayni komut tekrar sahiplenilemez.'
        $taskAffected = Invoke-TaskSql "EXEC dbo.SP_MakineKomutlar_GUNCELLE @id=$taskId, @durum_parametre_id=$taskSuccessId, @detay=N'SQL test sonucu'"
        Assert-Task ($taskAffected -eq 1) 'Sonuclandir row count 1 olmali.'
        $taskFinal = Get-TaskTable "EXEC dbo.SP_MakineKomutlar_DOLDUR @id=$taskId"
        Assert-Task ($taskFinal.Rows[0].durum_parametre_id -eq $taskSuccessId -and $taskFinal.Rows[0].deneme_sayisi -eq 1) 'Basari sonucu yanlis.'
        $taskActive = Get-TaskTable "EXEC dbo.SP_MakineKomutlar_AKTIF_KOMUT_VAR_MI @makineler_id=$taskMachineId, @islem_tur_parametre_id=$taskTypeId"
        Assert-Task ($taskActive.Rows[0][0] -eq 0) 'Tamamlanan komut yeni talebi engellememeli.'
        $null = Invoke-TaskSql $taskInsertSql
    }
    Test-Task 'ISLENIYOR durumunda cift talep engeli' {
        $null = Invoke-TaskSql $taskInsertSql
        $null = Get-TaskClaim
        Assert-TaskSqlError $taskInsertSql 51104
    }
    Test-Task 'BASARISIZ sonuc ve ikinci sonuclandirma engeli' {
        $null = Invoke-TaskSql $taskInsertSql
        $taskClaim = Get-TaskClaim
        $taskId = [int]$taskClaim.Rows[0].id
        $null = Invoke-TaskSql "EXEC dbo.SP_MakineKomutlar_GUNCELLE @id=$taskId, @durum_parametre_id=$taskFailureId, @detay=N'Kontrollu hata'"
        $taskFinal = Get-TaskTable "EXEC dbo.SP_MakineKomutlar_DOLDUR @id=$taskId"
        Assert-Task ($taskFinal.Rows[0].durum_parametre_id -eq $taskFailureId -and $taskFinal.Rows[0].detay -eq 'Kontrollu hata') 'Hata sonucu yanlis.'
        Assert-TaskSqlError "EXEC dbo.SP_MakineKomutlar_GUNCELLE @id=$taskId, @durum_parametre_id=$taskSuccessId" 51106
    }
    Test-Task 'BEKLIYOR komut dogrudan sonuclandirilamaz' {
        $null = Invoke-TaskSql $taskInsertSql
        $taskRows = Get-TaskTable 'EXEC dbo.SP_MakineKomutlar_BEKLEYEN_GETIR'
        $taskId = [int]$taskRows.Rows[0].id
        Assert-TaskSqlError "EXEC dbo.SP_MakineKomutlar_GUNCELLE @id=$taskId, @durum_parametre_id=$taskSuccessId" 51106
    }
    Test-Task 'Gecersiz sonuc parametresi engeli' {
        $null = Invoke-TaskSql $taskInsertSql
        $taskClaim = Get-TaskClaim
        $taskId = [int]$taskClaim.Rows[0].id
        Assert-TaskSqlError "EXEC dbo.SP_MakineKomutlar_GUNCELLE @id=$taskId, @durum_parametre_id=$taskWaitingId" 51105
    }
    Test-Task 'Olmayan makine engeli' {
        Assert-TaskSqlError "EXEC dbo.SP_MakineKomutlar_EKLE @makineler_id=-1, @islem_tur_parametre_id=$taskTypeId$taskHazir" 51101
    }
    Test-Task 'Yanlis islem turu grubu engeli' {
        Assert-TaskSqlError "EXEC dbo.SP_MakineKomutlar_EKLE @makineler_id=$taskMachineId, @islem_tur_parametre_id=$taskSuccessId$taskHazir" 51102
    }
    # Iki baglanti ayni komutu almaya calisir. Ilk transaction acikken
    # ikinci baglanti beklemeli; ilk test rollback olunca bos kuyruk gormelidir.
    $taskSecondConnection = New-Object System.Data.SqlClient.SqlConnection($taskConnectionString)
    $taskSecondCommand = $null
    $taskSecondReader = $null
    try {
        $taskSecondConnection.Open()
        $script:taskTransaction = $taskConnection.BeginTransaction()
        $null = Invoke-TaskSql $taskInsertSql
        $taskFirstClaim = Get-TaskClaim
        Assert-Task ($taskFirstClaim.Rows.Count -eq 1) 'Ilk baglanti komutu almali.'
        $taskSecondCommand = $taskSecondConnection.CreateCommand()
        $taskSecondCommand.CommandText = 'BEGIN TRANSACTION; EXEC dbo.SP_MakineKomutlar_SIRADAKI_GETIR; ROLLBACK TRANSACTION;'
        $taskSecondCommand.CommandTimeout = 10
        $taskPendingClaim = $taskSecondCommand.ExecuteReaderAsync()
        Assert-Task (-not $taskPendingClaim.Wait(250)) 'Ikinci baglanti sahiplenilen satirin kilidini beklemeli.'
        $script:taskTransaction.Rollback()
        $script:taskTransaction.Dispose()
        $script:taskTransaction = $null
        $taskSecondReader = $taskPendingClaim.GetAwaiter().GetResult()
        Assert-Task (-not $taskSecondReader.Read()) 'Rollback sonrasi ikinci baglanti komut bulmamali.'
        Write-Output 'GECTI: Iki SQL baglantisinda eszamanli sahiplenme kilidi'
    } finally {
        if ($script:taskTransaction) {
            try { $script:taskTransaction.Rollback() } finally {
                $script:taskTransaction.Dispose()
                $script:taskTransaction = $null
            }
        }
        if ($taskSecondReader) { $taskSecondReader.Dispose() }
        if ($taskSecondCommand) { $taskSecondCommand.Dispose() }
        $taskSecondConnection.Dispose()
    }
    $taskAfter = Get-TaskTable 'SELECT COUNT(*) AS adet FROM dbo.MakineKomutlar'
    Assert-Task ($taskAfter.Rows[0].adet -eq $taskBefore.Rows[0].adet) 'Test komutlari geri alinmadi.'
    Write-Output 'GECTI: Tum test komutlari geri alindi; kuyruk bos.'
} finally { $taskConnection.Dispose() }
