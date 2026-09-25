/* Parametreler mimarisi: tablo, seed, kolonlar, migration, FK, SP guncellemeleri */
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.Parametreler', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Parametreler
    (
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        grup_kod NVARCHAR(50) NOT NULL,
        kod NVARCHAR(50) NOT NULL,
        ad NVARCHAR(100) NOT NULL,
        aciklama NVARCHAR(250) NULL,
        sira_no INT NULL,
        aktif_mi BIT NOT NULL CONSTRAINT DF_Parametreler_aktif_mi DEFAULT (1),
        ekleyen_id INT NULL,
        ekleyen_ip NVARCHAR(20) NULL,
        eklenme_tarih DATETIME NOT NULL CONSTRAINT DF_Parametreler_eklenme_tarih DEFAULT (GETDATE()),
        guncelleyen_id INT NULL,
        guncelleyen_ip NVARCHAR(20) NULL,
        guncellenme_tarih DATETIME NULL
    );

    ALTER TABLE dbo.Parametreler
    ADD CONSTRAINT UQ_Parametreler_GrupKod_Kod UNIQUE (grup_kod, kod);
END

;WITH Seed AS
(
    SELECT * FROM (VALUES
        (N'MAKINE_DURUMU', N'CALISIYOR', NCHAR(199)+N'al'+NCHAR(305)+NCHAR(351)+NCHAR(305)+N'yor', 1),
        (N'MAKINE_DURUMU', N'DURDURULDU', N'Durduruldu', 2),
        (N'MAKINE_DURUMU', N'BAGLANTI_YOK', N'Ba'+NCHAR(287)+N'lant'+NCHAR(305)+N' Yok', 3),
        (N'ISLEM_SONUCU', N'BASARILI', N'Ba'+NCHAR(351)+N'ar'+NCHAR(305)+N'l'+NCHAR(305), 1),
        (N'ISLEM_SONUCU', N'BASARISIZ', N'Ba'+NCHAR(351)+N'ar'+NCHAR(305)+N's'+NCHAR(305)+N'z', 2),
        (N'ISLEM_KAYNAGI', N'PERSONEL', N'Personel', 1),
        (N'ISLEM_KAYNAGI', N'SAHA', N'Saha', 2),
        (N'ISLEM_KAYNAGI', N'SISTEM', N'Sistem', 3),
        (N'ISLEM_TURU', N'DURDURMA', N'Durdurma', 1)
    ) AS V(grup_kod, kod, ad, sira_no)
)
INSERT INTO dbo.Parametreler (grup_kod, kod, ad, sira_no, aktif_mi, ekleyen_id, ekleyen_ip)
SELECT S.grup_kod, S.kod, S.ad, S.sira_no, 1, 0, N'SYSTEM'
FROM Seed S
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Parametreler P
    WHERE P.grup_kod = S.grup_kod
      AND P.kod = S.kod
);

IF COL_LENGTH('dbo.Makineler', 'durum_parametre_id') IS NULL
BEGIN
    ALTER TABLE dbo.Makineler ADD durum_parametre_id INT NULL;
END

IF COL_LENGTH('dbo.MakinelerLoglar', 'kaynak_parametre_id') IS NULL
BEGIN
    ALTER TABLE dbo.MakinelerLoglar ADD kaynak_parametre_id INT NULL;
END

IF COL_LENGTH('dbo.MakinelerLoglar', 'onceki_durum_parametre_id') IS NULL
BEGIN
    ALTER TABLE dbo.MakinelerLoglar ADD onceki_durum_parametre_id INT NULL;
END

IF COL_LENGTH('dbo.MakinelerLoglar', 'yeni_durum_parametre_id') IS NULL
BEGIN
    ALTER TABLE dbo.MakinelerLoglar ADD yeni_durum_parametre_id INT NULL;
END

IF COL_LENGTH('dbo.MakinelerLoglar', 'islem_tur_parametre_id') IS NULL
BEGIN
    ALTER TABLE dbo.MakinelerLoglar ADD islem_tur_parametre_id INT NULL;
END

IF COL_LENGTH('dbo.MakinelerLoglar', 'islem_sonuc_parametre_id') IS NULL
BEGIN
    ALTER TABLE dbo.MakinelerLoglar ADD islem_sonuc_parametre_id INT NULL;
