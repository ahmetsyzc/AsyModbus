using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using AsyModbus.AppCode;

namespace AsyModbus.Pages
{
    public partial class SifreSifirlama : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSifreSifirla_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMail.Text) ||
                string.IsNullOrWhiteSpace(txtCepNo.Text))
            {
                lblUyari.Text = "Mail ve Cep No boş bırakılamaz.";
                return;
            }
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            SqlDataReader sqlDataReader = null;
            try
            {
                veritabaniIslemleri.Baslat();
                Kullanici kullanici = new Kullanici(veritabaniIslemleri);
                kullanici.Mail = txtMail.Text.Trim();
                kullanici.CepNo = txtCepNo.Text.Trim();

                sqlDataReader = kullanici.MailCepNoKontrol();

                if (sqlDataReader.Read())
                {
                    kullanici.Id = Convert.ToInt16(sqlDataReader["id"]);
                    string ad = sqlDataReader["ad"].ToString();
                    string soyad = sqlDataReader["soyad"].ToString();

                    sqlDataReader.Close();
                    sqlDataReader = null;

                    kullanici.Sifre = kullanici.SifreOlustur(ad, soyad);

                    if (kullanici.SifreGuncelle())
                    {
                        lblUyari.Text = "Yeni şifreniz = " + kullanici.Sifre;
                    }
                    else
                    {
                        lblUyari.Text = "Şifre güncellenemedi.";
                    }
                }
                else
                {
                    lblUyari.Text = "Mail veya cep telefonu hatalı.";
                }
            }
            catch (Exception ex)
            {

                lblUyari.Text="Sistemsel Hata "+ex.Message;
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