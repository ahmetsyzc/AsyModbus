# AsyModbus

AsyModbus; C#, ASP.NET Web Forms, ADO.NET ve Microsoft SQL Server kullanılarak geliştirilen web tabanlı bir makine yönetim, takip ve kontrol uygulamasıdır.

Projenin amacı; tekstil makinelerinin merkezi bir web paneli üzerinden izlenmesi, durdurma taleplerinin yönetilmesi ve makine hareketlerinin kayıt altına alınmasıdır.

Sistem; web uygulaması, SQL Server veritabanı ve HW-584 ağ röle cihazlarını bir araya getirerek yazılım ile saha ekipmanları arasında iletişim sağlar.

> 🚧 Proje geliştirme ve saha entegrasyonu aşamasındadır. Sistem yalnızca uzaktan durdurma işlemini desteklemektedir. Gerçek makine durumlarının otomatik okunması için çalışmalar devam etmektedir.

## 🚀 Mevcut Özellikler

### Kullanıcı Yönetimi

* Kullanıcı ekleme, güncelleme, listeleme ve pasif silme
* Kullanıcı giriş ve oturum yönetimi
* Şifre sıfırlama ve şifre değiştirme
* Profil bilgileri ve profil resmi yönetimi
* Kullanıcıya rol atama
* Aktif kullanıcı ve rol kontrolleri
* Kullanıcı girişleri için doğrulama kontrolleri

### Makine Yönetimi

* Makine ekleme, güncelleme, listeleme ve pasif silme
* Makine adı, makine numarası, model, IP ve MFG bilgilerinin yönetilmesi
* Tekrarlanan makine numarası, IP ve MFG kayıtlarının kontrol edilmesi
* Makinelerin bant numarasına göre dinamik olarak gruplandırılması
* Bantların numarasına göre sıralanması
* Makinelerin saha yerleşimine uygun şekilde 10’lu kart düzeninde gösterilmesi
* Sistemde kayıtlı makine durumlarının merkezi panel üzerinden görüntülenmesi
* Yalnızca çalışan makineler için durdurma talebi oluşturulabilmesi
* Makine detaylarının Bootstrap modal üzerinden görüntülenmesi
* Son makine hareketi ve işlemi yapan kullanıcının görüntülenmesi
* Makine hareketlerinin ayrı operasyon loglarında tutulması
* Makine ile röle cihazı ve röle kanalı eşleştirilmesi
* Kullanımda olan röle kanallarının tekrar atanmasının engellenmesi

### Röle Cihaz Yönetimi

* HW-584 ağ röle cihazları ile haberleşme
* Birden fazla röle cihazının sistemde tanımlanabilmesi
* Röle cihazı IP ve port bilgilerinin yönetilmesi
* Röle cihazlarının makinelere atanabilmesi
* Her makine için ayrı röle kanalı tanımlanabilmesi
* Kullanılan kanalların yeni makine atamalarında gizlenmesi
* Makine güncellenirken mevcut kanalının korunabilmesi
* Rölenin belirli süre tetiklenip otomatik olarak bırakılması
* Röle bırakma işlemi başarısız olduğunda tekrar deneme
* Röle işlemlerinde hata bilgilerinin kayıt altına alınması

### Makine Komut Kuyruğu ve Servisi

* Durdurma taleplerinin `MakineKomutlar` tablosunda tutulması
* Web tarafındaki talep oluşturma işlemi ile komut işlemenin ayrılması
* Bekleyen komutların `MakineKomutServisi.KontrolEt()` üzerinden işlenmesi
* Aynı makine için mükerrer komut oluşturulmasının engellenmesi
* Atomik sahiplenme ile aynı komutun eş zamanlı işlenmesinin önlenmesi
* Komut sonucunun ve operasyon logunun transaction içerisinde birlikte kaydedilmesi
* Hata bilgilerinin sonuç ve log kayıtlarına aktarılması
* Devam eden işlemi gözeten güvenli kapanış yönetimi
* Kontrol aralığının Form timerı üzerinden 5000 ms olarak yönetilmesi

### Makine Sıralama Sistemi

