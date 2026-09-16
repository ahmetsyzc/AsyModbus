public static class Sayfalar
{
    public const string Default ="Default.aspx";
    public const string KullaniciListele ="KullaniciListele.aspx";
    public const string KullaniciEkle ="KullaniciEkle.aspx";
    public const string KullaniciDuzenle ="KullaniciDuzenle.aspx";
    public const string MakineListele ="MakineListele.aspx";
    public const string MakineEkle ="MakineEkle.aspx";
    public const string MakineDuzenle ="MakineDuzenle.aspx";
    public const string LogListele ="LogListele.aspx";
    public const string LogDetay ="LogDetay.aspx";
    public const string RolIslemleri ="RolIslemleri.aspx";
    public const string RolYetkileri ="RolYetkileri.aspx";
    public const string RoleCihazIslemleri ="RoleCihazIslemleri.aspx";
    public const string ProfilDuzenle ="ProfilDuzenle.aspx";
    public const string Login ="Login.aspx";
    public const string SifreSifirlama ="SifreSifirlama.aspx";

    public static string[] YetkilendirilenSayfalar
    {
        get
        {
            return new string[]
            {
            KullaniciListele,
            KullaniciEkle,
            KullaniciDuzenle,
            MakineListele,
            MakineEkle,
            MakineDuzenle,
            LogListele,
            LogDetay,
            RolIslemleri,
            RolYetkileri,
            RoleCihazIslemleri
            };
        }
    }
}

