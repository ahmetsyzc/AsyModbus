# AsyModbus

AsyModbus; C#, ASP.NET Web Forms, ADO.NET ve Microsoft SQL Server kullanılarak geliştirilen web tabanlı bir makine yönetim, takip ve kontrol uygulamasıdır.

Projenin temel amacı; tekstil makinelerinin merkezi bir web paneli üzerinden izlenmesi, güvenli şekilde durdurulması ve gerçekleşen makine hareketlerinin kayıt altına alınmasıdır.

Makine durumlarının gerçek zamanlı okunabilmesi için gerekli saha, kablolama ve Modbus haberleşme çalışmaları devam etmektedir. Mevcut aşamada durum ve durdurma akışları SQL Server üzerinden test edilmektedir.

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
* Çalışıyor, durduruldu ve bağlantı yok durumlarının renklerle gösterilmesi
* Yalnızca çalışan makineler için durdurma işleminin aktif edilmesi
* Makine detaylarının Bootstrap modal üzerinden görüntülenmesi
* Son makine hareketi ve işlemi yapan kullanıcının görüntülenmesi
* Makine hareketlerinin ayrı operasyon loglarında tutulması

### Makine Sıralama Sistemi

* Her bant için ayrı sıralama ekranı
* Bootstrap modal üzerinden makine sıralama
* JavaScript ile sürükle-bırak desteği
* Makine sıralarının `sira_no` alanında saklanması
* Makine numarasına göre varsayılan sıralamaya dönme
* Sıralama işlemlerinde transaction ve rollback desteği
* Sıralama sonrası ana matris ve modal verilerinin yenilenmesi
* `NULL` sıra numaralarının listenin sonunda gösterilmesi
* Sayısal olmayan makine numaralarının güvenli şekilde sıralanması
* Toplu sıralama işleminin tek bir log kaydıyla kaydedilmesi

### Rol ve Yetkilendirme

* Rol ekleme, güncelleme ve pasif silme
* Sayfa ve işlem bazlı rol yetkilendirmesi
* Getirme, ekleme, güncelleme ve silme yetkileri
* Yetkiye göre menülerin gösterilmesi
* Yetkiye göre sayfa butonlarının gösterilmesi
* Yetkisiz doğrudan sayfa erişimlerinin engellenmesi
* Pasif role sahip kullanıcıların girişinin engellenmesi
* Oturum sırasında rolü pasif hâle getirilen kullanıcının sistemden çıkarılması

### Ortak Altyapı

* Stored procedure tabanlı veritabanı işlemleri
* Merkezi ADO.NET bağlantı ve komut yönetimi
* Transaction, commit ve rollback desteği
* Otomatik uygulama loglama sistemi
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
* HTML
* CSS
* Bootstrap
* JavaScript
* jQuery
* Toastr
* Font Awesome
* Git ve GitHub
* Visual Studio

## 🏗️ Proje Yapısı

Proje; kullanıcı arayüzü, iş mantığı, veritabanı işlemleri ve tekrar kullanılabilir bileşenlerin birbirinden ayrıldığı düzenli bir yapı üzerine kurulmuştur.

Başlıca proje bileşenleri:

* **AsyModbus** — Web sayfaları ve kullanıcı arayüzü
* **BusinessLayer** — Entity sınıfları, iş kuralları ve veritabanı işlemleri
* **MasterPages** — Ortak sayfa düzeni, menü ve erişim kontrolleri
* **UserControls** — Tekrar kullanılabilir arayüz bileşenleri
* **Scripts** — Sürükle-bırak ve diğer istemci taraflı JavaScript işlemleri
* **Styles** — Uygulamaya özel responsive CSS dosyaları
* **SQL Server** — Veritabanı tabloları ve stored procedure’ler

## ⚙️ Makine Kontrol Paneli

Ana sayfada aktif makineler bant numaralarına göre otomatik olarak gruplandırılmaktadır.

Her makine kartında aşağıdaki bilgiler gösterilmektedir:

