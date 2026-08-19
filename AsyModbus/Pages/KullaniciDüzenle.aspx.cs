using System;
using System.Web.UI;


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

                    if (kullanicilar.Doldur())
                    {
                        txtID.Text = kullanicilar.Id.ToString();
                        txtKullanıcıKod.Text = kullanicilar.KullaniciKod.ToString();
                        txtAd.Text = kullanicilar.Ad.ToString();
                        txtSoyad.Text = kullanicilar.Soyad.ToString();
                        txtTckno.Text = kullanicilar.Tckno.ToString();
                        txtMail.Text = kullanicilar.Mail.ToString();
                        txtSifre.Text = kullanicilar.Sifre.ToString();
                        ucCepNo.Text = kullanicilar.CepNo.ToString();
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
            if (!AlanlarUygunMu())
            {
                return;
            }

            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            DosyaIslemleri dosyaIslemleri = new DosyaIslemleri();
            string yeniResimYolu = "";
            string silinecekResimYolu = "";
            bool sonuc = false;

            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);

                if (!ucCepNo.CepNoUygunMu())
                {
                    lblUyari.Text = "Telefon numarası 10 haneli olmalıdır!";
                    return;
                }
                kullanicilar.CepNo = ucCepNo.CepNoAl();

                // Mevcut resim yolu
                string eskiResimYolu = imgProfil.ImageUrl.Replace("~/", "");
                // Varsayılan olarak eski resim korunur
                string resimYolu = eskiResimYolu;
                // Yeni resim seçildiyse kaydet
                if (FileUpload1.HasFile)
                {
                    if (!dosyaIslemleri.ResimUzantisiGecerliMi(FileUpload1.FileName))
                    {
                        lblUyari.Text = "Sadece JPG, JPEG veya PNG dosyası yükleyebilirsiniz.";
                        return;
                    }
                    yeniResimYolu = dosyaIslemleri.ResimKaydet(DosyaIslemleri.C_Klasor_Kullanicilar, FileUpload1.PostedFile);
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
                kullanicilar.GuncelleyenId = currentInfo.KullaniciId;
                kullanicilar.GuncelleyenIp = currentInfo.Ip;
                sonuc = kullanicilar.Guncelle();

                if (sonuc)
                {
                    if (!string.IsNullOrEmpty(yeniResimYolu))
                    {
                        // Yeni resim seçildiyse artık eski resmi silebiliriz
                        silinecekResimYolu = eskiResimYolu;
                        imgProfil.ImageUrl = "~/" + yeniResimYolu;
                    }
                    lblUyari.Text = "Personel bilgileri güncellendi.";
                }
                else
                {
                    // DB başarısızsa yeni yüklenen resim gereksiz.
                    silinecekResimYolu = yeniResimYolu;
                    lblUyari.Text = "Personel bilgileri güncellenemedi.";
                }

            }
            catch (Exception ex)
            {
                // Yeni resim kaydedildi ama devamında hata olduysa yeni resmi temizle.
                silinecekResimYolu = yeniResimYolu;
                lblUyari.Text = "Hata: " + ex.Message;
            }
            finally
            {
                if (!string.IsNullOrEmpty(silinecekResimYolu))
                {
                    dosyaIslemleri.ResimSil(silinecekResimYolu);
                }
                veritabaniIslemleri.Bitir();
            }
        }

        protected void btnSil_Click(object sender, EventArgs e)
        {
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            try
            {
                Sessionlar sessionlar = new Sessionlar();
                CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);
                kullanicilar.Id = Convert.ToInt32(id);

                if (!kullanicilar.Doldur())
                {
                    lblUyari.Text = "Silinecek kullanıcı bulunamadı.";
                    return;
                }
                kullanicilar.GuncelleyenId = currentInfo.KullaniciId;
                kullanicilar.GuncelleyenIp = currentInfo.Ip;
                if (kullanicilar.Sil())
                {
                    int aktifKullaniciId = currentInfo.KullaniciId;

                    if (kullanicilar.Id == aktifKullaniciId)
                    {
                        sessionlar.Current._CurrentInfo = null;
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

        private bool AlanlarUygunMu()
        {
            bool sonuc = true;
            string mesaj = "";

            if (txtAd.Text.Trim().Length == 0)
            {
                mesaj += " Ad";
                sonuc = false;
            }
            else if (txtAd.Text.Trim().Length < 2)
            {
                lblUyari.Text = "Ad en az 2 karakter olmalıdır.";
                return false;
            }

            if (txtSoyad.Text.Trim().Length == 0)
            {
                mesaj += " Soyad";
                sonuc = false;
            }
            else if (txtSoyad.Text.Trim().Length < 2)
            {
                lblUyari.Text = "Soyad en az 2 karakter olmalıdır.";
                return false;
            }

            if (txtTckno.Text.Trim().Length == 0)
            {
                mesaj += " TCKNO";
                sonuc = false;
            }
            if (txtMail.Text.Trim().Length == 0)
            {
                mesaj += " Mail";
                sonuc = false;
            }
            if (string.IsNullOrWhiteSpace(ucCepNo.Text))
            {
                mesaj += " Cep No";
                sonuc = false;
            }
            if (txtDogumTarihi.Text.Trim().Length == 0)
            {
                mesaj += " Doğum Tarihi";
                sonuc = false;
            }
            if (mesaj != "")
            {
                lblUyari.Text = mesaj + " Bilgisi/Bilgileri Zorunludur.";
            }
            return sonuc;
        }

    }      
}