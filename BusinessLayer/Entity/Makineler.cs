using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

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
    public const string C_Sp_DurumGuncelle = "dbo.SP_Makineler_DURUM_GUNCELLE";
    public const string C_Sp_SiraGuncelle = "dbo.SP_Makineler_SIRA_GUNCELLE";
    public const string C_Sp_MaxSiraGetir = "dbo.SP_Makineler_MAX_SIRA_GETIR";
    public const string C_Sp_KullanilanKanallariGetir = "dbo.SP_Makineler_KULLANILAN_KANALLARI_GETIR";

    public const string C_Sutun_makine_ad = "makine_ad";
    public const string C_Sutun_model_ad = "model_ad";
    public const string C_Sutun_entegrasyon_kod = "entegrasyon_kod";
    public const string C_Sutun_gg_no = "gg_no";
    public const string C_Sutun_makine_no = "makine_no";
    public const string C_Sutun_band_no = "band_no";
    public const string C_Sutun_ip = "ip";
    public const string C_Sutun_mfg = "mfg";
    public const string C_Sutun_durum = "durum";
    public const string C_Sutun_son_durum_tarih = "son_durum_tarih";
    public const string C_Sutun_sira_no = "sira_no";
    public const string C_Sutun_rolecihazlar_id = "rolecihazlar_id";
    public const string C_Sutun_role_kanal_no = "role_kanal_no";

    #endregion

    #region Nesneler

    private string makineAd;
    public string MakineAd
    {
        get { return makineAd; }
        set { makineAd = value; }
    }

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

    private string durum;
    public string Durum
    {
        get { return durum; }
        set { durum = value; }
    }

    private DateTime? sonDurumTarihi;
    public DateTime? SonDurumTarihi
    {
        get { return sonDurumTarihi; }
        set { sonDurumTarihi = value; }
    }

    private int? siraNo;
    public int? SiraNo
    {
        get { return siraNo; }
        set { siraNo = value; }
    }

    private int? roleCihazlarId;
    public int? RoleCihazlarId
    {
        get { return roleCihazlarId; }
        set { roleCihazlarId = value; }
    }

    private int? roleKanalNo;
    public int? RoleKanalNo
    {
        get { return roleKanalNo; }
        set { roleKanalNo = value; }
    }

    #endregion

    #region Metotlar

    public bool Ekle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Ekle;
        VeritabaniIslem.ParametreEkle(C_Sutun_makine_ad, MakineAd);
        VeritabaniIslem.ParametreEkle(C_Sutun_model_ad, ModelAd);
        VeritabaniIslem.ParametreEkle(C_Sutun_entegrasyon_kod, EntegrasyonKod);
        VeritabaniIslem.ParametreEkle(C_Sutun_gg_no, GgNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_makine_no, MakineNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_band_no, BandNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_ip, Ip);
        VeritabaniIslem.ParametreEkle(C_Sutun_mfg, Mfg);
        VeritabaniIslem.ParametreEkle(C_Sutun_sira_no, SiraNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_rolecihazlar_id, NullDeger(RoleCihazlarId));
        VeritabaniIslem.ParametreEkle(C_Sutun_role_kanal_no, NullDeger(RoleKanalNo));
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, AktifMi);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_id, EkleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_ip, EkleyenIp);
        return VeritabaniIslem.Calistir();
    }

    public bool Guncelle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Guncelle;
        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_makine_ad, MakineAd);
        VeritabaniIslem.ParametreEkle(C_Sutun_model_ad, ModelAd);
        VeritabaniIslem.ParametreEkle(C_Sutun_entegrasyon_kod, EntegrasyonKod);
        VeritabaniIslem.ParametreEkle(C_Sutun_gg_no, GgNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_makine_no, MakineNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_band_no, BandNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_ip, Ip);
        VeritabaniIslem.ParametreEkle(C_Sutun_mfg, Mfg);
        VeritabaniIslem.ParametreEkle(C_Sutun_sira_no, SiraNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_rolecihazlar_id, NullDeger(RoleCihazlarId));
        VeritabaniIslem.ParametreEkle(C_Sutun_role_kanal_no, NullDeger(RoleKanalNo));
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_id, GuncelleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_ip, GuncelleyenIp);
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
        MakineAd = VeriSatiri[C_Sutun_makine_ad].ToString();
        ModelAd = VeriSatiri[C_Sutun_model_ad].ToString();
        EntegrasyonKod = VeriSatiri[C_Sutun_entegrasyon_kod].ToString();
        GgNo = VeriSatiri[C_Sutun_gg_no].ToString();
        MakineNo = VeriSatiri[C_Sutun_makine_no].ToString();
        BandNo = VeriSatiri[C_Sutun_band_no].ToString();
        Ip = VeriSatiri[C_Sutun_ip].ToString();
        Mfg = VeriSatiri[C_Sutun_mfg].ToString();
        RoleCihazlarId = NullIntGetir(VeriSatiri[C_Sutun_rolecihazlar_id]);
        RoleKanalNo = NullIntGetir(VeriSatiri[C_Sutun_role_kanal_no]);
        Durum = VeriSatiri[C_Sutun_durum] == DBNull.Value ? "" : VeriSatiri[C_Sutun_durum].ToString();

        if (VeriSatiri[C_Sutun_son_durum_tarih] == DBNull.Value)
        {
            SonDurumTarihi = null;
        }
        else
        {
            SonDurumTarihi = Convert.ToDateTime(VeriSatiri[C_Sutun_son_durum_tarih]);
        }
        if (VeriSatiri[C_Sutun_sira_no] == DBNull.Value)
        {
            SiraNo = null;
        }
        else
        {
            SiraNo = Convert.ToInt32(VeriSatiri[C_Sutun_sira_no]);
        }

        AktifMi = Convert.ToBoolean(VeriSatiri[C_Sutun_aktif_mi]);
        EkleyenId = Convert.ToInt32(VeriSatiri[C_Sutun_ekleyen_id]);
        EkleyenIp = VeriSatiri[C_Sutun_ekleyen_ip].ToString();
        EklenmeTarih = Convert.ToDateTime(VeriSatiri[C_Sutun_eklenme_tarih]);

        if (VeriSatiri[C_Sutun_guncelleyen_id] == DBNull.Value)
        {
            GuncelleyenId = 0;
        }
        else
        {
            GuncelleyenId = Convert.ToInt32(VeriSatiri[C_Sutun_guncelleyen_id]);
        }

        if (VeriSatiri[C_Sutun_guncelleyen_ip] == DBNull.Value)
        {
            GuncelleyenIp = "";
        }
        else
        {
            GuncelleyenIp = VeriSatiri[C_Sutun_guncelleyen_ip].ToString();
        }

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

    public bool DurumGuncelle()
    {
        VeritabaniIslem.SpAdi = C_Sp_DurumGuncelle;
        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_durum, Durum);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_id, GuncelleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_ip, GuncelleyenIp);
        return VeritabaniIslem.Calistir();
    }

    public bool SiraGuncelle()
    {
        VeritabaniIslem.SpAdi = C_Sp_SiraGuncelle;
        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_sira_no, SiraNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_id, GuncelleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_ip, GuncelleyenIp);
        return VeritabaniIslem.Calistir();
    }

    public int MaxSiraGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_MaxSiraGetir;
        VeritabaniIslem.ParametreEkle(C_Sutun_band_no, BandNo);
        return VeritabaniIslem.DegerGetir();
    }

    public bool BandSiralariniSikistir()
    {
        string bandNo = BandNo;
        int guncelleyenId = GuncelleyenId;
        string guncelleyenIp = GuncelleyenIp;

        DataTable makinelerTablosu = TumunuGetir();
        DataRow[] siraliMakineler = makinelerTablosu.AsEnumerable()
            .Where(row => row[C_Sutun_band_no].ToString() == bandNo)
            .OrderBy(row => row.Field<int?>(C_Sutun_sira_no) ?? int.MaxValue)
            .ToArray();

        for (int i = 0; i < siraliMakineler.Length; i++)
        {
            Id = Convert.ToInt32(siraliMakineler[i][C_Sutun_id]);
            SiraNo = i + 1;
            GuncelleyenId = guncelleyenId;
            GuncelleyenIp = guncelleyenIp;

            if (!SiraGuncelle())
            {
                return false;
            }
        }

        return true;
    }

    public DataTable KullanilanKanallariGetir()
    {
        return KullanilanKanallariGetir(RoleCihazlarId, Id > 0 ? (int?)Id : null);
    }

    public DataTable KullanilanKanallariGetir(int? roleCihazlarId, int? id)
    {
        VeritabaniIslem.SpAdi = C_Sp_KullanilanKanallariGetir;
        VeritabaniIslem.ParametreEkle(C_Sutun_rolecihazlar_id, NullDeger(roleCihazlarId));
        VeritabaniIslem.ParametreEkle(C_Sutun_id, NullDeger(id));
        VeriTablosu = VeritabaniIslem.TabloGetir();
        return VeriTablosu;
    }

    public bool KanalKullanimdaMi(int? roleCihazlarId, int? kanalNo, int? id)
    {
        if (!roleCihazlarId.HasValue || !kanalNo.HasValue)
        {
            return false;
        }

        DataTable kullanilanKanallar = KullanilanKanallariGetir(roleCihazlarId, id);
        if (kullanilanKanallar == null)
        {
            return false;
        }

        foreach (DataRow satir in kullanilanKanallar.Rows)
        {
            if (satir[C_Sutun_role_kanal_no] != DBNull.Value &&
                Convert.ToInt32(satir[C_Sutun_role_kanal_no]) == kanalNo.Value)
            {
                return true;
            }
        }

        return false;
    }

    public void BosKanallariListele(DropDownList dropDownList, int kanalSayisi)
    {
        DataTable kullanilanKanallar = KullanilanKanallariGetir();
        dropDownList.Items.Clear();
        dropDownList.Items.Add(new ListItem("Seçiniz", "0"));

        HashSet<int> doluKanallar = new HashSet<int>();
        if (kullanilanKanallar != null)
        {
            foreach (DataRow satir in kullanilanKanallar.Rows)
            {
                if (satir[C_Sutun_role_kanal_no] != DBNull.Value)
                {
                    doluKanallar.Add(Convert.ToInt32(satir[C_Sutun_role_kanal_no]));
                }
            }
        }

        for (int kanalNo = 1; kanalNo <= kanalSayisi; kanalNo++)
        {
            if (!doluKanallar.Contains(kanalNo))
            {
                dropDownList.Items.Add(new ListItem("Kanal " + kanalNo, kanalNo.ToString()));
            }
        }
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

