using System;

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
                string.IsNullOrWhiteSpace(ucCepNo.Text))
            {
                lblUyari.Text = "Mail ve Cep No boş bırakılamaz.";
                return;
            }
            if (!ucCepNo.CepNoUygunMu())
            {
                lblUyari.Text = "Telefon numarası 10 haneli olmalıdır.";
                return;
            }
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);
                kullanicilar.Mail = txtMail.Text.Trim();
                kullanicilar.CepNo = ucCepNo.CepNoAl();

                if (kullanicilar.MailCepNoKontrol() != null)
                {
                    kullanicilar.Id = Convert.ToInt32(kullanicilar.VeriSatiri[Kullanicilar.C_Sutun_id]);
                    string ad = kullanicilar.VeriSatiri[Kullanicilar.C_Sutun_ad].ToString();
                    string soyad = kullanicilar.VeriSatiri[Kullanicilar.C_Sutun_soyad].ToString();
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
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }
    }
}