using System;
using BusinessLayer.Work;
using BusinessLayer.Entity;
using System.Data;

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
            try
            {
                Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);
                kullanicilar.Mail = txtMail.Text.Trim();
                kullanicilar.CepNo = txtCepNo.Text.Trim();
                DataRow dataRow = kullanicilar.MailCepNoKontrol();

                if (dataRow != null)
                {
                    kullanicilar.Id = Convert.ToInt32(dataRow[Kullanicilar.C_Sutun_id]);
                    string ad = dataRow[Kullanicilar.C_Sutun_ad].ToString();
                    string soyad = dataRow[Kullanicilar.C_Sutun_soyad].ToString();
                    kullanicilar.Sifre = kullanicilar.SifreOlustur(ad, soyad);

                    if (kullanicilar.SifreGuncelle())
                    {
                        lblUyari.Text = "Yeni şifreniz = " + kullanicilar.Sifre;
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
        }
    }
}