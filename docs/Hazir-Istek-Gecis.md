# Hazır röle isteği geçişi

15.10.2026 tarihinde sonraki kullanıcı onayıyla ASY_MODBUS veritabanına geçiş uygulandı. Tam yedek alınıp doğrulandı; 27 komut kaydı korundu. Ayrıntılar: [DB geçiş sonucu](Hazir-Istek-DB-Gecis-Sonucu.md).

## Mimari ve çalışma

1. Web oturumu, güncel kullanıcı/rol/yetki, aktif makine, çalışma durumu, aktif röle ve kanal doğrulanır.
2. `RoleKomutHazirlayici`, mevcut protokol hesabıyla tam ON/OFF adreslerini üretir. Kanal 1: `/00` → `/01`; kanal 3: `/04` → `/05`.
3. `MakineKomutlar.Ekle`, URL'leri NVARCHAR(2048) parametrelerle, 2000 ms süre, hedef röle kimliği/IP/port ve mevcut ekleyen_id bilgisiyle mevcut kuyruğa yazar. Uzun metin kesilmez; entity hata verir.
4. SQL, makine satırını kilitleyerek aynı makineye eşzamanlı ikinci aktif talebi engeller.
5. Formun mevcut 5000 ms zamanlayıcısı her kontrolde bir kayıt alır. Yoğun kuyrukta gecikme artar. Belgede belirtilen yaklaşık 3 dakika mevcut kodda bulunmadığından eklenmemiştir.
6. `SP_MakineKomutlar_SAHIPLEN`, tek UPDATE/OUTPUT ile en eski kaydı ISLENIYOR yapar ve döndürür. Merkezi log aynı transaction içindedir. Birden fazla tüketici aynı komutu alamaz.
7. Form yalnız komutta kayıtlı hedefi doğrular. Makinenin yeni röle/kanal atamasını kullanmaz. Kayıtlı röle silinmiş, pasif veya IP/portu değişmişse komut hata alır; başka hedefe yönlenmez. Devam eden ON/OFF sırasında hedef tekrar hesaplanmaz.
8. GET isteğinde 3000 ms toplam bağlantı/yanıt zaman aşımı, yönlendirme engeli, proxy kullanılmaması ve HTTP 200 doğrulaması vardır. `HttpWebRequest` yanıtsız kapanan bağlantıda GET isteğini kendiliğinden tekrarlayabildiği için RoleKontrol içindeki gönderim TcpClient/NetworkStream (HTTPS için SslStream) ile tek yazım yapar. Aynı HTTP GET adresi ve ON/OFF anlamı korunur; HTTP başlıkları 16 KB ile sınırlıdır. Mevcut protokol gövde, özel başlık veya kimlik bilgisi taşımadığı için sözleşmeye bunlar eklenmemiştir. Kullanıcı bilgisi, query ve fragment içeren URL'ler reddedilir.
9. ON denenir; yanıt başarısız/belirsiz olsa da süre beklenip finally içinde OFF çalışır. ON otomatik tekrarlanmaz. OFF toplam 3 denemedir (ilk deneme + 2 tekrar); aralar 500 ms'dir.
10. Sonuç ve operasyon logu aynı transaction içinde kaydedilir. Seçili geçici SQL hatalarında yalnız sonuç transaction'ı en fazla 3 kez denenir. Commit cevabı kaybolduysa aynı sonuç tekrar yazılmaz. Ağ komutları sonuç yazma tekrarının dışındadır.
11. Form kapanışı yeni timer işlemini durdurur, başlamış görevi ve OFF temizliğini bekler. Log yeni kayıt üstte, en fazla 10 kayıttır; komut numarası, makine kimliği, talep sahibinin ad-soyadı ve sonucu içerir. Hazır URL'ler genel otomatik loglara yazılmaz.
12. Web 5 saniyede bir yetkili oturumla yalnız durum sorgular; cihaz URL'leri tarayıcıya verilmez. Kartın mevcut durum etiketi güncellenir; kartta ek sonuç satırı veya boşluk oluşturulmaz. Sonuç ayrıntısı detay ekranındadır. Mevcut DURDURULDU alanı uygulama durumudur; sensör doğrulaması değildir.

## SQL ve dağıtım sırası

Aşağıdaki sıra ilk hazırlık aşamasında canlıya uygulanmadan belgelendi; sonradan kullanıcı onayıyla uygulandı. Önce yedek alın ve tüm eski Form örnekleriyle web komut oluşturma akışını durdurun. Çalışan tetiklemenin OFF temizliği tamamlanmadan Formu zorla kapatmayın.

