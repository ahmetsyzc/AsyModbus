/* ASAMA 3: Once MakineKomutlar_Migrasyon.sql ve MakineKomutlar_HazirIstek_Migrasyon.sql uygulanmalidir.
   sqlcmd veya SSMS SQLCMD Mode ile calistirilir.
   Hata durumunda baglanti kapanir ve acik DDL transaction geri alinir.
   EKLE / GUNCELLE row count dondurur: VeritabaniIslemleri.Calistir ile uyumludur.
   SIRADAKI_GETIR ve ISLEME_GUNCELLE ayni transaction icinde sahiplenir. Fiziksel role islemi DB transaction disinda yapilir.
*/
:ON ERROR EXIT
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.MakineKomutlar', N'U') IS NULL
    THROW 51100, N'Once MakineKomutlar migration dosyasini uygulayin.', 1;
GO

CREATE OR ALTER PROCEDURE dbo.SP_MakineKomutlar_EKLE
    @makineler_id INT,
    @islem_tur_parametre_id INT,
    @tetikleme_url NVARCHAR(2048),
    @kapatma_url NVARCHAR(2048),
    @hedef_rolecihaz_id INT,
    @hedef_ip NVARCHAR(45),
    @hedef_port INT,
    @tetikleme_suresi_ms INT,
    @detay NVARCHAR(MAX) = NULL,
    @ekleyen_id INT = NULL,
    @ekleyen_ip NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @kendi_transaction BIT = CASE WHEN @@TRANCOUNT = 0 THEN 1 ELSE 0 END;
    DECLARE @bekliyor INT, @isleniyor INT;
    BEGIN TRY
        IF @kendi_transaction = 1 BEGIN TRANSACTION;

        /* Ayni makinenin talepleri, arada aktif komut olmasa da siraya girer.
           Makine satiri kilidi kontrol + INSERT yarisini engeller. */
        IF NOT EXISTS (SELECT 1 FROM dbo.Makineler WITH (UPDLOCK, HOLDLOCK)
                       WHERE id = @makineler_id AND aktif_mi = 1)
            THROW 51101, N'Aktif makine bulunamadi.', 1;

        IF NOT EXISTS (SELECT 1 FROM dbo.Parametreler P
                       JOIN dbo.ParametreGruplari G ON G.id = P.parametre_grup_id
                       WHERE P.id = @islem_tur_parametre_id AND P.kod = N'DURDURMA'
                         AND G.kod = N'ISLEM_TURU' AND P.aktif_mi = 1 AND G.aktif_mi = 1)
            THROW 51102, N'Gecerli DURDURMA islem turu bulunamadi.', 1;

        SELECT @bekliyor = P.id FROM dbo.Parametreler P
        JOIN dbo.ParametreGruplari G ON G.id = P.parametre_grup_id
        WHERE G.kod = N'MAKINE_KOMUT_DURUM' AND P.kod = N'BEKLIYOR'
          AND P.aktif_mi = 1 AND G.aktif_mi = 1;
        SELECT @isleniyor = P.id FROM dbo.Parametreler P
        JOIN dbo.ParametreGruplari G ON G.id = P.parametre_grup_id
        WHERE G.kod = N'MAKINE_KOMUT_DURUM' AND P.kod = N'ISLENIYOR'
          AND P.aktif_mi = 1 AND G.aktif_mi = 1;
        IF @bekliyor IS NULL OR @isleniyor IS NULL
            THROW 51103, N'Komut durum parametreleri bulunamadi.', 1;

        IF EXISTS (SELECT 1 FROM dbo.MakineKomutlar
                   WHERE makineler_id = @makineler_id AND islem_tur_parametre_id = @islem_tur_parametre_id
                     AND aktif_mi = 1 AND durum_parametre_id IN (@bekliyor, @isleniyor))
            THROW 51104, N'Makine icin bekleyen veya islenen bir durdurma talebi zaten var.', 1;

        IF NULLIF(@tetikleme_url,N'') IS NULL OR NULLIF(@kapatma_url,N'') IS NULL
           OR @tetikleme_suresi_ms IS NULL OR @tetikleme_suresi_ms NOT BETWEEN 1 AND 10000
           OR @hedef_port IS NULL OR @hedef_port NOT BETWEEN 1 AND 65535
           OR NOT EXISTS (SELECT 1 FROM dbo.RoleCihazlar R WITH (HOLDLOCK)
              JOIN dbo.Makineler M WITH (HOLDLOCK) ON M.rolecihazlar_id=R.id
              WHERE M.id=@makineler_id AND R.id=@hedef_rolecihaz_id AND R.aktif_mi=1
                AND R.ip=@hedef_ip AND R.port=@hedef_port)
            THROW 51109, N'Hazir komut veya kayitli hedef bilgisi gecersiz.', 1;
        SET NOCOUNT OFF;
        INSERT INTO dbo.MakineKomutlar
            (makineler_id, islem_tur_parametre_id, durum_parametre_id, deneme_sayisi,
             tetikleme_url, kapatma_url, hedef_rolecihaz_id, hedef_ip, hedef_port,
             tetikleme_suresi_ms, detay, aktif_mi, ekleyen_id, ekleyen_ip, eklenme_tarih)
        VALUES (@makineler_id, @islem_tur_parametre_id, @bekliyor, 0,
                @tetikleme_url, @kapatma_url, @hedef_rolecihaz_id, @hedef_ip, @hedef_port,
                @tetikleme_suresi_ms, @detay, 1, @ekleyen_id, @ekleyen_ip, GETDATE());
        IF @kendi_transaction = 1 COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @kendi_transaction = 1 AND XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_MakineKomutlar_DOLDUR
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.MakineKomutlar WHERE id = @id AND aktif_mi = 1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_MakineKomutlar_TUMUNU_GETIR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT K.*, M.makine_ad, M.makine_no, T.kod AS islem_tur_kod,
           T.ad AS islem_tur_ad, D.kod AS durum_kod, D.ad AS durum_ad
    FROM dbo.MakineKomutlar K
    JOIN dbo.Makineler M ON M.id = K.makineler_id
    JOIN dbo.Parametreler T ON T.id = K.islem_tur_parametre_id
    JOIN dbo.Parametreler D ON D.id = K.durum_parametre_id
    WHERE K.aktif_mi = 1
    ORDER BY K.eklenme_tarih DESC, K.id DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_MakineKomutlar_BEKLEYEN_GETIR
