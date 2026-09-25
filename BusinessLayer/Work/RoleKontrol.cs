using System;
using System.Net;
using System.Threading;

public class RoleKontrol
{
    // Rölenin ON kalacağı süre.
    private const int C_TetiklemeSuresi = 2000;

    // OFF komutu başarısız olursa kaç kere tekrar denenecek.
    private const int C_KapatmaDenemeSayisi = 3;

    public string SonHata { get; private set; }

    public bool KanalTetikle(string ip, int port, int kanalNo)
    {
        SonHata = string.Empty;
        // Gerekli bağlantı bilgilerinin geçerli olup olmadığı kontrol edilir.
        if (string.IsNullOrEmpty(ip) || port <= 0 || kanalNo <= 0)
        {
            SonHata = "Röle bağlantı bilgileri geçersiz.";
            return false;
        }

        // İlgili röle kanalı ON yapılır.
        bool acildi = KanalAyarla(ip, port, kanalNo, false);

        // ON komutu başarısızsa KanalAyarla içerisindeki teknik hata SonHata'da zaten bulunmaktadır.
        if (!acildi)
        {
            return false;
        }

        // Röle 2 saniye ON konumunda tutulur.
        Thread.Sleep(C_TetiklemeSuresi);

        // OFF denemelerinde oluşan son teknik hatayı kaybetmemek için ayrı değişkende tutuyoruz.
        string sonKapatmaHatasi = string.Empty;


        // Röleyi tekrar OFF yapmak için 3 kez deneme yapılır.
        for (int deneme = 1; deneme <= C_KapatmaDenemeSayisi; deneme++)
        {
            bool kapandi = KanalAyarla(ip, port, kanalNo, true);

            // OFF işlemi başarılıysa tüm tetikleme başarılıdır.
            if (kapandi)
            {
                return true;
            }
            // KanalAyarla başarısız olduğunda oluşturduğu teknik hata burada saklanır.
            sonKapatmaHatasi = SonHata;

            // Son deneme başarılı değilse 500 ms bekleyip tekrar deniyoruz.
            if (deneme < C_KapatmaDenemeSayisi)
            {
                Thread.Sleep(500);
            }
        }


        // Üç OFF denemesi de başarısız oldu. Hem bizim anlaşılır mesajımız hem de son teknik hata korunur.
        SonHata = "Röle kapatma komutu gönderilemedi.";
        if (!string.IsNullOrEmpty(sonKapatmaHatasi))
        {
            SonHata += " Teknik hata: " + sonKapatmaHatasi;
        }
        return false;
    }


    private bool KanalAyarla(string ip, int port, int kanalNo, bool ac)
    {
        try
        {
            SonHata = string.Empty;
            int komutNo = ((kanalNo - 1) * 2) + (ac ? 1 : 0);
            string komut = komutNo.ToString("D2");
            // Cihaza gönderilecek adres oluşturulur.
            string adres = "http://" + ip + ":" + port + "/" + komut;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(adres);
            request.Method = "GET";
            // Cihazdan en fazla 3 saniye cevap beklenir.
            request.Timeout = 3000;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                // Cihaz 200 OK döndürdüyse işlem başarılıdır.
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                // Cihaz cevap verdi fakat cevap başarılı değilse hata bilgisi kaydedilir.
                SonHata = "Röle cihazı başarısız cevap verdi. HTTP Durum Kodu: " + (int)response.StatusCode;
                return false;
            }
        }
        catch (Exception ex)
        {
            // Cihaza ulaşılamaması, timeout veya ağ hatası gibi
            // durumlarda teknik hata bilgisi kaydedilir.
            SonHata = ex.Message;
            return false;
        }
    }
}