* Her bant için ayrı sıralama ekranı
* Bootstrap modal üzerinden makine sıralama
* JavaScript ile sürükle-bırak desteği
* Makine sıralarının `sira_no` alanında saklanması
* Makine numarasına göre varsayılan sıralamaya dönme
* Sıralama işlemlerinde transaction ve rollback desteği
* Sıralama sonrası ana panel ve modal verilerinin yenilenmesi
* `NULL` sıra numaralarının listenin sonunda gösterilmesi
* Sayısal olmayan makine numaralarının güvenli şekilde sıralanması
* Toplu sıralama işleminin tek bir log kaydıyla kaydedilmesi

### Parametre Yönetimi

Uygulama genelinde kullanılan ortak değerler merkezi parametre altyapısı üzerinden yönetilmektedir.

* Parametre grubu oluşturma ve yönetme
* Parametre ekleme ve güncelleme
* Parametreleri gruplarına göre listeleme
* Parametre kodlarının merkezi olarak tanımlanması
* Ortak parametre kodlarının uygulama genelinde kullanılması

Örnek parametre grupları:

* Makine durumları
* İşlem türleri
* İşlem sonuçları

### Rol ve Yetkilendirme

* Rol ekleme, güncelleme ve pasif silme
* Sayfa ve işlem bazlı rol yetkilendirmesi
* Getirme, ekleme, güncelleme ve silme yetkileri
* Yetkiye göre menü ve işlem butonlarının gösterilmesi
* Yetkisiz doğrudan sayfa erişimlerinin engellenmesi
* Pasif role sahip kullanıcıların girişinin engellenmesi
* Oturum sırasında rolü pasif hâle getirilen kullanıcının sistemden çıkarılması
* Yönetim ekranlarının merkezi yetkilendirme sistemine dahil edilmesi

### Ortak Altyapı

* Stored procedure tabanlı veritabanı işlemleri
* Merkezi ADO.NET bağlantı ve komut yönetimi
* Transaction, commit ve rollback desteği
* Merkezi uygulama loglama sistemi
* Makine hareketlerine özel operasyon logları
* Tekrar kullanılabilir ASP.NET UserControl bileşenleri
* İstemci taraflı arama ve sayfalama
* Kayıt sayısı seçimi
* Bootstrap tabanlı responsive arayüz
* Merkezi mesaj ve Toastr bildirim sistemi

## 🛠️ Kullanılan Teknolojiler

* C#
* .NET
* ASP.NET Web Forms
* ADO.NET
* Microsoft SQL Server
* Stored Procedures
* HTML ve CSS
* Bootstrap
* JavaScript ve jQuery
* Toastr
* Font Awesome
* Ağ üzerinden cihaz haberleşmesi
* Git ve GitHub
* Visual Studio

## 🏗️ Proje Yapısı

| Bileşen | Görevi |
| --- | --- |
| **AsyModbus** | Web sayfaları ve kullanıcı arayüzü |
| **BusinessLayer** | Entity sınıfları, iş kuralları ve veritabanı işlemleri |
| **MasterPages** | Ortak sayfa düzeni, menü ve erişim kontrolleri |
| **UserControls** | Tekrar kullanılabilir arayüz bileşenleri |
| **Scripts** | JavaScript dosyaları ve veritabanı migration scriptleri |
| **Styles** | Uygulamaya özel CSS dosyaları |
| **SQL Server** | Veritabanı tabloları ve stored procedure’ler |
| **HW-584** | Durdurma sinyalini tetikleyen ağ röle cihazları |

## ⚙️ Makine Durdurma Akışı

Web paneli üzerinden oluşturulan durdurma talepleri komut kuyruğuna kaydedilir. Komut servisi bekleyen talepleri işleyerek ilgili röle kanalını tetikler.

1. Kullanıcının oturumu ve işlem yetkisi kontrol edilir.
2. Makinenin mevcut durumu ve röle bağlantısı kontrol edilir.
3. Durdurma talebi `MakineKomutlar` tablosuna kaydedilir.
4. Servis, bekleyen komutu atomik olarak sahiplenir.
5. İlgili röle kanalı yaklaşık 2 saniye boyunca tetiklenir.
6. Röle normal konumuna alınır; bırakma işlemi başarısızsa tekrar denenir.
7. Komut sonucu ve operasyon logu aynı transaction içerisinde kaydedilir.

