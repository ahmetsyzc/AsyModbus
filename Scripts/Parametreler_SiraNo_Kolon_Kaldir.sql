/* ParametreGruplari / Parametreler sira_no kolonunu kaldir */
SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.SP_ParametreGruplari_EKLE', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_ParametreGruplari_EKLE;
GO
CREATE PROCEDURE dbo.SP_ParametreGruplari_EKLE
    @kod NVARCHAR(50),
    @ad NVARCHAR(100),
    @aciklama NVARCHAR(250) = NULL,
    @sistem_mi BIT,
    @ekleyen_id INT,
    @ekleyen_ip NVARCHAR(20)
AS
BEGIN
    INSERT INTO dbo.ParametreGruplari
    (kod, ad, aciklama, sistem_mi, aktif_mi, ekleyen_id, ekleyen_ip, eklenme_tarih)
    VALUES
    (@kod, @ad, @aciklama, @sistem_mi, 1, @ekleyen_id, @ekleyen_ip, GETDATE());
END
GO

IF OBJECT_ID(N'dbo.SP_ParametreGruplari_GUNCELLE', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_ParametreGruplari_GUNCELLE;
GO
CREATE PROCEDURE dbo.SP_ParametreGruplari_GUNCELLE
    @id INT,
    @kod NVARCHAR(50),
    @ad NVARCHAR(100),
    @aciklama NVARCHAR(250) = NULL,
    @guncelleyen_id INT,
    @guncelleyen_ip NVARCHAR(20)
AS
BEGIN
    UPDATE dbo.ParametreGruplari
    SET
        kod = @kod,
        ad = @ad,
        aciklama = @aciklama,
        guncelleyen_id = @guncelleyen_id,
        guncelleyen_ip = @guncelleyen_ip,
        guncellenme_tarih = GETDATE()
    WHERE id = @id
      AND aktif_mi = 1;
END
GO

IF OBJECT_ID(N'dbo.SP_Parametreler_EKLE', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_Parametreler_EKLE;
GO
CREATE PROCEDURE dbo.SP_Parametreler_EKLE
    @parametre_grup_id INT,
    @kod NVARCHAR(50),
    @ad NVARCHAR(100),
    @aciklama NVARCHAR(250) = NULL,
    @sistem_mi BIT,
    @ekleyen_id INT,
    @ekleyen_ip NVARCHAR(20)
AS
BEGIN
    INSERT INTO dbo.Parametreler
    (parametre_grup_id, kod, ad, aciklama, sistem_mi, aktif_mi, ekleyen_id, ekleyen_ip, eklenme_tarih)
    VALUES
    (@parametre_grup_id, @kod, @ad, @aciklama, @sistem_mi, 1, @ekleyen_id, @ekleyen_ip, GETDATE());
END
GO

IF OBJECT_ID(N'dbo.SP_Parametreler_GUNCELLE', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_Parametreler_GUNCELLE;
GO
CREATE PROCEDURE dbo.SP_Parametreler_GUNCELLE
    @id INT,
    @parametre_grup_id INT,
    @kod NVARCHAR(50),
    @ad NVARCHAR(100),
    @aciklama NVARCHAR(250) = NULL,
    @guncelleyen_id INT,
    @guncelleyen_ip NVARCHAR(20)
AS
BEGIN
    UPDATE dbo.Parametreler
    SET
        parametre_grup_id = @parametre_grup_id,
        kod = @kod,
        ad = @ad,
        aciklama = @aciklama,
        guncelleyen_id = @guncelleyen_id,
        guncelleyen_ip = @guncelleyen_ip,
        guncellenme_tarih = GETDATE()
    WHERE id = @id
      AND aktif_mi = 1;
END
GO

IF COL_LENGTH('dbo.ParametreGruplari', 'sira_no') IS NOT NULL
BEGIN
    ALTER TABLE dbo.ParametreGruplari DROP COLUMN sira_no;
END
GO

IF COL_LENGTH('dbo.Parametreler', 'sira_no') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Parametreler DROP COLUMN sira_no;
END
GO

SELECT TABLE_NAME, COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME IN ('ParametreGruplari', 'Parametreler')
  AND COLUMN_NAME = 'sira_no';
GO
