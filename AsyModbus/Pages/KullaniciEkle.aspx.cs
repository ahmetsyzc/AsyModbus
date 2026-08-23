using System;
using System.Web.UI;

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
                    Roller roller = new Roller(veritabaniIslemleri);
                    roller.Listele(DropDownList1);
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

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
            DosyaIslemleri dosyaIslemleri = new DosyaIslemleri();
            string resimYolu = "";
            bool resimSilinsinMi = false;
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);

                kullanicilar.CepNo = ucCepNo.CepNoAl();
                if (!ucCepNo.CepNoUygunMu())
                {
                    Mesaj.Ver(Mesajlar.TelefonHatali, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }

                kullanicilar.Tckno = txtTckno.Text.Trim();
                kullanicilar.Mail = txtMail.Text.Trim();

                //Kullanıcı daha önce kayıtlı mı kontrol ediyoruz
                if (kullanicilar.MailVarMi() > 0)
                {
                    Mesaj.Ver(Mesajlar.KullaniciKayitli, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }
                if (kullanicilar.CepNoVarMi() > 0)
                {
                    Mesaj.Ver(Mesajlar.KullaniciKayitli, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }
                /*if (kullanicilar.TcknoVarMi()>0)
                {
                    Mesaj.Ver(Mesajlar.KullaniciKayitli, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }*/

                // Şifre oluştur
                string sifre = kullanicilar.SifreOlustur(txtAd.Text.Trim(), txtSoyad.Text.Trim());

                // Resmin adını al - Resmi proje klasörüne kaydet - Veritabanına kaydedilecek yol
                if (!dosyaIslemleri.ResimUzantisiGecerliMi(FileUpload1.FileName))
                {
                    Mesaj.Ver(Mesajlar.ProfilResmiDosyaTipiYanlis, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }
                resimYolu = dosyaIslemleri.ResimKaydet(DosyaIslemleri.C_Klasor_Kullanicilar, FileUpload1.PostedFile);

                kullanicilar.RollerId = Convert.ToInt32(DropDownList1.SelectedValue);
                kullanicilar.Ad = txtAd.Text.Trim();
                kullanicilar.Soyad = txtSoyad.Text.Trim();
                kullanicilar.Sifre = sifre;
                kullanicilar.DogumTarih = Convert.ToDateTime(txtDogumTarihi.Text);
                kullanicilar.AktifMi = true;
                kullanicilar.ResimYol = resimYolu;
                kullanicilar.EkleyenId = currentInfo.KullaniciId;
                kullanicilar.EkleyenIp = currentInfo.Ip;


                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);

                if (kullanicilar.Ekle())
                {
                    int id = kullanicilar.MaxIdGetir();
                    kullanicilar.Id = id;
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
                        resimSilinsinMi = true;
                    }
                }
                else
                {
                    veritabaniIslemleri.GeriAl();
                    resimSilinsinMi = true;
                }

                Mesaj.Ver(Mesajlar.KayitEklemeIslemiBasarisiz, Mesaj.MesajTurleri.FAIL, Master);
            }
            catch (Exception ex)
            {
                veritabaniIslemleri.GeriAl();
                resimSilinsinMi = true;
                Mesaj.Ver(Mesajlar.SistemselHata(ex.Message), Mesaj.MesajTurleri.FAIL, Master);
            }
            finally
            {
                if (resimSilinsinMi && !string.IsNullOrEmpty(resimYolu))
                {
                    dosyaIslemleri.ResimSil(resimYolu);
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
                Mesaj.Ver(Mesajlar.KullaniciAdEnAzIkiKarakter, Mesaj.MesajTurleri.WARNING, Master);
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
            if (!FileUpload1.HasFile)
            {
                mesaj += " Profil Resmi";
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
