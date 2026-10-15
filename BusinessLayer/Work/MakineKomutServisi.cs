using System;
using System.Net;
using System.Threading;

// Formdan bagimsiz calisir. Form eventlerini UI thread'ine BeginInvoke ile tasir.
public class MakineKomutServisi : IDisposable
{
    private readonly object kilit = new object();
    private readonly IRoleKontrol roleKontrol;
    private readonly string servisIp;
    private bool altyapiHatasiVar;
    private bool kapatildi;
    private int islemDevamEdiyor;
    private DateTime? sonKontrolZamani;
    private string sonHata = string.Empty;

    public MakineKomutServisi() : this(new RoleKontrol(), ServisIpGetir()) { }

    // Testlerde sahte role kullanilarak gercek donanima baglanilmaz.
    public MakineKomutServisi(IRoleKontrol _roleKontrol, string _servisIp)
    {
        if (_roleKontrol == null) throw new ArgumentNullException("_roleKontrol");
        IPAddress servisIpAdresi;
        if (!IPAddress.TryParse(_servisIp, out servisIpAdresi)
            || servisIpAdresi.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            throw new ArgumentException(Mesajlar.ServisKimligiGecersiz, "_servisIp");
        roleKontrol = _roleKontrol;
        servisIp = _servisIp;
    }

    public bool AltyapiHatasiVar { get { lock (kilit) return altyapiHatasiVar; } }
    public bool IslemDevamEdiyor { get { return Interlocked.CompareExchange(ref islemDevamEdiyor, 0, 0) != 0; } }
    public DateTime? SonKontrolZamani { get { lock (kilit) return sonKontrolZamani; } }
    public string SonHata { get { lock (kilit) return sonHata; } }

    public event EventHandler<MakineKomutServisiLogEventArgs> LogOlustu;

    // Formun timer'i her kontrolde bu metodu arka planda cagirir.
    // Sinif kendi timer'ini veya surekli dongusunu baslatmaz.
    public bool KontrolEt()
    {
        lock (kilit)
        {
            if (kapatildi) throw new ObjectDisposedException("MakineKomutServisi");
            if (Interlocked.CompareExchange(ref islemDevamEdiyor, 1, 0) != 0) return false;
        }
        try
        {
            lock (kilit)
            {
                sonKontrolZamani = DateTime.Now;
                sonHata = string.Empty;
                altyapiHatasiVar = false;
            }
            MakineKomutlar makineKomutlar;
            lock (kilit)
            {
                if (kapatildi) return false;
                makineKomutlar = KomutGetir();
            }
            if (makineKomutlar == null) return false;

            KomutuUygula(makineKomutlar);
            return true;
        }
        catch (Exception exception)
        {
            lock (kilit)
            {
                sonHata = exception.Message;
                altyapiHatasiVar = true;
            }
            DurumBildir(Mesajlar.KomutServisiHatasi(exception.Message), true);
            return false;
        }
        finally { Interlocked.Exchange(ref islemDevamEdiyor, 0); }
    }

    private static string ServisIpGetir()
    {
        foreach (IPAddress ipAdresi in Dns.GetHostAddresses(Dns.GetHostName()))
        {
            if (ipAdresi.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                && !IPAddress.IsLoopback(ipAdresi)) return ipAdresi.ToString();
        }
        return IPAddress.Loopback.ToString();
    }

    private int ParametreIdGetir(Parametreler parametreler, string grupKod, string kod)
    {
        int? parametreId = parametreler.IdGetir(grupKod, kod);
        if (!parametreId.HasValue)
        {
            throw new InvalidOperationException(Mesajlar.KomutParametresiEksik(grupKod, kod));
        }
        return parametreId.Value;
    }

    private MakineKomutlar KomutGetir()
    {
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
        try
        {
            // Okuma kilidi, guncelleme ve merkezi log ayni transaction icindedir.
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
            Parametreler parametreler = new Parametreler(veritabaniIslemleri);
            ParametreIdGetir(parametreler, SistemParametreGruplari.IslemTuru, SistemParametreKodlari.Durdurma);
            ParametreIdGetir(parametreler, SistemParametreGruplari.MakineDurumu, SistemParametreKodlari.Calisiyor);
            ParametreIdGetir(parametreler, SistemParametreGruplari.IslemKaynagi, SistemParametreKodlari.Servis);
            ParametreIdGetir(parametreler, SistemParametreGruplari.MakineKomutDurumu, SistemParametreKodlari.Basarili);
            ParametreIdGetir(parametreler, SistemParametreGruplari.MakineKomutDurumu, SistemParametreKodlari.Basarisiz);
            ParametreIdGetir(parametreler, SistemParametreGruplari.IslemSonucu, SistemParametreKodlari.Basarili);
            ParametreIdGetir(parametreler, SistemParametreGruplari.IslemSonucu, SistemParametreKodlari.Basarisiz);

            MakineKomutlar makineKomutlar = new MakineKomutlar(veritabaniIslemleri);
            makineKomutlar.GuncelleyenIp = servisIp;
            if (!makineKomutlar.SahiplenGuncelle()) return null;
            veritabaniIslemleri.Uygula();
            return makineKomutlar;
        }
        finally
        {
            try { veritabaniIslemleri.GeriAl(); }
            finally { veritabaniIslemleri.Bitir(); }
        }
    }

    private void KomutuUygula(MakineKomutlar makineKomutlar)
    {
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
        Makineler makineler = new Makineler(veritabaniIslemleri);
        RoleCihazlar roleCihazlar = new RoleCihazlar(veritabaniIslemleri);
        Parametreler parametreler = new Parametreler(veritabaniIslemleri);
        string talepEden = "Kullanıcı ID: " + makineKomutlar.EkleyenId;
        bool basarili = false;
        string detay;
        int? oncekiDurumParametreId = null;
        try
        {
            try
            {
                try
                {
                    veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                    int durdurmaParametreId = ParametreIdGetir(parametreler,
                        SistemParametreGruplari.IslemTuru, SistemParametreKodlari.Durdurma);
                    if (makineKomutlar.IslemTurParametreId != durdurmaParametreId)
                        throw new InvalidOperationException(Mesajlar.KomutIslemTuruGecersiz);
                    var kullanici = new Kullanicilar(veritabaniIslemleri) { Id = makineKomutlar.EkleyenId };
                    if (kullanici.Doldur()) talepEden = (kullanici.Ad + " " + kullanici.Soyad).Trim();
                    DurumBildir(Mesajlar.KomutIslemeAlindi(makineKomutlar.Id, makineKomutlar.MakinelerId)
                        + ". Talep eden: " + talepEden, false);
                    makineler.Id = makineKomutlar.MakinelerId;
                    if (!makineler.Doldur() || !makineler.AktifMi)
                        throw new InvalidOperationException(Mesajlar.MakineBulunamadi);
                    oncekiDurumParametreId = makineler.DurumParametreId;
                    int calisiyorParametreId = ParametreIdGetir(parametreler,
                        SistemParametreGruplari.MakineDurumu, SistemParametreKodlari.Calisiyor);
                    if (makineler.DurumParametreId != calisiyorParametreId)
                        throw new InvalidOperationException(Mesajlar.MakineCalismiyor);
                    RoleKontrol.Dogrula(makineKomutlar);
                    // Hedef kayit dogrulanir; makine-role eslestirmesi yeniden hesaplanmaz.
                    roleCihazlar.Id = makineKomutlar.HedefRoleCihazId;
                    if (!roleCihazlar.Doldur()
                        || roleCihazlar.Ip != makineKomutlar.HedefIp
                        || roleCihazlar.Port != makineKomutlar.HedefPort)
                        throw new InvalidOperationException("Komutun kayıtlı röle hedefi değişmiş veya pasif; komut başka cihaza yönlendirilmeyecek.");
                }
                finally { veritabaniIslemleri.Bitir(); }

                // Fiziksel islem boyunca DB baglantisi ve transaction acik tutulmaz.
                basarili = roleKontrol.KanalTetikleAsync(makineKomutlar).GetAwaiter().GetResult();
                detay = basarili ? Mesajlar.RoleKomutuUygulandi : roleKontrol.SonHata;
            }
            catch (Exception exception)
            {
                detay = Mesajlar.SistemselHata(exception.Message);
            }

            // Yalnizca sonuc yazimi tekrarlanir; ON/OFF bu dongunun disindadir.
            for (int deneme = 1; ; deneme++)
            {
                try { SonucuKaydet(makineKomutlar, basarili, detay, oncekiDurumParametreId); break; }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    if (deneme >= 3 || (ex.Number != -2 && ex.Number != 1205
                        && ex.Number != 64 && ex.Number != 233 && ex.Number != 10054)) throw;
                    Thread.Sleep(500);
                }
            }
            if (!basarili) lock (kilit) sonHata = detay;
            DurumBildir(Mesajlar.KomutSonucu(makineKomutlar.Id, basarili, detay)
                + " Makine ID: " + makineKomutlar.MakinelerId + ". Talep eden: " + talepEden, !basarili);
        }
        finally
        {
            try { veritabaniIslemleri.GeriAl(); }
            finally { veritabaniIslemleri.Bitir(); }
        }
    }

