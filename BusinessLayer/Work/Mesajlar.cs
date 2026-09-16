
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

    public static string RolPasifOturumSonlandirildi
    {
        get { return "Rolünüz pasif duruma getirildiği için oturumunuz sonlandırıldı."; }
    }

    public static string KullaniciPasifOturumSonlandirildi
    {
        get { return "Hesabınız silindi, oturumunuz sonlandırıldı."; }
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

    public static string KullaniciRoluPasif
    {
        get { return "Kullanıcı rolünüz aktif değildir. Sistem yöneticisiyle iletişime geçiniz."; }
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

    /**************** MAKİNE YÖNETİMİ MESAJLARI ****************/

    public static string MakineBulunamadi
    {
        get { return "Makine bulunamadı."; }
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

    /**************** Log Mesajlari ****************/

    public static string LogSistemeGiris
    {
        get { return "Sisteme Giriş"; }
    }

    public static string SistemeBasariliGirisYapildi
    {
        get { return "Sisteme başarılı giriş yapıldı."; }
    }

    public static string LogSistemdenCikis
    {
        get { return "Sistemden Çıkış"; }
    }

    public static string SistemdenBasariliCikisYapildi
    {
        get { return "Sistemden başarılı çıkış yapıldı."; }
    }

    public static string LogSistem
    {
        get { return "Sistem"; }
    }

    public static string OtomatikLogIslemAdi(string sinifAdi, string metotAdi)
    {
        return sinifAdi + " tablosunda " + metotAdi + " işlemi";
    }

    public static string OtomatikLogDetay(string sinifAdi, string metotAdi)
    {
        return OtomatikLogIslemAdi(sinifAdi, metotAdi) + " başarıyla tamamlandı.";
    }

    public static string LogEskiKayit
    {
        get { return "ESKİ KAYIT"; }
    }

    public static string LogYeniKayit
    {
        get { return "YENİ KAYIT"; }
    }

    public static string LogKayitBulunamadi
    {
        get { return "Kayıt bulunamadı."; }
    }

    public static string LogBulunamadi
    {
        get { return "Log Kayıtları bulunamadı."; }
    }

    public static string RoleCihazAdiBos
    {
        get { return "Cihaz adı boş bırakılamaz."; }
    }

    public static string RoleCihazIpBos
    {
        get { return "IP boş bırakılamaz."; }
    }

    public static string RoleCihazPortGecersiz
    {
        get { return "Port geçerli bir sayı olmalıdır."; }
    }

    public static string RoleCihazKanalSayisiGecersiz
    {
        get { return "Kanal sayısı geçerli bir sayı olmalıdır."; }
    }

    public static string RoleCihazEklendi
    {
        get { return "Cihaz başarıyla eklendi."; }
    }

    public static string RoleCihazEklenemedi
    {
        get { return "Cihaz eklenemedi."; }
    }

    public static string RoleCihazGuncellendi
    {
        get { return "Cihaz başarıyla güncellendi."; }
    }

    public static string RoleCihazGuncellenemedi
    {
        get { return "Cihaz güncellenemedi."; }
    }

    public static string RoleCihazSilindi
    {
        get { return "Cihaz başarıyla silindi."; }
    }

    public static string RoleCihazSilinemedi
    {
        get { return "Cihaz silinemedi."; }
    }

    public static string RoleKanalSecilmedi
    {
        get { return "Röle cihazı seçildiğinde kanal seçimi zorunludur."; }
    }

    public static string RoleKanalKullanimda
    {
        get { return "Seçilen röle kanalı başka bir makinede kullanılmaktadır."; }
    }

    public static string RoleBosKanalKalmadi
    {
        get { return "Bu rölede boş kanal kalmadı."; }
    }

    public static string RolAdiBos
    {
        get { return "Rol adı boş bırakılamaz."; }
    }

    public static string RolEklendi
    {
        get { return "Rol başarıyla eklendi."; }
    }

    public static string RolEklenemedi
    {
        get { return "Rol eklenemedi."; }
    }

    public static string RolGuncellendi
    {
        get { return "Rol başarıyla güncellendi."; }
    }

    public static string RolGuncellenemedi
    {
        get { return "Rol güncellenemedi."; }
    }

    public static string RolSilindi
    {
        get { return "Rol başarıyla silindi."; }
    }

    public static string RolSilinemedi
    {
        get { return "Rol silinemedi."; }
    }

    public static string RolSilmeOnayi
    {
        get { return "Seçili rolü silmek istediğinize emin misiniz?"; }
    }

    public static string SayfaErisimYetkisiYok
    {
        get { return "Bu sayfaya erişim yetkiniz bulunmamaktadır."; }
    }

    public static string YetkisizIslem
    {
        get { return "Bu işlemi gerçekleştirme yetkiniz bulunmamaktadır."; }
    }

    public static string RolSeciniz
    {
        get { return "Lütfen bir rol seçiniz."; }
    }

    public static string RolYetkileriGuncellendi
    {
        get { return "Rol yetkileri başarıyla güncellendi."; }
    }

    public static string RolYetkileriGuncellenemedi
    {
        get { return "Rol yetkileri güncellenemedi."; }
    }

    public static string SuperAdminRoluSilinemez
    {
        get { return "Super Admin rolü silinemez."; }
    }

    #region Makine Durdurma Mesajları

    public static string MakineCalismiyor
    {
        get { return "Yalnızca çalışan makineler durdurulabilir."; }
    }

    public static string MakineBasariylaDurduruldu
    {
        get { return "Makine başarıyla durduruldu."; }
    }

    public static string MakineDurdurmaHatasi
    {
        get { return "Makine durdurulurken bir hata oluştu."; }
    }

    public static string MakineLoglamaHatasi
    {
        get { return "Makine durduruldu ancak log kaydı oluşturulamadı."; }
    }

    #endregion

    public static string MakineSirasiBos
    {
        get { return "Makine sıralaması alınamadı."; }
    }

    public static string MakineSirasiGecersiz
    {
        get { return "Makine sıralamasında geçersiz bir değer bulundu."; }
    }

    public static string MakineSirasiGuncellenemedi
    {
        get { return "Makine sıralaması güncellenirken hata oluştu."; }
    }

    public static string MakineSirasiGuncellendi
    {
        get { return "Makine sıralaması başarıyla güncellendi."; }
    }

    public static string MakineSirasiLoglamaHatasi
    {
        get { return "İşlem tamamlandı ancak log kaydı oluşturulamadı."; }
    }
}