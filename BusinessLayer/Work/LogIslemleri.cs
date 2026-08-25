using System;
using System.Web;

public enum LogIslemTipleri
{
    Insert,
    Update,
    Delete,
    Select,
    Login,
    Exit
}

public static class LogIslemleri
{
    public static bool IslemKaydet(string tabloAd, string islemAd, LogIslemTipleri islemTip, string detay)
    {
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
        veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
        try
        {
            int ekleyenId = 0;
            string ekleyenIp = string.Empty;
            string url = string.Empty;

            if (HttpContext.Current != null)
            {
                url = HttpContext.Current.Request.Url.ToString();
                ekleyenIp = HttpContext.Current.Request.UserHostAddress;
            }

            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;

            if (currentInfo != null && currentInfo.LoginYapildiMi)
            {
                ekleyenId = currentInfo.KullaniciId;
                if (!string.IsNullOrEmpty(currentInfo.Ip))
                {
                    ekleyenIp = currentInfo.Ip;
                }
            }
            Loglar loglar = new Loglar(veritabaniIslemleri);
            loglar.Url = url;
            loglar.TabloAd = tabloAd;
            loglar.IslemAd = islemAd;
            loglar.IslemTip = islemTip.ToString();
            loglar.Detay = detay;
            loglar.AktifMi = true;
            loglar.EkleyenId = ekleyenId;
            loglar.EkleyenIp = ekleyenIp;
            return loglar.Ekle();
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