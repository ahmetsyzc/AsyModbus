using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

// MakineKomutServisi_Dogrula.ps1 tarafindan yalnizca gecici test DB'sinde calistirilir.
class MakineKomutServisiDogrula
{
    class SahteRole : IRoleKontrol
    {
        public string SonHata { get { return "Kontrollu OFF hatasi"; } }
        public bool Basarili = true;
        public bool HataFirlat;
        public int Cagri;
        public string Ip;
        public int Port, Kanal;
        public ManualResetEvent Giris = new ManualResetEvent(false);
        public ManualResetEvent Devam = new ManualResetEvent(true);
        public bool KanalTetikle(string ip, int port, int kanal)
        {
            Interlocked.Increment(ref Cagri);
            Ip = ip; Port = port; Kanal = kanal;
            Giris.Set();
            if (!Devam.WaitOne(10000)) throw new Exception("Test senkronizasyon suresi asildi.");
            if (HataFirlat) throw new Exception("Kontrollu role exception");
            return Basarili;
        }
    }

    static void Kontrol(bool ok, string mesaj) { if (!ok) throw new Exception(mesaj); }
    static object Deger(string sql)
    {
        using (var db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlBaglanti"].ConnectionString))
        { db.Open(); using (var cmd = new SqlCommand(sql, db)) return cmd.ExecuteScalar(); }
    }
    static void Sql(string sql) { Deger(sql); }
    static string Parametre(string grup, string kod)
    {
        return "(SELECT P.id FROM dbo.Parametreler P JOIN dbo.ParametreGruplari G ON G.id=P.parametre_grup_id WHERE G.kod=N'" + grup + "' AND P.kod=N'" + kod + "')";
    }
    static void Temizle()
    {
        // Bu executable yalnizca benzersiz adli gecici veritabaninda calisabilir.
        Sql("DELETE FROM dbo.MakineKomutlar; DELETE FROM dbo.MakinelerLoglar; DELETE FROM dbo.Loglar; "
            + "UPDATE dbo.Makineler SET aktif_mi=1, rolecihazlar_id=1, role_kanal_no=3, son_durum_tarih='20000101', durum_parametre_id=" + Parametre("MAKINE_DURUMU","CALISIYOR") + "; "
            + "UPDATE dbo.RoleCihazlar SET aktif_mi=1, ip=N'127.0.0.1', port=12345, kanal_sayisi=16; "
            + "UPDATE dbo.Parametreler SET ad=N'1' WHERE kod=N'KOMUT_KONTROL_SURESI_SN';");
    }
    static void Talep()
    {
        Sql("DECLARE @tur INT=" + Parametre("ISLEM_TURU", "DURDURMA") + "; EXEC dbo.SP_MakineKomutlar_EKLE @makineler_id=1,@islem_tur_parametre_id=@tur,@ekleyen_ip=N'TEST',@detay=N'Sahte role testi';");
    }
    static void Sonuc(string kod)
    {
        Kontrol(Convert.ToInt32(Deger("SELECT COUNT(*) FROM dbo.MakineKomutlar WHERE durum_parametre_id=" + Parametre("MAKINE_KOMUT_DURUM", kod) + " AND deneme_sayisi=1")) == 1, "Komut sonucu yanlis.");
        Kontrol(Convert.ToInt32(Deger("SELECT COUNT(*) FROM dbo.MakinelerLoglar WHERE kaynak_parametre_id=" + Parametre("ISLEM_KAYNAGI", "SERVIS") + " AND islem_sonuc_parametre_id=" + Parametre("ISLEM_SONUCU", kod))) == 1, "SERVIS operasyon logu yanlis.");
    }
    static void Test(string ad, Action test)
    {
        Temizle(); test(); Console.WriteLine("GECTI: " + ad);
    }
    static int Main()
    {
        try
        {
            string dbName = Convert.ToString(Deger("SELECT DB_NAME()"));
            Kontrol(dbName.StartsWith("AsyModbus_KomutTest_", StringComparison.Ordinal) && dbName.Length == 52,
                "Test yalnizca gecici veritabaninda calistirilir.");
            Test("Gecersiz sure parametresi komut islemeyi engellemez", () => {
                foreach(string deger in new [] { "", "abc", "0", "-1", "2147484" }) {
                    Temizle(); Talep();
                    Sql("UPDATE dbo.Parametreler SET ad=N'"+deger+"' WHERE kod=N'KOMUT_KONTROL_SURESI_SN';");
                    var role = new SahteRole();
                    using(var servis = new MakineKomutServisi(role,"127.0.0.1")) {
                        Kontrol(servis.KontrolEt() && role.Cagri==1 && !servis.AltyapiHatasiVar,"Eski sure parametresi komutu engelledi.");
                        Sonuc("BASARILI");
                    }
                }
            });
            Test("Sure parametresi ve grubu bulunmadan komut islenir", () => {
                Talep();
                // Yalnizca gecici test DB'sinde kodlari degistirerek yoklugu taklit et.
                Sql("UPDATE dbo.Parametreler SET kod=N'TEST_ESKI_SURE' WHERE kod=N'KOMUT_KONTROL_SURESI_SN'; UPDATE dbo.ParametreGruplari SET kod=N'TEST_ESKI_SERVIS' WHERE kod=N'SERVIS_AYARLARI';");
                try {
                    var role = new SahteRole();
                    using(var servis = new MakineKomutServisi(role,"127.0.0.1")) {
                        Kontrol(servis.KontrolEt() && role.Cagri==1 && !servis.AltyapiHatasiVar,"Sure parametresi zorunlu tutuldu.");
                        Sonuc("BASARILI");
                    }
                } finally {
                    Sql("UPDATE dbo.Parametreler SET kod=N'KOMUT_KONTROL_SURESI_SN' WHERE kod=N'TEST_ESKI_SURE'; UPDATE dbo.ParametreGruplari SET kod=N'SERVIS_AYARLARI' WHERE kod=N'TEST_ESKI_SERVIS';");
                }
            });
            Test("Kart aktif komut listesi bekleyen islenen ve tamamlanan", () => {
                Talep();
                var veritabaniIslemleri = new VeritabaniIslemleri();
                try {
                    veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
                    var makineKomutlar = new MakineKomutlar(veritabaniIslemleri);
                    var aktifKomutlar = makineKomutlar.AktifleriGetir();
                    Kontrol(aktifKomutlar.Rows.Count==1 && aktifKomutlar.Rows[0][MakineKomutlar.C_Sutun_durum_kod].ToString()=="BEKLIYOR", "Bekleyen kart durumu yanlis.");
                    Kontrol(makineKomutlar.SiradakiGetir() && makineKomutlar.IslemeGuncelle(), "Komut alinamadi.");
                    aktifKomutlar = makineKomutlar.AktifleriGetir();
                    Kontrol(aktifKomutlar.Rows.Count==1 && aktifKomutlar.Rows[0][MakineKomutlar.C_Sutun_durum_kod].ToString()=="ISLENIYOR", "Islenen kart durumu yanlis.");
                    makineKomutlar.DurumParametreId=Convert.ToInt32(Deger("SELECT "+Parametre("MAKINE_KOMUT_DURUM","BASARILI")));
                    Kontrol(makineKomutlar.Guncelle(), "Sonuc kaydedilemedi.");
                    Kontrol(makineKomutlar.AktifleriGetir().Rows.Count==0, "Tamamlanan komut butonu engelliyor.");
                } finally { try { veritabaniIslemleri.GeriAl(); } finally { veritabaniIslemleri.Bitir(); } }
            });
            Test("Bos kuyruk ve parametre okuma", () => {
                var role = new SahteRole(); using (var servis = new MakineKomutServisi(role, "127.0.0.1")) {
                    Kontrol(!servis.KontrolEt() && role.Cagri==0 && servis.SonKontrolZamani.HasValue, "Bos kuyruk hatasi.");
                }
            });
            Test("Basari durum tarihi ve operasyon logu guncellenir", () => {
                Talep(); Sql("UPDATE dbo.RoleCihazlar SET ip=N'127.0.0.2',port=23456; UPDATE dbo.Makineler SET role_kanal_no=7;");
                var role = new SahteRole(); using (var servis = new MakineKomutServisi(role, "127.0.0.1")) {
                    servis.LogOlustu += (s,e) => { throw new Exception("Form log hatasi"); };
                    Kontrol(servis.KontrolEt(), "Komut islenmedi."); Sonuc("BASARILI");
                    Kontrol(Convert.ToInt32(Deger("SELECT COUNT(*) FROM dbo.Makineler WHERE son_durum_tarih>'20000101'"))==1,"Son durum tarihi guncellenmedi.");
                    Kontrol(Convert.ToInt32(Deger("SELECT COUNT(*) FROM dbo.MakinelerLoglar WHERE onceki_durum_parametre_id="+Parametre("MAKINE_DURUMU","CALISIYOR")+" AND yeni_durum_parametre_id="+Parametre("MAKINE_DURUMU","DURDURULDU")))==1,"Onceki/yeni durum logu yanlis.");
                    Kontrol(Convert.ToInt32(Deger("SELECT COUNT(*) FROM dbo.MakineKomutlar WHERE detay LIKE N'Durdurma komutu gönderildi.%'"))==1,"Basari mesaji yanlis.");
                    Kontrol(role.Cagri==1 && role.Ip=="127.0.0.2" && role.Port==23456 && role.Kanal==7, "Guncel role bilgisi okunmadi.");
                    Kontrol(!servis.KontrolEt() && role.Cagri==1, "Komut yeniden uygulandi.");
                    Kontrol(Convert.ToInt32(Deger("SELECT durum_parametre_id FROM dbo.Makineler WHERE id=1"))==Convert.ToInt32(Deger("SELECT " + Parametre("MAKINE_DURUMU","DURDURULDU"))), "Makine durumu guncellenmedi.");
                }
            });
            Test("Role sirasinda degisen diger makine alanlari korunur", () => {
                Talep(); var role=new SahteRole(); role.Devam.Reset();
                using(var servis=new MakineKomutServisi(role,"127.0.0.1")) {
                    var islem=Task.Run(() => servis.KontrolEt());
                    try {
                        Kontrol(role.Giris.WaitOne(5000),"Role cagrisi baslamadi.");
                        Sql("UPDATE dbo.Makineler SET model_ad=N'Yeni Model', ip=N'127.0.0.9', role_kanal_no=8 WHERE id=1");
                    } finally { role.Devam.Set(); }
                    Kontrol(islem.Wait(5000) && islem.Result,"Islem tamamlanmadi.");
                    Kontrol(Convert.ToInt32(Deger("SELECT COUNT(*) FROM dbo.Makineler WHERE model_ad=N'Yeni Model' AND ip=N'127.0.0.9' AND role_kanal_no=8"))==1,"Eski alanlar geri yazildi.");
                    Sonuc("BASARILI");
                }
            });
            Test("Merkezi kuyruk loglari ve servis IP bilgisi", () => {
                Talep();
                using (var servis = new MakineKomutServisi(new SahteRole(), "127.0.0.1")) {
                    Kontrol(servis.KontrolEt(), "Komut tamamlanmadi.");
                    Kontrol(Convert.ToInt32(Deger("SELECT COUNT(*) FROM dbo.Loglar WHERE tablo_ad=N'MakineKomutlar' AND islem_tip=N'Update' AND ekleyen_ip=N'127.0.0.1'"))==2,"Iki durum gecisi merkezi loga yazilmadi.");
                }
            });
            Test("Beklerken duran makine tetiklenmez", () => {
                Talep(); Sql("UPDATE dbo.Makineler SET durum_parametre_id="+Parametre("MAKINE_DURUMU","DURDURULDU"));
                var role=new SahteRole();
                using(var servis=new MakineKomutServisi(role,"127.0.0.1")) {
                    Kontrol(servis.KontrolEt() && role.Cagri==0,"Duran makine tetiklendi."); Sonuc("BASARISIZ");
                }
            });
            Test("Sahiplenme genel log hatasinda rollback", () => {
                Talep(); Sql("CREATE TRIGGER dbo.TR_TestGenelLogHatasi ON dbo.Loglar AFTER INSERT AS BEGIN THROW 51998,N'Kontrollu genel log hatasi',1; END;");
                var role=new SahteRole();
                try {
                    using(var servis=new MakineKomutServisi(role,"127.0.0.1")) {
                        Kontrol(!servis.KontrolEt() && servis.AltyapiHatasiVar && role.Cagri==0,"Log hatasina ragmen tetiklendi.");
                        Kontrol(Convert.ToInt32(Deger("SELECT deneme_sayisi FROM dbo.MakineKomutlar"))==0,"Sahiplenme rollback olmadi.");
                    }
                } finally { Sql("DROP TRIGGER dbo.TR_TestGenelLogHatasi"); }
            });
            Test("Sonuc genel log hatasinda rollback ve tekrar yok", () => {
                Talep();
                Sql("CREATE TRIGGER dbo.TR_TestSonucGenelLogHatasi ON dbo.Loglar AFTER INSERT AS BEGIN IF EXISTS (SELECT 1 FROM dbo.MakineKomutlar WHERE durum_parametre_id="+Parametre("MAKINE_KOMUT_DURUM","BASARILI")+") THROW 51997,N'Kontrollu sonuc genel log hatasi',1; END;");
                var role=new SahteRole();
                try {
                    using(var servis=new MakineKomutServisi(role,"127.0.0.1")) {
                        Kontrol(!servis.KontrolEt() && servis.AltyapiHatasiVar && role.Cagri==1,"Sonuc log hatasi durdurmadi.");
                        Kontrol(Convert.ToInt32(Deger("SELECT COUNT(*) FROM dbo.MakineKomutlar WHERE durum_parametre_id="+Parametre("MAKINE_KOMUT_DURUM","ISLENIYOR")))==1,"Sonuc rollback olmadi.");
                        Kontrol(Convert.ToInt32(Deger("SELECT COUNT(*) FROM dbo.Loglar WHERE tablo_ad=N'MakineKomutlar'"))==1,"Yalniz sahiplenme logu kalmali.");
                        Kontrol(!servis.KontrolEt() && role.Cagri==1,"Fiziksel komut tekrarlandi.");
                    }
                } finally { Sql("DROP TRIGGER dbo.TR_TestSonucGenelLogHatasi"); }
            });
            Test("Yeniden baslatmada belirsiz komut tekrar tetiklenmez", () => {
                Talep();
                var veritabaniIslemleri = new VeritabaniIslemleri();
                try {
                    veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
                    var makineKomutlar = new MakineKomutlar(veritabaniIslemleri);
                    Kontrol(makineKomutlar.SiradakiGetir() && makineKomutlar.IslemeGuncelle(),"Sahiplenilemedi.");
                    veritabaniIslemleri.Uygula();
                } finally { veritabaniIslemleri.Bitir(); }
                var role=new SahteRole();
                using(var servis=new MakineKomutServisi(role,"127.0.0.1"))
                    Kontrol(!servis.KontrolEt() && role.Cagri==0,"Belirsiz komut tekrarlandi.");
            });
            Test("Role OFF hatasi sonucu", () => {
                Talep(); var role = new SahteRole { Basarili=false };
                using (var servis = new MakineKomutServisi(role,"127.0.0.1")) {
                    Kontrol(servis.KontrolEt(),"Hata sonucu kaydedilmedi."); Sonuc("BASARISIZ");
                    Kontrol(servis.SonHata.Contains("OFF"),"Teknik hata kayboldu.");
                    Kontrol(Convert.ToInt32(Deger("SELECT COUNT(*) FROM dbo.Makineler WHERE durum_parametre_id="+Parametre("MAKINE_DURUMU","CALISIYOR")+" AND son_durum_tarih='20000101'"))==1,"Basarisiz role makine durumunu degistirdi.");
                    Kontrol(Convert.ToInt32(Deger("SELECT COUNT(*) FROM dbo.MakinelerLoglar WHERE onceki_durum_parametre_id=yeni_durum_parametre_id"))==1,"Basarisiz islem durum logu yanlis.");
                }
            });
            Test("Role exception sonucu", () => {
                Talep(); var role = new SahteRole { HataFirlat=true };
                using (var servis = new MakineKomutServisi(role,"127.0.0.1")) {
                    Kontrol(servis.KontrolEt(),"Exception sonucu kaydedilmedi."); Sonuc("BASARISIZ");
                }
            });
            Test("Atanmamis role", () => {
                Talep(); Sql("UPDATE dbo.Makineler SET rolecihazlar_id=NULL;");
                var role=new SahteRole(); using(var servis=new MakineKomutServisi(role,"127.0.0.1")) {
                    Kontrol(servis.KontrolEt() && role.Cagri==0,"Role cagri yapildi."); Sonuc("BASARISIZ");
                }
            });
            Test("Pasif role", () => {
                Talep(); Sql("UPDATE dbo.RoleCihazlar SET aktif_mi=0;");
                var role=new SahteRole(); using(var servis=new MakineKomutServisi(role,"127.0.0.1")) {
                    Kontrol(servis.KontrolEt() && role.Cagri==0,"Pasif role cagrildi."); Sonuc("BASARISIZ");
                }
            });
            Test("Gecersiz kanal", () => {
                Talep(); Sql("UPDATE dbo.Makineler SET role_kanal_no=17;");
                var role=new SahteRole(); using(var servis=new MakineKomutServisi(role,"127.0.0.1")) {
                    Kontrol(servis.KontrolEt() && role.Cagri==0,"Gecersiz kanal cagrildi."); Sonuc("BASARISIZ");
                }
            });
            Test("Pasif makine", () => {
                Talep(); Sql("UPDATE dbo.Makineler SET aktif_mi=0;");
                var role=new SahteRole(); using(var servis=new MakineKomutServisi(role,"127.0.0.1")) {
                    Kontrol(servis.KontrolEt() && role.Cagri==0,"Pasif makine cagrildi."); Sonuc("BASARISIZ");
                }
            });
            Test("Form cagrisi, cakisan kontrol ve guvenli kapatma", () => {
                Talep(); var role=new SahteRole(); role.Devam.Reset();
                using(var servis=new MakineKomutServisi(role,"127.0.0.1")) {
                    Task<bool> islem = Task.Run(() => servis.KontrolEt());
                    try {
                        Kontrol(role.Giris.WaitOne(5000),"Form cagrisi komutu almadi.");
                        Kontrol(!servis.KontrolEt() && role.Cagri==1,"Cakisan kontrol role cagirdi.");
                        servis.Dispose(); Kontrol(servis.IslemDevamEdiyor,"Devam eden islem kesildi.");
                    } finally { role.Devam.Set(); }
                    Kontrol(islem.Wait(5000) && islem.Result,"Islem tamamlanmadi."); Sonuc("BASARILI");
                    Kontrol(role.Cagri==1,"Fiziksel komut tekrarlandi.");
                }
            });
            Test("Diger zorunlu parametre eksikse komut sahiplenilmez", () => {
                Talep(); Sql("UPDATE dbo.Parametreler SET aktif_mi=0 WHERE id="+Parametre("ISLEM_KAYNAGI","SERVIS"));
                try {
                    var role=new SahteRole(); using(var servis=new MakineKomutServisi(role,"127.0.0.1")) {
                        Kontrol(!servis.KontrolEt() && role.Cagri==0 && servis.AltyapiHatasiVar,"Zorunlu log kaynagi denetimi kayboldu.");
                        Kontrol(Convert.ToInt32(Deger("SELECT deneme_sayisi FROM dbo.MakineKomutlar"))==0,"Komut sahiplenildi.");
                    }
                } finally { Sql("UPDATE dbo.Parametreler SET aktif_mi=1 WHERE id="+Parametre("ISLEM_KAYNAGI","SERVIS")); }
            });
            Test("Log hatasinda sonuc rollback, fiziksel tekrar yok", () => {
                Talep(); Sql("CREATE TRIGGER dbo.TR_TestLogHatasi ON dbo.MakinelerLoglar AFTER INSERT AS BEGIN THROW 51999,N'Kontrollu log hatasi',1; END;");
                var role=new SahteRole(); using(var servis=new MakineKomutServisi(role,"127.0.0.1")) {
                    Kontrol(!servis.KontrolEt() && role.Cagri==1 && servis.AltyapiHatasiVar,"Log hatasi servisi durdurmadi.");
                    Kontrol(Convert.ToInt32(Deger("SELECT COUNT(*) FROM dbo.MakineKomutlar WHERE durum_parametre_id=" + Parametre("MAKINE_KOMUT_DURUM","ISLENIYOR")))==1,"Sonuc geri alinmadi.");
                    Kontrol(Convert.ToInt32(Deger("SELECT COUNT(*) FROM dbo.MakinelerLoglar"))==0,"Log geri alinmadi.");
                    Kontrol(Convert.ToInt32(Deger("SELECT COUNT(*) FROM dbo.Makineler WHERE durum_parametre_id="+Parametre("MAKINE_DURUMU","CALISIYOR")+" AND son_durum_tarih='20000101'"))==1,"Makine durum/tarih rollback olmadi.");
                    Kontrol(Convert.ToInt32(Deger("SELECT COUNT(*) FROM dbo.Loglar WHERE tablo_ad=N'Makineler'"))==0,"Makine genel logu rollback olmadi.");
                    Sql("DROP TRIGGER dbo.TR_TestLogHatasi;");
                    Kontrol(!servis.KontrolEt() && role.Cagri==1,"Belirsiz komut yeniden uygulandi.");
                }
            });
            Temizle(); Console.WriteLine("20 servis ve parametre senaryosu gecti. Gercek roleye baglanilmadi."); return 0;
        }
        catch(Exception ex) { Console.WriteLine(ex.ToString()); return 1; }
    }
}
