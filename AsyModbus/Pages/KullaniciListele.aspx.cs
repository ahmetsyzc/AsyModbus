using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using AsyModbus.UserControls;

namespace AsyModbus.Pages
{
    public partial class KullaniciListele : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            ucMyGrid1.DetayURL = "~/Pages/KullaniciDüzenle.aspx?kullanici_id=";

            ucMyGrid1.KolonEkle(Kullanicilar.C_Sutun_id, "Kullanıcı ID");
            ucMyGrid1.KolonEkle(Kullanicilar.C_Sutun_ad, "Kullanıcı Ad");
            ucMyGrid1.KolonEkle(Kullanicilar.C_Sutun_soyad, "Kullanıcı Soyad");
            ucMyGrid1.KolonEkle(Kullanicilar.C_Sutun_tckno, "Kullanıcı TCKNO");

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);
                ucMyGrid1.VeriBagla(kullanicilar.TumunuGetir());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }
    }
}