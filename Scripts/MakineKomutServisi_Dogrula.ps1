# Benzersiz gecici SQL DB + sahte role. Uygulama DB'sine yazmaz.
# Gerekli izin: yerel SQL Server'da CREATE/DROP DATABASE.
$ErrorActionPreference = 'Stop'
[xml]$taskConfig = Get-Content (Join-Path $PSScriptRoot '../AsyModbus/Web.config')
$taskBuilder = New-Object System.Data.SqlClient.SqlConnectionStringBuilder
$taskBuilder.set_ConnectionString(($taskConfig.configuration.connectionStrings.add | Where-Object { $_.name -eq 'SqlBaglanti' }).connectionString)
$taskSourceDb = $taskBuilder.InitialCatalog
$taskTestDb = 'AsyModbus_KomutTest_' + [guid]::NewGuid().ToString('N')
$taskCreated = $false
$taskTemp = Join-Path $env:TEMP $taskTestDb
New-Item -ItemType Directory -Path $taskTemp | Out-Null
$taskBuilder.set_InitialCatalog('master')
$taskMaster = New-Object System.Data.SqlClient.SqlConnection($taskBuilder.ConnectionString)
$taskTestConnection = $null
function Invoke-TaskCommand($connection, [string]$sql) {
    $taskCommand = $connection.CreateCommand()
    $taskCommand.CommandText = $sql
    $taskCommand.CommandTimeout = 30
    try { $null = $taskCommand.ExecuteNonQuery() } finally { $taskCommand.Dispose() }
}
try {
    $taskMaster.Open()
    Invoke-TaskCommand $taskMaster "CREATE DATABASE [$taskTestDb]"
    $taskCreated = $true
    $taskBuilder.set_InitialCatalog($taskTestDb)
    $taskTestConnection = New-Object System.Data.SqlClient.SqlConnection($taskBuilder.ConnectionString)
    $taskTestConnection.Open()
    # Yalnizca parametreler kopyalanir; makine/role/log tablolari bos sema olarak alinir.
    $taskSafeSource = $taskSourceDb.Replace(']', ']]')
    foreach ($taskTable in @('Kullanicilar','Roller','Makineler','RoleCihazlar','MakinelerLoglar','Loglar','Parametreler','ParametreGruplari')) {
        $taskTop = if ($taskTable -in @('Parametreler','ParametreGruplari')) { '' } else { 'TOP (0)' }
        Invoke-TaskCommand $taskTestConnection "SELECT $taskTop * INTO dbo.[$taskTable] FROM [$taskSafeSource].dbo.[$taskTable]; ALTER TABLE dbo.[$taskTable] ADD PRIMARY KEY (id);"
    }
    foreach ($taskProcedure in @('SP_Kullanicilar_DOLDUR','SP_Makineler_DOLDUR','SP_Makineler_DURUM_GUNCELLE','SP_RoleCihazlar_DOLDUR','SP_Parametreler_KODDAN_GETIR','SP_Parametreler_GUNCELLE','SP_MakinelerLoglar_EKLE','SP_Loglar_EKLE')) {
        $taskRead = $taskMaster.CreateCommand()
        $taskRead.CommandText = "SELECT M.definition FROM [$taskSafeSource].sys.sql_modules M JOIN [$taskSafeSource].sys.objects O ON O.object_id=M.object_id WHERE O.name=@name"
        $null = $taskRead.Parameters.AddWithValue('@name', $taskProcedure)
        try { $taskDefinition = [string]$taskRead.ExecuteScalar() } finally { $taskRead.Dispose() }
        if (-not $taskDefinition) { throw "Gerekli procedure bulunamadi: $taskProcedure" }
        # sp_rename sonrasinda sys.sql_modules eski CREATE adini tutabilir.
        $taskHeader = New-Object System.Text.RegularExpressions.Regex('(?is)(?:CREATE\s+OR\s+ALTER|CREATE|ALTER)\s+PROC(?:EDURE)?\s+[^\s(]+')
        $taskDefinition = $taskHeader.Replace($taskDefinition, ('CREATE PROCEDURE dbo.[' + $taskProcedure + ']'), 1)
        Invoke-TaskCommand $taskTestConnection $taskDefinition
    }
    Invoke-TaskCommand $taskTestConnection (Get-Content (Join-Path $PSScriptRoot 'MakineKomutlar_Migrasyon.sql') -Raw)
    Invoke-TaskCommand $taskTestConnection (Get-Content (Join-Path $PSScriptRoot 'MakineKomutlar_HazirIstek_Migrasyon.sql') -Raw)
    $taskProcedures = Get-Content (Join-Path $PSScriptRoot 'MakineKomutlar_StoredProcedure.sql') -Raw
    $taskProcedures = [regex]::Replace($taskProcedures, '(?m)^:ON ERROR EXIT\s*$', '')
    foreach ($taskBatch in [regex]::Split($taskProcedures, '(?im)^GO\s*$')) {
        if ($taskBatch.Trim()) { Invoke-TaskCommand $taskTestConnection $taskBatch }
    }
    Invoke-TaskCommand $taskTestConnection @'
INSERT dbo.Kullanicilar (roller_id,kullanici_kod,ad,soyad,tckno,mail,sifre,cep_no,dogum_tarih,aktif_mi,resim_yol,ekleyen_id,ekleyen_ip,eklenme_tarih)
VALUES (1,N'TEST',N'Test',N'Kullanici',N'',N'',N'',N'','20000101',1,N'',0,N'TEST',GETDATE());
INSERT dbo.RoleCihazlar (ad,ip,port,kanal_sayisi,aktif_mi,eklenme_tarih)
VALUES (N'Test Role',N'127.0.0.1',12345,16,1,GETDATE());
INSERT dbo.Makineler (makine_ad,makine_no,band_no,rolecihazlar_id,role_kanal_no,durum_parametre_id,aktif_mi,eklenme_tarih,ekleyen_id,ekleyen_ip)
SELECT N'Test Makine',N'TEST',N'1',1,3,P.id,1,GETDATE(),0,N'TEST'
FROM dbo.Parametreler P JOIN dbo.ParametreGruplari G ON G.id=P.parametre_grup_id
WHERE G.kod=N'MAKINE_DURUMU' AND P.kod=N'CALISIYOR';
'@
    # Eski kayit gecisi ve idempotency yalnizca bu gecici DB'de denenir.
    Invoke-TaskCommand $taskTestConnection @'
INSERT dbo.MakineKomutlar (makineler_id,islem_tur_parametre_id,durum_parametre_id,deneme_sayisi,aktif_mi,eklenme_tarih)
SELECT 1,T.id,D.id,0,1,GETDATE() FROM dbo.Parametreler T
JOIN dbo.ParametreGruplari TG ON TG.id=T.parametre_grup_id
CROSS JOIN dbo.Parametreler D JOIN dbo.ParametreGruplari DG ON DG.id=D.parametre_grup_id
WHERE TG.kod=N'ISLEM_TURU' AND T.kod=N'DURDURMA'
  AND DG.kod=N'MAKINE_KOMUT_DURUM' AND D.kod IN (N'BEKLIYOR',N'ISLENIYOR');
'@
    1..2 | ForEach-Object {
        Invoke-TaskCommand $taskTestConnection (Get-Content (Join-Path $PSScriptRoot 'MakineKomutlar_HazirIstek_Migrasyon.sql') -Raw)
    }
    Invoke-TaskCommand $taskTestConnection @'
IF (SELECT COUNT(*) FROM dbo.MakineKomutlar K JOIN dbo.Parametreler D ON D.id=K.durum_parametre_id
    WHERE D.kod=N'BASARISIZ' AND K.guncellenme_tarih IS NOT NULL) <> 1
    THROW 51990,N'Eski bekleyen kayit donusmedi.',1;
IF (SELECT COUNT(*) FROM dbo.MakineKomutlar K JOIN dbo.Parametreler D ON D.id=K.durum_parametre_id
    WHERE D.kod=N'ISLENIYOR' AND K.detay LIKE N'%belirsiz%') <> 1
    THROW 51991,N'Eski islenen kayit belirsiz birakilmadi.',1;
DELETE FROM dbo.MakineKomutlar;
'@
    Write-Output 'GECTI: Eski kayit gecisi ve migration tekrar calistirma'
    Invoke-TaskCommand $taskTestConnection 'ALTER TABLE dbo.MakineKomutlar ADD talep_eden_ad_soyad NVARCHAR(250) NULL, baslama_tarih DATETIME NULL, bitis_tarih DATETIME NULL;'
    1..2 | ForEach-Object {
        Invoke-TaskCommand $taskTestConnection (Get-Content (Join-Path $PSScriptRoot 'MakineKomutlar_GereksizAlanlariKaldir.sql') -Raw)
    }
    Write-Output 'GECTI: Uc gereksiz kolonun kaldirilmasi ve tekrar calistirma'
    $taskTestConnection.Close()
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot '../BusinessLayer/bin/Debug/BusinessLayer.dll') -Destination $taskTemp
    $taskTestConfig = New-Object System.Xml.XmlDocument
    $taskTestConfig.LoadXml('<configuration><connectionStrings><add name="SqlBaglanti" providerName="System.Data.SqlClient" /></connectionStrings></configuration>')
    $taskTestConfig.configuration.connectionStrings.add.SetAttribute('connectionString', $taskBuilder.ConnectionString)
    $taskTestConfig.Save((Join-Path $taskTemp 'ServisTest.exe.config'))
    & 'C:/Windows/Microsoft.NET/Framework64/v4.0.30319/csc.exe' /nologo /out:"$taskTemp/ServisTest.exe" /reference:"$taskTemp/BusinessLayer.dll" /reference:System.Data.dll /reference:System.Configuration.dll (Join-Path $PSScriptRoot 'MakineKomutServisi_Dogrula.cs')
    if ($LASTEXITCODE -ne 0) { throw 'Servis test derlemesi basarisiz.' }
    & (Join-Path $taskTemp 'ServisTest.exe')
    if ($LASTEXITCODE -ne 0) { throw 'Servis testleri basarisiz.' }
} finally {
    if ($taskTestConnection) { $taskTestConnection.Dispose() }
    if ($taskCreated) {
        # Silinen DB bu calistirmada olusturulan benzersiz test DB'si olmalidir.
        if ($taskTestDb -notmatch '^AsyModbus_KomutTest_[a-f0-9]{32}$' -or $taskTestDb -eq $taskSourceDb) {
            throw 'Test veritabani temizligi guvenlik kontrolu basarisiz.'
        }
        [System.Data.SqlClient.SqlConnection]::ClearAllPools()
        Invoke-TaskCommand $taskMaster "ALTER DATABASE [$taskTestDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$taskTestDb];"
        Write-Output 'Gecici test veritabani kaldirildi. Uygulama veritabani degistirilmedi.'
    }
    $taskMaster.Dispose()
}