END

COMMIT TRANSACTION;
GO

/* ALTER TABLE ADD sonrası ayrı batch: migration */
UPDATE M
SET M.durum_parametre_id = P.id
FROM dbo.Makineler M
INNER JOIN dbo.Parametreler P
    ON P.grup_kod = N'MAKINE_DURUMU'
   AND P.kod = M.durum
   AND P.aktif_mi = 1
WHERE M.durum IS NOT NULL
  AND M.durum_parametre_id IS NULL;

UPDATE ML
SET ML.kaynak_parametre_id = P.id
FROM dbo.MakinelerLoglar ML
INNER JOIN dbo.Parametreler P
    ON P.grup_kod = N'ISLEM_KAYNAGI'
   AND P.kod = ML.kaynak
   AND P.aktif_mi = 1
WHERE ML.kaynak IS NOT NULL
  AND ML.kaynak_parametre_id IS NULL;

UPDATE ML
SET ML.onceki_durum_parametre_id = P.id
FROM dbo.MakinelerLoglar ML
INNER JOIN dbo.Parametreler P
    ON P.grup_kod = N'MAKINE_DURUMU'
   AND P.kod = ML.onceki_durum
   AND P.aktif_mi = 1
WHERE ML.onceki_durum IS NOT NULL
  AND ML.onceki_durum_parametre_id IS NULL;

UPDATE ML
SET ML.yeni_durum_parametre_id = P.id
FROM dbo.MakinelerLoglar ML
INNER JOIN dbo.Parametreler P
    ON P.grup_kod = N'MAKINE_DURUMU'
   AND P.kod = ML.yeni_durum
   AND P.aktif_mi = 1
WHERE ML.yeni_durum IS NOT NULL
  AND ML.yeni_durum_parametre_id IS NULL;

UPDATE ML
SET ML.islem_tur_parametre_id = P.id
FROM dbo.MakinelerLoglar ML
INNER JOIN dbo.Parametreler P
    ON P.grup_kod = N'ISLEM_TURU'
   AND P.kod = ML.islem_tur
   AND P.aktif_mi = 1
WHERE ML.islem_tur IS NOT NULL
  AND ML.islem_tur_parametre_id IS NULL;

UPDATE ML
SET ML.islem_sonuc_parametre_id = P.id
FROM dbo.MakinelerLoglar ML
INNER JOIN dbo.Parametreler P
    ON P.grup_kod = N'ISLEM_SONUCU'
   AND P.kod = ML.islem_sonuc
   AND P.aktif_mi = 1
WHERE ML.islem_sonuc IS NOT NULL
  AND ML.islem_sonuc_parametre_id IS NULL;
GO

/* Eslesmeyen kayit raporu */
SELECT N'Makineler.durum' AS alan, COUNT(*) AS eslesmeyen
FROM dbo.Makineler
WHERE durum IS NOT NULL AND durum_parametre_id IS NULL
UNION ALL
SELECT N'MakinelerLoglar.kaynak', COUNT(*)
FROM dbo.MakinelerLoglar
WHERE kaynak IS NOT NULL AND kaynak_parametre_id IS NULL
UNION ALL
SELECT N'MakinelerLoglar.onceki_durum', COUNT(*)
FROM dbo.MakinelerLoglar
WHERE onceki_durum IS NOT NULL AND onceki_durum_parametre_id IS NULL
UNION ALL
SELECT N'MakinelerLoglar.yeni_durum', COUNT(*)
FROM dbo.MakinelerLoglar
WHERE yeni_durum IS NOT NULL AND yeni_durum_parametre_id IS NULL
UNION ALL
SELECT N'MakinelerLoglar.islem_tur', COUNT(*)
FROM dbo.MakinelerLoglar
WHERE islem_tur IS NOT NULL AND islem_tur_parametre_id IS NULL
UNION ALL
SELECT N'MakinelerLoglar.islem_sonuc', COUNT(*)
FROM dbo.MakinelerLoglar
WHERE islem_sonuc IS NOT NULL AND islem_sonuc_parametre_id IS NULL;
GO

/* FK'ler */
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Makineler_DurumParametre')
BEGIN
    ALTER TABLE dbo.Makineler
    ADD CONSTRAINT FK_Makineler_DurumParametre
    FOREIGN KEY (durum_parametre_id) REFERENCES dbo.Parametreler(id);
