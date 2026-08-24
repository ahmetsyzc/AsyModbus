using System;
using System.Web.UI;

namespace AsyModbus.Pages
{
    public partial class ProfilDuzenle : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;

            if (currentInfo == null || !currentInfo.LoginYapildiMi)
            {
                Response.Redirect("~/Pages/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!Page.IsPostBack)
            {
                VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                try
                {
                    Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);
                    kullanicilar.Id = currentInfo.KullaniciId;
                    if (kullanicilar.Doldur())

                    {
                        txtAd.Text = kullanicilar.Ad.ToString();
                        txtSoyad.Text = kullanicilar.Soyad.ToString();
                        txtMail.Text = kullanicilar.Mail.ToString();
                        txtSifre.Text = kullanicilar.Sifre.ToString();
                        ucCepNo.Text = kullanicilar.CepNo.ToString();
                        if (!string.IsNullOrWhiteSpace(kullanicilar.ResimYol))
                        {
                            imgProfil.ImageUrl = "~/" + kullanicilar.ResimYol;
                        }
                    }
                    else
                    {
                        Mesaj.Ver(Mesajlar.KullaniciBulunamadi, Mesaj.MesajTurleri.WARNING, Master);
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

                kullanicilar.Id = currentInfo.KullaniciId;
                kullanicilar.Ad = txtAd.Text.Trim();
                kullanicilar.Soyad = txtSoyad.Text.Trim();
                kullanicilar.Mail = txtMail.Text.Trim();
                kullanicilar.Sifre = txtSifre.Text.Trim();
                kullanicilar.ResimYol = resimYolu;
                sonuc = kullanicilar.ProfilGuncelle();

                if (sonuc)
                {
                    currentInfo.Ad = kullanicilar.Ad;
                    currentInfo.Soyad = kullanicilar.Soyad;
                    sessionlar.Current._CurrentInfo = currentInfo;

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
            if (txtMail.Text.Trim().Length == 0)
            {
                mesaj += " Mail";
                sonuc = false;
            }
            if (txtSifre.Text.Trim().Length == 0)
            {
                mesaj += " Şifre";
                sonuc = false;
            }
            else if (txtSifre.Text.Trim().Length < 6)
            {
                Mesaj.Ver(Mesajlar.SifreEnAzAltiKarakter, Mesaj.MesajTurleri.WARNING, Master);
                return false;
            }
            if (string.IsNullOrWhiteSpace(ucCepNo.Text))
            {
                mesaj += " Cep No";
                sonuc = false;
            }
            if (mesaj != "")
            {
                Mesaj.Ver(Mesajlar.ZorunluAlanlar(mesaj), Mesaj.MesajTurleri.WARNING, Master);
            }
            return sonuc;
        }
    }
}