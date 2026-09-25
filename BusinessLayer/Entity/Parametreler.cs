using System;
using System.Data;
using System.Web.UI.WebControls;

public class Parametreler : OrtakAlanlar, IOrtakMetotlar
{
    public Parametreler(VeritabaniIslemleri _veritabaniIslemleri)
    {
        VeritabaniIslem = _veritabaniIslemleri;
    }

    ~Parametreler()
    {
        VeriTablosu = new DataTable();
        VeriTablosu = null;
    }

    #region Sabitler

    public const string C_Tablo = "dbo.Parametreler";

    public const string C_Sp_TumunuGetir = "dbo.SP_Parametreler_TUMUNU_GETIR";
    public const string C_Sp_Ekle = "dbo.SP_Parametreler_EKLE";
    public const string C_Sp_Sil = "dbo.SP_Parametreler_SIL";
    public const string C_Sp_Guncelle = "dbo.SP_Parametreler_GUNCELLE";
    public const string C_Sp_Doldur = "dbo.SP_Parametreler_DOLDUR";
    public const string C_Sp_GrubaGoreGetir = "dbo.SP_Parametreler_GRUBA_GORE_GETIR";
    public const string C_Sp_KoddanGetir = "dbo.SP_Parametreler_KODDAN_GETIR";

    public const string C_Sutun_parametre_grup_id = "parametre_grup_id";
    public const string C_Sutun_kod = "kod";
    public const string C_Sutun_ad = "ad";
    public const string C_Sutun_aciklama = "aciklama";
    public const string C_Sutun_sistem_mi = "sistem_mi";
    public const string C_Sutun_grup_kod = "grup_kod";

    #endregion

    #region Nesneler

    private int parametreGrupId;
    public int ParametreGrupId
    {
        get { return parametreGrupId; }
        set { parametreGrupId = value; }
    }

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
        VeritabaniIslem.ParametreEkle(C_Sutun_parametre_grup_id, ParametreGrupId);
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
        VeritabaniIslem.ParametreEkle(C_Sutun_parametre_grup_id, ParametreGrupId);
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

    public DataTable GrubaGoreGetir(int parametreGrupId)
    {
        VeritabaniIslem.SpAdi = C_Sp_GrubaGoreGetir;
        VeritabaniIslem.ParametreEkle(C_Sutun_parametre_grup_id, parametreGrupId);
        VeriTablosu = VeritabaniIslem.TabloGetir();
        return VeriTablosu;
    }

    public void GrubaGoreListele(DropDownList dropDownList, int parametreGrupId)
    {
        GrubaGoreGetir(parametreGrupId);

        if (VeriTablosu == null)
        {
            throw new Exception("Parametre kayıtları getirilemedi.");
        }

        dropDownList.DataTextField = C_Sutun_ad;
        dropDownList.DataValueField = C_Sutun_id;
        dropDownList.DataSource = VeriTablosu;
        dropDownList.DataBind();
    }

    public bool KoddanGetir(string grupKod, string kod)
    {
        VeritabaniIslem.SpAdi = C_Sp_KoddanGetir;
        VeritabaniIslem.ParametreEkle(C_Sutun_grup_kod, grupKod);
        VeritabaniIslem.ParametreEkle(C_Sutun_kod, kod);
        VeriSatiri = VeritabaniIslem.SatirGetir();

        if (VeriSatiri == null)
        {
            return false;
        }

        return SatirdanDoldur();
    }

    public int? IdGetir(string grupKod, string kod)
    {
        if (!KoddanGetir(grupKod, kod))
        {
            return null;
        }

        return Id;
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

        return SatirdanDoldur();
    }

    public bool KodKullanimdaMi(int parametreGrupId, string kod, int? haricId)
    {
        DataTable parametreler = GrubaGoreGetir(parametreGrupId);
        if (parametreler == null)
        {
            return false;
        }

        foreach (DataRow satir in parametreler.Rows)
        {
            if (satir[C_Sutun_kod] == DBNull.Value)
            {
                continue;
            }

            if (!string.Equals(satir[C_Sutun_kod].ToString(), kod, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            int id = Convert.ToInt32(satir[C_Sutun_id]);
            if (haricId.HasValue && haricId.Value == id)
            {
                continue;
            }

            return true;
        }

        return false;
    }

    private bool SatirdanDoldur()
    {
        Id = Convert.ToInt32(VeriSatiri[C_Sutun_id]);
        ParametreGrupId = Convert.ToInt32(VeriSatiri[C_Sutun_parametre_grup_id]);
        Kod = VeriSatiri[C_Sutun_kod] == DBNull.Value ? string.Empty : VeriSatiri[C_Sutun_kod].ToString();
        Ad = VeriSatiri[C_Sutun_ad] == DBNull.Value ? string.Empty : VeriSatiri[C_Sutun_ad].ToString();
        Aciklama = VeriSatiri[C_Sutun_aciklama] == DBNull.Value ? string.Empty : VeriSatiri[C_Sutun_aciklama].ToString();
        SistemMi = VeriSatiri[C_Sutun_sistem_mi] == DBNull.Value ? false : Convert.ToBoolean(VeriSatiri[C_Sutun_sistem_mi]);
        AktifMi = VeriSatiri[C_Sutun_aktif_mi] == DBNull.Value ? false : Convert.ToBoolean(VeriSatiri[C_Sutun_aktif_mi]);
        return true;
    }

    #endregion
}
