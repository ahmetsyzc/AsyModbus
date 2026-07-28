using System;
using System.Web.UI;
using AsyModbus.AppCode;
using System.Text.RegularExpressions;

namespace AsyModbus.Pages
{
    public partial class KullaniciDüzenle : System.Web.UI.Page
    {
        string id;

        protected void Page_Load(object sender, EventArgs e)
        {
            id = Request.QueryString["kullanici_id"];

            if (Page.IsPostBack == false)
            {
                txtDogumTarihi.Attributes["max"] = DateTime.Now.ToString("yyyy-MM-dd");
                VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

                try
                {
                    veritabaniIslemleri.Baslat();

                    //Rol Listele
                    Rol rol = new Rol(veritabaniIslemleri);
                    rol.Listele(DropDownList1);

                    //Verileri Getirme
                    Kullanici kullanici = new Kullanici(veritabaniIslemleri);
                    kullanici.Id = Convert.ToInt16(id);
                    if (kullanici.TekKayitGetir())
                    {
                        txtID.Text = kullanici.Id.ToString();
                        txtAd.Text = kullanici.Ad;
                        txtSoyad.Text = kullanici.Soyad;
                        txtTckno.Text = kullanici.Tckno;
                        txtMail.Text = kullanici.Mail;
                        txtSifre.Text = kullanici.Sifre;
                        txtCepNo.Text = kullanici.CepNo;
                        txtDogumTarihi.Text = kullanici.DogumTarih.ToString("yyyy-MM-dd");
                        imgProfil.ImageUrl = "~/" + kullanici.ResimYol;
                        DropDownList1.SelectedValue = kullanici.RollerId.ToString();
                    }
                    else
                    {
                        lblUyari.Text = "Kullanıcı bulunamadı.";
                    }
                }
                catch (Exception ex)
                {
                    lblUyari.Text = "Veriler Yüklenemedi " + ex.Message;
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
 string.IsNullOrWhiteSpace(txtCepNo.Text))
            {
                lblUyari.Text = "Lütfen tüm alanları doldurunuz.";
                return;
            }

            if (txtAd.Text.Trim().Length <= 2 || txtSoyad.Text.Trim().Length <= 2)
            {
                lblUyari.Text = "Ad ve Soyad en az 2 karakter olmalıdır.";
                return;
            }

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            try
            {
                veritabaniIslemleri.Baslat();

                // Eski resmi korumak için mevcut ImageUrl'i alıyoruz
                string resimYolu = imgProfil.ImageUrl.Replace("~/", "");

                // Eğer kullanıcı yeni fotoğraf seçtiyse
                if (FileUpload1.HasFile)
                {
                    string eskiResimYolu = resimYolu;

                    string dosyaAdi = FileUpload1.FileName;

                    FileUpload1.SaveAs(Server.MapPath("~/Files/Images/Kullanicilar/") + dosyaAdi);

                    resimYolu = "Files/Images/Kullanicilar/" + dosyaAdi;

                    imgProfil.ImageUrl = "~/" + resimYolu;

                    if (!string.IsNullOrEmpty(eskiResimYolu))
                    {
                        string fizikselYol = Server.MapPath("~/" + eskiResimYolu);

                        if (System.IO.File.Exists(fizikselYol))
                        {
                            System.IO.File.Delete(fizikselYol);
                        }
                    }

                }
                string telefon = Regex.Replace(txtCepNo.Text, @"\D", "");
                if (telefon.Length != 10)
                {
                    lblUyari.Text = "Telefon numarası 10 haneli olmalıdır!";
                    return;
                }

                Kullanici kullanici = new Kullanici(veritabaniIslemleri);
                kullanici.Id = Convert.ToInt16(txtID.Text.Trim());
                kullanici.RollerId = Convert.ToInt16(DropDownList1.SelectedValue);
                kullanici.Ad = txtAd.Text.Trim();
                kullanici.Soyad = txtSoyad.Text.Trim();
                kullanici.Tckno = txtTckno.Text.Trim();
                kullanici.Mail = txtMail.Text.Trim();
                kullanici.Sifre = txtSifre.Text.Trim();
                kullanici.CepNo = telefon;
                kullanici.DogumTarih = Convert.ToDateTime(txtDogumTarihi.Text);
                kullanici.ResimYol = resimYolu;
                if (kullanici.Guncelle())
                {
                    lblUyari.Text = "Personel bilgileri güncellendi.";
                }
                else
                {
                    lblUyari.Text = "Personel bilgileri güncellenemedi.";
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

        protected void btnSil_Click(object sender, EventArgs e)
        {
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            try
            {
                veritabaniIslemleri.Baslat();
                Kullanici kullanici = new Kullanici(veritabaniIslemleri);
                kullanici.Id = Convert.ToInt16(id);

                if (!kullanici.TekKayitGetir())
                {
                    lblUyari.Text = "Silinecek kullanıcı bulunamadı.";
                    return;
                }

                string resimYolu = kullanici.ResimYol;

                if (kullanici.Sil())
                {
                    if (!string.IsNullOrEmpty(resimYolu))
                    {
                        string fizikselYol = Server.MapPath("~/" + resimYolu);

                        if (System.IO.File.Exists(fizikselYol))
                        {
                            System.IO.File.Delete(fizikselYol);
                        }
                    }
                    Response.Redirect("~/Pages/KullaniciListele.aspx");
                }
                else
                {
                    lblUyari.Text = "Kullanıcı silinemedi.";
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