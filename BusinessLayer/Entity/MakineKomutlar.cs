using System;
using System.Data;

public class MakineKomutlar : OrtakAlanlar, IOrtakMetotlar
{
    public MakineKomutlar(VeritabaniIslemleri _veritabaniIslemleri)
    {
        VeritabaniIslem = _veritabaniIslemleri;
    }

    #region Sabitler

    public const string C_Tablo = "dbo.MakineKomutlar";
    public const string C_Sp_Ekle = "dbo.SP_MakineKomutlar_EKLE";
    public const string C_Sp_Doldur = "dbo.SP_MakineKomutlar_DOLDUR";
    public const string C_Sp_TumunuGetir = "dbo.SP_MakineKomutlar_TUMUNU_GETIR";
    public const string C_Sp_AktifleriGetir = "dbo.SP_MakineKomutlar_AKTIFLERI_GETIR";
    public const string C_Sp_BekleyenGetir = "dbo.SP_MakineKomutlar_BEKLEYEN_GETIR";
    public const string C_Sp_AktifKomutVarMi = "dbo.SP_MakineKomutlar_AKTIF_KOMUT_VAR_MI";
    public const string C_Sp_IslemeGuncelle = "dbo.SP_MakineKomutlar_ISLEME_GUNCELLE";
    public const string C_Sp_Guncelle = "dbo.SP_MakineKomutlar_GUNCELLE";
    public const string C_Sp_SiradakiGetir = "dbo.SP_MakineKomutlar_SIRADAKI_GETIR";

    public const string C_Sutun_makineler_id = "makineler_id";
    public const string C_Sutun_islem_tur_parametre_id = "islem_tur_parametre_id";
    public const string C_Sutun_durum_parametre_id = "durum_parametre_id";
    public const string C_Sutun_deneme_sayisi = "deneme_sayisi";
    public const string C_Sutun_durum_kod = "durum_kod";
    public const string C_Sutun_detay = "detay";

    #endregion

    #region Nesneler

    private int makinelerId;
    public int MakinelerId
    {
        get { return makinelerId; }
        set { makinelerId = value; }
    }

    private int islemTurParametreId;
    public int IslemTurParametreId
    {
        get { return islemTurParametreId; }
        set { islemTurParametreId = value; }
    }

    private int durumParametreId;
    public int DurumParametreId
    {
        get { return durumParametreId; }
        set { durumParametreId = value; }
    }