* Makine adı
* Makine numarası
* Model adı
* IP adresi
* MFG numarası
* Güncel çalışma durumu

Desteklenen durumlar:

* **Çalışıyor** — Makine çalışmaktadır ve durdurma butonu kullanılabilir.
* **Durduruldu** — Makine durmaktadır ve durdurma butonu devre dışıdır.
* **Bağlantı Yok** — Makineden güncel durum alınamamaktadır.

Sistem güvenliği nedeniyle web paneli üzerinden yalnızca makine durdurma işlemi planlanmaktadır. Makinelerin uzaktan başlatılması desteklenmeyecektir.

Mevcut geliştirme aşamasında durdurma işlemi SQL Server üzerinde simüle edilmektedir. Gerçek makine komutları, Modbus entegrasyonu tamamlandıktan sonra devreye alınacaktır.

## 🧭 Bant ve Makine Sıralaması

Makineler fiziksel saha düzenini temsil eden bantlara göre gösterilmektedir.

Her bandın başlığında bulunan **Düzenle** butonu ile sıralama modalı açılmaktadır. Kullanıcı makineleri sürükleyip bırakarak bant içerisindeki sıralamayı değiştirebilir.

Yeni sıralama:

* JavaScript tarafından makine ID’leri üzerinden hazırlanır.
* HiddenField aracılığıyla sunucu tarafına gönderilir.
* Her makinenin `sira_no` alanına kaydedilir.
* İşlem transaction içerisinde gerçekleştirilir.
* Bir güncelleme başarısız olursa tüm değişiklikler rollback ile geri alınır.
* İşlem tamamlandığında ana ekran yeni sırayla tekrar oluşturulur.

**Varsayılana Dön** işlemi, seçilen banttaki makineleri makine numarasına göre yeniden sıralar.

## 🔐 Rol ve Yetkilendirme Sistemi

Uygulamada sayfa ve işlem bazlı rol yetkilendirme sistemi bulunmaktadır.

Her rol için aşağıdaki yetkiler ayrı ayrı yönetilebilir:

* **Getirme** — Sayfaya erişme ve kayıtları görüntüleme
* **Ekleme** — Yeni kayıt oluşturma
* **Güncelleme** — Mevcut kayıtları değiştirme
* **Silme** — Kayıtları pasif hâle getirme

Yetkilendirme sistemi kapsamında:

* Menüler kullanıcının getirme yetkisine göre gösterilir.
* Yetkisiz kullanıcıların doğrudan URL ile sayfalara erişmesi engellenir.
* Sayfalardaki butonlar ilgili işlem yetkisine göre gösterilir.
* Sunucu tarafında işlem öncesinde tekrar yetki kontrolü yapılır.
* Listeleme bileşenindeki **Aç** butonu hedef sayfanın yetkisine göre gizlenir.
* Yeni rol oluşturulduğunda sayfa yetkileri otomatik olarak oluşturulur.
* Yeni rolün işlem yetkileri başlangıçta kapalıdır.
* Rol pasif hâle getirildiğinde bağlı yetki kayıtları da pasif hâle getirilir.
* Pasif bir role sahip kullanıcı sisteme giriş yapamaz.
* Oturum sırasında rolü pasif hâle getirilen kullanıcı sistemden çıkarılır.

Sayfa adları merkezi `Sayfalar` sınıfında tutulmaktadır. Yetki kontrolleri merkezi `IslemYetki` sınıfı üzerinden gerçekleştirilmektedir.

## 🧩 Tekrar Kullanılabilir Bileşenler

Tekrarlanan arayüz kodlarını azaltmak amacıyla ASP.NET UserControl bileşenleri kullanılmaktadır.

Mevcut bileşenler:

* Telefon numarası giriş bileşeni
* Tekrar kullanılabilir veri listeleme bileşeni (`ucMyGrid`)
* İstemci taraflı arama
* Kayıt sayısı seçimi
* İstemci taraflı sayfalama
* Dinamik kolon oluşturma
* Yetkiye göre detay butonu gösterme
* Toplam kayıt sayısını gösterme

