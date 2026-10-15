/* Once guncel MakineKomutlar_StoredProcedure.sql uygulanir. Mevcut audit alanlari korunur. */
SET XACT_ABORT ON;
IF COL_LENGTH(N'dbo.MakineKomutlar',N'talep_eden_ad_soyad') IS NOT NULL
    ALTER TABLE dbo.MakineKomutlar DROP COLUMN talep_eden_ad_soyad;
IF COL_LENGTH(N'dbo.MakineKomutlar',N'baslama_tarih') IS NOT NULL
    ALTER TABLE dbo.MakineKomutlar DROP COLUMN baslama_tarih;
IF COL_LENGTH(N'dbo.MakineKomutlar',N'bitis_tarih') IS NOT NULL
    ALTER TABLE dbo.MakineKomutlar DROP COLUMN bitis_tarih;
