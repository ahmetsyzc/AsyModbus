using System;
using System.Data;
using System.Text.RegularExpressions;


public class Kullanicilar : OrtakAlanlar, IOrtakMetotlar
{

    public Kullanicilar(VeritabaniIslemleri _veritabaniIslemleri)
    {
        VeritabaniIslem = _veritabaniIslemleri;
    }

    ~Kullanicilar()
    {
        VeriTablosu = new DataTable();
        VeriTablosu = null;
    }

    #region Sabitler

    public const string C_Tablo = "dbo.Kullanicilar";

    public const string C_Sp_Ekle = "dbo.SP_Kullanicilar_EKLE";
    public const string C_Sp_Sil = "dbo.SP_Kullanicilar_SIL";
    public const string C_Sp_Guncelle = "dbo.SP_Kullanicilar_GUNCELLE";
    public const string C_Sp_TumunuGetir = "dbo.SP_Kullanicilar_TUMUNU_GETIR";
    public const string C_Sp_Doldur = "dbo.SP_Kullanicilar_DOLDUR";//Tek Kayıt Getirir
    public const string C_Sp_SifreKontrol = "dbo.SP_Kullanicilar_SIFRE_KONTROL";
    public const string C_Sp_SifreGuncelle = "dbo.SP_Kullanicilar_SIFRE_GUNCELLE";
    public const string C_Sp_MailCepNoKontrol = "dbo.SP_Kullanicilar_MAIL_CEPNO_KONTROL";
    public const string C_Sp_TcknoVarMi = "dbo.SP_Kullanicilar_TCKNO_VAR_MI";
    public const string C_Sp_MailVarMi = "dbo.SP_Kullanicilar_MAIL_VAR_MI";
    public const string C_Sp_CepNoVarMi = "dbo.SP_Kullanicilar_CEPNO_VAR_MI";
    public const string C_Sp_KullaniciKodOlustur = "dbo.SP_Kullanicilar_KULLANICI_KOD_OLUSTUR";
    public const string C_Sp_MaxIdGetir = "dbo.SP_Kullanicilar_MAX_ID_GETIR";
    public const string C_Sp_ProfilGuncelle = "dbo.SP_Kullanicilar_PROFIL_GUNCELLE";


    public const string C_Sutun_kullanici_kod = "kullanici_kod";
    public const string C_Sutun_roller_id = "roller_id";
    public const string C_Sutun_ad = "ad";
    public const string C_Sutun_soyad = "soyad";
    public const string C_Sutun_tckno = "tckno";
    public const string C_Sutun_mail = "mail";
    public const string C_Sutun_sifre = "sifre";
    public const string C_Sutun_cep_no = "cep_no";
    public const string C_Sutun_dogum_tarih = "dogum_tarih";
    public const string C_Sutun_resim_yol = "resim_yol";


    #endregion


    #region Nesneler

    private int rollerId;
    public int RollerId
    {
        get { return rollerId; }
        set { rollerId = value; }
    }

    private string ad;
    public string Ad
    {
        get { return ad; }
        set { ad = value; }
    }

    private string soyad;
    public string Soyad
    {
        get { return soyad; }
        set { soyad = value; }
    }

    private string tckno;
    public string Tckno
    {
        get { return tckno; }
        set { tckno = value; }
    }

    private string mail;
    public string Mail
    {
        get { return mail; }
        set { mail = value; }
    }

    private string sifre;
    public string Sifre
    {
        get { return sifre; }
        set { sifre = value; }
    }

    private string kullaniciKod;
    public string KullaniciKod
    {
        get { return kullaniciKod; }
        set { kullaniciKod = value; }
    }

    private string cepNo;
    public string CepNo
    {
        get { return cepNo; }
        set
        {
            if (string.IsNullOrEmpty(value))
                cepNo = value;
            else
                cepNo = Regex.Replace(value, @"\D", "");
        }
    }

    private DateTime dogumTarih;
    public DateTime DogumTarih
    {
        get { return dogumTarih; }
        set { dogumTarih = value; }
    }

    private string resimYol;
    public string ResimYol
    {
        get { return resimYol; }
        set { resimYol = value; }
    }

    #endregion


    #region Metotlar


