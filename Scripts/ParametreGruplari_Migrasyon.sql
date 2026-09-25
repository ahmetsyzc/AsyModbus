/* ParametreGruplari + Parametreler.parametre_grup_id gecisi */
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID(N'dbo.ParametreGruplari', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ParametreGruplari
    (
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        kod NVARCHAR(50) NOT NULL,
        ad NVARCHAR(100) NOT NULL,
        aciklama NVARCHAR(250) NULL,
        sira_no INT NULL,
        sistem_mi BIT NOT NULL CONSTRAINT DF_ParametreGruplari_sistem_mi DEFAULT (0),
        aktif_mi BIT NOT NULL CONSTRAINT DF_ParametreGruplari_aktif_mi DEFAULT (1),
        ekleyen_id INT NULL,
        ekleyen_ip NVARCHAR(20) NULL,
        eklenme_tarih DATETIME NOT NULL CONSTRAINT DF_ParametreGruplari_eklenme_tarih DEFAULT (GETDATE()),
        guncelleyen_id INT NULL,
        guncelleyen_ip NVARCHAR(20) NULL,
        guncellenme_tarih DATETIME NULL,
        CONSTRAINT UQ_ParametreGruplari_Kod UNIQUE (kod)
    );
END
GO

;WITH GrupSeed AS
(
    SELECT * FROM (VALUES
        (N'MAKINE_DURUMU', N'Makine Durumu', N'Makine calisma durumlari', 1),
        (N'ISLEM_SONUCU', N'Islem Sonucu', N'Islem sonuclari', 2),
        (N'ISLEM_KAYNAGI', N'Islem Kaynagi', N'Islemin kaynagi', 3),
        (N'ISLEM_TURU', N'Islem Turu', N'Makine islem turleri', 4)
    ) AS V(kod, ad, aciklama, sira_no)
)
INSERT INTO dbo.ParametreGruplari (kod, ad, aciklama, sira_no, sistem_mi, aktif_mi, ekleyen_id, ekleyen_ip)
SELECT S.kod, S.ad, S.aciklama, S.sira_no, 1, 1, 0, N'SYSTEM'
FROM GrupSeed S
WHERE NOT EXISTS (SELECT 1 FROM dbo.ParametreGruplari G WHERE G.kod = S.kod);

/* Mevcut grup_kod degerlerinden eksik gruplari ekle */
INSERT INTO dbo.ParametreGruplari (kod, ad, aciklama, sira_no, sistem_mi, aktif_mi, ekleyen_id, ekleyen_ip)
SELECT DISTINCT P.grup_kod, P.grup_kod, NULL, 99, 1, 1, 0, N'SYSTEM'
FROM dbo.Parametreler P
WHERE P.grup_kod IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM dbo.ParametreGruplari G WHERE G.kod = P.grup_kod);
GO

/* Turkce grup adlari */
UPDATE dbo.ParametreGruplari SET ad = N'Makine Durumu', aciklama = N'Makine ' + NCHAR(231) + N'al' + NCHAR(305) + NCHAR(351) + N'ma durumlar' + NCHAR(305) WHERE kod = N'MAKINE_DURUMU';
UPDATE dbo.ParametreGruplari SET ad = NCHAR(304) + N'slem Sonucu', aciklama = NCHAR(304) + N'slem sonu' + NCHAR(231) + N'lar' + NCHAR(305) WHERE kod = N'ISLEM_SONUCU';
UPDATE dbo.ParametreGruplari SET ad = NCHAR(304) + N'slem Kayna' + NCHAR(287) + NCHAR(305), aciklama = NCHAR(304) + N'slemin kayna' + NCHAR(287) + NCHAR(305) WHERE kod = N'ISLEM_KAYNAGI';
UPDATE dbo.ParametreGruplari SET ad = NCHAR(304) + N'slem T' + NCHAR(252) + N'r' + NCHAR(252), aciklama = N'Makine i' + NCHAR(351) + N'lem t' + NCHAR(252) + N'rleri' WHERE kod = N'ISLEM_TURU';
GO

IF COL_LENGTH('dbo.Parametreler', 'parametre_grup_id') IS NULL
BEGIN
    ALTER TABLE dbo.Parametreler ADD parametre_grup_id INT NULL;
END
GO

IF COL_LENGTH('dbo.Parametreler', 'sistem_mi') IS NULL
BEGIN
    ALTER TABLE dbo.Parametreler ADD sistem_mi BIT NOT NULL CONSTRAINT DF_Parametreler_sistem_mi DEFAULT (0);
END
GO

UPDATE P
SET P.parametre_grup_id = G.id
FROM dbo.Parametreler P
INNER JOIN dbo.ParametreGruplari G ON G.kod = P.grup_kod
WHERE P.parametre_grup_id IS NULL;

UPDATE dbo.Parametreler SET sistem_mi = 1 WHERE sistem_mi = 0;
GO

SELECT N'Parametreler.eslesmeyen' AS rapor, COUNT(*) AS adet
FROM dbo.Parametreler
WHERE parametre_grup_id IS NULL;
GO

IF EXISTS (SELECT 1 FROM dbo.Parametreler WHERE parametre_grup_id IS NULL)
BEGIN
    RAISERROR(N'Parametreler migration: eslesmeyen kayit var.', 16, 1);
    RETURN;
END
GO

/* Unique constraint eski */
IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = N'UQ_Parametreler_GrupKod_Kod' AND parent_object_id = OBJECT_ID(N'dbo.Parametreler'))
BEGIN
    ALTER TABLE dbo.Parametreler DROP CONSTRAINT UQ_Parametreler_GrupKod_Kod;
END
GO

