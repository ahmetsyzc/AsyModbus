using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Loglar : OrtakAlanlar, IOrtakMetotlar
{
    public Loglar(VeritabaniIslemleri _veritabaniIslemleri)
    {
        VeritabaniIslem = _veritabaniIslemleri;
    }

    ~Loglar()
    {
        VeriTablosu = new DataTable();
        VeriTablosu = null;
    }

    #region Sabitler

    public const string C_Tablo = "dbo.Loglar";

    public const string C_Sp_Ekle = "dbo.SP_Loglar_EKLE";
    public const string C_Sp_Sil = "dbo.SP_Loglar_SIL";
    public const string C_Sp_Guncelle = "dbo.SP_Loglar_GUNCELLE";
    public const string C_Sp_TumunuGetir = "dbo.SP_Loglar_TUMUNU_GETIR";
    public const string C_Sp_Doldur = "dbo.SP_Loglar_DOLDUR";//Tek Kayıt Getirir


    public const string C_Sutun_url = "url";
    public const string C_Sutun_tablo_ad = "tablo_ad";
    public const string C_Sutun_islem_ad = "islem_ad";
    public const string C_Sutun_islem_tip = "islem_tip";
    public const string C_Sutun_detay = "detay";

    #endregion

    #region Nesneler

    private string url;
    public string Url
    {
        get { return url; }
        set { url = value; }
    }

    private string tabloAd;
    public string TabloAd
    {
        get { return tabloAd; }
        set { tabloAd = value; }
    }

    private string islemAd;
    public string IslemAd
    {
        get { return islemAd; }
        set { islemAd = value; }
    }

    private string islemTip;
    public string IslemTip
    {
        get { return islemTip; }
        set { islemTip = value; }
    }

    private string detay;
    public string Detay
    {
        get { return detay; }
        set { detay = value; }
    }

    #endregion

    #region Metotlar

    public bool Ekle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Ekle;
        VeritabaniIslem.ParametreEkle(C_Sutun_url, Url);
        VeritabaniIslem.ParametreEkle(C_Sutun_tablo_ad, TabloAd);
        VeritabaniIslem.ParametreEkle(C_Sutun_islem_ad, IslemAd);
        VeritabaniIslem.ParametreEkle(C_Sutun_islem_tip, IslemTip);
        VeritabaniIslem.ParametreEkle(C_Sutun_detay, Detay);
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, AktifMi);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_id, EkleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_ip, EkleyenIp);
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
        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);

        VeriSatiri = VeritabaniIslem.SatirGetir();

        if (VeriSatiri != null)
        {
            Id = Convert.ToInt32(VeriSatiri[C_Sutun_id]);
            Url = VeriSatiri[C_Sutun_url].ToString();
            TabloAd = VeriSatiri[C_Sutun_tablo_ad].ToString();
            IslemAd = VeriSatiri[C_Sutun_islem_ad].ToString();
            IslemTip = VeriSatiri[C_Sutun_islem_tip].ToString();
            Detay = VeriSatiri[C_Sutun_detay].ToString();
            AktifMi = Convert.ToBoolean(VeriSatiri[C_Sutun_aktif_mi]);
            EkleyenId = Convert.ToInt32(VeriSatiri[C_Sutun_ekleyen_id]);
            EkleyenIp = VeriSatiri[C_Sutun_ekleyen_ip].ToString();
            EklenmeTarih = Convert.ToDateTime(VeriSatiri[C_Sutun_eklenme_tarih]);

            return true;
        }

        return false;
    }

    public bool Guncelle()
    {
        return false;
    }

    public bool Sil()
    {
        return false;
    }
    
    #endregion
}