Bu yapı sayesinde ortak listeleme işlemleri farklı sayfalarda tekrar kod yazılmadan kullanılabilmektedir.

## 🗄️ Veritabanı

Uygulamada Microsoft SQL Server kullanılmaktadır. Veritabanı işlemleri ağırlıklı olarak stored procedure’ler üzerinden gerçekleştirilmektedir.

Stored procedure kullanılan başlıca işlemler:

* Kullanıcı ekleme, güncelleme, silme ve listeleme
* Kullanıcı giriş ve şifre kontrolleri
* Makine ekleme, güncelleme, silme ve listeleme
* Makine durumu ve sıra numarası güncelleme
* Tekrarlanan makine kayıtlarının kontrol edilmesi
* Makine operasyon loglarının oluşturulması
* Rol ekleme, güncelleme, silme ve listeleme
* Rol yetkilerinin eklenmesi ve güncellenmesi
* Sayfa ve işlem yetkilerinin kontrol edilmesi
* Genel log kayıtlarının oluşturulması
* Tek kayıt ve tablo verilerinin getirilmesi

ADO.NET, uygulama ile SQL Server arasındaki iletişimi sağlamaktadır. Bağlantı, komut, parametre ve transaction işlemleri merkezi `VeritabaniIslemleri` sınıfı üzerinden yönetilmektedir.

## 🔄 Transaction Yönetimi

Birbirine bağlı birden fazla veritabanı işleminin güvenli şekilde yürütülmesi için transaction desteği kullanılmaktadır.

Transaction kullanılan başlıca işlemler:

* Yeni rol ve role ait sayfa yetkilerinin birlikte oluşturulması
* Rol ve bağlı yetki kayıtlarının birlikte pasif hâle getirilmesi
* Bir role ait birden fazla yetkinin toplu olarak güncellenmesi
* Bant içerisindeki makinelerin toplu olarak yeniden sıralanması
* Makinelerin varsayılan sıralamaya döndürülmesi

İşlemlerden biri başarısız olduğunda yapılan bütün değişiklikler rollback ile geri alınmaktadır. Tüm işlemler başarılı olduğunda transaction commit edilmektedir.

## 📋 Log Sistemi

Projede iki farklı log yapısı bulunmaktadır.

### Genel Uygulama Logları

Kullanıcıların uygulama içerisinde gerçekleştirdiği ekleme, güncelleme, silme, görüntüleme, giriş ve çıkış işlemleri merkezi olarak loglanmaktadır.

Genel log kayıtlarında:

* Kullanıcı
* İşlem yapılan tablo
* İşlem adı
* İşlem türü
* URL
* IP adresi
* İşlem tarihi
* İşlem detayı

bilgileri tutulmaktadır.

Toplu makine sıralama işlemleri, her makine için ayrı kayıt oluşturmak yerine tek işlem olarak loglanmaktadır.

### Makine Operasyon Logları

Makine durum değişiklikleri `MakinelerLoglar` yapısı üzerinden ayrıca kaydedilmektedir.

Makine operasyon loglarında:

* İlgili makine
* Önceki durum
* Yeni durum
* İşlem kaynağı
* İşlem türü
* İşlem sonucu
* İşlemi yapan kullanıcı
* IP adresi
* İşlem tarihi
* Açıklama

bilgileri tutulmaktadır.

Bu ayrım sayesinde uygulama kullanıcı hareketleri ile makinelerin operasyonel geçmişi birbirinden bağımsız olarak takip edilebilmektedir.

## 🎨 Kullanıcı Arayüzü

Uygulama arayüzü Bootstrap kullanılarak responsive olacak şekilde hazırlanmıştır.

Mevcut arayüz özellikleri:

