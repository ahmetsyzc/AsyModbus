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

    public DataTable TumunuGetir()
    {
        throw new NotImplementedException();
    }

    public bool Doldur()
    {
        return false;
    }

    #endregion
}
