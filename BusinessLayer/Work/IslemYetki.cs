using System.IO;
using System.Web;

public static class IslemYetki
{
    public static bool Kontrol(YetkiIslemTurleri islemTuru)
    {
        string sayfaAdi = Path.GetFileName(HttpContext.Current.Request.Url.AbsolutePath);
        return Kontrol(sayfaAdi, islemTuru);
    }

    public static bool Kontrol(string sayfaAdi, YetkiIslemTurleri islemTuru)
    {
        Sessionlar sessionlar = new Sessionlar();
        CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;

        if (currentInfo == null || currentInfo.LoginYapildiMi == false)
            return false;

        if (string.IsNullOrWhiteSpace(sayfaAdi))
            return false;

        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
            RolYetkiler rolYetkiler = new RolYetkiler(veritabaniIslemleri);
            rolYetkiler.RollerId = currentInfo.RolId;
            rolYetkiler.SayfaAdi = sayfaAdi;
            return rolYetkiler.YetkiVarMi(islemTuru);
        }
        catch
        {
            return false;
        }
        finally
        {
            veritabaniIslemleri.Bitir();
        }
    }
}