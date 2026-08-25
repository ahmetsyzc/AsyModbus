using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AsyModbus.Pages
{
    public partial class LogListele : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ucMyGrid1.DetayURL = "~/Pages/LogDetay.aspx?log_id=";

            ucMyGrid1.KolonEkle(Loglar.C_Sutun_ekleyen_id, "Kullanıcı ID");
            ucMyGrid1.KolonEkle(Loglar.C_Sutun_eklenme_tarih, "Eklenme Tarih");
            ucMyGrid1.KolonEkle(Loglar.C_Sutun_ekleyen_ip, "Kullanıcı IP");
            ucMyGrid1.KolonEkle(Loglar.C_Sutun_islem_ad, "İşlem Ad");
            ucMyGrid1.KolonEkle(Loglar.C_Sutun_islem_tip, "İşlem Tip");

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Loglar loglar = new Loglar(veritabaniIslemleri);
                ucMyGrid1.VeriBagla(loglar.TumunuGetir());
            }
            catch (Exception ex)
            {
                Mesaj.Ver(Mesajlar.SistemselHata(ex.Message), Mesaj.MesajTurleri.FAIL, Master);
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }
    }
}