using System;
using System.Web.UI;
using BusinessLayer.Work;
using BusinessLayer.Entity;


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
                    //Rol Listele
                    veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                    Roller rol = new Roller(veritabaniIslemleri);
                    rol.Listele(DropDownList1);

                    //Verileri Getirme
                    Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);
                    kullanicilar.Id = Convert.ToInt32(id);

                    if (kullanicilar.TekKayitGetir())
                    {
                        txtID.Text = kullanicilar.Id.ToString();
                        txtKullanıcıKod.Text = kullanicilar.KullaniciKod.ToString();
                        txtAd.Text = kullanicilar.Ad.ToString();
                        txtSoyad.Text = kullanicilar.Soyad.ToString();
                        txtTckno.Text = kullanicilar.Tckno.ToString();
                        txtMail.Text = kullanicilar.Mail.ToString();
                        txtSifre.Text = kullanicilar.Sifre.ToString();
                        txtCepNo.Text = kullanicilar.CepNo.ToString();
                        txtDogumTarihi.Text = kullanicilar.DogumTarih.ToString("yyyy-MM-dd");
                        imgProfil.ImageUrl = "~/" + kullanicilar.ResimYol.ToString();
                        DropDownList1.SelectedValue = kullanicilar.RollerId.ToString();
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

            if (txtAd.Text.Trim().Length < 2 || txtSoyad.Text.Trim().Length < 2)
            {
                lblUyari.Text = "Ad ve Soyad en az 2 karakter olmalıdır.";
                return;
            }

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            string yeniResimYolu = "";

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

                // Mevcut resim yolu
                string eskiResimYolu = imgProfil.ImageUrl.Replace("~/", "");
                // Varsayılan olarak eski resim korunur
                string resimYolu = eskiResimYolu;

                // Yeni resim seçildiyse kaydet
                if (FileUpload1.HasFile)
                {
                    string uzanti = System.IO.Path.GetExtension(FileUpload1.FileName);
                    string dosyaAdi = Guid.NewGuid().ToString() + uzanti;
                    FileUpload1.SaveAs(Server.MapPath("~/Files/Images/Kullanicilar/") + dosyaAdi);
                    yeniResimYolu = "Files/Images/Kullanicilar/" + dosyaAdi;
                    resimYolu = yeniResimYolu;
                }

                kullanicilar.Id = Convert.ToInt32(txtID.Text.Trim());
                kullanicilar.RollerId = Convert.ToInt32(DropDownList1.SelectedValue);
                kullanicilar.Ad = txtAd.Text.Trim();
                kullanicilar.Soyad = txtSoyad.Text.Trim();
                kullanicilar.Tckno = txtTckno.Text.Trim();
                kullanicilar.Mail = txtMail.Text.Trim();
                kullanicilar.Sifre = txtSifre.Text.Trim();
                kullanicilar.ResimYol = resimYolu;
                kullanicilar.DogumTarih = Convert.ToDateTime(txtDogumTarihi.Text);
                kullanicilar.GuncelleyenId = Convert.ToInt32(Session["KullaniciId"]);
                kullanicilar.GuncelleyenIp = Request.UserHostAddress;

                if (kullanicilar.Guncelle())
                {
                    // Yeni resim seçildiyse artık eski resmi silebiliriz
                    if (!string.IsNullOrEmpty(yeniResimYolu))
                    {
                        ResimSil(eskiResimYolu);
                        imgProfil.ImageUrl = "~/" + yeniResimYolu;
                    }

                    lblUyari.Text = "Personel bilgileri güncellendi.";
                }
                else
                {
                    // DB güncellenmediyse yeni yüklenen resmi sil
                    if (!string.IsNullOrEmpty(yeniResimYolu))
                    {
                        ResimSil(yeniResimYolu);
                    }

                    lblUyari.Text = "Personel bilgileri güncellenemedi.";
                }

            }
            catch (Exception ex)
            {
                // Resim kaydedilmiş fakat sonrasında hata oluşmuş olabilir
                if (!string.IsNullOrEmpty(yeniResimYolu))
                {
                    ResimSil(yeniResimYolu);
                }

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
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);
                kullanicilar.Id = Convert.ToInt32(id);
                if (!kullanicilar.TekKayitGetir())
                {
                    lblUyari.Text = "Silinecek kullanıcı bulunamadı.";
                    return;
                }
                kullanicilar.GuncelleyenId = Convert.ToInt32(Session["KullaniciId"]);
                kullanicilar.GuncelleyenIp = Request.UserHostAddress;
                if (kullanicilar.Sil())
                {
                    int aktifKullaniciId = Convert.ToInt32(Session["KullaniciId"]);

                    if (kullanicilar.Id == aktifKullaniciId)
                    {
                        Session.Clear();
                        Session.Abandon();

                        Response.Redirect("~/Pages/Login.aspx", false);
                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    }

                    Response.Redirect("~/Pages/KullaniciListele.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
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

        private void ResimSil(string resimYolu)
        {
            if (!string.IsNullOrEmpty(resimYolu))
            {
                string fizikselYol =
                    Server.MapPath("~/" + resimYolu);

                if (System.IO.File.Exists(fizikselYol))
                {
                    System.IO.File.Delete(fizikselYol);
                }
            }
        }
    }
}