END

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MakinelerLoglar_KaynakParametre')
BEGIN
    ALTER TABLE dbo.MakinelerLoglar
    ADD CONSTRAINT FK_MakinelerLoglar_KaynakParametre
    FOREIGN KEY (kaynak_parametre_id) REFERENCES dbo.Parametreler(id);
END

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MakinelerLoglar_OncekiDurumParametre')
BEGIN
    ALTER TABLE dbo.MakinelerLoglar
    ADD CONSTRAINT FK_MakinelerLoglar_OncekiDurumParametre
    FOREIGN KEY (onceki_durum_parametre_id) REFERENCES dbo.Parametreler(id);
END

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MakinelerLoglar_YeniDurumParametre')
BEGIN
    ALTER TABLE dbo.MakinelerLoglar
    ADD CONSTRAINT FK_MakinelerLoglar_YeniDurumParametre
    FOREIGN KEY (yeni_durum_parametre_id) REFERENCES dbo.Parametreler(id);
END

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MakinelerLoglar_IslemTurParametre')
BEGIN
    ALTER TABLE dbo.MakinelerLoglar
    ADD CONSTRAINT FK_MakinelerLoglar_IslemTurParametre
    FOREIGN KEY (islem_tur_parametre_id) REFERENCES dbo.Parametreler(id);
END

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MakinelerLoglar_IslemSonucParametre')
BEGIN
    ALTER TABLE dbo.MakinelerLoglar
    ADD CONSTRAINT FK_MakinelerLoglar_IslemSonucParametre
    FOREIGN KEY (islem_sonuc_parametre_id) REFERENCES dbo.Parametreler(id);
END
GO

