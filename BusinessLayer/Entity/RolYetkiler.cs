using System;
using System.Data;

public class RolYetkiler : OrtakAlanlar, IOrtakMetotlar
{

    #region CONSTRUCTOR

    public RolYetkiler(VeritabaniIslemleri veritabaniIslemleri)
    {
        VeritabaniIslem = veritabaniIslemleri;
    }

    ~RolYetkiler()
    {
        VeriTablosu = null;
        VeriSatiri = null;
    }

    #endregion


    #region SABİTLER

    public const string C_Tablo = "dbo.RolYetkiler";

    public const string C_Sp_Ekle = "dbo.SP_RolYetkiler_EKLE";
    public const string C_Sp_Sil = "dbo.SP_RolYetkiler_SIL";
    public const string C_Sp_Guncelle = "dbo.SP_RolYetkiler_GUNCELLE";
    public const string C_Sp_Doldur = "dbo.SP_RolYetkiler_DOLDUR";
    public const string C_Sp_TumunuGetir = "dbo.SP_RolYetkiler_TUMUNU_GETIR";

    public const string C_Sutun_roller_id = "roller_id";
    public const string C_Sutun_sayfa_adi = "sayfa_adi";
    public const string C_Sutun_getirme = "getirme";
    public const string C_Sutun_ekleme = "ekleme";
    public const string C_Sutun_silme = "silme";
    public const string C_Sutun_guncelleme = "guncelleme";

    #endregion

    #region NESNELER

    private int rollerId;
    public int RollerId
    {
        get { return rollerId; }
        set { rollerId = value; }
    }

    private string sayfaAdi;
    public string SayfaAdi
    {
        get { return sayfaAdi; }
        set { sayfaAdi = value; }
    }

    private bool getirme;
    public bool Getirme
    {
        get { return getirme; }
        set { getirme = value; }
    }

    private bool ekleme;
    public bool Ekleme
    {
        get { return ekleme; }
        set { ekleme = value; }
    }

    private bool silme;
    public bool Silme
    {
        get { return silme; }
        set { silme = value; }
    }

    private bool guncelleme;
    public bool Guncelleme
    {
        get { return guncelleme; }
        set { guncelleme = value; }
    }

    #endregion

    #region METOTLAR

    public bool Ekle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Ekle;
        VeritabaniIslem.ParametreEkle(C_Sutun_roller_id, RollerId);
        VeritabaniIslem.ParametreEkle(C_Sutun_sayfa_adi, SayfaAdi);
        VeritabaniIslem.ParametreEkle(C_Sutun_getirme, Getirme);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleme, Ekleme);
        VeritabaniIslem.ParametreEkle(C_Sutun_silme, Silme);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleme, Guncelleme);
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, AktifMi);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_id, EkleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_ip, EkleyenIp);
        return VeritabaniIslem.Calistir();
    }

    public bool Guncelle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Guncelle;
        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_roller_id, RollerId);
        VeritabaniIslem.ParametreEkle(C_Sutun_sayfa_adi, SayfaAdi);
        VeritabaniIslem.ParametreEkle(C_Sutun_getirme, Getirme);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleme, Ekleme);
        VeritabaniIslem.ParametreEkle(C_Sutun_silme, Silme);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleme, Guncelleme);
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, AktifMi);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_id, GuncelleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_ip, GuncelleyenIp);
        return VeritabaniIslem.Calistir();
    }

    public bool Sil()
    {
        VeritabaniIslem.SpAdi = C_Sp_Sil;
        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        return VeritabaniIslem.Calistir();
    }

    public DataTable TumunuGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_TumunuGetir;
        VeriTablosu = VeritabaniIslem.TabloGetir();
        return VeriTablosu;
    }

    public bool Doldur()
    {
        VeritabaniIslem.SpAdi = C_Sp_Doldur;
        VeritabaniIslem.ParametreEkle(C_Sutun_roller_id, RollerId);
        VeritabaniIslem.ParametreEkle(C_Sutun_sayfa_adi, SayfaAdi);
        VeriSatiri = VeritabaniIslem.SatirGetir();
        if (VeriSatiri == null)
            return false;
        Id = Convert.ToInt32(VeriSatiri[C_Sutun_id]);
        RollerId = Convert.ToInt32(VeriSatiri[C_Sutun_roller_id]);
        SayfaAdi = VeriSatiri[C_Sutun_sayfa_adi].ToString();
        Getirme = Convert.ToBoolean(VeriSatiri[C_Sutun_getirme]);
        Ekleme = Convert.ToBoolean(VeriSatiri[C_Sutun_ekleme]);
        Silme = Convert.ToBoolean(VeriSatiri[C_Sutun_silme]);
        Guncelleme = Convert.ToBoolean(VeriSatiri[C_Sutun_guncelleme]);
        AktifMi = Convert.ToBoolean(VeriSatiri[C_Sutun_aktif_mi]);
        return true;
    }

    #endregion
}