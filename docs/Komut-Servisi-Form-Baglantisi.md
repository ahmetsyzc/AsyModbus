# Komut servisini Windows Forms uygulamasına bağlama

Forms uygulaması kendi `App.config` dosyasındaki `SqlBaglanti` bağlantısıyla
web ile aynı veritabanına bağlanır. Tek görevi kendi timer'ından bekleyen komutu
kontrol etmek, röleyi tetiklemek ve sonucu kaydetmektir. Form tasarımı ve timer
olayları kullanıcı tarafından hazırlanacaktır.

`MakineKomutServisi` kendi timer'ını veya sürekli döngüsünü başlatmaz. Her
`KontrolEt()` çağrısında en fazla bir bekleyen komut işlenir. Veritabanı işlemleri
mevcut Entity → VeritabaniIslemleri → Stored Procedure yapısından geçer.

## Proje bağlantısı

Solution'a .NET Framework 4.7.2 Windows Forms projesi ekleyip BusinessLayer'a
**Project Reference** verin. Forms uygulamasının `App.config` dosyasında web ile
aynı bağlantıyı tanımlayın. BusinessLayer'ın `app.config` dosyası çalışan Forms
uygulamasının bağlantı ayarının yerine geçmez.

```xml
<configuration>
  <connectionStrings>
    <add name="SqlBaglanti"
         connectionString="Data Source=SUNUCU;Initial Catalog=ASY_MODBUS;Integrated Security=True"
         providerName="System.Data.SqlClient" />
  </connectionStrings>
</configuration>
```

`SUNUCU` değerini kendi SQL Server instance'ınızla değiştirin.

## Formun kullanacağı üyeler

| Üye | Görev |
| --- | --- |
| `KontrolEt()` | En fazla bir bekleyen komutu sahiplenir, uygular ve sonuçlandırır. |
| `IslemDevamEdiyor` | Çalışan kontrol varsa true döner; aynı nesnede ikinci kontrol engellenir. |
| `AltyapiHatasiVar` | Son kontrolün bağlantı, parametre veya sonuç kaydı hatasıdır; form timer'ı durdurmalıdır. |
| `SonKontrolZamani` | Son kontrol zamanıdır. |
| `SonKomutId`, `SonMakineId` | Son sahiplenilen komut ve makinedir. |
| `SonHata` | Son kontrolün hata bilgisidir. |
| `LogOlustu` | Yalnızca formda gösterim için tarih, mesaj ve hata bildirimi gönderir. |
| `Dispose()` | Yeni kontrol çağrılarını kapatır; başlamış fiziksel işlemi iptal etmez. |

`KontrolEt()` bir komutun başarı veya hata sonucunu kaydettiğinde `true` döner.
Kuyruk boşsa, başka kontrol sürüyorsa veya altyapı hatası varsa `false` döner.
Altyapı hatası ile boş kuyruk ayrımı `AltyapiHatasiVar` üzerinden yapılır.

## Timer bağlantısı örneği

Form boyunca tek servis nesnesi kullanın. Aşağıdaki `timerKomut`, sizin
oluşturacağınız Windows Forms Timer kontrolüdür. Form constructor içinde
`InitializeComponent()` sonrasında `timerKomut.Interval = 5000;` yazın.
Bu beş saniyelik kontrol aralığını yalnızca Form yönetir.

```csharp
private readonly MakineKomutServisi komutServisi = new MakineKomutServisi();
private bool kontrolSuruyor;
private bool kontrolAcik;

private async void timerKomut_Tick(object sender, EventArgs e)
{
    if (!kontrolAcik || kontrolSuruyor) return;
    kontrolSuruyor = true;
    timerKomut.Stop();
    try
    {
        await System.Threading.Tasks.Task.Run(() => komutServisi.KontrolEt());
        if (komutServisi.AltyapiHatasiVar) kontrolAcik = false;
    }
    finally
    {
        kontrolSuruyor = false;
        if (kontrolAcik && !IsDisposed && !Disposing) timerKomut.Start();
    }
}
```

Başlat butonu `kontrolAcik = true` ve
`timerKomut.Start()` işlemlerini yapar. Durdur butonu `kontrolAcik = false`
ve `timerKomut.Stop()` işlemlerini yapar. Başlamış komut tamamlanmaya devam eder.
Form kapanırken timer'ı durdurun, devam eden kontrolü UI'ı engellemeden bekleyin,
ardından `komutServisi.Dispose()` çağırın. İşlem sürerken process'i sonlandırmak
OFF veya sonuç kaydını yarıda bırakabilir.

