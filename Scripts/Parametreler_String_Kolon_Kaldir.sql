/* Eski string kolonlarin kaldirilmasi */
SET NOCOUNT ON;
SET XACT_ABORT ON;

/* Once SP'leri string kolon kullanmayacak sekilde guncelle */
IF OBJECT_ID(N'dbo.SP_Makineler_DURUM_GUNCELLE', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_Makineler_DURUM_GUNCELLE;
GO
CREATE PROCEDURE dbo.SP_Makineler_DURUM_GUNCELLE
    @id INT,
    @durum_parametre_id INT,
    @guncelleyen_id INT,
    @guncelleyen_ip NVARCHAR(50)
AS
BEGIN
    UPDATE dbo.Makineler
    SET
        durum_parametre_id = @durum_parametre_id,
        son_durum_tarih = GETDATE(),
        guncelleyen_id = NULLIF(@guncelleyen_id, 0),
        guncelleyen_ip = @guncelleyen_ip,
        guncellenme_tarih = GETDATE()
    WHERE id = @id
      AND aktif_mi = 1;
END
GO

IF OBJECT_ID(N'dbo.SP_MakinelerLoglar_EKLE', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_MakinelerLoglar_EKLE;
GO
CREATE PROCEDURE dbo.SP_MakinelerLoglar_EKLE
    @makineler_id INT,
    @detay NVARCHAR(MAX) = NULL,
    @kaynak_parametre_id INT,
    @onceki_durum_parametre_id INT = NULL,
    @yeni_durum_parametre_id INT,
    @islem_tur_parametre_id INT,
    @islem_sonuc_parametre_id INT,
    @aktif_mi BIT,
    @ekleyen_id INT = NULL,
    @ekleyen_ip NVARCHAR(20) = NULL
AS
BEGIN
    INSERT INTO dbo.MakinelerLoglar
    (
        makineler_id,
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

/* Kolonlari dusur */
IF COL_LENGTH('dbo.Makineler', 'durum') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Makineler DROP COLUMN durum;
END
GO

IF COL_LENGTH('dbo.MakinelerLoglar', 'kaynak') IS NOT NULL
BEGIN
    ALTER TABLE dbo.MakinelerLoglar DROP COLUMN kaynak;
END
GO

IF COL_LENGTH('dbo.MakinelerLoglar', 'onceki_durum') IS NOT NULL
BEGIN
    ALTER TABLE dbo.MakinelerLoglar DROP COLUMN onceki_durum;
END
GO

IF COL_LENGTH('dbo.MakinelerLoglar', 'yeni_durum') IS NOT NULL
BEGIN
    ALTER TABLE dbo.MakinelerLoglar DROP COLUMN yeni_durum;
END
GO

IF COL_LENGTH('dbo.MakinelerLoglar', 'islem_tur') IS NOT NULL
BEGIN
    ALTER TABLE dbo.MakinelerLoglar DROP COLUMN islem_tur;
END
GO

IF COL_LENGTH('dbo.MakinelerLoglar', 'islem_sonuc') IS NOT NULL
BEGIN
    ALTER TABLE dbo.MakinelerLoglar DROP COLUMN islem_sonuc;
END
GO

SELECT TABLE_NAME, COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME IN ('Makineler', 'MakinelerLoglar')
  AND COLUMN_NAME IN ('durum', 'kaynak', 'onceki_durum', 'yeni_durum', 'islem_tur', 'islem_sonuc');
GO
