/* Bu calismada eklenen sayisal deger genislemesini geri alir.
   Mevcut parametre mimarisi: grup + kod + ad + aciklama.
   Kontrol suresi mevcut Ad alaninda tutulur; diger parametreler degistirilmez.
*/
:ON ERROR EXIT
SET XACT_ABORT ON;
BEGIN TRANSACTION;
GO
CREATE OR ALTER PROCEDURE dbo.SP_Parametreler_EKLE
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
END;
GO
CREATE OR ALTER PROCEDURE dbo.SP_Parametreler_GUNCELLE
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
    SET parametre_grup_id = @parametre_grup_id, kod = @kod, ad = @ad,
        aciklama = @aciklama, guncelleyen_id = @guncelleyen_id,
        guncelleyen_ip = @guncelleyen_ip, guncellenme_tarih = GETDATE()
    WHERE id = @id AND aktif_mi = 1;
END;
GO
IF OBJECT_ID(N'dbo.SP_Parametreler_SAYISAL_DEGER_GUNCELLE', N'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_Parametreler_SAYISAL_DEGER_GUNCELLE;

IF COL_LENGTH(N'dbo.Parametreler', N'sayisal_deger') IS NOT NULL
BEGIN
    /* Baska kayitlara sonradan deger girilmisse sessizce kaybetme. */
    EXEC sys.sp_executesql N'
        IF EXISTS (SELECT 1 FROM dbo.Parametreler P
                   JOIN dbo.ParametreGruplari G ON G.id=P.parametre_grup_id
                   WHERE P.sayisal_deger IS NOT NULL
                     AND NOT (G.kod=N''SERVIS_AYARLARI'' AND P.kod=N''KOMUT_KONTROL_SURESI_SN''))
            THROW 51203, N''Baska parametrelerde deger var; kolon kaldirilmadan once kontrol edin.'', 1;
        UPDATE P SET ad=CONVERT(NVARCHAR(100),P.sayisal_deger),
                     aciklama=N''Ad alaninda pozitif tam sayi olarak saniye cinsinden kontrol suresi''
        FROM dbo.Parametreler P JOIN dbo.ParametreGruplari G ON G.id=P.parametre_grup_id
        WHERE G.kod=N''SERVIS_AYARLARI'' AND P.kod=N''KOMUT_KONTROL_SURESI_SN''
          AND P.sayisal_deger IS NOT NULL;';
    ALTER TABLE dbo.Parametreler DROP COLUMN sayisal_deger;
END;
COMMIT TRANSACTION;
PRINT N'Mevcut parametre mimarisine donuldu.';
GO