`LogOlustu` arka plan thread'inden gelebilir; kontrol güncellemelerini
`BeginInvoke` ile form thread'ine taşıyın. Sınıftaki `DurumBildir` metodu DB'ye
log yazmaz. Genel uygulama logları mevcut `VeritabaniIslemleri.Calistir()`
mekanizmasından, operasyon logları `MakinelerLoglar.Ekle()` üzerinden aynı
veritabanı altyapısından geçer.

## İşlem davranışı

Komut `SiradakiGetir()` ile transaction içinde kilitlenir;
`IslemeGuncelle()` ve merkezi genel log aynı transaction içinde çalışır.
Komut atomik olarak `ISLENIYOR` yapılır ve sahiplenme fiziksel çağrıdan önce
kalıcı olur. Röle IP, port ve kanal bilgileri güncel tablolardan okunur.
Komut sonucu ve `SERVIS` kaynaklı operasyon logu aynı transaction'da kaydedilir.
Röle başarısı makine durumunu otomatik değiştirmez.

Sonuç kaydı başarısızsa komut `ISLENIYOR` kalır; fiziksel tetikleme otomatik
tekrarlanmaz. Form `AltyapiHatasiVar` gördüğünde timer'ı durdurmalı ve hatayı
bildirmelidir. Belirsiz komutlar incelenmeden `BEKLIYOR` durumuna döndürülmez.

Formda makine, röle veya kanal yönetim ekranı bulunmaz; yönetim webde kalır.

## Kontrol aralığı Form tarafından yönetilir

Formda `timerKomut.Interval = 5000;` kullanılır. Servis çağrıldığında en fazla
bir bekleyen komut işler; kendi timerı veya süre ayarı yoktur. Eski
`SERVIS_AYARLARI / KOMUT_KONTROL_SURESI_SN` veritabanı kaydı korunur fakat
okunmaz. Eksik veya geçersiz olması işlemeyi engellemez. Diğer iş parametreleri
mevcut yapıda okunmaya devam eder. Rölenin iki saniyelik tetikleme süresi ayrıdır.

## Doğrulama

BusinessLayer Debug derlemesinden sonra `Scripts/MakineKomutServisi_Dogrula.ps1`
sahte röleyle geçici SQL veritabanında test yapar ve test DB'sini kaldırır.
Uygulama veritabanına yazmaz. Gerçek HW-584 ve fiziksel duruş testi form
bağlantısından sonra ayrıca yapılacaktır.

## Mimari düzeltmeleri

Serviste mevcut `VeritabaniIslemleri veritabaniIslemleri`, `Parametreler parametreler`,
`Makineler makineler` isimlendirmeleri kullanılır. Özel `ServisParametreleri` ara
sınıfı ve çoklu `out` parametreleri kaldırılmıştır. Sonuç kaydı entity'nin mevcut
`Guncelle()` sözleşmesiyle yapılır; `IslemeGuncelle()` ve `Guncelle()` merkezi
`Calistir()` üzerinden genel güncelleme logu üretir. Transaction içindeki genel
log hatası da ana işlemi başarısız kılar. Servis IP alanlarına IPv4 adresi yazar;
çok ağ kartlı bilgisayarda uygun adres constructor ile belirtilebilir.

Röle tetiklenmeden önce makinenin güncel CALISIYOR durumu tekrar doğrulanır.
Servis metinleri `Mesajlar` sınıfında tutulur. Formun kullandığı `KontrolEt`,
`LogOlustu` ve süre dışındaki durum özellikleri korunmuştur.

İşleniyor durumundaki belirsiz komutlar yeniden başlatmada otomatik uygulanmaz.
Yetkili inceleme sırasında tüm komut işleyicileri durdurulmalı ve fiziksel sonuç
belirlenmelidir. Kayıt silinmemeli veya BEKLIYOR yapılmamalıdır. Genel listeleme
`MakineKomutlar.TumunuGetir()` üzerinden yapılabilir. Sonuç kapatma işlemi mevcut
`Guncelle()` ve `MakinelerLoglar.Ekle()` ile tek BAGIMLI transaction içinde,
inceleme açıklaması ve işlemi yapan kullanıcı bilgileriyle uygulanmalıdır.
Bu yönetim işlemi Formun normal timer döngüsüne eklenmez.
