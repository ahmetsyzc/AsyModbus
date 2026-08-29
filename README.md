# AsyModbus

AsyModbus; C#, ASP.NET Web Forms, ADO.NET ve Microsoft SQL Server kullanılarak geliştirilen web tabanlı bir makine yönetim ve takip uygulamasıdır.

Projenin amacı; web geliştirme, veritabanı işlemleri, yetkilendirme, tekrar kullanılabilir bileşenler ve katmanlı uygulama tasarımı konularında uygulamalı deneyim kazanmaktır.

## 🚀 Mevcut Özellikler

* Kullanıcı ekleme, güncelleme, listeleme ve pasif silme
* Kullanıcı giriş ve oturum yönetimi
* Şifre sıfırlama ve şifre değiştirme
* Profil bilgileri ve profil resmi yönetimi
* Makine ekleme, güncelleme, listeleme ve pasif silme
* Rol ekleme, güncelleme ve pasif silme
* Rol bazlı sayfa ve işlem yetkilendirmesi
* Getirme, ekleme, güncelleme ve silme yetkileri
* Yetkiye göre menü ve işlem butonlarının gösterilmesi
* Yetkisiz doğrudan sayfa erişimlerinin engellenmesi
* Rolü pasif hâle getirilen kullanıcıların sisteme girişinin engellenmesi
* Uygulama işlemlerinin otomatik olarak loglanması
* SQL Server ve stored procedure tabanlı veritabanı işlemleri
* Transaction ve rollback desteği
* Tekrar kullanılabilir ASP.NET UserControl bileşenleri
* İstemci taraflı arama ve sayfalama
* Listelenecek kayıt sayısının seçilebilmesi
* Bootstrap tabanlı responsive arayüz
* Merkezi mesaj ve Toastr bildirim sistemi
* Kullanıcı girişleri için doğrulama kontrolleri

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
* Git ve GitHub
* Visual Studio

## 🏗️ Proje Yapısı

Proje; kullanıcı arayüzü, iş mantığı, veritabanı işlemleri ve tekrar kullanılabilir bileşenlerin birbirinden ayrıldığı düzenli bir yapı üzerine kurulmuştur.

Başlıca proje bileşenleri:

* **AsyModbus** — Web sayfaları ve kullanıcı arayüzü
* **BusinessLayer** — Entity sınıfları, iş kuralları ve veritabanı işlemleri
* **MasterPages** — Ortak sayfa düzeni, menü ve erişim kontrolleri
* **UserControls** — Tekrar kullanılabilir arayüz bileşenleri
* **SQL Server** — Veritabanı tabloları ve stored procedure'ler
* **Scripts** — İstemci taraflı JavaScript işlemleri
* **Styles** — Uygulamaya özel CSS dosyaları

## 👤 Kullanıcı Yönetimi

Uygulamada kullanıcılar için aşağıdaki işlemler bulunmaktadır:

* Kullanıcı ekleme
* Kullanıcı bilgilerini güncelleme
* Kullanıcıları listeleme
* Kullanıcıları pasif hâle getirme
* Kullanıcı giriş kontrolü
* Aktif ve pasif kullanıcı kontrolü
* Şifre sıfırlama ve değiştirme
* Profil bilgilerini düzenleme
* Profil resmi yükleme
* Kullanıcı verilerini doğrulama
* Kullanıcıya rol atama
* Oturum bilgilerini merkezi olarak yönetme

## ⚙️ Makine Yönetimi

Makine modülü aşağıdaki işlemleri desteklemektedir:

* Makine ekleme
* Makine bilgilerini güncelleme
* Makineleri listeleme
* Makine kayıtlarını pasif hâle getirme
* Makine detaylarını görüntüleme
* Makine numarası, IP ve MFG kayıt kontrolleri
* Kullanıcı girişlerinin doğrulanması

Makine bilgilerinin gerçek sistemlerden alınması için gerekli fiziksel bağlantı ve Modbus haberleşme çalışmaları projenin sonraki aşamasında gerçekleştirilecektir.

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
* Buton işlemlerinde sunucu tarafında tekrar yetki kontrolü yapılır.
* Listeleme bileşenindeki **Aç** butonu hedef sayfanın yetkisine göre gizlenir.
* Yeni rol oluşturulduğunda bütün yetkilendirilen sayfalar otomatik olarak eklenir.
* Yeni rolün işlem yetkileri başlangıçta kapalı olarak oluşturulur.
* Rol pasif hâle getirildiğinde role ait yetki kayıtları da pasif hâle getirilir.
* Pasif bir role sahip kullanıcı sisteme giriş yapamaz.
* Oturum sırasında rolü pasif hâle getirilen kullanıcı sistemden çıkarılır.

