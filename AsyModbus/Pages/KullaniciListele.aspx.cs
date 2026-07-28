using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using AsyModbus.AppCode;

namespace AsyModbus.Pages
{
    public partial class KullaniciListele : System.Web.UI.Page
    {
        SqlBaglanti sqlBaglanti = new SqlBaglanti();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
                try
                {
                    veritabaniIslemleri.Baslat();

                    Kullanici kullanici = new Kullanici(veritabaniIslemleri);
                    DataTable dataTable = kullanici.TumKayitGetir();
                    Repeater1.DataSource = dataTable;
                    Repeater1.DataBind();
                }
                catch (Exception)
                {
                    
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

            Response.Redirect("~/Pages/KullaniciDüzenle.aspx?kullanici_id=" + id);
        }
    }
}