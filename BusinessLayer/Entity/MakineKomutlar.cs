using System;
using System.Data;

public class MakineKomutlar : OrtakAlanlar, IOrtakMetotlar
{
    public MakineKomutlar(VeritabaniIslemleri _veritabaniIslemleri)
    {
        VeritabaniIslem = _veritabaniIslemleri;
    }

    #region Sabitler

    public const int C_UrlUzunlugu = 2048;
    public const string C_Sp_Sahiplen = "dbo.SP_MakineKomutlar_SAHIPLEN";
    public const string C_Sp_SonDurumlariGetir = "dbo.SP_MakineKomutlar_SON_DURUMLARI_GETIR";
    public const string C_Sutun_tetikleme_url = "tetikleme_url";
    public const string C_Sutun_kapatma_url = "kapatma_url";
    public const string C_Sutun_hedef_rolecihaz_id = "hedef_rolecihaz_id";
    public const string C_Sutun_hedef_ip = "hedef_ip";
    public const string C_Sutun_hedef_port = "hedef_port";
    public const string C_Sutun_tetikleme_suresi_ms = "tetikleme_suresi_ms";

    public const string C_Tablo = "dbo.MakineKomutlar";
    public const string C_Sp_Ekle = "dbo.SP_MakineKomutlar_EKLE";
    public const string C_Sp_Doldur = "dbo.SP_MakineKomutlar_DOLDUR";
    public const string C_Sp_TumunuGetir = "dbo.SP_MakineKomutlar_TUMUNU_GETIR";
    public const string C_Sp_AktifleriGetir = "dbo.SP_MakineKomutlar_AKTIFLERI_GETIR";
    public const string C_Sp_AktifKomutVarMi = "dbo.SP_MakineKomutlar_AKTIF_KOMUT_VAR_MI";
    public const string C_Sp_Guncelle = "dbo.SP_MakineKomutlar_GUNCELLE";

    public const string C_Sutun_makineler_id = "makineler_id";
    public const string C_Sutun_islem_tur_parametre_id = "islem_tur_parametre_id";
    public const string C_Sutun_durum_parametre_id = "durum_parametre_id";
    public const string C_Sutun_deneme_sayisi = "deneme_sayisi";
    public const string C_Sutun_durum_kod = "durum_kod";
    public const string C_Sutun_detay = "detay";

    #endregion

    #region Nesneler

    private string tetiklemeUrl;
    public string TetiklemeUrl
    {
        get { return tetiklemeUrl; }
        set { tetiklemeUrl = value; }
    }

    private string kapatmaUrl;
    public string KapatmaUrl
    {
        get { return kapatmaUrl; }
        set { kapatmaUrl = value; }
    }

    private int hedefRoleCihazId;
    public int HedefRoleCihazId
    {
        get { return hedefRoleCihazId; }
        set { hedefRoleCihazId = value; }
    }

    private string hedefIp;
    public string HedefIp
    {
        get { return hedefIp; }
        set { hedefIp = value; }
    }

    private int hedefPort;
    public int HedefPort
    {
        get { return hedefPort; }
        set { hedefPort = value; }
    }

    private int tetiklemeSuresiMs;
    public int TetiklemeSuresiMs
    {
        get { return tetiklemeSuresiMs; }
        set { tetiklemeSuresiMs = value; }
    }

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
        RoleKontrol.Dogrula(this);
        // Ilk durum BEKLIYOR ve deneme sayisi 0 SP tarafindan atanir.
        // Ayni makineye cift aktif talep kontrolu de SP tarafinda yapilir.
        VeritabaniIslem.SpAdi = C_Sp_Ekle;
        VeritabaniIslem.MetinParametreEkle(C_Sutun_tetikleme_url, TetiklemeUrl, C_UrlUzunlugu);
        VeritabaniIslem.MetinParametreEkle(C_Sutun_kapatma_url, KapatmaUrl, C_UrlUzunlugu);
        VeritabaniIslem.ParametreEkle(C_Sutun_hedef_rolecihaz_id, HedefRoleCihazId);
        VeritabaniIslem.MetinParametreEkle(C_Sutun_hedef_ip, HedefIp, 45);
        VeritabaniIslem.ParametreEkle(C_Sutun_hedef_port, HedefPort);
        VeritabaniIslem.ParametreEkle(C_Sutun_tetikleme_suresi_ms, TetiklemeSuresiMs);
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

    public DataTable AktifleriGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_AktifleriGetir;
        VeriTablosu = VeritabaniIslem.TabloGetir();
        return VeriTablosu;
    }

    public DataTable SonDurumlariGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_SonDurumlariGetir;
        return VeritabaniIslem.TabloGetir();
    }

    public bool AktifKomutVarMi()
    {
        VeritabaniIslem.SpAdi = C_Sp_AktifKomutVarMi;
        VeritabaniIslem.ParametreEkle(C_Sutun_makineler_id, MakinelerId);
        VeritabaniIslem.ParametreEkle(C_Sutun_islem_tur_parametre_id, IslemTurParametreId);
        return VeritabaniIslem.DegerGetir() > 0;
    }

    public bool SahiplenGuncelle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Sahiplen;
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_ip, GuncelleyenIp ?? string.Empty);
        VeriSatiri = VeritabaniIslem.SatirGetir();
        if (!SatirdanDoldur()) return false;
        // OUTPUT ile okunan degisiklik, merkezi logla ayni BAGIMLI transaction icindedir.
        var log = new Loglar(VeritabaniIslem);
        log.Url = string.Empty;
        log.TabloAd = "MakineKomutlar";
        log.IslemAd = "Komut sahiplenme";
        log.IslemTip = LogIslemTipleri.Update.ToString();
        log.Detay = "Komut ID: " + Id + ". BEKLIYOR → ISLENIYOR.";
        log.EkleyenIp = GuncelleyenIp ?? string.Empty;
        log.AktifMi = true;
        if (!log.Ekle()) throw new InvalidOperationException(Mesajlar.KomutSahiplenilemedi);
        return true;
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
            TetiklemeUrl = KapatmaUrl = HedefIp = string.Empty;
            HedefRoleCihazId = HedefPort = TetiklemeSuresiMs = 0;
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
        TetiklemeUrl = VeriSatiri[C_Sutun_tetikleme_url].ToString();
        KapatmaUrl = VeriSatiri[C_Sutun_kapatma_url].ToString();
        HedefIp = VeriSatiri[C_Sutun_hedef_ip].ToString();
        HedefRoleCihazId = NullIntGetir(VeriSatiri[C_Sutun_hedef_rolecihaz_id]);
        HedefPort = NullIntGetir(VeriSatiri[C_Sutun_hedef_port]);
        TetiklemeSuresiMs = NullIntGetir(VeriSatiri[C_Sutun_tetikleme_suresi_ms]);
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
