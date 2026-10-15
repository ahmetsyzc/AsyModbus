/* Eski Form ve web yazicilarini durdurun. Once temel migration, sonra bu dosya,
   sonra MakineKomutlar_StoredProcedure.sql. Canliya otomatik uygulanmaz. */
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;
    IF OBJECT_ID(N'dbo.MakineKomutlar', N'U') IS NULL
        THROW 51200, N'Once MakineKomutlar_Migrasyon.sql uygulanmalidir.', 1;
    IF COL_LENGTH(N'dbo.MakineKomutlar', N'tetikleme_url') IS NULL
        ALTER TABLE dbo.MakineKomutlar ADD tetikleme_url NVARCHAR(2048) NULL;
    IF COL_LENGTH(N'dbo.MakineKomutlar', N'kapatma_url') IS NULL
        ALTER TABLE dbo.MakineKomutlar ADD kapatma_url NVARCHAR(2048) NULL;
    IF COL_LENGTH(N'dbo.MakineKomutlar', N'hedef_rolecihaz_id') IS NULL
        ALTER TABLE dbo.MakineKomutlar ADD hedef_rolecihaz_id INT NULL;
    IF COL_LENGTH(N'dbo.MakineKomutlar', N'hedef_ip') IS NULL
        ALTER TABLE dbo.MakineKomutlar ADD hedef_ip NVARCHAR(45) NULL;
    IF COL_LENGTH(N'dbo.MakineKomutlar', N'hedef_port') IS NULL
        ALTER TABLE dbo.MakineKomutlar ADD hedef_port INT NULL;
    IF COL_LENGTH(N'dbo.MakineKomutlar', N'tetikleme_suresi_ms') IS NULL
        ALTER TABLE dbo.MakineKomutlar ADD tetikleme_suresi_ms INT NULL;
    -- Yeni kolonlar ayni batch icinde derlenmeden once dinamik SQL ile kullanilir.
    EXEC sys.sp_executesql N'
        UPDATE K SET durum_parametre_id=B.id,
          detay=N''Geçiş: hazır cihaz adresi olmayan eski bekleyen komut çalıştırılmadı.'',
          guncellenme_tarih=GETDATE()
        FROM dbo.MakineKomutlar K
        JOIN dbo.Parametreler D ON D.id=K.durum_parametre_id
        JOIN dbo.ParametreGruplari G ON G.id=D.parametre_grup_id
        JOIN dbo.Parametreler B ON B.parametre_grup_id=G.id AND B.kod=N''BASARISIZ''
        WHERE G.kod=N''MAKINE_KOMUT_DURUM'' AND D.kod=N''BEKLIYOR''
          AND (NULLIF(K.tetikleme_url,N'''') IS NULL OR NULLIF(K.kapatma_url,N'''') IS NULL);
        UPDATE K SET detay=N''Geçiş: yarım kalan işlemin sonucu belirsiz. Otomatik ON tekrarı yapılmayacak.''
        FROM dbo.MakineKomutlar K
        JOIN dbo.Parametreler D ON D.id=K.durum_parametre_id
        JOIN dbo.ParametreGruplari G ON G.id=D.parametre_grup_id
        WHERE G.kod=N''MAKINE_KOMUT_DURUM'' AND D.kod=N''ISLENIYOR''
          AND K.tetikleme_url IS NULL;';
    COMMIT;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK;
    THROW;
END CATCH;
