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
                Mesaj.Ver(Mesajlar.SifreSifirlamaMailCepNoBos, Mesaj.MesajTurleri.WARNING, this);
                return;
            }
            if (!ucCepNo.CepNoUygunMu())
            {
                Mesaj.Ver(Mesajlar.TelefonHatali, Mesaj.MesajTurleri.WARNING, this);
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
                        Mesaj.Ver(Mesajlar.SifreSifirlamaBasarili(kullanicilar.Sifre), Mesaj.MesajTurleri.SUCCESS, this);
                    }
                    else
                    {
                        Mesaj.Ver(Mesajlar.SifreSifirlamaBasarisiz, Mesaj.MesajTurleri.FAIL, this);
                    }
                }
                else
                {
                    Mesaj.Ver(Mesajlar.SifreSifirlamaBilgilerHatali, Mesaj.MesajTurleri.WARNING, this);
                }
            }
            catch (Exception ex)
            {

                Mesaj.Ver(Mesajlar.SistemselHata(ex.Message), Mesaj.MesajTurleri.FAIL, this);
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }
    }
}