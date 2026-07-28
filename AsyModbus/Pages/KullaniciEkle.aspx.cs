using System;
using System.Web.UI;
using AsyModbus.AppCode;
using System.Text.RegularExpressions;

namespace AsyModbus.Pages
{
    public partial class KullaniciEkle : System.Web.UI.Page
    {
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                txtDogumTarihi.Attributes["max"] = DateTime.Now.ToString("yyyy-MM-dd");

                try
                {
                    //Rol Listele
                    veritabaniIslemleri.Baslat();
                    Rol rol = new Rol(veritabaniIslemleri);
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

            //trim() sadece baştaki ve sondaki boşlukları siler
            // Telefon numarasındaki rakam olmayan tüm karakterleri siler.
            // Örnek:
            // (532)-555-1234  →  5325551234
            // \D = Rakam olmayan karakterler
            // "" = Bulduğu rakam olmayan karakterleri siler.
            string telefon = Regex.Replace(txtCepNo.Text, @"\D", "");

            // Telefon numarası sadece rakamlardan oluşacağı için
            // uzunluğu 10 hane olmalıdır.
            if (telefon.Length != 10)
            {
                lblUyari.Text = "Telefon numarası 10 haneli olmalıdır!";
                return;
            }
           
            try
            {
                veritabaniIslemleri.Baslat();
                Kullanici kullanici = new Kullanici(veritabaniIslemleri);

                //Kullanıcı daha önce kayıtlı mı kontrol ediyoruz
                if (kullanici.KayitVarMi("mail", txtMail.Text.Trim()))
                {
                    lblUyari.Text = "Bu Mail Hesabı Sistemde Kayıtlı !";
                    return;
                }
                /*if (kullanici.KayitVarMi("tckno", txtTckno.Text.Trim()))
                {
                    lblUyari.Text = "Bu Tckno Sistemde Kayıtlı !";
                    return;
                } */
                if (kullanici.KayitVarMi("cep_no", telefon))
                {
                    lblUyari.Text = "Bu Telefon Numarası Sistemde Kayıtlı !";
                    return;
                }
                if (txtAd.Text.Trim().Length < 2 || txtSoyad.Text.Trim().Length < 2)
                {
                    lblUyari.Text = "Ad ve Soyad en az 2 karakter olmalıdır.";
                    return;
                }

                // Şifre oluştur
                string sifre = kullanici.SifreOlustur(txtAd.Text.Trim(), txtSoyad.Text.Trim());
                txtSifre.Text = sifre;

                // Resmin adını al - Resmi proje klasörüne kaydet - Veritabanına kaydedilecek yol
                string dosyaAdi = FileUpload1.FileName;
                FileUpload1.SaveAs(Server.MapPath("~/Files/Images/Kullanicilar/") + dosyaAdi);
                string resimYolu = "Files/Images/Kullanicilar/" + dosyaAdi;

                kullanici.RollerId = Convert.ToInt16(DropDownList1.SelectedValue);
                kullanici.Ad = txtAd.Text.Trim();
                kullanici.Soyad = txtSoyad.Text.Trim();
                kullanici.Tckno = txtTckno.Text.Trim();
                kullanici.Mail = txtMail.Text.Trim();
                kullanici.Sifre = sifre;
                kullanici.CepNo = telefon;
                kullanici.DogumTarih = Convert.ToDateTime(txtDogumTarihi.Text);
                kullanici.AktifMi = true;
                kullanici.ResimYol = resimYolu;

                if (kullanici.Ekle())
                {
                    Response.Redirect("~/Pages/KullaniciListele.aspx");
                }
                else
                {
                    lblUyari.Text = "Kullanıcı eklenemedi.";
                }
            }
            catch (Exception ex)
            {
                lblUyari.Text = "Hata: " + ex.Message;
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }
    }
}