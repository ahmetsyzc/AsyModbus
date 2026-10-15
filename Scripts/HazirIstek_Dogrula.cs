using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// Yalniz loopback sahte sunucusu. Gercek cihaz veya uygulama DB'si kullanilmaz.
class HazirIstekDogrula
{
    class Sunucu : IDisposable
    {
        readonly TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
        readonly Func<string, int, string> cevap;
        public readonly List<string> Yollar = new List<string>();
        public readonly List<long> Zamanlar = new List<long>();
        readonly Stopwatch saat = Stopwatch.StartNew();
        public int Port { get { return ((IPEndPoint)listener.LocalEndpoint).Port; } }
        public Sunucu(Func<string,int,string> f)
        {
            cevap = f; listener.Start();
            Task.Run(async () => {
                try {
                    while (true) {
                        var client = await listener.AcceptTcpClientAsync();
                        var islemGorevi = Task.Run(() => Isle(client));
                    }
                } catch (ObjectDisposedException) {} catch (SocketException) {}
            });
        }
        void Isle(TcpClient client)
        {
            using (client) {
                try {
                    var stream = client.GetStream();
                    var reader = new StreamReader(stream, Encoding.ASCII, false, 1024, true);
                    string ilk = reader.ReadLine();
                    if (ilk == null) return;
                    string satir;
                    while (!string.IsNullOrEmpty(satir = reader.ReadLine())) {}
                    string yol = ilk.Split(' ')[1];
                    int sira;
                    lock (Yollar) { Yollar.Add(yol); Zamanlar.Add(saat.ElapsedMilliseconds); sira = Yollar.Count; }
                    string sonuc = cevap(yol,sira);
                    if (sonuc == "TIMEOUT") { Thread.Sleep(4000); return; }
                    if (sonuc == "DROP") return;
                    byte[] veri = Encoding.ASCII.GetBytes("HTTP/1.1 " + sonuc + "\r\nContent-Length: 0\r\nConnection: close\r\n\r\n");
                    stream.Write(veri,0,veri.Length);
                } catch (IOException) {} catch (SocketException) {}
            }
        }
        public MakineKomutlar Komut(int sure)
        {
            return new MakineKomutlar(null) { TetiklemeUrl="http://127.0.0.1:"+Port+"/00",
                KapatmaUrl="http://127.0.0.1:"+Port+"/01", HedefIp="127.0.0.1", HedefPort=Port, TetiklemeSuresiMs=sure };
        }
        public void Dispose() { listener.Stop(); }
    }
    static void Kontrol(bool ok, string mesaj) { if (!ok) throw new Exception(mesaj); }
    static void Test(string ad, Action test) { test(); Console.WriteLine("GECTI: " + ad); }
    static int Main()
    {
        ServicePointManager.DefaultConnectionLimit = 20;
        try {
            Test("ON /00, en az 2000 ms, OFF /01", () => {
                using (var s = new Sunucu((y,n)=>"200 OK")) {
                    var role = new RoleKontrol();
                    Kontrol(role.KanalTetikleAsync(s.Komut(2000)).Result,role.SonHata);
                    Kontrol(string.Join(",",s.Yollar)=="/00,/01","Sira yanlis");
                    Kontrol(s.Zamanlar[1]-s.Zamanlar[0]>=1950,"Sure yanlis");
                }
            });
            Test("OFF toplam 3 deneme, 500 ms aralik; basarisiz", () => {
                using (var s = new Sunucu((y,n)=> y=="/00" ? "200 OK" : "500 Error")) {
                    var role = new RoleKontrol();
                    Kontrol(!role.KanalTetikleAsync(s.Komut(20)).Result,"Basarili sayildi");
                    Kontrol(string.Join(",",s.Yollar)=="/00,/01,/01,/01","Deneme sayisi yanlis");
                    Kontrol(s.Zamanlar[3]-s.Zamanlar[1]>=950,"OFF araliklari korunmadi");
                    Kontrol(role.SonHata.Contains("Kapatma"),"OFF ayrintisi yok");
                }
            });
            Test("OFF ikinci denemede basari", () => {
                using (var s = new Sunucu((y,n)=> n==2 ? "500 Error" : "200 OK")) {
                    Kontrol(new RoleKontrol().KanalTetikleAsync(s.Komut(20)).Result,"Basari yok");
                    Kontrol(s.Yollar.Count==3,"Gereksiz tekrar");
                }
            });
            Test("OFF zaman asiminda toplam 3 deneme", () => {
                using (var s = new Sunucu((y,n)=> n==2 || n==3 ? "TIMEOUT" : "200 OK")) {
                    Kontrol(new RoleKontrol().KanalTetikleAsync(s.Komut(20)).Result,"OFF temizligi basaramadi");
                    Kontrol(string.Join(",",s.Yollar)=="/00,/01,/01,/01","OFF timeout deneme sayisi yanlis");
                }
            });
            Test("ON HTTP hatasi sonrasi OFF, ON tekrari yok", () => {
                using (var s = new Sunucu((y,n)=> y=="/00" ? "500 Error" : "200 OK")) {
                    var role = new RoleKontrol();
                    Kontrol(!role.KanalTetikleAsync(s.Komut(20)).Result,"Basarili sayildi");
                    Kontrol(string.Join(",",s.Yollar)=="/00,/01","ON tekrarlandi veya OFF yok");
                    Kontrol(role.SonHata.Contains("ON"),"ON ayrintisi yok");
                }
            });
            Test("ON zaman asimi sonrasi OFF; otomatik ON tekrari yok", () => {
                using (var s = new Sunucu((y,n)=> y=="/00" ? "TIMEOUT" : "200 OK")) {
                    var role = new RoleKontrol();
                    Kontrol(!role.KanalTetikleAsync(s.Komut(20)).Result,"Timeout basarili sayildi");
                    Kontrol(string.Join(",",s.Yollar)=="/00,/01","Timeout temizligi yanlis");
                }
            });
            Test("ON yaniti kesilirse OFF denenir ve ON tekrar edilmez", () => {
                using (var s = new Sunucu((y,n)=> y=="/00" ? "DROP" : "200 OK")) {
                    Kontrol(!new RoleKontrol().KanalTetikleAsync(s.Komut(20)).Result,"Kesilen yanit basarili sayildi");
                    Kontrol(string.Join(",",s.Yollar)=="/00,/01","ON tekrar edildi veya OFF yok");
                }
            });
            Test("HTTP yonlendirmesi izlenmez", () => {
                using (var s = new Sunucu((y,n)=> y=="/00" ? "302 Found\r\nLocation: http://127.0.0.1:1/forbidden" : "200 OK")) {
                    Kontrol(!new RoleKontrol().KanalTetikleAsync(s.Komut(20)).Result,"Yonlendirme basarili sayildi");
                    Kontrol(s.Yollar.Count==2,"OFF eksik");
                }
            });
            Test("Bozuk URL, hedef, sema ve sure ag istegi yapmaz", () => {
                using (var s = new Sunucu((y,n)=>"200 OK")) {
                    foreach (string adres in new [] { "", "ftp://127.0.0.1/00", "http://127.0.0.2/00", "http://u:p@127.0.0.1/00", "http://127.0.0.1/00?secret=x" }) {
                        var k=s.Komut(20); k.TetiklemeUrl=adres;
                        Kontrol(!new RoleKontrol().KanalTetikleAsync(k).Result,"Gecersiz URL kabul edildi");
                    }
                    var komut=s.Komut(10001);
                    Kontrol(!new RoleKontrol().KanalTetikleAsync(komut).Result,"Gecersiz sure kabul edildi");
                    Kontrol(s.Yollar.Count==0,"Ag istegi yapildi");
                }
            });
            Test("Web hazirlayicisi mevcut kanal adreslerini korur", () => {
                var k=new MakineKomutlar(null);
                RoleKomutHazirlayici.Hazirla(k,new RoleCihazlar(null) {Id=7, Ip="127.0.0.1", Port=12345, KanalSayisi=16},3);
                Kontrol(k.TetiklemeUrl=="http://127.0.0.1:12345/04" && k.KapatmaUrl=="http://127.0.0.1:12345/05" && k.TetiklemeSuresiMs==2000,"Protokol degisti");
            });
            return 0;
        } catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
