
public class Mesajlar
{
    /**************** GENEL KAYIT MESAJLARI ****************/

    public static string KayitEklemeIslemiBasarisiz
    {
        get { return "Kayıt ekleme işlemi başarısız!"; }
    }

    public static string KayitGuncellemeBasarili
    {
        get { return "Kayıt güncelleme işlemi başarılı."; }
    }

    public static string KayitGuncellemeBasarisiz
    {
        get { return "Kayıt güncelleme işlemi başarısız!"; }
    }

    public static string KayitSilmeBasarisiz
    {
        get { return "Kayıt silme işlemi başarısız!"; }
    }

    public static string TelefonHatali
    {
        get { return "Telefon numarası 10 haneli olmalıdır."; }
    }

    public static string SistemselHata(string hataMesaji)
    {
        return "Sistemsel Hata " + hataMesaji;
    }

    /**************** KULLANICI GİRİŞ MESAJLARI ****************/

    public static string KullaniciGirisHataliGiris
    {
        get { return "Mail adresi veya şifre hatalı!"; }
    }

    public static string KullaniciGirisCaptchaBos
    {
        get { return "Lütfen güvenlik kodunu giriniz."; }
    }

    public static string KullaniciGirisCaptchaHatali
    {
        get { return "Güvenlik kodu hatalı!"; }
    }

    public static string KullaniciGirisAlanlarBos
    {
        get { return "Mail ve Şifre boş bırakılamaz."; }
    }

    /**************** ŞİFRE SIFIRLAMA MESAJLARI ****************/

    public static string SifreSifirlamaMailCepNoBos
    {
        get { return "Mail ve Cep No boş bırakılamaz."; }
    }
    

    public static string SifreSifirlamaBasarili(string yeniSifre)
    {
        return "Yeni şifreniz = " + yeniSifre;
    }

    public static string SifreSifirlamaBasarisiz
    {
        get { return "Şifre güncellenemedi."; }
    }

    public static string SifreSifirlamaBilgilerHatali
    {
        get { return "Mail veya cep telefonu hatalı."; }
    }

    /**************** KULLANICI YÖNETİMİ MESAJLARI ****************/

    public static string KullaniciBulunamadi
    {
        get { return "Kullanıcı bulunamadı."; }
    }

    public static string KullaniciKayitli
    {
        get { return "Bu kullanıcı sistemde kayıtlı."; }
    }


    /**************** FORM KONTROL MESAJLARI ****************/


    public static string KullaniciAdEnAzIkiKarakter
    {
        get { return "Ad en az 2 karakter olmalıdır."; }
    }

    public static string KullaniciSoyadEnAzIkiKarakter
    {
        get { return "Soyad en az 2 karakter olmalıdır."; }
    }

    public static string KullaniciTcKimlikNoOnBirHane
    {
        get { return "Tc Kimlik No 11 hane olmalıdır!"; }
    }

    public static string ZorunluAlanlar(string alanlar)
    {
        return alanlar + " Bilgisi/Bilgileri Zorunludur.";
    }


    /**************** PROFİL RESMİ MESAJLARI ****************/

    public static string ProfilResmiDosyaTipiYanlis
    {
        get { return "Lütfen geçerli bir profil resmi seçiniz."; }
    }

    public static string ProfilResmiZorunlu 
    {
        get { return "Profil resmi seçilmesi zorunludur."; }
    }

    public static string SifreEnAzAltiKarakter
    {
        get { return "Şifre en az 6 karakter olmalıdır."; }
    }
}
