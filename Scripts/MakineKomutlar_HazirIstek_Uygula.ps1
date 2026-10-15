# Varsayilan salt okunur; onayli DB gecisi icin -Uygula gerekir.
param([switch]$Uygula)
$ErrorActionPreference = 'Stop'
[xml]$taskConfig = Get-Content (Join-Path $PSScriptRoot '../AsyModbus/Web.config')
$taskBuilder = New-Object System.Data.SqlClient.SqlConnectionStringBuilder
$taskBuilder.set_ConnectionString(($taskConfig.configuration.connectionStrings.add | Where-Object name -eq 'SqlBaglanti').connectionString)
if ($taskBuilder.InitialCatalog -ne 'ASY_MODBUS') { throw 'Hedef ASY_MODBUS degil; gecis durduruldu.' }
$taskDb = New-Object System.Data.SqlClient.SqlConnection($taskBuilder.ConnectionString)
$taskTransaction = $null
function Invoke-TaskSql([string]$sql, [int]$timeout = 60) {
    $taskCommand = $taskDb.CreateCommand()
    $taskCommand.CommandText = $sql
    $taskCommand.CommandTimeout = $timeout
    if ($script:taskTransaction) { $taskCommand.Transaction = $script:taskTransaction }
    try { $null = $taskCommand.ExecuteNonQuery() } finally { $taskCommand.Dispose() }
}
function Get-TaskTable([string]$sql) {
    $taskCommand = $taskDb.CreateCommand()
    $taskCommand.CommandText = $sql
    $taskCommand.CommandTimeout = 30
    if ($script:taskTransaction) { $taskCommand.Transaction = $script:taskTransaction }
    try {
        $taskReader = $taskCommand.ExecuteReader()
        try {
            $taskTable = New-Object System.Data.DataTable
            $taskTable.Load($taskReader)
            return ,$taskTable
        } finally { $taskReader.Dispose() }
    } finally { $taskCommand.Dispose() }
}
try {
    $taskDb.Open()
    $taskInfo = Get-TaskTable "SELECT @@SERVERNAME AS sunucu, DB_NAME() AS veritabani, CONVERT(nvarchar(4000), SERVERPROPERTY('InstanceDefaultBackupPath')) AS yedek_dizini"
    $taskCounts = Get-TaskTable 'SELECT COUNT_BIG(*) AS adet FROM dbo.MakineKomutlar'
    $taskBefore = [long]$taskCounts.Rows[0].adet
    Write-Output ('Hedef: ' + $taskInfo.Rows[0].sunucu + ' / ' + $taskInfo.Rows[0].veritabani)
    Write-Output ('Gecis oncesi komut sayisi: ' + $taskBefore)
    if (-not $Uygula) { Write-Output 'Salt okunur kontrol; degisiklik icin -Uygula gerekir.'; return }
    $taskProcesses = @(Get-Process -Name 'AsyModbus.KomutForm' -ErrorAction SilentlyContinue)
    if ($taskProcesses.Count -gt 0) { throw 'Form veya web sureci calisiyor. Guvenli kapanis tamamlanmali.' }
    if (@(Get-Process -Name 'iisexpress','w3wp' -ErrorAction SilentlyContinue).Count -gt 0 -and
        -not (Test-Path (Join-Path $PSScriptRoot '../AsyModbus/app_offline.htm'))) {
        throw 'Web gecis sirasinda app_offline.htm ile duraklatilmali.'
    }
    $taskActive = Get-TaskTable @'
SELECT COUNT(*) AS adet FROM dbo.MakineKomutlar K JOIN dbo.Parametreler P ON P.id=K.durum_parametre_id
JOIN dbo.ParametreGruplari G ON G.id=P.parametre_grup_id
WHERE K.aktif_mi=1 AND G.kod=N'MAKINE_KOMUT_DURUM' AND P.kod=N'ISLENIYOR';
'@
    if ([int]$taskActive.Rows[0].adet -gt 0) { throw 'ISLENIYOR komut var; OFF temizligi kontrol edilmeli.' }
    $taskBackupDir = [string]$taskInfo.Rows[0].yedek_dizini
    if ([string]::IsNullOrWhiteSpace($taskBackupDir)) { throw 'SQL yedek dizini belirlenemedi.' }
    $taskBackupFile = $taskBackupDir.TrimEnd('\') + '\ASY_MODBUS_HazirIstek_Oncesi_' + [DateTime]::UtcNow.ToString('yyyyMMdd_HHmmss') + '_' + [guid]::NewGuid().ToString('N') + '.bak'
    $taskSafeBackupFile = $taskBackupFile.Replace("'", "''")
    Invoke-TaskSql "BACKUP DATABASE [ASY_MODBUS] TO DISK=N'$taskSafeBackupFile' WITH COPY_ONLY, CHECKSUM;" 180
    Invoke-TaskSql "RESTORE VERIFYONLY FROM DISK=N'$taskSafeBackupFile' WITH CHECKSUM;" 180
    Write-Output ('Dogrulanan tam yedek: ' + $taskBackupFile)
    $script:taskTransaction = $taskDb.BeginTransaction()
    # Sema ve SP degisiklikleri beraber commit edilir; eski tuketici araya giremez.
    Invoke-TaskSql @'
SET LOCK_TIMEOUT 10000;
DECLARE @adet BIGINT;
SELECT @adet=COUNT_BIG(*) FROM dbo.MakineKomutlar WITH (TABLOCKX,HOLDLOCK);
SELECT @adet=COUNT_BIG(*) FROM dbo.Makineler WITH (TABLOCKX,HOLDLOCK);
IF EXISTS (SELECT 1 FROM dbo.MakineKomutlar K JOIN dbo.Parametreler P ON P.id=K.durum_parametre_id
 JOIN dbo.ParametreGruplari G ON G.id=P.parametre_grup_id
 WHERE K.aktif_mi=1 AND G.kod=N'MAKINE_KOMUT_DURUM' AND P.kod=N'ISLENIYOR')
 THROW 51210,N'Yeni islenen komut olustu; islem geri alinacak.',1;
'@
    Invoke-TaskSql (Get-Content (Join-Path $PSScriptRoot 'MakineKomutlar_HazirIstek_Migrasyon.sql') -Raw)
    $taskProcedures = Get-Content (Join-Path $PSScriptRoot 'MakineKomutlar_StoredProcedure.sql') -Raw
    $taskProcedures = [regex]::Replace($taskProcedures, '(?m)^:ON ERROR EXIT\s*$', '')
    foreach ($taskBatch in [regex]::Split($taskProcedures, '(?im)^GO\s*$')) {
        if ($taskBatch.Trim()) { Invoke-TaskSql $taskBatch }
    }
    Invoke-TaskSql (Get-Content (Join-Path $PSScriptRoot 'MakineKomutlar_GereksizAlanlariKaldir.sql') -Raw)
    Invoke-TaskSql @'
IF @@TRANCOUNT<>1 THROW 51211,N'Beklenmeyen transaction sayisi.',1;
IF (SELECT COUNT(*) FROM sys.columns WHERE object_id=OBJECT_ID(N'dbo.MakineKomutlar')
 AND name IN (N'tetikleme_url',N'kapatma_url',N'hedef_rolecihaz_id',N'hedef_ip',N'hedef_port',
 N'tetikleme_suresi_ms'))<>6
 THROW 51212,N'Yeni komut alanlari eksik.',1;
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID(N'dbo.MakineKomutlar')
 AND name IN (N'talep_eden_ad_soyad',N'baslama_tarih',N'bitis_tarih'))
 THROW 51217,N'Gereksiz alanlar kaldirilamadi.',1;
IF (SELECT COUNT(*) FROM sys.columns WHERE object_id=OBJECT_ID(N'dbo.MakineKomutlar')
 AND name IN (N'tetikleme_url',N'kapatma_url') AND max_length=4096 AND TYPE_NAME(user_type_id)=N'nvarchar')<>2
 THROW 51213,N'URL kolon tipi/uzunlugu uyumsuz.',1;
IF OBJECT_ID(N'dbo.SP_MakineKomutlar_SAHIPLEN',N'P') IS NULL
 OR OBJECT_ID(N'dbo.SP_MakineKomutlar_SON_DURUMLARI_GETIR',N'P') IS NULL
 THROW 51214,N'Yeni SP eksik.',1;
IF (SELECT COUNT(*) FROM sys.parameters WHERE object_id=OBJECT_ID(N'dbo.SP_MakineKomutlar_EKLE')
 AND name IN (N'@tetikleme_url',N'@kapatma_url') AND max_length=4096)<>2
 THROW 51215,N'URL SP parametreleri uyumsuz.',1;
IF OBJECT_DEFINITION(OBJECT_ID(N'dbo.SP_MakineKomutlar_ISLEME_GUNCELLE')) NOT LIKE N'%51110%'
 THROW 51216,N'Eski tuketici engeli eksik.',1;
'@
    $taskAfter = Get-TaskTable 'SELECT COUNT_BIG(*) AS adet FROM dbo.MakineKomutlar'
    if ([long]$taskAfter.Rows[0].adet -ne $taskBefore) { throw 'Komut kayit sayisi degisti; gecis geri alinacak.' }
    $null = Get-TaskTable 'EXEC dbo.SP_MakineKomutlar_SON_DURUMLARI_GETIR'
    $script:taskTransaction.Commit()
    $script:taskTransaction.Dispose()
    $script:taskTransaction = $null
    Write-Output 'BASARILI: Sema ve stored procedure degisiklikleri birlikte commit edildi.'
    $taskState = Get-TaskTable @'
SELECT P.kod AS durum,COUNT(*) AS adet FROM dbo.MakineKomutlar K
JOIN dbo.Parametreler P ON P.id=K.durum_parametre_id GROUP BY P.kod;
'@
    $taskState | Format-Table -AutoSize | Out-String | Write-Output
    @(
        '# ASY_MODBUS hazir istek DB gecisi', '',
        'Sonuc: Basarili; sema ve SP degisiklikleri tek dis transaction icinde uygulandi.',
        ('Tam yedek (COPY_ONLY, CHECKSUM ve RESTORE VERIFYONLY): ' + $taskBackupFile),
        ('Korunan komut kaydi: ' + $taskBefore),
        'Uygulanan: MakineKomutlar_HazirIstek_Migrasyon.sql, MakineKomutlar_StoredProcedure.sql.',
        'Dogrulanan: 6 komut alani; 3 gereksiz kolon kaldirildi. NVARCHAR(2048) URL kolonlari/parametreleri, yeni SP ve durum sorgusu dogrulandi.',
        'Roleye ag istegi gonderilmedi; komut eklenmedi veya sahiplenilmedi.'
    ) | Set-Content (Join-Path $PSScriptRoot '../docs/Hazir-Istek-DB-Gecis-Sonucu.md') -Encoding utf8
} catch {
    if ($script:taskTransaction) {
        try { $script:taskTransaction.Rollback() } catch { Write-Warning 'Transaction SQL tarafinda zaten geri alinmis olabilir.' }
    }
    throw
} finally {
    if ($script:taskTransaction) { $script:taskTransaction.Dispose() }
    $taskDb.Dispose()
}