1. Mevcut kurulumda temel kuyruk yoksa `Scripts/MakineKomutlar_Migrasyon.sql`.
2. `Scripts/MakineKomutlar_HazirIstek_Migrasyon.sql`.
3. SQLCMD modunda `Scripts/MakineKomutlar_StoredProcedure.sql` (`:ON ERROR EXIT` içerir).
4. Daha önceki 9 alanlı sürüm kurulduysa `Scripts/MakineKomutlar_GereksizAlanlariKaldir.sql` uygulanır.
4. Aynı derlemeden güncel BusinessLayer ile web ve Form dağıtılır; eski DLL/Form örnekleriyle karışık çalıştırılmaz.
5. Önce web ve Formu kontrollü masa ortamında doğrulayın, ardından normal kullanıma açın.

Migration tekrar çalıştırılabilir ve geçmiş kayıtları silmez. URL'siz eski BEKLIYOR kayıtlar açıklamalı BASARISIZ olur. Eski ISLENIYOR kayıtlar tekrar kuyruğa alınmaz; açıklamalı belirsiz sonuç olarak bırakılır. Eski `ISLEME_GUNCELLE` SP'si 51110 hatasıyla durur: eski tüketici yeni komutu IP/kanal hesabıyla çalıştıramaz. Yeni Form `SAHIPLEN` kullanır. Kullanılmayan BekleyenGetir, SiradakiGetir ve IslemeGuncelle entity metotları ile SP sabitleri kaldırıldı. SQL okuma prosedürleri mevcut doğrulama scriptleri için, eski ISLEME_GUNCELLE prosedürü eski tüketiciyi engellemek için korunur.

İşlem başlangıcından iki dakika sonra hâlâ ISLENIYOR olan kayıt webde “sonuç belirsiz” gösterilir; bu görünüm kaydı tekrar kuyruğa almaz. İlgili makinede yeni talep engeli sürer. Önce gerçek cihazın güvenli OFF durumu ve servis/DB kayıtları incelenmelidir. Elle ISLENIYOR → BEKLIYOR çevirerek ON tekrarını başlatmayın. Kontrollü müdahale ve doğrulama sonrasında mevcut sonuç güncelleme yöntemiyle açıklamalı sonuçlandırın.

## Yapılandırma

Mevcut `SqlBaglanti` bağlantısı ve katmanlar korunur; yeni web endpoint adresi veya Form URL şablonu gerekmez. Webin DB erişimi, Formun DB ve cihaz ağı erişimi olmalıdır. Cihaz HTTP kullanır; HTTPS'i yalnız cihaz desteği doğrulanarak ayrıca yapılandırın. Genel sertifika doğrulama kapatma yoktur.

## Doğrulama

- Derleme: Visual Studio MSBuild ile `AsyModbus.sln /t:Build /p:Configuration=Debug`.
- `Scripts/HazirIstek_Dogrula.ps1`: yalnız loopback sahte HTTP sunucusu; cihaz ağına ve DB'ye erişmez. ON/OFF sırası, 2000 ms, toplam 3 OFF denemesi, ON hatası/zaman aşımı sonrası OFF, redirect engeli, geçersiz hedef/süre ve web adres hazırlama kontrol edilir.
- `Scripts/MakineKomutServisi_Dogrula.ps1`: benzersiz geçici DB oluşturur; mevcut DB'den yalnız şema, SP tanımları ve sistem parametrelerini okur. Makine/röle/log verilerini kopyalamaz; sahte röle kullanır ve geçici DB'yi finally içinde kaldırır. Windows SQL erişimi ve CREATE/DROP DATABASE izni gerekir. Gerçek röle isteği göndermez.
- `Scripts/MakineKomutlar_Dogrula.ps1`: mevcut boş test kuyruğunda transaction/rollback ile düşük seviyeli SP kontrolleri; yalnız test veritabanında çalıştırın. IDENTITY boşluğu bırakabilir.
- Gerçek web/Form ekranı ve üretim donanımıyla doğrulama ayrıca yapılmalıdır; sahte cihaz testi bunların yapılmış olduğu anlamına gelmez.

## Masa testi

