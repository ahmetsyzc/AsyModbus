using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AsyModbus.Pages
{
    public partial class MakineListele : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ucMyGrid1.DetayURL = "~/Pages/MakineDuzenle.aspx?id=";

            if (IsPostBack)
            {
                return;
            }
            
            ucMyGrid1.KolonEkle(Makineler.C_Sutun_makine_no, "Makine Numarası");
            ucMyGrid1.KolonEkle(Makineler.C_Sutun_model_ad, "Model Adı");
            ucMyGrid1.KolonEkle(Makineler.C_Sutun_gg_no, "GG Numarası");
            ucMyGrid1.KolonEkle(Makineler.C_Sutun_ip, "IP Numarası");
            ucMyGrid1.KolonEkle(Makineler.C_Sutun_mfg, "MFG Numarası");

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Makineler makineler = new Makineler(veritabaniIslemleri);
                ucMyGrid1.DetayGoster = IslemYetki.Kontrol(Sayfalar.MakineDuzenle, YetkiIslemTurleri.Getirme);
                ucMyGrid1.VeriBagla(makineler.TumunuGetir());
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