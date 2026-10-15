using System.Threading.Tasks;
public interface IRoleKontrol
{
    string SonHata { get; }
    Task<bool> KanalTetikleAsync(MakineKomutlar komut);
}