Sayfa adları merkezi `Sayfalar` sınıfında tutulmaktadır. Yetki kontrolleri ise merkezi `IslemYetki` sınıfı üzerinden gerçekleştirilmektedir.

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

Uygulamada Microsoft SQL Server kullanılmaktadır.

Veritabanı işlemleri ağırlıklı olarak stored procedure'ler üzerinden gerçekleştirilmektedir.

Stored procedure kullanılan başlıca işlemler:

* Kullanıcı ekleme, güncelleme, silme ve listeleme
* Kullanıcı giriş ve şifre kontrolleri
* Makine ekleme, güncelleme, silme ve listeleme
* Tekrarlanan makine kayıtlarının kontrol edilmesi
* Rol ekleme, güncelleme, silme ve listeleme
* Rol yetkilerinin eklenmesi ve güncellenmesi
* Sayfa ve işlem yetkilerinin kontrol edilmesi
* Log kayıtlarının oluşturulması
* Tek kayıt ve tablo verilerinin getirilmesi

ADO.NET, uygulama ile SQL Server arasındaki iletişimi sağlamaktadır.

Bağlantı, komut, parametre ve transaction işlemleri merkezi `VeritabaniIslemleri` sınıfı üzerinden yönetilmektedir.

## 🔄 Transaction Yönetimi

Birbirine bağlı birden fazla veritabanı işleminin güvenli şekilde yürütülmesi için transaction desteği kullanılmaktadır.

Transaction kullanılan başlıca işlemler:

* Yeni rol ve role ait sayfa yetkilerinin birlikte oluşturulması
* Rol ve role ait yetki kayıtlarının birlikte pasif hâle getirilmesi
* Bir role ait birden fazla yetki kaydının toplu olarak güncellenmesi

İşlemlerden biri başarısız olduğunda yapılan bütün değişiklikler rollback ile geri alınmaktadır. Tüm işlemler başarılı olduğunda transaction commit edilmektedir.

## 📋 Log Sistemi

Uygulamada gerçekleştirilen önemli işlemler merkezi olarak loglanmaktadır.

Loglanan işlem türleri:

* Kayıt ekleme
* Kayıt güncelleme
* Kayıt silme
* Kayıt görüntüleme
* Sisteme giriş
* Sistemden çıkış

Log kayıtlarında kullanıcı, işlem yapılan tablo, işlem türü, IP adresi, tarih ve işlem detayları tutulmaktadır.

## 🎨 Kullanıcı Arayüzü

Uygulama arayüzü Bootstrap kullanılarak responsive olacak şekilde hazırlanmıştır.

Mevcut arayüz özellikleri:

* Responsive sayfa düzenleri
* Bootstrap kart ve form yapıları
* Responsive giriş ve şifre sıfırlama sayfaları
* Ortak MasterPage yapısı
* Yetkiye göre oluşturulan dinamik menü
* Tekrar kullanılabilir tablo tasarımı
* Arama ve sayfalama kontrolleri
* Sabit tablo başlıkları
* Bootstrap butonları ve form bileşenleri
* Toastr bildirimleri

Projeye özel tasarım ihtiyaçlarında özel CSS dosyaları kullanılmaktadır.

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

🚧 **Proje geliştirme aşamasındadır.**

Web uygulamasının temel yazılım altyapısı büyük ölçüde tamamlanmıştır.

Tamamlanan ana bölümler:

* Kullanıcı yönetimi
* Makine yönetimi
* Rol yönetimi
* Rol bazlı yetkilendirme
* Oturum ve erişim kontrolü
* Log sistemi
* Tekrar kullanılabilir bileşenler
* Merkezi mesaj sistemi
* Transaction altyapısı
* Responsive kullanıcı arayüzü

Sonraki geliştirme aşamasında yapılacak çalışmalar:

* Makine kablolamalarının tamamlanması
* Makinelerin sisteme fiziksel olarak bağlanması
* Modbus haberleşmesinin kurulması
* Makinelerden canlı veri alınması
* Gerçek saha testlerinin yapılması
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
* Loglama
* Tekrar kullanılabilir bileşen geliştirme
* Yazılım mimarisi
* Temiz ve sürdürülebilir kod yazımı
* Endüstriyel sistem ve Modbus entegrasyonu