AS
BEGIN
    SET NOCOUNT ON;
    /* Gozlem icindir. Komut uygulamadan once mutlaka ISLEME_AL cagrilir. */
    SELECT K.* FROM dbo.MakineKomutlar K
    JOIN dbo.Parametreler P ON P.id = K.durum_parametre_id
    JOIN dbo.ParametreGruplari G ON G.id = P.parametre_grup_id
    WHERE K.aktif_mi = 1 AND G.kod = N'MAKINE_KOMUT_DURUM' AND P.kod = N'BEKLIYOR'
      AND P.aktif_mi = 1 AND G.aktif_mi = 1
    ORDER BY K.eklenme_tarih, K.id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_MakineKomutlar_AKTIFLERI_GETIR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT K.makineler_id, D.kod AS durum_kod
    FROM dbo.MakineKomutlar K
    JOIN dbo.Parametreler D ON D.id=K.durum_parametre_id
    JOIN dbo.ParametreGruplari G ON G.id=D.parametre_grup_id
    JOIN dbo.Parametreler T ON T.id=K.islem_tur_parametre_id
    JOIN dbo.ParametreGruplari TG ON TG.id=T.parametre_grup_id
    WHERE K.aktif_mi=1 AND G.kod=N'MAKINE_KOMUT_DURUM'
      AND D.kod IN (N'BEKLIYOR',N'ISLENIYOR')
      AND TG.kod=N'ISLEM_TURU' AND T.kod=N'DURDURMA'
    ORDER BY K.id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_MakineKomutlar_AKTIF_KOMUT_VAR_MI
    @makineler_id INT,
    @islem_tur_parametre_id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CASE WHEN EXISTS
        (SELECT 1 FROM dbo.MakineKomutlar K
         JOIN dbo.Parametreler P ON P.id = K.durum_parametre_id
         JOIN dbo.ParametreGruplari G ON G.id = P.parametre_grup_id
         WHERE K.makineler_id = @makineler_id AND K.islem_tur_parametre_id = @islem_tur_parametre_id
           AND K.aktif_mi = 1 AND G.kod = N'MAKINE_KOMUT_DURUM'
           AND P.kod IN (N'BEKLIYOR', N'ISLENIYOR')) THEN 1 ELSE 0 END;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_MakineKomutlar_SIRADAKI_GETIR
AS
BEGIN
    SET NOCOUNT ON;
    IF @@TRANCOUNT = 0
        THROW 51107, N'Siradaki komut BAGIMLI transaction icinde okunmalidir.', 1;
    SELECT TOP (1) K.*
    FROM dbo.MakineKomutlar K WITH (UPDLOCK, HOLDLOCK, ROWLOCK)
    JOIN dbo.Parametreler P ON P.id=K.durum_parametre_id
    JOIN dbo.ParametreGruplari G ON G.id=P.parametre_grup_id
    WHERE K.aktif_mi=1 AND P.aktif_mi=1 AND G.aktif_mi=1
      AND G.kod=N'MAKINE_KOMUT_DURUM' AND P.kod=N'BEKLIYOR'
    ORDER BY K.eklenme_tarih, K.id;
END;
GO

-- Eski Form surumleri yeni komutlari IP/kanal hesaplayarak calistiramaz.
CREATE OR ALTER PROCEDURE dbo.SP_MakineKomutlar_ISLEME_GUNCELLE
    @id INT,
    @guncelleyen_id INT = NULL,
    @guncelleyen_ip NVARCHAR(20) = NULL