    private void SonucuKaydet(MakineKomutlar makineKomutlar, bool basarili, string detay, int? oncekiDurumParametreId)
    {
        var veritabaniIslemleri = new VeritabaniIslemleri();
        var parametreler = new Parametreler(veritabaniIslemleri);
        try
        {
            // Sonuc kaydi hatasi fiziksel islemin catch bloguna girmez, tekrar tetiklemez.
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
            var mevcut = new MakineKomutlar(veritabaniIslemleri) { Id = makineKomutlar.Id };
            int beklenenDurum = ParametreIdGetir(parametreler, SistemParametreGruplari.MakineKomutDurumu,
                basarili ? SistemParametreKodlari.Basarili : SistemParametreKodlari.Basarisiz);
            if (mevcut.Doldur() && mevcut.DurumParametreId == beklenenDurum && mevcut.Detay == detay)
            { veritabaniIslemleri.Uygula(); return; }
            int? yeniDurumParametreId = oncekiDurumParametreId;
            if (basarili)
            {
                yeniDurumParametreId = ParametreIdGetir(parametreler,
                    SistemParametreGruplari.MakineDurumu, SistemParametreKodlari.Durduruldu);
                Makineler durumMakineler = new Makineler(veritabaniIslemleri);
                durumMakineler.Id = makineKomutlar.MakinelerId;
                durumMakineler.DurumParametreId = yeniDurumParametreId;
                durumMakineler.GuncelleyenId = makineKomutlar.EkleyenId;
                durumMakineler.GuncelleyenIp = servisIp;
                if (!durumMakineler.DurumGuncelle())
                    throw new InvalidOperationException(Mesajlar.MakineDurdurmaHatasi);
            }
            string sonucKod = basarili ? SistemParametreKodlari.Basarili : SistemParametreKodlari.Basarisiz;
            MakineKomutlar sonucMakineKomutlar = new MakineKomutlar(veritabaniIslemleri);
            sonucMakineKomutlar.Id = makineKomutlar.Id;
            sonucMakineKomutlar.DurumParametreId = ParametreIdGetir(parametreler,
                SistemParametreGruplari.MakineKomutDurumu, sonucKod);
            sonucMakineKomutlar.Detay = detay;
            sonucMakineKomutlar.GuncelleyenIp = servisIp;
            if (!sonucMakineKomutlar.Guncelle())
                throw new InvalidOperationException(Mesajlar.KomutSonucuKaydedilemedi);

            MakinelerLoglar makinelerLoglar = new MakinelerLoglar(veritabaniIslemleri);
            makinelerLoglar.MakinelerId = makineKomutlar.MakinelerId;
            makinelerLoglar.KaynakParametreId = ParametreIdGetir(parametreler,
                SistemParametreGruplari.IslemKaynagi, SistemParametreKodlari.Servis);
            makinelerLoglar.IslemTurParametreId = makineKomutlar.IslemTurParametreId;
            makinelerLoglar.IslemSonucParametreId = ParametreIdGetir(parametreler,
                SistemParametreGruplari.IslemSonucu, sonucKod);
            makinelerLoglar.OncekiDurumParametreId = oncekiDurumParametreId;
            makinelerLoglar.YeniDurumParametreId = yeniDurumParametreId;
            makinelerLoglar.Detay = Mesajlar.KomutOperasyonDetayi(makineKomutlar.Id, detay, makineKomutlar.EkleyenIp);
            makinelerLoglar.AktifMi = true;
            makinelerLoglar.EkleyenId = makineKomutlar.EkleyenId;
            makinelerLoglar.EkleyenIp = servisIp;
            if (!makinelerLoglar.Ekle())
                throw new InvalidOperationException(Mesajlar.KomutOperasyonLoguKaydedilemedi);
            veritabaniIslemleri.Uygula();
        }
        finally
        {
            try { veritabaniIslemleri.GeriAl(); }
            finally { veritabaniIslemleri.Bitir(); }
        }
    }
    private void DurumBildir(string mesaj, bool hataMi)
    {
        EventHandler<MakineKomutServisiLogEventArgs> olayIsleyici = LogOlustu;
        if (olayIsleyici == null) return;
        MakineKomutServisiLogEventArgs olayBilgileri = new MakineKomutServisiLogEventArgs(mesaj, hataMi);
        foreach (EventHandler<MakineKomutServisiLogEventArgs> abone in olayIsleyici.GetInvocationList())
        {
            // Formun kapanmasi veya log gosterim hatasi fiziksel islemi kesmemeli.
            try { abone(this, olayBilgileri); } catch { }
        }
    }

    public void Dispose()
    {
        lock (kilit)
        {
            if (kapatildi) return;
            kapatildi = true;
        }
        // Devam eden islem sonuclanir. Form, kapanmadan once IslemDevamEdiyor'u beklemelidir.
    }


}

public class MakineKomutServisiLogEventArgs : EventArgs
{
    public DateTime Tarih { get; private set; }
    public string Mesaj { get; private set; }
    public bool HataMi { get; private set; }
    public MakineKomutServisiLogEventArgs(string mesaj, bool hataMi)
    {
        Tarih = DateTime.Now;
        Mesaj = mesaj;
        HataMi = hataMi;
    }
}
