using System;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class RoleKontrol : IRoleKontrol
{
    private const int C_KapatmaDenemeSayisi = 3; // Toplam deneme; iki ek tekrar.
    public string SonHata { get; private set; }

    public static void Dogrula(MakineKomutlar komut)
    {
        if (komut == null || komut.TetiklemeSuresiMs < 1 || komut.TetiklemeSuresiMs > 10000)
            throw new InvalidOperationException("Tetikleme süresi 1–10000 ms aralığında olmalıdır.");
        Uri on = AdresDogrula(komut.TetiklemeUrl, komut);
        Uri off = AdresDogrula(komut.KapatmaUrl, komut);
        if (on.Scheme != off.Scheme || on.AbsoluteUri == off.AbsoluteUri)
            throw new InvalidOperationException("ON/OFF adresleri geçersiz.");
    }

    private static Uri AdresDogrula(string adres, MakineKomutlar komut)
    {
        Uri uri;
        IPAddress ip, hedef;
        if (string.IsNullOrWhiteSpace(adres) || adres.Length > MakineKomutlar.C_UrlUzunlugu
            || !Uri.TryCreate(adres, UriKind.Absolute, out uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            || !string.IsNullOrEmpty(uri.UserInfo) || !string.IsNullOrEmpty(uri.Fragment)
            || !string.IsNullOrEmpty(uri.Query)
            || !IPAddress.TryParse(uri.Host.Trim('[', ']'), out ip)
            || !IPAddress.TryParse(komut.HedefIp, out hedef) || !ip.Equals(hedef)
            || uri.Port != komut.HedefPort)
            throw new InvalidOperationException("Hazır röle adresi eksik veya izin verilen hedefle uyumsuz.");
        return uri;
    }

    public async Task<bool> KanalTetikleAsync(MakineKomutlar komut)
    {
        SonHata = string.Empty;
        try { Dogrula(komut); }
        catch (Exception ex) { SonHata = ex.Message; return false; }
        bool acildi = false, kapandi = false;
        string onHata = string.Empty, offHata = string.Empty;
        try
        {
            acildi = await IstekGonder(komut.TetiklemeUrl).ConfigureAwait(false);
            onHata = SonHata;
            // Yaniti kaybolan ON da uygulanmis olabilir; sureyi ve OFF temizligini koru.
            await Task.Delay(komut.TetiklemeSuresiMs).ConfigureAwait(false);
        }
        finally
        {
            for (int deneme = 1; deneme <= C_KapatmaDenemeSayisi; deneme++)
            {
                kapandi = await IstekGonder(komut.KapatmaUrl).ConfigureAwait(false);
                offHata = SonHata;
                if (kapandi) break;
                if (deneme < C_KapatmaDenemeSayisi) await Task.Delay(500).ConfigureAwait(false);
            }
        }
        SonHata = (!acildi ? "ON komutu doğrulanamadı. " + onHata : string.Empty)
            + (!kapandi ? " Kapatma komutu doğrulanamadı. " + offHata : string.Empty);
        return acildi && kapandi;
    }

    private async Task<bool> IstekGonder(string adres)
    {
        // HttpWebRequest/HttpClient altindaki otomatik GET tekrarina izin verme.
        // Mevcut cihaz sozlesmesi: govdesiz HTTP GET, yalniz HTTP 200 dogrulamasi.
        try
        {
            SonHata = string.Empty;
            var uri = new Uri(adres);
            using (var client = new TcpClient())
            using (var timeout = new CancellationTokenSource(3000))
            using (timeout.Token.Register(client.Close))
            {
                await client.ConnectAsync(IPAddress.Parse(uri.Host.Trim('[', ']')), uri.Port).ConfigureAwait(false);
                using (Stream stream = await AkisGetir(client, uri).ConfigureAwait(false))
                {
                    byte[] istek = Encoding.ASCII.GetBytes("GET " + uri.PathAndQuery
                        + " HTTP/1.1\r\nHost: " + uri.Authority + "\r\nConnection: close\r\n\r\n");
                    // Tek WriteAsync: kayip cevap halinde yeniden baglanma veya ON tekrari yok.
                    await stream.WriteAsync(istek, 0, istek.Length, timeout.Token).ConfigureAwait(false);
                    byte[] birBayt = new byte[1];
                    var baslik = new StringBuilder();
                    while (baslik.Length < 16384)
                    {
                        int okunan = await stream.ReadAsync(birBayt, 0, 1, timeout.Token).ConfigureAwait(false);
                        if (okunan == 0) throw new IOException("Eksik HTTP yaniti");
                        baslik.Append((char)birBayt[0]);
                        int uzunluk = baslik.Length;
                        if (uzunluk >= 4 && baslik[uzunluk-4]=='\r' && baslik[uzunluk-3]=='\n'
                            && baslik[uzunluk-2]=='\r' && baslik[uzunluk-1]=='\n')
                        {
                            string ilkSatir = baslik.ToString().Split(new[] { "\r\n" }, StringSplitOptions.None)[0];
                            string[] parcalar = ilkSatir.Split(' ');
                            int kod;
                            if (parcalar.Length < 2 || (parcalar[0]!="HTTP/1.1" && parcalar[0]!="HTTP/1.0")
                                || !int.TryParse(parcalar[1], out kod)) throw new IOException("Gecersiz HTTP yaniti");
                            if (kod == 200) return true;
                            SonHata = "HTTP durum kodu: " + kod;
                            return false; // 3xx dahil: Location izlenmez.
                        }
                    }
                    throw new IOException("HTTP basligi siniri asildi");
                }
            }
        }
        catch (AuthenticationException) { SonHata = "HTTPS sertifikası veya TLS bağlantısı doğrulanamadı."; }
        catch (Exception) { SonHata = "Röle yanıtı doğrulanamadı; ağ kesintisi, eksik yanıt veya 3000 ms zaman aşımı."; }
        return false;
    }

    private static async Task<Stream> AkisGetir(TcpClient client, Uri uri)
    {
        if (uri.Scheme == Uri.UriSchemeHttp) return client.GetStream();
        // Varsayilan sertifika/host dogrulamasi ve iptal yerine socket kapanisi korunur.
        var tls = new SslStream(client.GetStream(), false);
        try
        {
            await tls.AuthenticateAsClientAsync(uri.Host.Trim('[', ']'), null,
                SslProtocols.None, true).ConfigureAwait(false);
            return tls;
        }
        catch { tls.Dispose(); throw; }
    }
}
