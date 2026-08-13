using System;
using System.Web.UI;
using BusinessLayer.Work;
using BusinessLayer.Entity;

namespace AsyModbus.Pages
{
    public partial class KullaniciEkle : System.Web.UI.Page
    {
        

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                txtDogumTarihi.Attributes["max"] = DateTime.Now.ToString("yyyy-MM-dd");
                VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
                try
                {
                    //Rol Listele
                    veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                    Roller rol = new Roller(veritabaniIslemleri);
                    rol.Listele(DropDownList1);
                }
                catch (Exception ex)
                {
                    lblUyari.Text = "Sistemsel Hata " + ex.Message;
                }
                finally
                {
                    veritabaniIslemleri.Bitir();
                }
            }
        }

        protected void btnKaydet_Click(object sender, EventArgs e)
        {
             
            if (string.IsNullOrWhiteSpace(txtAd.Text) ||
                string.IsNullOrWhiteSpace(txtSoyad.Text) ||
                string.IsNullOrWhiteSpace(txtTckno.Text) ||
                string.IsNullOrWhiteSpace(txtMail.Text) ||
                string.IsNullOrWhiteSpace(txtCepNo.Text) ||
                string.IsNullOrWhiteSpace(txtDogumTarihi.Text) ||
                !FileUpload1.HasFile)
            {
                lblUyari.Text = "Lütfen tüm alanları doldurunuz.";
                return;
            }
            if (txtTckno.Text.Trim().Length != 11)
            {
                lblUyari.Text = "Tc Kimlik No 11 Hane Olmalıdır!";
                return;
            }

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            string resimYolu="";
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);
                
                kullanicilar.CepNo = txtCepNo.Text.Trim();
                if (kullanicilar.CepNo.Length != 10)
                {
                    lblUyari.Text = "Telefon numarası 10 haneli olmalıdır!";
                    return;
                }

                kullanicilar.Tckno = txtTckno.Text.Trim();
                kullanicilar.Mail = txtMail.Text.Trim();

                //Kullanıcı daha önce kayıtlı mı kontrol ediyoruz
                if (kullanicilar.MailVarMi()>0)
                {
                    lblUyari.Text = "Bu Mail Hesabı Sistemde Kayıtlı !";
                    return;
                }
                if (kullanicilar.CepNoVarMi() > 0)
                {
                    lblUyari.Text = "Bu Telefon Numarası Sistemde Kayıtlı !";
                    return;
                }
                /*if (kullanicilar.TcknoVarMi()>0)
                {
                    lblUyari.Text = "Bu Tckno Sistemde Kayıtlı !";
                    return;
                }*/

                if (txtAd.Text.Trim().Length < 2 || txtSoyad.Text.Trim().Length < 2)
                {
                    lblUyari.Text = "Ad ve Soyad en az 2 karakter olmalıdır.";
                    return;
                }

                // Şifre oluştur
                string sifre = kullanicilar.SifreOlustur(txtAd.Text.Trim(), txtSoyad.Text.Trim());

                // Resmin adını al - Resmi proje klasörüne kaydet - Veritabanına kaydedilecek yol
                string uzanti = System.IO.Path.GetExtension(FileUpload1.FileName);
                string dosyaAdi = Guid.NewGuid().ToString() + uzanti;
                FileUpload1.SaveAs(Server.MapPath("~/Files/Images/Kullanicilar/") + dosyaAdi);
                resimYolu = "Files/Images/Kullanicilar/" + dosyaAdi;

                kullanicilar.RollerId = Convert.ToInt32(DropDownList1.SelectedValue);
                kullanicilar.Ad = txtAd.Text.Trim();
                kullanicilar.Soyad = txtSoyad.Text.Trim();
                kullanicilar.Sifre = sifre;
                kullanicilar.DogumTarih = Convert.ToDateTime(txtDogumTarihi.Text);
                kullanicilar.AktifMi = true;
                kullanicilar.ResimYol = resimYolu;
                kullanicilar.EkleyenId = Convert.ToInt32(Session["KullaniciId"]);
                kullanicilar.EkleyenIp = Request.UserHostAddress;


                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
                if (kullanicilar.Ekle())
                {
                    kullanicilar.KullaniciKodHesapla();

                    if (kullanicilar.KullaniciKodOlustur())
                    {
                        veritabaniIslemleri.Uygula();

                        Response.Redirect("~/Pages/KullaniciListele.aspx", false);
                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    }
                    else
                    {
                        veritabaniIslemleri.GeriAl();
                        ResimSil(resimYolu);
                    }
                }
                else
                {
                    veritabaniIslemleri.GeriAl();
                    ResimSil(resimYolu);
                }

                lblUyari.Text = "Kullanıcı eklenemedi.";
            }
            catch (Exception ex)
            {
                veritabaniIslemleri.GeriAl();
                ResimSil(resimYolu);
                lblUyari.Text = "Hata: " + ex.Message;
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }

        private void ResimSil(string resimYolu)
        {
            if (!string.IsNullOrEmpty(resimYolu))
            {
                string fizikselYol = Server.MapPath("~/" + resimYolu);

                if (System.IO.File.Exists(fizikselYol))
                {
                    System.IO.File.Delete(fizikselYol);
                }
            }
        }
    }
}