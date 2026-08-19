using System;
using System.Data;
using System.Web.UI.WebControls;


public class Roller : OrtakAlanlar, IOrtakMetotlar
{

    private VeritabaniIslemleri veritabaniIslemleri;
    public Roller(VeritabaniIslemleri _veritabaniIslemleri)
    {
        veritabaniIslemleri = _veritabaniIslemleri;
    }


    #region Sabitler

    public const string C_Tablo = "dbo.Roller";

    public const string C_Sp_TumunuGetir = "dbo.SP_Roller_TUMUNU_GETIR";

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
        return false;
    }
    public bool Sil()
    {
        return false;
    }
    public bool Guncelle()
    {
        return false;
    }

    public DataTable TumunuGetir()
    {
        veritabaniIslemleri.SpAdi = C_Sp_TumunuGetir;
        VeriTablosu = veritabaniIslemleri.TabloGetir();
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

    #endregion

}