Üretimden ayrılmış röle ve test DB ile: webden bir durdurma talebi oluşturun; DB'de tam ON/OFF adresleri ve 2000 ms değerini kontrol edin. Formu başlatın, tek ON ve yaklaşık 2 saniye sonra OFF olduğunu doğrulayın. Kartın yenileme yapmadan sonucu göstermesini, Form logunda kullanıcı ad-soyadını kontrol edin. Ağ kesintisi ve Form kapanışı kontrollerini yalnız güvenli masa düzeneğinde yapın. Uygulama başarısını fiziksel makine duruşu ölçümüyle karıştırmayın.

## Dosyalar ve amaçları

- `BusinessLayer/Entity/MakineKomutlar.cs`: hazır komut alanları, sınırlı SQL parametreleri, atomik sahiplenme ve durum sorgusu.
- `BusinessLayer/Work/RoleKomutHazirlayici.cs`: webde mevcut protokole uygun URL üretimi.
- `BusinessLayer/InterFace/IRoleKontrol.cs`, `BusinessLayer/Work/RoleKontrol.cs`: hazır komut sözleşmesi, asenkron ağ, doğrulama ve OFF temizliği.
- `BusinessLayer/Work/MakineKomutServisi.cs`: snapshot hedef denetimi, sonuç kaydı tekrarları ve kullanıcı bilgili loglar.
- `BusinessLayer/Work/VeritabaniIslemleri.cs`: uzunluğu belirlenmiş metin parametresi ve URL alanlarını logdan çıkarma.
- `AsyModbus/Pages/Default.aspx.cs`, `Default.aspx`, `AsyModbus/Scripts/Default.js`: komut hazırlama ve kart sonuç sorgusu.
- `AsyModbus.KomutForm/FrmKomutServisi.cs`: tüm log girişlerinde 10 kayıt sınırı.
- Proje dosyaları: yeni sınıf derlemeye eklenir; webin eski DLL referansı mevcut BusinessLayer proje referansına çevrilir.
- `Scripts/MakineKomutlar_HazirIstek_Migrasyon.sql`, `MakineKomutlar_StoredProcedure.sql`: şema, geçiş ve kuyruk sözleşmesi.
- Doğrulama scriptleri ve README: testler, geçiş ve kullanım bilgisi.

Commit önerisi: `Röle adreslerini webde hazırlayıp Formda güvenli çalıştır`

## Bu çalışmanın doğrulama sonucu

- Web, BusinessLayer ve Form Debug derlemesi başarılı. Önceden var olan `Sessionlar._currentInfo` kullanılmayan alan uyarısı sürüyor.
- Loopback sahte HTTP sunucusunda 10 senaryo geçti. Yanıtın tamamen kesilmesi ve OFF zaman aşımı da denendi; otomatik ON tekrarı yok.
- Geçici SQL veritabanında 26 servis/kuyruk senaryosu ve eski kayıt geçişi/migration tekrar çalıştırma kontrolü geçti. Test DB kaldırıldı. İlk doğrulamada uygulama DB'sine migration uygulanmadı; sonraki kullanıcı onayıyla yedek alınıp geçiş tamamlandı.
- `node Scripts/WebKomutDurumu_Dogrula.js`: kart durum geçişleri ve sorgu hatası denetimi geçti.
- JavaScript sözdizimi ve `git diff --check` kontrolü başarılı.
- Gerçek tarayıcı/Form ekranıyla uçtan uca masa testi, gerçek cihaz ve HTTPS cihaz bağlantısı denenmedi. SQL testleri Windows kimliğiyle çalışma izni gerektirdi; ilk kısıtlı ortam denemesinde SSPI hatası vardı, uygun erişimle geçici DB testleri tamamlandı.
## Audit alanlarının sadeleştirilmesi

Kullanıcı isteğiyle talep_eden_ad_soyad, baslama_tarih ve bitis_tarih kolonları ve entity karşılıkları kaldırıldı. Talep sahibi mevcut ekleyen_id üzerinden Kullanicilar kaydından okunur; kullanıcının güncel adı kullanılır. Oluşturma zamanı eklenme_tarih, sahiplenme/sonuç güncelleme zamanı guncellenme_tarih ile tutulur. Ayrı başlangıç/bitiş zamanları saklanmaz; yarım kalan işlem kontrolü ISLENIYOR durumundaki guncellenme_tarih üzerinden yapılır.

Sadeleştirme ASY_MODBUS veritabanına tam yedek sonrasında uygulandı; 28 komut kaydı ve bekleyen hazır isteğin URL'leri korundu. Geçici test DB'sinde 26 servis senaryosu, kullanıcı adının ekleyen_id üzerinden loglanması, kolon kaldırma ve tekrar çalıştırma kontrolleri geçti.
