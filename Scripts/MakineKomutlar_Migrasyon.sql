/* ASAMA 1: Makine komut kuyrugu ve sistem parametreleri.
   Hedef veritabani SSMS uzerinden secilerek calistirilir.
   Bu dosya stored procedure veya servis olusturmaz.
   Mevcut parametreler degistirilmez; eksikler eklenir.
   Tek transaction: hata durumunda bu calistirmanin degisiklikleri geri alinir.
*/
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.Makineler', N'U') IS NULL
        OR OBJECT_ID(N'dbo.Parametreler', N'U') IS NULL
        OR OBJECT_ID(N'dbo.ParametreGruplari', N'U') IS NULL
        THROW 51000, N'Makineler, Parametreler ve ParametreGruplari tablolari bulunmalidir.', 1;

    IF COL_LENGTH(N'dbo.Parametreler', N'parametre_grup_id') IS NULL
        THROW 51001, N'Parametreler.parametre_grup_id gecisi tamamlanmalidir.', 1;

    DECLARE @Gruplar TABLE (kod NVARCHAR(50), ad NVARCHAR(100), aciklama NVARCHAR(250));
    INSERT INTO @Gruplar VALUES
        (N'MAKINE_KOMUT_DURUM', N'Makine Komut Durumu', N'Komut kuyrugu yasam dongusu'),
        (N'SERVIS_AYARLARI', N'Servis Ayarlari', N'Komut servisinin calisma ayarlari'),
        (N'ISLEM_TURU', N'Islem Turu', N'Makine islem turleri'),
        (N'ISLEM_SONUCU', N'Islem Sonucu', N'Islem sonuclari'),
        (N'ISLEM_KAYNAGI', N'Islem Kaynagi', N'Islemin kaynagi');

    INSERT INTO dbo.ParametreGruplari
        (kod, ad, aciklama, sistem_mi, aktif_mi, ekleyen_id, ekleyen_ip, eklenme_tarih)
    SELECT S.kod, S.ad, S.aciklama, 1, 1, NULL, N'SYSTEM', GETDATE()
    FROM @Gruplar S
    WHERE NOT EXISTS
        (SELECT 1 FROM dbo.ParametreGruplari G WITH (UPDLOCK, HOLDLOCK) WHERE G.kod = S.kod);

    IF EXISTS
        (SELECT G.kod FROM dbo.ParametreGruplari G
         INNER JOIN @Gruplar S ON S.kod = G.kod
         GROUP BY G.kod HAVING COUNT(*) <> 1)
        THROW 51002, N'Gerekli parametre gruplarinda yinelenen kod var.', 1;

    IF EXISTS
        (SELECT 1 FROM dbo.ParametreGruplari G INNER JOIN @Gruplar S ON S.kod = G.kod
         WHERE G.aktif_mi = 0)
        THROW 51003, N'Gerekli parametre grubu pasif. Mevcut kaydi kontrol edin.', 1;

    DECLARE @Parametreler TABLE
        (grup_kod NVARCHAR(50), kod NVARCHAR(50), ad NVARCHAR(100), aciklama NVARCHAR(250));
    INSERT INTO @Parametreler VALUES
        (N'MAKINE_KOMUT_DURUM', N'BEKLIYOR', N'Bekliyor', N'Komut henuz servis tarafindan alinmadi'),
        (N'MAKINE_KOMUT_DURUM', N'ISLENIYOR', N'Isleniyor', N'Komut servis tarafindan sahiplenildi'),
        (N'MAKINE_KOMUT_DURUM', N'BASARILI', N'Basarili', N'Role tetikleme islemi basarili; fiziksel makine durusunu dogrulamaz'),
        (N'MAKINE_KOMUT_DURUM', N'BASARISIZ', N'Basarisiz', N'Komut uygulama veya dogrulama islemi basarisiz'),
        (N'ISLEM_TURU', N'DURDURMA', N'Durdurma', N'Makine durdurma talebi'),
        (N'ISLEM_SONUCU', N'BASARILI', N'Basarili', N'Islem basariyla tamamlandi'),
        (N'ISLEM_SONUCU', N'BASARISIZ', N'Basarisiz', N'Islem basarisiz oldu'),
        (N'ISLEM_KAYNAGI', N'SERVIS', N'Servis', N'Makine komut servisi'),
        (N'SERVIS_AYARLARI', N'KOMUT_KONTROL_SURESI_SN', N'5', N'Ad alaninda pozitif tam sayi olarak saniye cinsinden kontrol suresi');

    INSERT INTO dbo.Parametreler
        (parametre_grup_id, kod, ad, aciklama, sistem_mi, aktif_mi, ekleyen_id, ekleyen_ip, eklenme_tarih)
    SELECT G.id, S.kod, S.ad, S.aciklama, 1, 1, NULL, N'SYSTEM', GETDATE()
    FROM @Parametreler S
    INNER JOIN dbo.ParametreGruplari G ON G.kod = S.grup_kod
    WHERE NOT EXISTS
        (SELECT 1 FROM dbo.Parametreler P WITH (UPDLOCK, HOLDLOCK)
         WHERE P.parametre_grup_id = G.id AND P.kod = S.kod);

    IF EXISTS
        (SELECT S.grup_kod, S.kod FROM @Parametreler S
         INNER JOIN dbo.ParametreGruplari G ON G.kod = S.grup_kod
         INNER JOIN dbo.Parametreler P ON P.parametre_grup_id = G.id AND P.kod = S.kod
         GROUP BY S.grup_kod, S.kod HAVING COUNT(*) <> 1)
        THROW 51004, N'Gerekli parametrelerde yinelenen kod var.', 1;

    IF EXISTS
        (SELECT 1 FROM @Parametreler S
         INNER JOIN dbo.ParametreGruplari G ON G.kod = S.grup_kod
         INNER JOIN dbo.Parametreler P ON P.parametre_grup_id = G.id AND P.kod = S.kod
         WHERE P.aktif_mi = 0)
        THROW 51005, N'Gerekli parametre pasif. Mevcut kaydi kontrol edin.', 1;

    IF OBJECT_ID(N'dbo.MakineKomutlar', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.MakineKomutlar
        (
            id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_MakineKomutlar PRIMARY KEY,
            makineler_id INT NOT NULL,
            islem_tur_parametre_id INT NOT NULL,
            durum_parametre_id INT NOT NULL,
            deneme_sayisi INT NOT NULL CONSTRAINT DF_MakineKomutlar_deneme_sayisi DEFAULT (0),
            detay NVARCHAR(MAX) NULL,
            aktif_mi BIT NOT NULL CONSTRAINT DF_MakineKomutlar_aktif_mi DEFAULT (1),
            ekleyen_id INT NULL,
            ekleyen_ip NVARCHAR(20) NULL,
            eklenme_tarih DATETIME NOT NULL CONSTRAINT DF_MakineKomutlar_eklenme_tarih DEFAULT (GETDATE()),
            guncelleyen_id INT NULL,
            guncelleyen_ip NVARCHAR(20) NULL,
            guncellenme_tarih DATETIME NULL,
            CONSTRAINT FK_MakineKomutlar_Makineler FOREIGN KEY (makineler_id) REFERENCES dbo.Makineler(id),
            CONSTRAINT FK_MakineKomutlar_IslemTuru FOREIGN KEY (islem_tur_parametre_id) REFERENCES dbo.Parametreler(id),
            CONSTRAINT FK_MakineKomutlar_Durum FOREIGN KEY (durum_parametre_id) REFERENCES dbo.Parametreler(id),
            CONSTRAINT CK_MakineKomutlar_DenemeSayisi CHECK (deneme_sayisi >= 0)
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes
                   WHERE object_id = OBJECT_ID(N'dbo.MakineKomutlar') AND name = N'IX_MakineKomutlar_Kuyruk')
        CREATE INDEX IX_MakineKomutlar_Kuyruk
        ON dbo.MakineKomutlar (aktif_mi, durum_parametre_id, eklenme_tarih, id)
        INCLUDE (makineler_id, islem_tur_parametre_id);

    IF NOT EXISTS (SELECT 1 FROM sys.indexes
                   WHERE object_id = OBJECT_ID(N'dbo.MakineKomutlar') AND name = N'IX_MakineKomutlar_MakineDurum')
        CREATE INDEX IX_MakineKomutlar_MakineDurum
        ON dbo.MakineKomutlar (makineler_id, islem_tur_parametre_id, aktif_mi, durum_parametre_id);

    /* FK'ler kayit varligini denetler. Dogru parametre grubu, durum gecisleri
       ve ayni makineye cift aktif talep kontrolu ASAMA 3 SP'lerinde uygulanir.
       Parametre ID'leri ortamdan ortama degistigi icin sabit ID kullanilmaz. */
    COMMIT TRANSACTION;

    SELECT N'MakineKomutlar temeli hazirlandi.' AS sonuc;
    SELECT G.kod AS grup_kod, P.id, P.kod, P.ad, P.aktif_mi
    FROM dbo.Parametreler P
    INNER JOIN dbo.ParametreGruplari G ON G.id = P.parametre_grup_id
    INNER JOIN @Parametreler S ON S.grup_kod = G.kod AND S.kod = P.kod
    ORDER BY G.kod, P.kod;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