> Durdurma talebinin oluşturulması, makinenin fiziksel olarak durduğunu doğrulamaz. Komutun işlenme sonucu ayrıca kayıt altına alınır.

## 🧭 Makine Kontrol Paneli ve Sıralama

Aktif ve röle bağlantısı tanımlanmış makineler bantlara göre gruplandırılır. Makine kartlarında temel bilgiler ve sistemde kayıtlı çalışma durumu gösterilir.

Her bandın **Düzenle** butonu ile açılan modal üzerinden makineler sürüklenerek sıralanabilir.

* Yeni sıralama makine ID’leri üzerinden hazırlanır.
* Her makinenin sırası `sira_no` alanına kaydedilir.
* Güncellemeler transaction içerisinde gerçekleştirilir.
* Bir güncelleme başarısız olursa değişiklikler rollback ile geri alınır.
* İşlem tamamlandığında panel yeni sıralamayla yenilenir.

## 🔐 Yetkilendirme Sistemi

Her rol için **Getirme**, **Ekleme**, **Güncelleme** ve **Silme** yetkileri ayrı ayrı yönetilir.

Menüler ve işlem butonları kullanıcının yetkisine göre gösterilir. Yetkisiz URL erişimleri engellenir ve kritik işlemlerden önce sunucu tarafında yeniden yetki kontrolü yapılır.

Sayfa adları merkezi `Sayfalar` sınıfında, yetki kontrolleri ise `IslemYetki` sınıfında yönetilir.

## 🧩 Tekrar Kullanılabilir Bileşenler

* Telefon numarası giriş bileşeni
* Veri listeleme bileşeni (`ucMyGrid`)
* İstemci taraflı arama ve sayfalama
* Kayıt sayısı seçimi
* Dinamik kolon oluşturma
* Yetkiye göre detay butonu gösterme
* Toplam kayıt sayısını gösterme

## 🗄️ Veritabanı ve Transaction Yönetimi

Veritabanı işlemleri ağırlıklı olarak stored procedure’ler üzerinden gerçekleştirilir. Bağlantı, komut, parametre ve transaction işlemleri merkezi `VeritabaniIslemleri` sınıfıyla yönetilir.

Transaction kullanılan başlıca işlemler:

* Rol ve sayfa yetkilerinin birlikte oluşturulması
* Rol ve bağlı yetki kayıtlarının birlikte pasif hâle getirilmesi
* Bir role ait yetkilerin toplu güncellenmesi
* Bant içerisindeki makinelerin yeniden sıralanması
* Makinelerin varsayılan sıralamaya döndürülmesi
* Komut sonucu ve makine operasyon logunun birlikte kaydedilmesi

İşlemlerden biri başarısız olduğunda ilgili veritabanı değişiklikleri rollback ile geri alınır.

## 📋 Log Sistemi

### Genel Uygulama Logları

Kullanıcıların ekleme, güncelleme, silme, görüntüleme, giriş ve çıkış işlemleri merkezi olarak kaydedilir.

### Makine Operasyon Logları

Makine işlemlerine ait kayıtlarda aşağıdaki bilgiler tutulur:

* İlgili makine
* Önceki ve yeni durum
* İşlem kaynağı
* İşlem türü ve sonucu
* İşlemi yapan kullanıcı
* IP adresi
* İşlem tarihi
* İşlem detayı

Durdurma talepleri ve işlenme sonuçları `MakineKomutlar` üzerinden, makine hareketleri ise operasyon logları üzerinden takip edilir.

## 🎨 Arayüz ve Bildirimler

* Bootstrap tabanlı responsive sayfa düzenleri
* Bant bazlı makine kartları
* Makine detay ve sıralama modalları
* Yetkiye göre oluşturulan menüler
* Ortak MasterPage yapısı
* Tekrar kullanılabilir veri tabloları
* Toastr bildirimleri

Uygulama mesajları merkezi `Mesajlar` sınıfında tutulur. Başarı, uyarı, hata ve bilgilendirme mesajları Toastr üzerinden gösterilir.