/* Parametreler SP'leri */
IF OBJECT_ID(N'dbo.SP_Parametreler_EKLE', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_Parametreler_EKLE;
GO
CREATE PROCEDURE dbo.SP_Parametreler_EKLE
    @grup_kod NVARCHAR(50),
    @kod NVARCHAR(50),
    @ad NVARCHAR(100),
    @aciklama NVARCHAR(250) = NULL,
    @sira_no INT = NULL,
    @ekleyen_id INT,
    @ekleyen_ip NVARCHAR(20)
AS
BEGIN
    INSERT INTO dbo.Parametreler
    (
        grup_kod,
        kod,
        ad,
        aciklama,
        sira_no,
        aktif_mi,
        ekleyen_id,
        ekleyen_ip,
        eklenme_tarih
    )
    VALUES
    (
        @grup_kod,
        @kod,
        @ad,
        @aciklama,
        @sira_no,
        1,
        @ekleyen_id,
        @ekleyen_ip,
        GETDATE()
    );
END
GO

IF OBJECT_ID(N'dbo.SP_Parametreler_GUNCELLE', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_Parametreler_GUNCELLE;
GO
CREATE PROCEDURE dbo.SP_Parametreler_GUNCELLE
    @id INT,
    @grup_kod NVARCHAR(50),
    @kod NVARCHAR(50),
    @ad NVARCHAR(100),
    @aciklama NVARCHAR(250) = NULL,
    @sira_no INT = NULL,
    @guncelleyen_id INT,
    @guncelleyen_ip NVARCHAR(20)
AS
BEGIN
    UPDATE dbo.Parametreler
    SET
        grup_kod = @grup_kod,
        kod = @kod,
        ad = @ad,
        aciklama = @aciklama,
        sira_no = @sira_no,
        guncelleyen_id = @guncelleyen_id,
        guncelleyen_ip = @guncelleyen_ip,
        guncellenme_tarih = GETDATE()
    WHERE id = @id
      AND aktif_mi = 1;
END
GO

IF OBJECT_ID(N'dbo.SP_Parametreler_SIL', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_Parametreler_SIL;
GO
CREATE PROCEDURE dbo.SP_Parametreler_SIL
    @id INT,
    @guncelleyen_id INT,
    @guncelleyen_ip NVARCHAR(20)
AS
BEGIN
    UPDATE dbo.Parametreler
    SET
        aktif_mi = 0,
        guncelleyen_id = @guncelleyen_id,
        guncelleyen_ip = @guncelleyen_ip,
        guncellenme_tarih = GETDATE()
    WHERE id = @id;
END
GO

IF OBJECT_ID(N'dbo.SP_Parametreler_DOLDUR', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_Parametreler_DOLDUR;
GO
CREATE PROCEDURE dbo.SP_Parametreler_DOLDUR
    @id INT
AS
BEGIN
    SELECT *
    FROM dbo.Parametreler
    WHERE id = @id
      AND aktif_mi = 1;
END
GO

IF OBJECT_ID(N'dbo.SP_Parametreler_TUMUNU_GETIR', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_Parametreler_TUMUNU_GETIR;
GO
CREATE PROCEDURE dbo.SP_Parametreler_TUMUNU_GETIR
AS
BEGIN
    SELECT *
    FROM dbo.Parametreler
    WHERE aktif_mi = 1
    ORDER BY grup_kod, sira_no, id;
END
GO

IF OBJECT_ID(N'dbo.SP_Parametreler_GRUBA_GORE_GETIR', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_Parametreler_GRUBA_GORE_GETIR;
GO
CREATE PROCEDURE dbo.SP_Parametreler_GRUBA_GORE_GETIR
    @grup_kod NVARCHAR(50)
AS
BEGIN
    SELECT *
    FROM dbo.Parametreler
    WHERE grup_kod = @grup_kod
      AND aktif_mi = 1
    ORDER BY sira_no, id;
END
GO

IF OBJECT_ID(N'dbo.SP_Parametreler_KODDAN_GETIR', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_Parametreler_KODDAN_GETIR;
GO
CREATE PROCEDURE dbo.SP_Parametreler_KODDAN_GETIR
    @grup_kod NVARCHAR(50),
    @kod NVARCHAR(50)
AS
BEGIN
    SELECT TOP 1 *
    FROM dbo.Parametreler
    WHERE grup_kod = @grup_kod
      AND kod = @kod
      AND aktif_mi = 1;
END
GO

/* Makineler / MakinelerLoglar SP guncellemeleri */
IF OBJECT_ID(N'dbo.SP_Makineler_DURUM_GUNCELLE', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_Makineler_DURUM_GUNCELLE;
GO
CREATE PROCEDURE dbo.SP_Makineler_DURUM_GUNCELLE
    @id INT,
    @durum NVARCHAR(20),
    @durum_parametre_id INT = NULL,
    @guncelleyen_id INT,
    @guncelleyen_ip NVARCHAR(50)
AS
BEGIN
    UPDATE dbo.Makineler
    SET
        durum = @durum,
        durum_parametre_id = @durum_parametre_id,
        son_durum_tarih = GETDATE(),
        guncelleyen_id = NULLIF(@guncelleyen_id, 0),
        guncelleyen_ip = @guncelleyen_ip,
        guncellenme_tarih = GETDATE()
    WHERE id = @id
      AND aktif_mi = 1;
END
GO

IF OBJECT_ID(N'dbo.SP_Makineler_TUMUNU_GETIR', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_Makineler_TUMUNU_GETIR;
GO
CREATE PROCEDURE dbo.SP_Makineler_TUMUNU_GETIR
AS
BEGIN
    SELECT
        M.*,
        P.kod AS durum_kod,
        P.ad AS durum_ad
    FROM dbo.Makineler M
    LEFT JOIN dbo.Parametreler P
        ON M.durum_parametre_id = P.id
    WHERE M.aktif_mi = 1;
END
GO

IF OBJECT_ID(N'dbo.SP_MakinelerLoglar_EKLE', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_MakinelerLoglar_EKLE;
GO
CREATE PROCEDURE dbo.SP_MakinelerLoglar_EKLE
    @makineler_id INT,
    @kaynak NVARCHAR(20),
    @onceki_durum NVARCHAR(20) = NULL,
    @yeni_durum NVARCHAR(20),
    @islem_tur NVARCHAR(30),
    @islem_sonuc NVARCHAR(20),
    @detay NVARCHAR(MAX) = NULL,
    @kaynak_parametre_id INT = NULL,
    @onceki_durum_parametre_id INT = NULL,
    @yeni_durum_parametre_id INT = NULL,
    @islem_tur_parametre_id INT = NULL,
    @islem_sonuc_parametre_id INT = NULL,
    @aktif_mi BIT,
    @ekleyen_id INT = NULL,
    @ekleyen_ip NVARCHAR(20) = NULL
AS
BEGIN
    INSERT INTO dbo.MakinelerLoglar
    (
        makineler_id,
        kaynak,
        onceki_durum,
        yeni_durum,
        islem_tur,
        islem_sonuc,
        detay,
        kaynak_parametre_id,
        onceki_durum_parametre_id,
        yeni_durum_parametre_id,
        islem_tur_parametre_id,
        islem_sonuc_parametre_id,
        aktif_mi,
        eklenme_tarih,
        ekleyen_id,
        ekleyen_ip
    )
    VALUES
    (
        @makineler_id,
        @kaynak,
        @onceki_durum,
        @yeni_durum,
        @islem_tur,
        @islem_sonuc,
        @detay,
        @kaynak_parametre_id,
        @onceki_durum_parametre_id,
        @yeni_durum_parametre_id,
        @islem_tur_parametre_id,
        @islem_sonuc_parametre_id,
        @aktif_mi,
        GETDATE(),
        NULLIF(@ekleyen_id, 0),
        @ekleyen_ip
    );
END
GO

IF OBJECT_ID(N'dbo.SP_MakinelerLoglar_SON_KAYIT_GETIR', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_MakinelerLoglar_SON_KAYIT_GETIR;
GO
CREATE PROCEDURE dbo.SP_MakinelerLoglar_SON_KAYIT_GETIR
    @makineler_id INT
AS
BEGIN
    SELECT TOP 1
        ML.*,
        PK.ad AS kaynak_ad,
        PK.kod AS kaynak_kod,
        POD.ad AS onceki_durum_ad,
        POD.kod AS onceki_durum_kod,
        PYD.ad AS yeni_durum_ad,
        PYD.kod AS yeni_durum_kod,
        PIT.ad AS islem_tur_ad,
        PIT.kod AS islem_tur_kod,
        PIS.ad AS islem_sonuc_ad,
        PIS.kod AS islem_sonuc_kod
    FROM dbo.MakinelerLoglar ML
    LEFT JOIN dbo.Parametreler PK
        ON ML.kaynak_parametre_id = PK.id
    LEFT JOIN dbo.Parametreler POD
        ON ML.onceki_durum_parametre_id = POD.id
    LEFT JOIN dbo.Parametreler PYD
        ON ML.yeni_durum_parametre_id = PYD.id
    LEFT JOIN dbo.Parametreler PIT
        ON ML.islem_tur_parametre_id = PIT.id
    LEFT JOIN dbo.Parametreler PIS
        ON ML.islem_sonuc_parametre_id = PIS.id
    WHERE ML.makineler_id = @makineler_id
      AND ML.aktif_mi = 1
    ORDER BY ML.id DESC;
END
GO

IF OBJECT_ID(N'dbo.SP_MakinelerLoglar_TUMUNU_GETIR', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_MakinelerLoglar_TUMUNU_GETIR;
GO
CREATE PROCEDURE dbo.SP_MakinelerLoglar_TUMUNU_GETIR
AS
BEGIN
    SELECT
        ML.*,
        PK.ad AS kaynak_ad,
        PK.kod AS kaynak_kod,
        POD.ad AS onceki_durum_ad,
        POD.kod AS onceki_durum_kod,
        PYD.ad AS yeni_durum_ad,
        PYD.kod AS yeni_durum_kod,
        PIT.ad AS islem_tur_ad,
        PIT.kod AS islem_tur_kod,
        PIS.ad AS islem_sonuc_ad,
        PIS.kod AS islem_sonuc_kod
    FROM dbo.MakinelerLoglar ML
    LEFT JOIN dbo.Parametreler PK
        ON ML.kaynak_parametre_id = PK.id
    LEFT JOIN dbo.Parametreler POD
        ON ML.onceki_durum_parametre_id = POD.id
    LEFT JOIN dbo.Parametreler PYD
        ON ML.yeni_durum_parametre_id = PYD.id
    LEFT JOIN dbo.Parametreler PIT
        ON ML.islem_tur_parametre_id = PIT.id
    LEFT JOIN dbo.Parametreler PIS
        ON ML.islem_sonuc_parametre_id = PIS.id
    WHERE ML.aktif_mi = 1
    ORDER BY ML.id DESC;
END
GO
