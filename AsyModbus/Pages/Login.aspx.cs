using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using AsyModbus.AppCode;


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

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            SqlDataReader sqlDataReader = null;
            try
            {
                veritabaniIslemleri.Baslat();
                Kullanici kullanici = new Kullanici(veritabaniIslemleri);

                kullanici.Mail = txtMail.Text.Trim();
                kullanici.Sifre = txtSifre.Text.Trim();
                sqlDataReader = kullanici.SifreKontrol();

                if (sqlDataReader.Read())
                {
                    Session["KullaniciID"] = sqlDataReader["id"].ToString();
                    Session["AktifKullanici"] = sqlDataReader["ad"].ToString() + " " + sqlDataReader["soyad"].ToString();
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
            finally
            {
                if (sqlDataReader != null)
                {
                    sqlDataReader.Close();
                }
                veritabaniIslemleri.Bitir();
            }
        }
    }
}