AS
BEGIN
    THROW 51110, N'Eski komut tuketicisi desteklenmiyor. Hazir istek destekli Form surumunu kullanin.', 1;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_MakineKomutlar_GUNCELLE
    @id INT,
    @durum_parametre_id INT,
    @detay NVARCHAR(MAX) = NULL,
    @guncelleyen_id INT = NULL,
    @guncelleyen_ip NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF NOT EXISTS (SELECT 1 FROM dbo.Parametreler P
                   JOIN dbo.ParametreGruplari G ON G.id = P.parametre_grup_id
                   WHERE P.id = @durum_parametre_id AND G.kod = N'MAKINE_KOMUT_DURUM'
                     AND P.kod IN (N'BASARILI', N'BASARISIZ') AND P.aktif_mi = 1 AND G.aktif_mi = 1)
        THROW 51105, N'Sonuc yalnizca komut grubundaki BASARILI veya BASARISIZ olabilir.', 1;

    SET NOCOUNT OFF;
    UPDATE K SET durum_parametre_id = @durum_parametre_id, detay = @detay,
                 guncelleyen_id = @guncelleyen_id, guncelleyen_ip = @guncelleyen_ip,
                 guncellenme_tarih = GETDATE()
    FROM dbo.MakineKomutlar K
    JOIN dbo.Parametreler P ON P.id = K.durum_parametre_id
    JOIN dbo.ParametreGruplari G ON G.id = P.parametre_grup_id
    WHERE K.id = @id AND K.aktif_mi = 1 AND G.kod = N'MAKINE_KOMUT_DURUM'
      AND P.kod = N'ISLENIYOR';
    IF @@ROWCOUNT <> 1
        THROW 51106, N'Komut bulunamadi veya ISLENIYOR durumunda degil.', 1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SP_MakineKomutlar_SAHIPLEN
    @guncelleyen_ip NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF @@TRANCOUNT=0 THROW 51107, N'Sahiplenme BAGIMLI transaction gerektirir.', 1;
    DECLARE @bekliyor INT, @isleniyor INT;
    SELECT @bekliyor=P.id FROM dbo.Parametreler P
      JOIN dbo.ParametreGruplari G ON G.id=P.parametre_grup_id
      WHERE G.kod=N'MAKINE_KOMUT_DURUM' AND P.kod=N'BEKLIYOR' AND P.aktif_mi=1 AND G.aktif_mi=1;
    SELECT @isleniyor=P.id FROM dbo.Parametreler P
      JOIN dbo.ParametreGruplari G ON G.id=P.parametre_grup_id
      WHERE G.kod=N'MAKINE_KOMUT_DURUM' AND P.kod=N'ISLENIYOR' AND P.aktif_mi=1 AND G.aktif_mi=1;
    IF @bekliyor IS NULL OR @isleniyor IS NULL THROW 51103, N'Komut durum parametreleri bulunamadi.', 1;
    ;WITH Siradaki AS (
      SELECT TOP (1) * FROM dbo.MakineKomutlar WITH (UPDLOCK,HOLDLOCK,ROWLOCK)
      WHERE aktif_mi=1 AND durum_parametre_id=@bekliyor ORDER BY eklenme_tarih,id
    )
    UPDATE Siradaki SET durum_parametre_id=@isleniyor, deneme_sayisi=deneme_sayisi+1,
      guncellenme_tarih=GETDATE(), guncelleyen_ip=@guncelleyen_ip
    OUTPUT inserted.*;
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_MakineKomutlar_SON_DURUMLARI_GETIR
AS
BEGIN
    SET NOCOUNT ON;
    SELECT M.id AS makineler_id, M.rolecihazlar_id,
        MD.kod AS makine_durum_kod, MD.ad AS makine_durum_ad,
        K.id AS komut_id, D.kod AS durum_kod,
        CAST(CASE WHEN D.kod=N'ISLENIYOR'
          AND (K.guncellenme_tarih IS NULL OR K.guncellenme_tarih < DATEADD(MINUTE,-2,GETDATE()))
          THEN 1 ELSE 0 END AS BIT) AS belirsiz
    FROM dbo.Makineler M
    LEFT JOIN dbo.Parametreler MD ON MD.id=M.durum_parametre_id
    OUTER APPLY (SELECT TOP (1) C.* FROM dbo.MakineKomutlar C
        WHERE C.makineler_id=M.id AND C.aktif_mi=1 ORDER BY C.id DESC) K
    LEFT JOIN dbo.Parametreler D ON D.id=K.durum_parametre_id
    WHERE M.aktif_mi=1;
END;
GO
IF OBJECT_ID(N'dbo.SP_MakineKomutlar_ISLEME_AL', N'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_MakineKomutlar_ISLEME_AL;
IF OBJECT_ID(N'dbo.SP_MakineKomutlar_SONUCLANDIR', N'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_MakineKomutlar_SONUCLANDIR;
GO
COMMIT TRANSACTION;
PRINT N'MakineKomutlar stored procedure asamasi tamamlandi.';
GO
