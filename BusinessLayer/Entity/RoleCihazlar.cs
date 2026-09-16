using System;
using System.Data;
using System.Web.UI.WebControls;

public class RoleCihazlar : OrtakAlanlar, IOrtakMetotlar
{
    public RoleCihazlar(VeritabaniIslemleri _veritabaniIslemleri)
    {
        VeritabaniIslem = _veritabaniIslemleri;
    }

    ~RoleCihazlar()
    {
        VeriTablosu = new DataTable();
        VeriTablosu = null;
    }

    #region Sabitler

    public const string C_Tablo = "dbo.RoleCihazlar";

    public const string C_Sp_TumunuGetir = "dbo.SP_RoleCihazlar_TUMUNU_GETIR";
    public const string C_Sp_Ekle = "dbo.SP_RoleCihazlar_EKLE";
    public const string C_Sp_Sil = "dbo.SP_RoleCihazlar_SIL";
    public const string C_Sp_Guncelle = "dbo.SP_RoleCihazlar_GUNCELLE";
    public const string C_Sp_Doldur = "dbo.SP_RoleCihazlar_DOLDUR";

    public const string C_Sutun_ad = "ad";
    public const string C_Sutun_ip = "ip";
    public const string C_Sutun_port = "port";
    public const string C_Sutun_kanal_sayisi = "kanal_sayisi";

    #endregion

    #region Nesneler

    private string ad;
    public string Ad
    {
        get { return ad; }
        set { ad = value; }
    }

    private string ip;
    public string Ip
    {
        get { return ip; }
        set { ip = value; }
    }

    private int? port;
    public int? Port
    {
        get { return port; }
        set { port = value; }
    }

    private int? kanalSayisi;
    public int? KanalSayisi
    {
        get { return kanalSayisi; }
        set { kanalSayisi = value; }
    }

    #endregion

    #region Metotlar

    public bool Ekle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Ekle;
        VeritabaniIslem.ParametreEkle(C_Sutun_ad, Ad);
        VeritabaniIslem.ParametreEkle(C_Sutun_ip, Ip);
        VeritabaniIslem.ParametreEkle(C_Sutun_port, Port);
        VeritabaniIslem.ParametreEkle(C_Sutun_kanal_sayisi, KanalSayisi);
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
        VeritabaniIslem.ParametreEkle(C_Sutun_ip, Ip);
        VeritabaniIslem.ParametreEkle(C_Sutun_port, Port);
        VeritabaniIslem.ParametreEkle(C_Sutun_kanal_sayisi, KanalSayisi);
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
            throw new Exception("Role cihaz kayıtları getirilemedi.");
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
        Ad = VeriSatiri[C_Sutun_ad] == DBNull.Value ? string.Empty : VeriSatiri[C_Sutun_ad].ToString();
        Ip = VeriSatiri[C_Sutun_ip] == DBNull.Value ? string.Empty : VeriSatiri[C_Sutun_ip].ToString();

        if (VeriSatiri[C_Sutun_port] == DBNull.Value)
        {
            Port = null;
        }
        else
        {
            Port = Convert.ToInt32(VeriSatiri[C_Sutun_port]);
        }

        if (VeriSatiri[C_Sutun_kanal_sayisi] == DBNull.Value)
        {
            KanalSayisi = null;
        }
        else
        {
            KanalSayisi = Convert.ToInt32(VeriSatiri[C_Sutun_kanal_sayisi]);
        }

        return true;
    }

    #endregion
}
