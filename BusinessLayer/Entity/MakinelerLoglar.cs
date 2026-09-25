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
    public const string C_Sutun_detay = "detay";
    public const string C_Sutun_kaynak_parametre_id = "kaynak_parametre_id";
    public const string C_Sutun_onceki_durum_parametre_id = "onceki_durum_parametre_id";
    public const string C_Sutun_yeni_durum_parametre_id = "yeni_durum_parametre_id";
    public const string C_Sutun_islem_tur_parametre_id = "islem_tur_parametre_id";
    public const string C_Sutun_islem_sonuc_parametre_id = "islem_sonuc_parametre_id";
    public const string C_Sutun_kaynak_ad = "kaynak_ad";
    public const string C_Sutun_kaynak_kod = "kaynak_kod";
    public const string C_Sutun_onceki_durum_ad = "onceki_durum_ad";
    public const string C_Sutun_onceki_durum_kod = "onceki_durum_kod";
    public const string C_Sutun_yeni_durum_ad = "yeni_durum_ad";
    public const string C_Sutun_yeni_durum_kod = "yeni_durum_kod";
    public const string C_Sutun_islem_tur_ad = "islem_tur_ad";
    public const string C_Sutun_islem_sonuc_ad = "islem_sonuc_ad";

    #endregion

    #region Nesneler

    private int makinelerId;
    public int MakinelerId
    {
        get { return makinelerId; }
        set { makinelerId = value; }
    }

    private string detay;
    public string Detay
    {
        get { return detay; }
        set { detay = value; }
    }

    private int? kaynakParametreId;
    public int? KaynakParametreId
    {
        get { return kaynakParametreId; }
        set { kaynakParametreId = value; }
    }

    private int? oncekiDurumParametreId;
    public int? OncekiDurumParametreId
    {
        get { return oncekiDurumParametreId; }
        set { oncekiDurumParametreId = value; }
    }

    private int? yeniDurumParametreId;
    public int? YeniDurumParametreId
    {
        get { return yeniDurumParametreId; }
        set { yeniDurumParametreId = value; }
    }

    private int? islemTurParametreId;
    public int? IslemTurParametreId
    {
        get { return islemTurParametreId; }
        set { islemTurParametreId = value; }
    }

    private int? islemSonucParametreId;
    public int? IslemSonucParametreId
    {
        get { return islemSonucParametreId; }
        set { islemSonucParametreId = value; }
    }

    #endregion

    #region Metotlar

    public bool Ekle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Ekle;
        VeritabaniIslem.ParametreEkle(C_Sutun_makineler_id, MakinelerId);
        VeritabaniIslem.ParametreEkle(C_Sutun_detay, Detay);
        VeritabaniIslem.ParametreEkle(C_Sutun_kaynak_parametre_id, NullDeger(KaynakParametreId));
        VeritabaniIslem.ParametreEkle(C_Sutun_onceki_durum_parametre_id, NullDeger(OncekiDurumParametreId));
        VeritabaniIslem.ParametreEkle(C_Sutun_yeni_durum_parametre_id, NullDeger(YeniDurumParametreId));
        VeritabaniIslem.ParametreEkle(C_Sutun_islem_tur_parametre_id, NullDeger(IslemTurParametreId));
        VeritabaniIslem.ParametreEkle(C_Sutun_islem_sonuc_parametre_id, NullDeger(IslemSonucParametreId));
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
        Detay = VeriSatiri[C_Sutun_detay] == DBNull.Value ? "" : VeriSatiri[C_Sutun_detay].ToString();
        KaynakParametreId = NullIntGetir(VeriSatiri[C_Sutun_kaynak_parametre_id]);
        OncekiDurumParametreId = NullIntGetir(VeriSatiri[C_Sutun_onceki_durum_parametre_id]);
        YeniDurumParametreId = NullIntGetir(VeriSatiri[C_Sutun_yeni_durum_parametre_id]);
        IslemTurParametreId = NullIntGetir(VeriSatiri[C_Sutun_islem_tur_parametre_id]);
        IslemSonucParametreId = NullIntGetir(VeriSatiri[C_Sutun_islem_sonuc_parametre_id]);
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

    private object NullDeger(int? deger)
    {
        if (deger.HasValue)
        {
            return deger.Value;
        }

        return DBNull.Value;
    }

    private int? NullIntGetir(object deger)
    {
        if (deger == null || deger == DBNull.Value)
        {
            return null;
        }

        return Convert.ToInt32(deger);
    }

    #endregion
}
