# AsyModbus

AsyModbus; C#, ASP.NET Web Forms, ADO.NET ve Microsoft SQL Server kullanılarak geliştirilen web tabanlı bir makine yönetim, takip ve kontrol uygulamasıdır.

Projenin temel amacı; tekstil makinelerinin merkezi bir web paneli üzerinden izlenmesi, güvenli şekilde durdurulması ve gerçekleşen makine hareketlerinin kayıt altına alınmasıdır.

Sistem; web uygulaması, SQL Server veritabanı ve ağ üzerinden kontrol edilen röle cihazlarını bir araya getirerek yazılım ile saha ekipmanları arasında kontrollü bir yapı oluşturmayı amaçlamaktadır.

Makine durumlarının gerçek zamanlı olarak otomatik okunması için gerekli saha ve haberleşme çalışmaları devam etmektedir. Makine durdurma işlemi ise HW-584 röle cihazları kullanılarak test edilmiş ve web uygulamasından fiziksel röle tetikleme işlemi gerçekleştirilmiştir.

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
* Makine çalışma durumlarının merkezi panel üzerinden görüntülenmesi
* Yalnızca çalışan makineler için durdurma işleminin aktif edilmesi
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
* Web uygulamasından fiziksel röle tetikleme
* Rölenin belirli süre aktif tutulup otomatik olarak bırakılması
* Röle bırakma işlemi başarısız olduğunda tekrar deneme
* Röle işlemlerinde hata bilgisinin kayıt altına alınması

### Makine Durdurma Sistemi

Makine kontrol paneli üzerinden yalnızca güvenli durdurma işlemi gerçekleştirilmektedir. Sistem üzerinden makinelerin uzaktan başlatılması desteklenmemektedir.

Durdurma işlemi sırasında:

1. Kullanıcının oturumu ve yetkisi kontrol edilir.
2. Makinenin mevcut durumu kontrol edilir.
3. Makineye atanmış röle cihazı ve kanal bilgileri alınır.
4. İlgili HW-584 röle cihazına ağ üzerinden komut gönderilir.
5. Röle kanalı yaklaşık 2 saniye boyunca tetiklenir.
6. Süre sonunda röle tekrar normal konumuna alınır.
7. İşlemin başarılı veya başarısız sonucu operasyon loguna kaydedilir.

Rölenin normal konumuna dönememesi durumunda bırakma komutu tekrar gönderilmektedir.

Bu yapı gerçek HW-584 cihazları üzerinde test edilmiş ve web uygulamasından röle tetikleme işlemi doğrulanmıştır.

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

### Parametre Yönetimi

Uygulama içerisinde kullanılan ortak ve tekrar eden değerlerin kod içerisine sabit olarak yazılması yerine merkezi olarak yönetilebilmesi için genel bir parametre altyapısı oluşturulmuştur.

Parametre yapısı iki temel bölümden oluşmaktadır:

* **Parametre Grupları** — Benzer parametrelerin kategorize edilmesini sağlar.
* **Parametreler** — İlgili gruba ait kod ve değerlerin tutulmasını sağlar.

Bu yapı sayesinde makine durumu, işlem türü ve işlem sonucu gibi sistem genelinde kullanılan değerler merkezi olarak yönetilebilmektedir.

Örnek parametre grupları:

* Makine durumları
* İşlem türleri
* İşlem sonuçları

Parametre yönetimi kapsamında:

* Parametre grubu oluşturma ve yönetme
* Parametre ekleme ve güncelleme
* Parametreleri gruplarına göre listeleme
* Sistem tarafından kullanılan parametre kodlarının merkezi olarak tanımlanması
* Parametre kodlarının uygulama içerisinde ortak şekilde kullanılması
* Sistem parametrelerinin kontrollü şekilde yönetilmesi

işlemleri desteklenmektedir.

### Rol ve Yetkilendirme

* Rol ekleme, güncelleme ve pasif silme
* Sayfa ve işlem bazlı rol yetkilendirmesi
* Getirme, ekleme, güncelleme ve silme yetkileri
* Yetkiye göre menülerin gösterilmesi
* Yetkiye göre sayfa butonlarının gösterilmesi
* Yetkisiz doğrudan sayfa erişimlerinin engellenmesi
* Pasif role sahip kullanıcıların girişinin engellenmesi
* Oturum sırasında rolü pasif hâle getirilen kullanıcının sistemden çıkarılması
* Yeni yönetim ekranlarının merkezi yetkilendirme sistemine dahil edilmesi

