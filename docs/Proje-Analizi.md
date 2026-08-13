# MODBUS PROJESİ

## Yazılım Gereksinim ve Proje Analiz Dokümanı

## 1. Giriş ve Proje Amacı

Bu doküman, firmada aktif olarak faaliyet gösteren 20 adet tekstil makinesinin uzaktan izlenmesi ve merkezi bir web arayüzü üzerinden durdurulabilmesi amacıyla geliştirilecek olan Modbus Projesi'nin fonksiyonel gereksinimlerini, rol yetkilendirmelerini ve ekran detaylarını kapsamaktadır.

**Temel Amaç:** Sahaya fiziksel olarak gitmeye gerek kalmadan endüstriyel haberleşme protokolleri (Modbus) vasıtasıyla makinelerin anlık durum yönetimini dijitalleştirmek ve operasyonel duruş sürelerine anında müdahale edebilmektir.

Mevcut panoların donanım yetersizlikleri ve saha güvenlik prosedürleri sebebiyle makinelerin uzaktan tekrar başlatılması (start verilmesi) teknik olarak mümkün olmayıp, sistem üzerinden yalnızca güvenli durdurma aksiyonu gerçekleştirilecektir.

---

## 2. Kullanıcı Rolleri ve Yetkilendirme

Sistem güvenliği ve operasyonel hiyerarşi açısından uygulama üzerinde iki temel rol grubu tanımlanmıştır:

### Süper Admin (Yönetici)

Sistemdeki tüm personellerin ve makinelerin yönetiminden (ekleme, düzenleme, listeleme) ve acil durdurma aksiyonlarından tam yetkili kullanıcı grubudur.

### Saha Operatörü (Personel)

Makinelerin anlık durumunu izleme, durdurma aksiyonu alma ve kendi profil bilgilerini güncelleme yetkisine sahip teknik saha personelidir.

### Yetkilendirme Tablosu

| Ekran | Süper Admin | Saha Operatörü | Açıklama |
|---|---|---|---|
| Login (Giriş) | Açık | Açık | Kimlik doğrulama ve güvenli şifre sıfırlama süreçleri |
| Anasayfa (Dashboard) | Tam Yetki | Görüntüleme & Durdurma | Makinelerin anlık durumunu görüntüleme ve müdahalede bulunma |
| Profil Sayfası | Düzenleyebilir | Düzenleyebilir | Kullanıcının kendi kişisel bilgilerinin yönetimi |
| Personel Listeleme | Tam Yetki | Erişim Yok | Sistemdeki tüm personellerin listesi ve düzenleme girişi |
| Personel Ekleme | Tam Yetki | Erişim Yok | Yeni personel kaydı veya var olan bilgilerin güncellenmesi |
| Makine Listeleme | Tam Yetki | Erişim Yok | Makinelerin teknik parametrelerinin listelendiği tablo |
| Makine Ekleme | Tam Yetki | Erişim Yok | Modbus bağlantı, IP ve port bilgilerinin yönetimi |
| Makine Sıralama | Tam Yetki | Erişim Yok | Sürükle-bırak ile makinelerin matris dizilimini yönetme |

---

## 3. Fonksiyonel Gereksinimler ve Ekran Detayları

### 3.1. Login (Kullanıcı Girişi ve Şifre Yenileme)

- Kullanıcı adı ve şifre ile güvenli giriş sağlanacaktır.
- Giriş ekranı her iki rol grubu için de tasarımsal olarak ortaktır.
- **Şifremi Unuttum** bağlantısına tıklandığında kullanıcıdan kayıtlı e-posta adresi istenecektir.
- Sistemde eşleşen bir hesap bulunması durumunda şifre sıfırlama bağlantısı e-posta olarak otomatik gönderilecektir.

---

### 3.2. Anasayfa (Makine Kontrol Paneli)

#### Matris Düzeni

Sahadaki 20 adet makine, ekranın ana gövdesinde karşılıklı 10'arlı iki sıra (10-10 matrisi) halinde toplam 20 bağımsız kart olarak sıralanacaktır.

Bu dizilim varsayılan olarak Makine No'suna göre veya Makine Sıralama ekranından belirlenen endekse göre şekillenecektir.

#### Çalışan Makineler (Yeşil)

- Çalışan makinelerin durum ışığı yeşil yanacaktır.
- Kart üzerinde **Makinayı Durdur** butonu aktif ve tıklanabilir olacaktır.
- Butona basıldığında arka planda ilgili makinenin IP adresi ve Modbus Özel Kodu üzerinden durdurma komutu tetiklenecektir.

#### Kapalı Makineler (Gri)

- Gücü veya şalteri kapalı olan makinelerin durum ışığı gri renkte görüntülenecektir.
- Bu makinelerin durdurma butonu pasif (`disabled`) olacaktır.

#### Duran Makineler (Kırmızı)

