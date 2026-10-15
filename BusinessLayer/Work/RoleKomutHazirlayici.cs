using System;
using System.Net;

// Yalniz web tarafinda cagrilir; ag istegi gondermez.
public static class RoleKomutHazirlayici
{
    public static void Hazirla(MakineKomutlar komut, RoleCihazlar cihaz, int kanal)
    {
        IPAddress ip;
        if (cihaz == null || !IPAddress.TryParse(cihaz.Ip, out ip)
            || !cihaz.Port.HasValue || cihaz.Port < 1 || cihaz.Port > 65535
            || !cihaz.KanalSayisi.HasValue || kanal < 1 || kanal > cihaz.KanalSayisi.Value)
            throw new InvalidOperationException("Röle bağlantı veya kanal bilgileri geçersiz.");
        komut.HedefRoleCihazId = cihaz.Id;
        komut.HedefIp = cihaz.Ip;
        komut.HedefPort = cihaz.Port.Value;
        komut.TetiklemeSuresiMs = 2000;
        // Mevcut cihaz protokolu: HTTP GET; cift komut ON, tek komut OFF.
        komut.TetiklemeUrl = new UriBuilder("http", komut.HedefIp, komut.HedefPort,
            ((kanal - 1) * 2).ToString("D2")).Uri.AbsoluteUri;
        komut.KapatmaUrl = new UriBuilder("http", komut.HedefIp, komut.HedefPort,
            (((kanal - 1) * 2) + 1).ToString("D2")).Uri.AbsoluteUri;
    }
}
