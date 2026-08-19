using System;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace AsyModbus.Pages
{
    public partial class KullaniciListele : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
                try
                {
                    veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                    Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);
                    Repeater1.DataSource = kullanicilar.TumunuGetir();
                    Repeater1.DataBind();
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

        protected void btnGuncelle_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            string id = button.CommandArgument;

            Response.Redirect("~/Pages/KullaniciDüzenle.aspx?kullanici_id=" + id , false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}