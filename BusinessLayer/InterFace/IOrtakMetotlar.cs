using System.Data;



public interface IOrtakMetotlar
{

    bool Ekle();

    bool Sil();

    bool Guncelle();

    DataTable TumunuGetir();

    bool Doldur();

}
