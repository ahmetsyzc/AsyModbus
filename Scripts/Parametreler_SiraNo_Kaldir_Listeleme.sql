/* Parametre listelerinde sira_no yerine id ile siralama */
IF OBJECT_ID(N'dbo.SP_ParametreGruplari_TUMUNU_GETIR', N'P') IS NOT NULL DROP PROCEDURE dbo.SP_ParametreGruplari_TUMUNU_GETIR;
GO
CREATE PROCEDURE dbo.SP_ParametreGruplari_TUMUNU_GETIR
AS
BEGIN
    SELECT *
    FROM dbo.ParametreGruplari
    WHERE aktif_mi = 1
    ORDER BY id;
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
    ORDER BY parametre_grup_id, id;
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
    ORDER BY id;
END
GO