## 🧪 Test ve Doğrulama

* Önceki doğrudan web tetikleme akışı gerçek HW-584 cihazları üzerinde test edilmiştir.
* Yeni komut kuyruğu ve servis akışı sahte röle kullanılarak 13 senaryo ile doğrulanmıştır.
* Yeni akışın Form, gerçek röle ve makineyle uçtan uca saha testleri devam eden çalışmalar kapsamındadır.

## 📌 Geliştirme Durumu

### Tamamlanan Temel Bölümler

* Kullanıcı ve makine yönetimi
* Makine kontrol paneli ve bant bazlı gruplandırma
* Sürükle-bırak makine sıralama
* Rol ve işlem bazlı yetkilendirme
* Oturum ve erişim kontrolleri
* Röle cihaz yönetimi ve kanal eşleştirmesi
* HW-584 röle tetikleme altyapısı
* Makine komut kuyruğu ve komut servisi
* Mükerrer komut kontrolü ve atomik sahiplenme
* Komut sonucu ve operasyon logu için transaction desteği
* Güvenli kapanış yönetimi
* Genel uygulama ve makine operasyon logları
* Merkezi parametre yönetimi
* Merkezi mesaj ve bildirim sistemi
* Responsive kullanıcı arayüzü

### Devam Eden ve Planlanan Çalışmalar

* Röle iletişiminin `AsyModbus.KomutForm` uygulamasına taşınması
* Yeni komut akışının gerçek cihazlarla uçtan uca test edilmesi
* Gerçek makine durumlarının periyodik olarak okunması
* Okunan durumların sisteme aktarılması
* Saha kaynaklı durum değişikliklerinin otomatik loglanması
* Makine, pano ve saha kablolamalarının tamamlanması
* Bağlantı kopması ve zaman aşımı yönetiminin geliştirilmesi
* Saha testlerinin genişletilmesi
* Genel kod temizliği ve son kullanıcı testleri

## 📄 Dokümantasyon

Projenin gereksinimleri ve analiz dokümanı Türkçe olarak hazırlanmıştır.

* [Proje Analizi](docs/Proje-Analizi.md)

## 🎯 Projenin Amacı

Bu proje yalnızca bir web uygulaması geliştirmek değil, aynı zamanda yazılım ile gerçek endüstriyel saha ekipmanları arasındaki entegrasyonu deneyimlemek amacıyla geliştirilmektedir.

Proje kapsamında:

* Backend geliştirme
* Frontend geliştirme
* Veritabanı tasarımı
* ASP.NET Web Forms
* ADO.NET
* Nesne yönelimli programlama
* Rol ve yetkilendirme sistemleri
* Transaction yönetimi
* Uygulama ve operasyon loglama
* Merkezi parametre yönetimi
* Tekrar kullanılabilir bileşen geliştirme
* Responsive arayüz geliştirme
* Ağ üzerinden cihaz haberleşmesi
* Röle kontrolü
* Endüstriyel sistem entegrasyonu
* Temiz ve sürdürülebilir kod geliştirme

konularında uygulamalı deneyim kazanılması hedeflenmektedir.

## Webde hazırlanan röle istekleri

Web, mevcut `BusinessLayer` entity/SP yapısını kullanarak ON/OFF cihaz adreslerini
`MakineKomutlar` kaydına yazar. Form, bu hazır adresleri doğrudan rölede çalıştırır.
Web cihazla ağ iletişimi kurmaz; Form makine-röle-kanal eşleştirmesinden URL üretmez.
Mevcut cihaz sözleşmesi HTTP GET ve HTTP 200 doğrulamasıdır; URL şeması HTTPS'e
zorlanmaz. HTTPS destekli hazır adreslerde normal sertifika doğrulaması korunur.
Mevcut cihaz modeli protokol seçimi alanı içermediğinden web hazırlayıcısı mevcut
HTTP protokolünü kullanır. Başarı fiziksel makine duruşunun sensörle doğrulanması değildir.

Geçiş, SQL sırası, testler ve kurtarma adımları:
[Hazır istek geçiş rehberi](docs/Hazir-Istek-Gecis.md).