    public bool Ekle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Ekle;
        VeritabaniIslem.ParametreEkle(C_Sutun_roller_id, RollerId);
        VeritabaniIslem.ParametreEkle(C_Sutun_ad, Ad);
        VeritabaniIslem.ParametreEkle(C_Sutun_soyad, Soyad);
        VeritabaniIslem.ParametreEkle(C_Sutun_tckno, Tckno);
        VeritabaniIslem.ParametreEkle(C_Sutun_mail, Mail);
        VeritabaniIslem.ParametreEkle(C_Sutun_sifre, Sifre);
        VeritabaniIslem.ParametreEkle(C_Sutun_cep_no, CepNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_dogum_tarih, DogumTarih);
        VeritabaniIslem.ParametreEkle(C_Sutun_resim_yol, ResimYol);
        VeritabaniIslem.ParametreEkle(C_Sutun_aktif_mi, AktifMi);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_id, EkleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_ekleyen_ip, EkleyenIp);
        return VeritabaniIslem.Calistir();
    }

    public int MaxIdGetir()
    {
        VeritabaniIslem.SpAdi = C_Sp_MaxIdGetir;
        return VeritabaniIslem.DegerGetir();
    }


    public bool Guncelle()
    {
        VeritabaniIslem.SpAdi = C_Sp_Guncelle;
        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_roller_id, RollerId);
        VeritabaniIslem.ParametreEkle(C_Sutun_ad, Ad);
        VeritabaniIslem.ParametreEkle(C_Sutun_soyad, Soyad);
        VeritabaniIslem.ParametreEkle(C_Sutun_tckno, Tckno);
        VeritabaniIslem.ParametreEkle(C_Sutun_mail, Mail);
        VeritabaniIslem.ParametreEkle(C_Sutun_sifre, Sifre);
        VeritabaniIslem.ParametreEkle(C_Sutun_cep_no, CepNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_dogum_tarih, DogumTarih);
        VeritabaniIslem.ParametreEkle(C_Sutun_resim_yol, ResimYol);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_id, GuncelleyenId);
        VeritabaniIslem.ParametreEkle(C_Sutun_guncelleyen_ip, GuncelleyenIp);
        return VeritabaniIslem.Calistir();
    }

    public bool ProfilGuncelle()
    {
        VeritabaniIslem.SpAdi = C_Sp_ProfilGuncelle;
        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_ad, Ad);
        VeritabaniIslem.ParametreEkle(C_Sutun_soyad, Soyad);
        VeritabaniIslem.ParametreEkle(C_Sutun_mail, Mail);
        VeritabaniIslem.ParametreEkle(C_Sutun_sifre, Sifre);
        VeritabaniIslem.ParametreEkle(C_Sutun_cep_no, CepNo);
        VeritabaniIslem.ParametreEkle(C_Sutun_resim_yol, ResimYol);
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

        RollerId = Convert.ToInt32(VeriSatiri[C_Sutun_roller_id]);

        KullaniciKod = VeriSatiri[C_Sutun_kullanici_kod].ToString();
        Ad = VeriSatiri[C_Sutun_ad].ToString();
        Soyad = VeriSatiri[C_Sutun_soyad].ToString();
        Tckno = VeriSatiri[C_Sutun_tckno].ToString();
        Mail = VeriSatiri[C_Sutun_mail].ToString();
        Sifre = VeriSatiri[C_Sutun_sifre].ToString();
        CepNo = VeriSatiri[C_Sutun_cep_no].ToString();
        DogumTarih = Convert.ToDateTime(VeriSatiri[C_Sutun_dogum_tarih]);
        AktifMi = Convert.ToBoolean(VeriSatiri[C_Sutun_aktif_mi]);
        ResimYol = VeriSatiri[C_Sutun_resim_yol].ToString();

        EkleyenId = Convert.ToInt32(VeriSatiri[C_Sutun_ekleyen_id]);
        EkleyenIp = VeriSatiri[C_Sutun_ekleyen_ip].ToString();
        EklenmeTarih = Convert.ToDateTime(VeriSatiri[C_Sutun_eklenme_tarih]);

        if (VeriSatiri[C_Sutun_guncelleyen_id] == DBNull.Value)
        {
            GuncelleyenId = 0;
        }
        else
        {
            GuncelleyenId =
                Convert.ToInt32(VeriSatiri[C_Sutun_guncelleyen_id]);
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
            GuncellenmeTarih =
                Convert.ToDateTime(VeriSatiri[C_Sutun_guncellenme_tarih]);
        }
        return true;
    }

    public bool SifreKontrol()
    {
        VeritabaniIslem.SpAdi = C_Sp_SifreKontrol;
        VeritabaniIslem.ParametreEkle(C_Sutun_mail, Mail);
        VeritabaniIslem.ParametreEkle(C_Sutun_sifre, Sifre);
        VeriSatiri = VeritabaniIslem.SatirGetir();
        return VeriSatiri != null;
    }

    public DataRow MailCepNoKontrol()
    {
        VeritabaniIslem.SpAdi = C_Sp_MailCepNoKontrol;
        VeritabaniIslem.ParametreEkle(C_Sutun_mail, Mail);
        VeritabaniIslem.ParametreEkle(C_Sutun_cep_no, CepNo);
        VeriSatiri = VeritabaniIslem.SatirGetir();
        return VeriSatiri;
    }

    public bool SifreGuncelle()
    {
        VeritabaniIslem.SpAdi = C_Sp_SifreGuncelle;
        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_sifre, Sifre);
        return VeritabaniIslem.Calistir();
    }

    public int MailVarMi()
    {
        VeritabaniIslem.SpAdi = C_Sp_MailVarMi;
        VeritabaniIslem.ParametreEkle(C_Sutun_mail, Mail);
        int sonuc = VeritabaniIslem.DegerGetir();
        return sonuc;
    }

    public int TcknoVarMi()
    {
        VeritabaniIslem.SpAdi = C_Sp_TcknoVarMi;
        VeritabaniIslem.ParametreEkle(C_Sutun_tckno, Tckno);
        int sonuc = VeritabaniIslem.DegerGetir();
        return sonuc;
    }

    public int CepNoVarMi()
    {
        VeritabaniIslem.SpAdi = C_Sp_CepNoVarMi;
        VeritabaniIslem.ParametreEkle(C_Sutun_cep_no, CepNo);
        int sonuc = VeritabaniIslem.DegerGetir();
        return sonuc;
    }

    public string SifreOlustur(string ad, string soyad)
    {
        Random random = new Random();
        return ad.Substring(0, 2) + soyad.Substring(0, 2) + "@" + random.Next(10000, 100000);
    }

    public void KullaniciKodHesapla()
    {
        KullaniciKod = DateTime.Now.Year.ToString() + Id.ToString("D3");
    }

    public bool KullaniciKodOlustur()
    {
        VeritabaniIslem.SpAdi = C_Sp_KullaniciKodOlustur;
        VeritabaniIslem.ParametreEkle(C_Sutun_id, Id);
        VeritabaniIslem.ParametreEkle(C_Sutun_kullanici_kod, KullaniciKod);
        return VeritabaniIslem.Calistir();
    }

    #endregion
}
