using System;
using System.Data;

public class MakinelerLoglar : OrtakAlanlar, IOrtakMetotlar
{
    public MakinelerLoglar(VeritabaniIslemleri _veritabaniIslemleri)
    {
        VeritabaniIslem = _veritabaniIslemleri;
    }

    ~MakinelerLoglar()
    {
        VeriTablosu = new DataTable();
        VeriTablosu = null;
    }

    #region Sabitler

    public const string C_Tablo = "dbo.MakinelerLoglar";

    public const string C_Sp_Ekle = "dbo.SP_MakinelerLoglar_EKLE";
    public const string C_Sp_Sil = "dbo.SP_MakinelerLoglar_SIL";
    public const string C_Sp_TumunuGetir = "dbo.SP_MakinelerLoglar_TUMUNU_GETIR";
    public const string C_Sp_Doldur = "dbo.SP_MakinelerLoglar_DOLDUR";
    public const string C_Sp_SonKayitGetir = "dbo.SP_MakinelerLoglar_SON_KAYIT_GETIR";

    public const string C_Sutun_makineler_id = "makineler_id";
    public const string C_Sutun_kaynak = "kaynak";
    public const string C_Sutun_onceki_durum = "onceki_durum";
    public const string C_Sutun_yeni_durum = "yeni_durum";
    public const string C_Sutun_islem_tur = "islem_tur";
    public const string C_Sutun_islem_sonuc = "islem_sonuc";
    public const string C_Sutun_detay = "detay";

    #endregion

    #region Nesneler

    private int makinelerId;
    public int MakinelerId
    {
        get { return makinelerId; }
        set { makinelerId = value; }
    }

    private string kaynak;
    public string Kaynak
    {
        get { return kaynak; }
        set { kaynak = value; }
    }

    private string oncekiDurum;
    public string OncekiDurum
    {
        get { return oncekiDurum; }
        set { oncekiDurum = value; }
    }

    private string yeniDurum;
    public string YeniDurum
    {
        get { return yeniDurum; }
        set { yeniDurum = value; }
    }

    private string islemTur;
    public string IslemTur
    {
        get { return islemTur; }
        set { islemTur = value; }
    }

    private string islemSonuc;
    public string IslemSonuc
    {
        get { return islemSonuc; }
        set { islemSonuc = value; }
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
        VeritabaniIslem.ParametreEkle(C_Sutun_makineler_id, MakinelerId);
        VeritabaniIslem.ParametreEkle(C_Sutun_kaynak, Kaynak);
        VeritabaniIslem.ParametreEkle(C_Sutun_onceki_durum, OncekiDurum);
        VeritabaniIslem.ParametreEkle(C_Sutun_yeni_durum, YeniDurum);
        VeritabaniIslem.ParametreEkle(C_Sutun_islem_tur, IslemTur);
        VeritabaniIslem.ParametreEkle(C_Sutun_islem_sonuc, IslemSonuc);
        VeritabaniIslem.ParametreEkle(C_Sutun_detay, Detay);
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, AktifMi);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_id, EkleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_ip, EkleyenIp);
        return VeritabaniIslem.Calistir();
    }

    public bool Guncelle()
    {
        return false;
    }

    public bool Sil()
    {
        VeritabaniIslem.SpAdi = C_Sp_Sil;
        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
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

        MakinelerId = Convert.ToInt32(VeriSatiri[C_Sutun_makineler_id]);
        Kaynak = VeriSatiri[C_Sutun_kaynak] == DBNull.Value ? "" : VeriSatiri[C_Sutun_kaynak].ToString();
        OncekiDurum = VeriSatiri[C_Sutun_onceki_durum] == DBNull.Value ? "" : VeriSatiri[C_Sutun_onceki_durum].ToString();
        YeniDurum = VeriSatiri[C_Sutun_yeni_durum] == DBNull.Value ? "" : VeriSatiri[C_Sutun_yeni_durum].ToString();
        IslemTur = VeriSatiri[C_Sutun_islem_tur] == DBNull.Value ? "" : VeriSatiri[C_Sutun_islem_tur].ToString();
        IslemSonuc = VeriSatiri[C_Sutun_islem_sonuc] == DBNull.Value ? "" : VeriSatiri[C_Sutun_islem_sonuc].ToString();
        Detay = VeriSatiri[C_Sutun_detay] == DBNull.Value ? "" : VeriSatiri[C_Sutun_detay].ToString();
        AktifMi = VeriSatiri[C_Sutun_aktif_mi] == DBNull.Value ? false : Convert.ToBoolean(VeriSatiri[C_Sutun_aktif_mi]);

        if (VeriSatiri[C_Sutun_ekleyen_id] == DBNull.Value)
        {
            EkleyenId = 0;
        }
        else
        {
            EkleyenId = Convert.ToInt32(VeriSatiri[C_Sutun_ekleyen_id]);
        }

        EkleyenIp = VeriSatiri[C_Sutun_ekleyen_ip] == DBNull.Value ? "" : VeriSatiri[C_Sutun_ekleyen_ip].ToString();

        if (VeriSatiri[C_Sutun_eklenme_tarih] == DBNull.Value)
        {
            EklenmeTarih = DateTime.MinValue;
        }
        else
        {
            EklenmeTarih = Convert.ToDateTime(VeriSatiri[C_Sutun_eklenme_tarih]);
        }

        if (VeriSatiri[C_Sutun_guncelleyen_id] == DBNull.Value)
        {
            GuncelleyenId = 0;
        }
        else
        {
            GuncelleyenId = Convert.ToInt32(VeriSatiri[C_Sutun_guncelleyen_id]);
        }

        GuncelleyenIp = VeriSatiri[C_Sutun_guncelleyen_ip] == DBNull.Value ? "" : VeriSatiri[C_Sutun_guncelleyen_ip].ToString();

        if (VeriSatiri[C_Sutun_guncellenme_tarih] == DBNull.Value)
        {
            GuncellenmeTarih = DateTime.MinValue;
        }
        else
        {
            GuncellenmeTarih = Convert.ToDateTime(VeriSatiri[C_Sutun_guncellenme_tarih]);
        }

        return true;
    }

    public DataRow SonKayitGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_SonKayitGetir;
        VeritabaniIslem.ParametreEkle(C_Sutun_makineler_id, MakinelerId);
        VeriSatiri = VeritabaniIslem.SatirGetir();
        return VeriSatiri;
    }

    #endregion
}
