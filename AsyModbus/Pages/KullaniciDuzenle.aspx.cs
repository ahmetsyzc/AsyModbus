using System;
using System.Web.UI;


namespace AsyModbus.Pages
{
    public partial class KullaniciDuzenle : System.Web.UI.Page
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
                        Mesaj.Ver(Mesajlar.KullaniciBulunamadi, Mesaj.MesajTurleri.FAIL, Master);
                    }
                }
                catch (Exception ex)
                {
                    Mesaj.Ver(Mesajlar.SistemselHata(ex.Message), Mesaj.MesajTurleri.FAIL, Master);
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
                    Mesaj.Ver(Mesajlar.TelefonHatali, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }
                kullanicilar.CepNo = ucCepNo.CepNoAl();

                // Mevcut resim yolu
                string eskiResimYolu = imgProfil.ImageUrl.Replace("~/", "");
                // Varsayılan olarak eski resim korunur
                string resimYolu = eskiResimYolu;

                // Eski resim yoksa ve yeni resim de seçilmediyse
                if (string.IsNullOrEmpty(eskiResimYolu) && !FileUpload1.HasFile)
                {
                    Mesaj.Ver(Mesajlar.ProfilResmiZorunlu, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }

                // Yeni resim seçildiyse kaydet
                if (FileUpload1.HasFile)
                {
                    if (!dosyaIslemleri.ResimUzantisiGecerliMi(FileUpload1.FileName))
                    {
                        Mesaj.Ver(Mesajlar.ProfilResmiDosyaTipiYanlis, Mesaj.MesajTurleri.WARNING, Master);
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
                    Mesaj.Ver(Mesajlar.KayitGuncellemeBasarili, Mesaj.MesajTurleri.SUCCESS, Master);
                }
                else
                {
                    // DB başarısızsa yeni yüklenen resim gereksiz.
                    silinecekResimYolu = yeniResimYolu;
                    Mesaj.Ver(Mesajlar.KayitGuncellemeBasarisiz, Mesaj.MesajTurleri.FAIL, Master);
                }

            }
            catch (Exception ex)
            {
                // Yeni resim kaydedildi ama devamında hata olduysa yeni resmi temizle.
                silinecekResimYolu = yeniResimYolu;
                Mesaj.Ver(Mesajlar.SistemselHata(ex.Message), Mesaj.MesajTurleri.FAIL, Master);
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
                    Mesaj.Ver(Mesajlar.KullaniciBulunamadi, Mesaj.MesajTurleri.FAIL, Master);
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
                    Mesaj.Ver(Mesajlar.KayitSilmeBasarisiz, Mesaj.MesajTurleri.FAIL, Master);
                }
            }
            catch (Exception ex)
            {
                Mesaj.Ver(Mesajlar.SistemselHata(ex.Message), Mesaj.MesajTurleri.FAIL, Master);
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
                Mesaj.Ver(Mesajlar.KullaniciAdEnAzIkiKarakter, Mesaj.MesajTurleri.WARNING, Master
                );
                return false;
            }
            if (txtSoyad.Text.Trim().Length == 0)
            {
                mesaj += " Soyad";
                sonuc = false;
            }
            else if (txtSoyad.Text.Trim().Length < 2)
            {
                Mesaj.Ver(Mesajlar.KullaniciSoyadEnAzIkiKarakter, Mesaj.MesajTurleri.WARNING, Master);
                return false;
            }
            if (txtTckno.Text.Trim().Length == 0)
            {
                mesaj += " TCKNO";
                sonuc = false;
            }
            else if (txtTckno.Text.Trim().Length != 11)
            {
                Mesaj.Ver(Mesajlar.KullaniciTcKimlikNoOnBirHane, Mesaj.MesajTurleri.WARNING, Master);
                return false;
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
                Mesaj.Ver(Mesajlar.ZorunluAlanlar(mesaj), Mesaj.MesajTurleri.WARNING, Master
                );
            }
            return sonuc;
        }

    }
}