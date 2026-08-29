using System;
using System.Data;
using System.Web.UI.WebControls;


public class Roller : OrtakAlanlar, IOrtakMetotlar
{
    public Roller(VeritabaniIslemleri _veritabaniIslemleri)
    {
        VeritabaniIslem = _veritabaniIslemleri;
    }

    ~Roller()
    {
        VeriTablosu = new DataTable();
        VeriTablosu = null;
    }

    #region Sabitler

    public const string C_Tablo = "dbo.Roller";

    public const string C_Sp_TumunuGetir = "dbo.SP_Roller_TUMUNU_GETIR";
    public const string C_Sp_Ekle = "dbo.SP_Roller_EKLE";
    public const string C_Sp_Sil = "dbo.SP_Roller_SIL";
    public const string C_Sp_Guncelle = "dbo.SP_Roller_GUNCELLE";
    public const string C_Sp_Doldur = "dbo.SP_Roller_DOLDUR";
    public const string C_Sp_MaxIdGetir = "dbo.SP_Roller_MAX_ID_GETIR";

    public const string C_Sutun_ad = "ad";

    #endregion


    #region Nesneler

    private string ad;
    public string Ad
    {
        get { return ad; }
        set { ad = value; }
    }

    #endregion


    #region Metotlar

    public bool Ekle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Ekle;
        VeritabaniIslem.ParametreEkle(C_Sutun_ad, Ad);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_id, EkleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_ip, EkleyenIp);
        return VeritabaniIslem.Calistir();
    }
    public bool Sil()
    {
        VeritabaniIslem.SpAdi = C_Sp_Sil;
        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_id, GuncelleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_ip, GuncelleyenIp);
        return VeritabaniIslem.Calistir();
    }
    public bool Guncelle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Guncelle;
        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_ad, Ad);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_id, GuncelleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_ip, GuncelleyenIp);
        return VeritabaniIslem.Calistir();
    }

    public DataTable TumunuGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_TumunuGetir;
        VeriTablosu = VeritabaniIslem.TabloGetir();
        return VeriTablosu;
    }

    public void Listele(DropDownList dropDownList)
    {
        TumunuGetir();

        if (VeriTablosu == null)
        {
            throw new Exception("Rol kayıtları getirilemedi.");
        }

        dropDownList.DataTextField = C_Sutun_ad;
        dropDownList.DataValueField = C_Sutun_id;
        dropDownList.DataSource = VeriTablosu;
        dropDownList.DataBind();
    }

    public bool Doldur()
    {
        VeritabaniIslem.SpAdi = C_Sp_Doldur;
        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeriSatiri = VeritabaniIslem.SatirGetir();
        if (VeriSatiri == null)
            return false;
        Id = Convert.ToInt32(VeriSatiri[C_Sutun_id]);
        Ad = VeriSatiri[C_Sutun_ad].ToString();
        return true;
    }

    public int MaxIdGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_MaxIdGetir;
        return VeritabaniIslem.DegerGetir();
    }

    #endregion

}