    private int denemeSayisi;
    public int DenemeSayisi
    {
        get { return denemeSayisi; }
        set { denemeSayisi = value; }
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
        // Ilk durum BEKLIYOR ve deneme sayisi 0 SP tarafindan atanir.
        // Ayni makineye cift aktif talep kontrolu de SP tarafinda yapilir.
        VeritabaniIslem.SpAdi = C_Sp_Ekle;
        VeritabaniIslem.ParametreEkle(C_Sutun_makineler_id, MakinelerId);
        VeritabaniIslem.ParametreEkle(C_Sutun_islem_tur_parametre_id, IslemTurParametreId);
        VeritabaniIslem.ParametreEkle(C_Sutun_detay, NullDeger(Detay));
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_id, NullId(EkleyenId));
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_ip, NullDeger(EkleyenIp));
        return VeritabaniIslem.Calistir();
    }

    public bool Sil()
    {
        // Bekleyen veya islenen bir komut CRUD silme ile kaybedilmemelidir.
        return false;
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
        return SatirdanDoldur();
    }

    public DataTable BekleyenGetir()
    {
        // Listeleme icindir; servis bu listeyi fiziksel komut uygulamak icin kullanmaz.
        VeritabaniIslem.SpAdi = C_Sp_BekleyenGetir;
        VeriTablosu = VeritabaniIslem.TabloGetir();
        return VeriTablosu;
    }

    public DataTable AktifleriGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_AktifleriGetir;
        VeriTablosu = VeritabaniIslem.TabloGetir();
        return VeriTablosu;
    }

    public bool AktifKomutVarMi()
    {
        VeritabaniIslem.SpAdi = C_Sp_AktifKomutVarMi;
        VeritabaniIslem.ParametreEkle(C_Sutun_makineler_id, MakinelerId);
        VeritabaniIslem.ParametreEkle(C_Sutun_islem_tur_parametre_id, IslemTurParametreId);
        return VeritabaniIslem.DegerGetir() > 0;
    }

    public bool SiradakiGetir()
    {
        // BAGIMLI transaction gerekli; secilen kayit commit'e kadar kilitlidir.
        VeritabaniIslem.SpAdi = C_Sp_SiradakiGetir;
        VeriSatiri = VeritabaniIslem.SatirGetir();
        return SatirdanDoldur();
    }

    public bool IslemeGuncelle()
    {
        VeritabaniIslem.SpAdi = C_Sp_IslemeGuncelle;
        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_id, NullId(GuncelleyenId));
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_ip, NullDeger(GuncelleyenIp));
        return VeritabaniIslem.Calistir();
    }

    public bool Guncelle()
    {
        // SP yalnizca ISLENIYOR kaydini BASARILI/BASARISIZ yapabilir.
        VeritabaniIslem.SpAdi = C_Sp_Guncelle;
        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_durum_parametre_id, DurumParametreId);
        VeritabaniIslem.ParametreEkle(C_Sutun_detay, NullDeger(Detay));
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_id, NullId(GuncelleyenId));
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_ip, NullDeger(GuncelleyenIp));
        return VeritabaniIslem.Calistir();
    }

    private bool SatirdanDoldur()
    {
        if (VeriSatiri == null)
        {
            // Bos kuyrukta onceki sahiplenilen komutun kimligi tutulmaz.
            Id = 0;
            MakinelerId = 0;
            IslemTurParametreId = 0;
            DurumParametreId = 0;
            DenemeSayisi = 0;
            Detay = string.Empty;
            AktifMi = false;
            EkleyenId = 0;
            EkleyenIp = string.Empty;
            EklenmeTarih = DateTime.MinValue;
            GuncelleyenId = 0;
            GuncelleyenIp = string.Empty;
            GuncellenmeTarih = DateTime.MinValue;
            return false;
        }

        Id = Convert.ToInt32(VeriSatiri[C_Sutun_id]);
        MakinelerId = Convert.ToInt32(VeriSatiri[C_Sutun_makineler_id]);
        IslemTurParametreId = Convert.ToInt32(VeriSatiri[C_Sutun_islem_tur_parametre_id]);
        DurumParametreId = Convert.ToInt32(VeriSatiri[C_Sutun_durum_parametre_id]);
        DenemeSayisi = Convert.ToInt32(VeriSatiri[C_Sutun_deneme_sayisi]);
        Detay = VeriSatiri[C_Sutun_detay].ToString();
        AktifMi = Convert.ToBoolean(VeriSatiri[C_Sutun_aktif_mi]);
        EkleyenId = NullIntGetir(VeriSatiri[C_Sutun_ekleyen_id]);
        EkleyenIp = VeriSatiri[C_Sutun_ekleyen_ip].ToString();
        EklenmeTarih = Convert.ToDateTime(VeriSatiri[C_Sutun_eklenme_tarih]);
        GuncelleyenId = NullIntGetir(VeriSatiri[C_Sutun_guncelleyen_id]);
        GuncelleyenIp = VeriSatiri[C_Sutun_guncelleyen_ip].ToString();
        GuncellenmeTarih = VeriSatiri[C_Sutun_guncellenme_tarih] == DBNull.Value
            ? DateTime.MinValue : Convert.ToDateTime(VeriSatiri[C_Sutun_guncellenme_tarih]);
        return true;
    }

    private object NullDeger(string deger)
    {
        return deger == null ? (object)DBNull.Value : deger;
    }

    private object NullId(int deger)
    {
        return deger > 0 ? (object)deger : DBNull.Value;
    }

    private int NullIntGetir(object deger)
    {
        return deger == DBNull.Value ? 0 : Convert.ToInt32(deger);
    }

    #endregion
}