- Sistem veya operatör tarafından durdurulmuş makinelerin durum ışığı kırmızı renkte yanacaktır.
- Makine kartının üzerinde makineyi kimin durdurduğu (**Sistem / Personel**) doğrudan gösterilecektir.
- Durdurma butonu pasif (`disabled`) tutularak mükerrer istek gönderilmesi engellenecektir.

---

### 3.3. Profil Sayfası

Kullanıcılar aşağıdaki bilgilerini bu ekran üzerinden güncelleyebilecektir:

- Ad
- Soyad
- E-posta
- Telefon
- Kurumsal profil resmi

Profil sayfası her iki rol grubu için de ortak olacaktır.

---

### 3.4. Personel Listeleme Sayfası

- Sadece **Süper Admin** rolüne açıktır.
- Sistemdeki tüm personellerin listelendiği gelişmiş veri tablosudur.

#### Düzenleme Yönlendirmesi

Tablonun sağ kısmında bulunan **Düzenle** butonuna tıklandığında sistem, ilgili personelin verilerini **Personel Ekleme** sayfasına otomatik olarak dolu bir şekilde aktaracaktır.

Personel bilgileri bu sayfa üzerinden güncellenebilecektir.

---

### 3.5. Personel Ekleme Sayfası

Sadece **Süper Admin** rolüne açıktır.

Form aşağıdaki alanları içerir:

- Ad
- Soyad
- TC Kimlik No
- Doğum Tarihi
- E-posta
- Telefon
- Adres
- Rol Seçimi (Süper Admin / Saha Operatörü)

#### Otomatik Hesap Oluşturma

Yeni personel kaydı sırasında **Kaydet** butonuna basıldığında sistem:

- Personel için benzersiz ve nümerik bir kullanıcı kimliği oluşturacaktır.
- Sistem giriş şifresini otomatik olarak oluşturacaktır.
- Oluşturulan kullanıcı kimliği değiştirilemez olacaktır.
- Oluşturulan giriş bilgileri işlem tamamlandıktan sonra Süper Admin ekranında görüntülenecektir.
- Bu bilgilere Personel Listeleme sayfasından da erişilebilecektir.
- **Mail Gönder** butonu aracılığıyla giriş bilgileri personelin e-posta adresine iletilecektir.

#### Düzenleme Modu

Personel Listeleme sayfasından düzenleme yönlendirmesi gelmişse form alanları mevcut personel bilgileriyle dolu olarak açılacaktır.

Kaydetme işlemi gerçekleştirildiğinde yeni kayıt oluşturmak yerine mevcut personel kaydı güncellenecektir.

---

### 3.6. Makine Listeleme Sayfası

- Sadece **Süper Admin** rolüne açıktır.
- Tüm makinelerin teknik parametrelerinin listelendiği tablodur.

Listelenen bilgiler arasında:

- Model
- IP
- Entegrasyon Kodu
- Diğer teknik parametreler

yer almaktadır.

#### Düzenleme Yönlendirmesi

Tablonun sağ kısmında bulunan **Düzenle** butonuna tıklandığında sistem, ilgili makinenin verilerini **Makine Ekleme** sayfasına otomatik olarak aktaracaktır.

Makine bilgileri bu sayfa üzerinden güncellenebilecektir.

---

### 3.7. Makine Ekleme Sayfası

Sadece **Süper Admin** rolüne açıktır.

Makineye ait aşağıdaki bilgiler girilecektir:

- Model Adı
- Entegrasyon Kodu
- Makine GG Bilgisi
- Makine No
- IP Numarası
- Makine Özel Kodu
- MFG

#### Düzenleme Modu

Makine Listeleme sayfasından yönlendirme gelmişse form alanları seçilen makinenin mevcut teknik bilgileriyle dolu olarak açılacaktır.

Kaydetme işlemi gerçekleştirildiğinde yeni kayıt oluşturmak yerine doğrudan mevcut makine kaydı güncellenecektir.

---

### 3.8. Makine Sıralama Ekranı

#### Amaç

Makinelerin anasayfada yalnızca Makine Numarasına göre değil, Süper Admin tarafından fiziksel saha düzenine uygun şekilde manuel olarak sıralanabilmesini sağlayan yönetim ekranıdır.

#### Sürükle-Bırak (Drag and Drop) Mimarisi

- Ekranın ortasında anasayfadaki gibi 10-10 düzeninde makine kartlarının küçük önizlemeleri yer alacaktır.
- Süper Admin herhangi bir makine kartını fare ile sürükleyerek istediği sıraya bırakabilecektir.
- Kartlar bırakıldıkları konuma göre anında yer değiştirecektir.

#### Hızlı Sıfırlama

Sıralama ekranının üst köşesinde:

**Varsayılana Dön (Makine No'ya Göre Sırala)**

butonu bulunacaktır.

Admin bu butona bastığında sistem tüm sıralama endekslerini otomatik olarak orijinal makine numarası sırasına döndürecektir.
