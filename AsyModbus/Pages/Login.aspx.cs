using System;
using System.Data;


namespace AsyModbus.Pages
{
    public partial class Login : System.Web.UI.Page
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
            if (currentInfo != null && currentInfo.LoginYapildiMi)
            {
                Response.Redirect("~/Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
        }


        protected void btnGiris_Click(object sender, EventArgs e)
        {
            Captcha1.ValidateCaptcha(txtCaptcha.Text.Trim());
            if (Captcha1.UserValidated)
            {
                if (string.IsNullOrWhiteSpace(txtMail.Text) ||
                    string.IsNullOrWhiteSpace(txtSifre.Text))
                {
                    lblUyari.Text = "Mail ve Şifre boş bırakılamaz.";
                    return;
                }
                VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
                try
                {
                    veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                    Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);

                    kullanicilar.Mail = txtMail.Text.Trim();
                    kullanicilar.Sifre = txtSifre.Text.Trim();

                    if (kullanicilar.SifreKontrol())
                    {
                        Sessionlar sessionlar = new Sessionlar();
                        CurrentInfo currentInfo = new CurrentInfo();

                        currentInfo.KullaniciId = Convert.ToInt32(kullanicilar.VeriSatiri[Kullanicilar.C_Sutun_id]);
                        currentInfo.Ad = kullanicilar.VeriSatiri[Kullanicilar.C_Sutun_ad].ToString();
                        currentInfo.Soyad = kullanicilar.VeriSatiri[Kullanicilar.C_Sutun_soyad].ToString();
                        currentInfo.RolId = Convert.ToInt32(kullanicilar.VeriSatiri[Kullanicilar.C_Sutun_roller_id]);
                        currentInfo.KullaniciKod = kullanicilar.VeriSatiri[Kullanicilar.C_Sutun_kullanici_kod].ToString();
                        currentInfo.Ip = Request.UserHostAddress;
                        currentInfo.LoginYapildiMi = true;
                        sessionlar.Current._CurrentInfo = currentInfo;

                        Response.Redirect("~/Default.aspx", false);
                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    }
                    else
                    {
                        lblUyari.Text =
                            "Mail veya şifre hatalı.";
                    }
                }
                catch (Exception ex)
                {
                    lblUyari.Text = "Sistemsel Hata ! " + ex.Message;
                }
                finally
                {
                    veritabaniIslemleri.Bitir();
                }
            }
            else
            {
                lblUyari.Text = "Güvenlik Kodu Hatalı.";
                return;
            }
        }
    }
}