ALTER TABLE dbo.Parametreler ALTER COLUMN parametre_grup_id INT NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Parametreler_ParametreGruplari')
BEGIN
    ALTER TABLE dbo.Parametreler
    ADD CONSTRAINT FK_Parametreler_ParametreGruplari
    FOREIGN KEY (parametre_grup_id) REFERENCES dbo.ParametreGruplari(id);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = N'UQ_Parametreler_Grup_Kod' AND parent_object_id = OBJECT_ID(N'dbo.Parametreler'))
BEGIN
    ALTER TABLE dbo.Parametreler
    ADD CONSTRAINT UQ_Parametreler_Grup_Kod UNIQUE (parametre_grup_id, kod);
END
GO

IF COL_LENGTH('dbo.Parametreler', 'grup_kod') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Parametreler DROP COLUMN grup_kod;
END
GO

/* ========== ParametreGruplari SP ========== */
IF OBJECT_ID(N'dbo.SP_ParametreGruplari_EKLE', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_ParametreGruplari_EKLE;
GO
CREATE PROCEDURE dbo.SP_ParametreGruplari_EKLE
    @kod NVARCHAR(50),
    @ad NVARCHAR(100),
    @aciklama NVARCHAR(250) = NULL,
    @sira_no INT = NULL,
    @sistem_mi BIT,
    @ekleyen_id INT,
    @ekleyen_ip NVARCHAR(20)
AS
BEGIN
    INSERT INTO dbo.ParametreGruplari
    (kod, ad, aciklama, sira_no, sistem_mi, aktif_mi, ekleyen_id, ekleyen_ip, eklenme_tarih)
    VALUES
    (@kod, @ad, @aciklama, @sira_no, @sistem_mi, 1, @ekleyen_id, @ekleyen_ip, GETDATE());
END
GO

IF OBJECT_ID(N'dbo.SP_ParametreGruplari_GUNCELLE', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_ParametreGruplari_GUNCELLE;
GO
CREATE PROCEDURE dbo.SP_ParametreGruplari_GUNCELLE
    @id INT,
    @kod NVARCHAR(50),
    @ad NVARCHAR(100),
    @aciklama NVARCHAR(250) = NULL,
    @sira_no INT = NULL,
    @guncelleyen_id INT,
    @guncelleyen_ip NVARCHAR(20)
AS
BEGIN
    UPDATE dbo.ParametreGruplari
    SET
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

IF OBJECT_ID(N'dbo.SP_ParametreGruplari_SIL', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_ParametreGruplari_SIL;
GO
CREATE PROCEDURE dbo.SP_ParametreGruplari_SIL
    @id INT,
    @guncelleyen_id INT,
    @guncelleyen_ip NVARCHAR(20)
AS
BEGIN
    UPDATE dbo.ParametreGruplari
    SET
        aktif_mi = 0,
        guncelleyen_id = @guncelleyen_id,
        guncelleyen_ip = @guncelleyen_ip,
        guncellenme_tarih = GETDATE()
    WHERE id = @id
      AND sistem_mi = 0;
END
GO

IF OBJECT_ID(N'dbo.SP_ParametreGruplari_DOLDUR', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_ParametreGruplari_DOLDUR;
GO
CREATE PROCEDURE dbo.SP_ParametreGruplari_DOLDUR
    @id INT
AS
BEGIN
    SELECT *
    FROM dbo.ParametreGruplari
    WHERE id = @id
      AND aktif_mi = 1;
END
GO

IF OBJECT_ID(N'dbo.SP_ParametreGruplari_TUMUNU_GETIR', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_ParametreGruplari_TUMUNU_GETIR;
GO
CREATE PROCEDURE dbo.SP_ParametreGruplari_TUMUNU_GETIR
AS
BEGIN
    SELECT *
    FROM dbo.ParametreGruplari
    WHERE aktif_mi = 1
    ORDER BY sira_no, id;
END
GO

/* ========== Parametreler SP ========== */
IF OBJECT_ID(N'dbo.SP_Parametreler_EKLE', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_Parametreler_EKLE;
GO
CREATE PROCEDURE dbo.SP_Parametreler_EKLE
    @parametre_grup_id INT,
    @kod NVARCHAR(50),
    @ad NVARCHAR(100),
    @aciklama NVARCHAR(250) = NULL,
    @sira_no INT = NULL,
    @sistem_mi BIT,
    @ekleyen_id INT,
    @ekleyen_ip NVARCHAR(20)
AS
BEGIN
    INSERT INTO dbo.Parametreler
    (parametre_grup_id, kod, ad, aciklama, sira_no, sistem_mi, aktif_mi, ekleyen_id, ekleyen_ip, eklenme_tarih)
    VALUES
    (@parametre_grup_id, @kod, @ad, @aciklama, @sira_no, @sistem_mi, 1, @ekleyen_id, @ekleyen_ip, GETDATE());
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
    @sira_no INT = NULL,
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
    WHERE id = @id
      AND sistem_mi = 0;
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
    ORDER BY parametre_grup_id, sira_no, id;
END
GO

IF OBJECT_ID(N'dbo.SP_Parametreler_GRUBA_GORE_GETIR', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_Parametreler_GRUBA_GORE_GETIR;
GO
CREATE PROCEDURE dbo.SP_Parametreler_GRUBA_GORE_GETIR
    @parametre_grup_id INT
AS
BEGIN
    SELECT *
    FROM dbo.Parametreler
    WHERE parametre_grup_id = @parametre_grup_id
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
    SELECT TOP 1 P.*
    FROM dbo.Parametreler P
    INNER JOIN dbo.ParametreGruplari G
        ON P.parametre_grup_id = G.id
    WHERE G.kod = @grup_kod
      AND P.kod = @kod
      AND G.aktif_mi = 1
      AND P.aktif_mi = 1;
END
GO
