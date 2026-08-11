using System;
using BusinessLayer.Work;
using BusinessLayer.Entity;
using System.Data;


namespace AsyModbus.Pages
{
    public partial class Login : System.Web.UI.Page
    {
        

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["KullaniciID"] != null)
            {
                Response.Redirect("~/Default.aspx",false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
        }


        protected void btnGiris_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMail.Text) ||
                string.IsNullOrWhiteSpace(txtSifre.Text))
            {
                lblUyari.Text = "Mail ve Şifre boş bırakılamaz.";
                return;
            }
            try
            {
                VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
                Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);

                kullanicilar.Mail = txtMail.Text.Trim();
                kullanicilar.Sifre = txtSifre.Text.Trim();
                DataRow dataRow = kullanicilar.SifreKontrol();

                if (dataRow!=null)
                {
                    Session["KullaniciId"] = dataRow[Kullanicilar.C_Sutun_id].ToString();
                    Session["AktifKullanici"] = dataRow[Kullanicilar.C_Sutun_ad].ToString() + " " + dataRow[Kullanicilar.C_Sutun_soyad].ToString();
                    Response.Redirect("~/Default.aspx",false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
                else
                {
                    lblUyari.Text = "Mail veya şifre hatalı.";
                }
            }
            catch (Exception ex)
            {
               lblUyari.Text = "Sistemsel Hata ! " + ex.Message;
            }
        }
    }
}