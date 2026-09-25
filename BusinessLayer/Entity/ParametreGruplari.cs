using System;
using System.Data;
using System.Web.UI.WebControls;

public class ParametreGruplari : OrtakAlanlar, IOrtakMetotlar
{
    public ParametreGruplari(VeritabaniIslemleri _veritabaniIslemleri)
    {
        VeritabaniIslem = _veritabaniIslemleri;
    }

    ~ParametreGruplari()
    {
        VeriTablosu = new DataTable();
        VeriTablosu = null;
    }

    #region Sabitler

    public const string C_Tablo = "dbo.ParametreGruplari";

    public const string C_Sp_TumunuGetir = "dbo.SP_ParametreGruplari_TUMUNU_GETIR";
    public const string C_Sp_Ekle = "dbo.SP_ParametreGruplari_EKLE";
    public const string C_Sp_Sil = "dbo.SP_ParametreGruplari_SIL";
    public const string C_Sp_Guncelle = "dbo.SP_ParametreGruplari_GUNCELLE";
    public const string C_Sp_Doldur = "dbo.SP_ParametreGruplari_DOLDUR";

    public const string C_Sutun_kod = "kod";
    public const string C_Sutun_ad = "ad";
    public const string C_Sutun_aciklama = "aciklama";
    public const string C_Sutun_sistem_mi = "sistem_mi";

    #endregion

    #region Nesneler

    private string kod;
    public string Kod
    {
        get { return kod; }
        set { kod = value; }
    }

    private string ad;
    public string Ad
    {
        get { return ad; }
        set { ad = value; }
    }

    private string aciklama;
    public string Aciklama
    {
        get { return aciklama; }
        set { aciklama = value; }
    }

    private bool sistemMi;
    public bool SistemMi
    {
        get { return sistemMi; }
        set { sistemMi = value; }
    }

    #endregion

    #region Metotlar

    public bool Ekle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Ekle;
        VeritabaniIslem.ParametreEkle(C_Sutun_kod, Kod);
        VeritabaniIslem.ParametreEkle(C_Sutun_ad, Ad);
        VeritabaniIslem.ParametreEkle(C_Sutun_aciklama, Aciklama);
        VeritabaniIslem.ParametreEkle(C_Sutun_sistem_mi, SistemMi);
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
        VeritabaniIslem.ParametreEkle(C_Sutun_kod, Kod);
        VeritabaniIslem.ParametreEkle(C_Sutun_ad, Ad);
        VeritabaniIslem.ParametreEkle(C_Sutun_aciklama, Aciklama);
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
            throw new Exception("Parametre grupları getirilemedi.");
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
        {
            return false;
        }

        Id = Convert.ToInt32(VeriSatiri[C_Sutun_id]);
        Kod = VeriSatiri[C_Sutun_kod] == DBNull.Value ? string.Empty : VeriSatiri[C_Sutun_kod].ToString();
        Ad = VeriSatiri[C_Sutun_ad] == DBNull.Value ? string.Empty : VeriSatiri[C_Sutun_ad].ToString();
        Aciklama = VeriSatiri[C_Sutun_aciklama] == DBNull.Value ? string.Empty : VeriSatiri[C_Sutun_aciklama].ToString();
        SistemMi = VeriSatiri[C_Sutun_sistem_mi] == DBNull.Value ? false : Convert.ToBoolean(VeriSatiri[C_Sutun_sistem_mi]);
        AktifMi = VeriSatiri[C_Sutun_aktif_mi] == DBNull.Value ? false : Convert.ToBoolean(VeriSatiri[C_Sutun_aktif_mi]);
        return true;
    }

    #endregion
}