* Responsive sayfa düzenleri
* Bootstrap kart ve form yapıları
* Dinamik makine kontrol paneli
* 10’lu makine kartı yerleşimi
* Bant bazlı makine gruplandırması
* Bootstrap detay ve sıralama modalları
* Sürükle-bırak makine sıralaması
* Responsive giriş ve şifre sıfırlama sayfaları
* Ortak MasterPage yapısı
* Sabit yan menü
* Yetkiye göre oluşturulan dinamik menü
* Tekrar kullanılabilir tablo tasarımı
* Arama ve sayfalama kontrolleri
* Sabit tablo başlıkları
* Bootstrap butonları ve form bileşenleri
* Toastr bildirimleri

Projeye özel tasarım ihtiyaçlarında ayrı CSS dosyaları kullanılmaktadır.

## 🔔 Mesaj ve Bildirim Sistemi

Uygulama mesajları merkezi `Mesajlar` sınıfında tutulmaktadır.

Mesaj sistemi aşağıdaki bölümleri birbirinden ayırmaktadır:

* Mesaj içeriği
* Mesaj türü
* Mesajın kullanıcıya gösterilmesi

Desteklenen bildirim türleri:

* Başarılı
* Uyarı
* Hata
* Bilgilendirme

Bildirimlerin kullanıcıya gösterilmesi için Toastr kullanılmaktadır. Bu yapı, aynı mesajların farklı sayfalarda tekrar yazılmasını önlemekte ve uygulama genelinde tutarlı bildirimler sağlamaktadır.

## 📌 Proje Durumu

🚧 **Proje geliştirme ve saha entegrasyonu aşamasındadır.**

Web uygulamasının temel yazılım altyapısı büyük ölçüde tamamlanmıştır.

Tamamlanan ana bölümler:

* Kullanıcı yönetimi
* Makine kayıt yönetimi
* Makine kontrol paneli
* Bant bazlı makine gruplandırması
* Sürükle-bırak makine sıralaması
* Makine durum ve operasyon logu altyapısı
* Rol yönetimi
* Rol bazlı yetkilendirme
* Oturum ve erişim kontrolü
* Genel log sistemi
* Tekrar kullanılabilir bileşenler
* Merkezi mesaj sistemi
* Transaction altyapısı
* Responsive kullanıcı arayüzü

Devam eden saha çalışmaları:

* Makine ve pano bağlantılarının incelenmesi
* Gerekli kablolamaların hazırlanması
* Modbus destekli donanımların yapılandırılması
* Ağ ve IP bağlantılarının doğrulanması
* Güvenli durdurma sinyalinin test edilmesi
* Gerçek makinelerle haberleşme senaryolarının hazırlanması

Sonraki geliştirme aşamasında:

* Modbus haberleşme servisinin geliştirilmesi
* Makinelerden periyodik olarak canlı durum okunması
* Okunan durumların SQL Server’a aktarılması
* Saha kaynaklı durum değişikliklerinin otomatik loglanması
* Web panelindeki durdurma komutunun gerçek sisteme bağlanması
* Bağlantı kopması ve zaman aşımı yönetimi
* Gerçek saha testlerinin gerçekleştirilmesi
* Hata yönetimi ve güvenlik kontrollerinin geliştirilmesi
* Genel kod temizliği ve son kullanıcı testleri

## 📄 Dokümantasyon

Projenin ayrıntılı gereksinimleri ve analiz dokümanı Türkçe olarak hazırlanmıştır.

* [Proje Analizi](docs/Proje-Analizi.md)

## 🎯 Projenin Amacı

Bu proje aşağıdaki alanlarda bilgi ve deneyim kazanmak amacıyla geliştirilmektedir:

* Backend geliştirme
* Frontend geliştirme
* Veritabanı tasarımı
* ASP.NET Web Forms
* ADO.NET
* Nesne yönelimli programlama
* Rol ve yetkilendirme sistemleri
* Transaction yönetimi
* Uygulama ve operasyon loglama
* Tekrar kullanılabilir bileşen geliştirme
* JavaScript ile sürükle-bırak işlemleri
* Responsive arayüz geliştirme
* Yazılım mimarisi
* Temiz ve sürdürülebilir kod yazımı
* Endüstriyel sistem ve Modbus entegrasyonu