### Ortak Altyapı

* Stored procedure tabanlı veritabanı işlemleri
* Merkezi ADO.NET bağlantı ve komut yönetimi
* Transaction, commit ve rollback desteği
* Otomatik uygulama loglama sistemi
* Makine hareketlerine özel operasyon logları
* Merkezi parametre yönetimi
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
* TCP/IP tabanlı cihaz haberleşmesi
* Git ve GitHub
* Visual Studio

## 🏗️ Proje Yapısı

Proje; kullanıcı arayüzü, iş mantığı, veritabanı işlemleri, saha cihazları ve tekrar kullanılabilir bileşenlerin birbirinden ayrıldığı düzenli bir yapı üzerine kurulmuştur.

Başlıca proje bileşenleri:

* **AsyModbus** — Web sayfaları ve kullanıcı arayüzü
* **BusinessLayer** — Entity sınıfları, iş kuralları ve veritabanı işlemleri
* **MasterPages** — Ortak sayfa düzeni, menü ve erişim kontrolleri
* **UserControls** — Tekrar kullanılabilir arayüz bileşenleri
* **Scripts** — İstemci taraflı JavaScript işlemleri ve veritabanı migration scriptleri
* **Styles** — Uygulamaya özel responsive CSS dosyaları
* **SQL Server** — Veritabanı tabloları ve stored procedure’ler
* **HW-584** — Makinelerin güvenli durdurma sinyalinin fiziksel olarak tetiklenmesini sağlayan ağ röle cihazları

## ⚙️ Makine Kontrol Paneli

Ana sayfada aktif ve gerekli röle bağlantısı tanımlanmış makineler bant numaralarına göre otomatik olarak gruplandırılmaktadır.

Her makine kartında makineye ait temel bilgiler ve güncel çalışma durumu görüntülenmektedir.

Desteklenen temel durumlar:

* **Çalışıyor** — Makine çalışmaktadır ve durdurma işlemi gerçekleştirilebilir.
* **Durduruldu** — Makine durmaktadır ve durdurma butonu devre dışıdır.
* **Bağlantı Yok** — Makineden güncel durum alınamamaktadır.

Sistem güvenliği nedeniyle web paneli üzerinden yalnızca makine durdurma işlemi desteklenmektedir. Makinelerin uzaktan başlatılması sistemin kapsamı dışındadır.

Durdurma komutu, makineye atanmış röle cihazı ve kanal üzerinden fiziksel olarak tetiklenmektedir.

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

## 🔐 Rol ve Yetkilendirme Sistemi

Uygulamada sayfa ve işlem bazlı rol yetkilendirme sistemi bulunmaktadır.

Her rol için aşağıdaki yetkiler ayrı ayrı yönetilebilir:

* **Getirme**
* **Ekleme**
* **Güncelleme**
* **Silme**

Menüler kullanıcının yetkisine göre oluşturulmakta, yetkisiz doğrudan URL erişimleri engellenmekte ve kritik işlemler gerçekleştirilmeden önce sunucu tarafında tekrar yetki kontrolü yapılmaktadır.

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

## 🗄️ Veritabanı

Uygulamada Microsoft SQL Server kullanılmaktadır. Veritabanı işlemleri ağırlıklı olarak stored procedure’ler üzerinden gerçekleştirilmektedir.

Stored procedure kullanılan başlıca işlemler:

* Kullanıcı yönetimi
* Kullanıcı giriş ve şifre işlemleri
* Makine yönetimi
* Makine durumu ve sıralama işlemleri
* Makine operasyon logları
* Röle cihaz yönetimi
* Kullanılan röle kanallarının belirlenmesi
* Rol ve yetki yönetimi
* Parametre grubu ve parametre yönetimi
* Genel uygulama logları

ADO.NET, uygulama ile SQL Server arasındaki iletişimi sağlamaktadır. Bağlantı, komut, parametre ve transaction işlemleri merkezi `VeritabaniIslemleri` sınıfı üzerinden yönetilmektedir.

## 🔄 Transaction Yönetimi

Birbirine bağlı birden fazla veritabanı işleminin güvenli şekilde yürütülmesi için transaction desteği kullanılmaktadır.

Transaction kullanılan başlıca işlemler:

* Yeni rol ve role ait sayfa yetkilerinin birlikte oluşturulması
* Rol ve bağlı yetki kayıtlarının birlikte pasif hâle getirilmesi
* Bir role ait birden fazla yetkinin toplu olarak güncellenmesi
* Bant içerisindeki makinelerin toplu olarak yeniden sıralanması
* Makinelerin varsayılan sıralamaya döndürülmesi

İşlemlerden biri başarısız olduğunda yapılan değişiklikler rollback ile geri alınmaktadır.

## 📋 Log Sistemi

Projede genel uygulama logları ve makine operasyon logları birbirinden ayrılmıştır.

### Genel Uygulama Logları

Kullanıcıların uygulama içerisinde gerçekleştirdiği ekleme, güncelleme, silme, görüntüleme, giriş ve çıkış gibi işlemler merkezi olarak loglanmaktadır.

### Makine Operasyon Logları

Makine üzerinde gerçekleştirilen işlemler ayrıca operasyon loglarında tutulmaktadır.

Bu kayıtlarda:

* İlgili makine
* Önceki durum
* Yeni durum
* İşlem kaynağı
* İşlem türü
* İşlem sonucu
* İşlemi yapan kullanıcı
* IP adresi
* İşlem tarihi
* İşlem detayı

gibi bilgiler saklanmaktadır.

Web üzerinden gerçekleştirilen röle tabanlı durdurma işlemlerinin başarılı veya başarısız sonuçları da bu yapıya kaydedilmektedir.

## 🎨 Kullanıcı Arayüzü

Uygulama arayüzü Bootstrap kullanılarak responsive olacak şekilde hazırlanmıştır.

Arayüz içerisinde:

* Responsive sayfa düzenleri
* Dinamik makine kontrol paneli
* Bant bazlı makine gruplandırması
* Makine detay modalları
* Sürükle-bırak sıralama
* Ortak MasterPage
* Yetkiye göre dinamik menü
* Tekrar kullanılabilir tablolar
* Arama ve sayfalama
* Toastr bildirimleri

kullanılmaktadır.

Projeye özel tasarım ihtiyaçlarında ayrı CSS dosyaları kullanılmaktadır.

## 🔔 Mesaj ve Bildirim Sistemi

Uygulama mesajları merkezi `Mesajlar` sınıfında tutulmaktadır.

Desteklenen bildirim türleri:

* Başarılı
* Uyarı
* Hata
* Bilgilendirme

Bildirimlerin kullanıcıya gösterilmesi için Toastr kullanılmaktadır.

## 📌 Proje Durumu

🚧 **Proje geliştirme ve saha entegrasyonu aşamasındadır.**

Web uygulamasının temel yazılım altyapısının büyük bölümü tamamlanmıştır.

Tamamlanan ana bölümler:

* Kullanıcı yönetimi
* Makine kayıt yönetimi
* Makine kontrol paneli
* Bant bazlı makine gruplandırması
* Makine sıralama sistemi
* Makine operasyon logları
* Rol ve yetkilendirme
* Oturum ve erişim kontrolü
* Genel log sistemi
* Röle cihaz yönetimi
* Makine-röle kanal eşleştirmesi
* HW-584 üzerinden röle tetikleme
* Web üzerinden fiziksel durdurma testleri
* Merkezi parametre altyapısı
* Parametre yönetim ekranı
* Merkezi mesaj sistemi
* Transaction altyapısı
* Responsive kullanıcı arayüzü

Devam eden saha çalışmaları:

* Gerçek makine durumlarının otomatik olarak okunması
* Makine ve pano bağlantılarının tamamlanması
* Saha kablolamalarının tamamlanması
* Ağ bağlantılarının kararlı hâle getirilmesi
* Gerçek makinelerde durdurma senaryolarının genişletilmesi
* Bağlantı kopması ve hata senaryolarının test edilmesi

Sonraki geliştirme aşamasında:

* Makinelerden periyodik olarak canlı durum okunması
* Okunan durumların sisteme aktarılması
* Saha kaynaklı durum değişikliklerinin otomatik loglanması
* Bağlantı kopması ve zaman aşımı yönetiminin geliştirilmesi
* Gerçek saha testlerinin genişletilmesi
* Hata yönetimi ve güvenlik kontrollerinin geliştirilmesi
* Genel kod temizliği
* Son kullanıcı testleri

## 📄 Dokümantasyon

Projenin ayrıntılı gereksinimleri ve analiz dokümanı Türkçe olarak hazırlanmıştır.

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
