using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Makineler : OrtakAlanlar, IOrtakMetotlar
{
    public Makineler(VeritabaniIslemleri _veritabaniIslemleri)
    {
        VeritabaniIslem = _veritabaniIslemleri;
    }

    ~Makineler()
    {
        VeriTablosu = new DataTable();
        VeriTablosu = null;
    }

    #region Sabitler

    public const string C_Tablo = "dbo.Makineler";

    public const string C_Sp_Ekle = "dbo.SP_Makineler_EKLE";
    public const string C_Sp_Sil = "dbo.SP_Makineler_SIL";
    public const string C_Sp_Guncelle = "dbo.SP_Makineler_GUNCELLE";
    public const string C_Sp_TumunuGetir = "dbo.SP_Makineler_TUMUNU_GETIR";
    public const string C_Sp_Doldur = "dbo.SP_Makineler_DOLDUR";//Tek Kayıt Getirir


    public const string C_Sutun_model_ad = "model_ad";
    public const string C_Sutun_entegrasyon_kod = "entegrasyon_kod";
    public const string C_Sutun_gg_no = "gg_no";
    public const string C_Sutun_makine_no = "makine_no";
    public const string C_Sutun_band_no = "band_no";
    public const string C_Sutun_ip = "ip";
    public const string C_Sutun_mfg = "mfg";

    #endregion

    #region Nesneler

    private string modelAd;
    public string ModelAd
    {
        get { return modelAd; }
        set { modelAd = value; }
    }

    private string entegrasyonKod;
    public string EntegrasyonKod
    {
        get { return entegrasyonKod; }
        set { entegrasyonKod = value; }
    }

    private string ggNo;
    public string GgNo
    {
        get { return ggNo; }
        set { ggNo = value; }
    }

    private string makineNo;
    public string MakineNo
    {
        get { return makineNo; }
        set { makineNo = value; }
    }

    private string bandNo;
    public string BandNo
    {
        get { return bandNo; }
        set { bandNo = value; }
    }

    private string ip;
    public string Ip
    {
        get { return ip; }
        set { ip = value; }
    }

    private string mfg;
    public string Mfg
    {
        get { return mfg; }
        set { mfg = value; }
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

