public interface IRoleKontrol
{
    string SonHata { get; }
    bool KanalTetikle(string ip, int port, int kanalNo);
}
