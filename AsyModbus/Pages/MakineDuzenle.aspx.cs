using System;

namespace AsyModbus.Pages
{
    public partial class MakineDuzenle : System.Web.UI.Page
    {
        string id;

        protected void Page_Load(object sender, EventArgs e)
        {
            id = Request.QueryString["id"];

            if (Page.IsPostBack == false)
            {
                VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
                try
                {
                    veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                    Makineler makineler = new Makineler(veritabaniIslemleri);
                    makineler.Id = Convert.ToInt32(id);

                    if (makineler.Doldur())
                    {
                        txtID.Text = makineler.Id.ToString();
                        txtModelAd.Text = makineler.ModelAd;
                        txtEntegrasyonKod.Text = makineler.EntegrasyonKod;
                        txtGgNo.Text = makineler.GgNo;
                        txtMakineNo.Text = makineler.MakineNo;
                        txtBandNo.Text = makineler.BandNo;
                        txtIp.Text = makineler.Ip;
                        txtMfg.Text = makineler.Mfg;
                    }
                    else
                    {
                        Mesaj.Ver(Mesajlar.MakineBulunamadi, Mesaj.MesajTurleri.FAIL, Master);
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
                btnKaydet.Visible = IslemYetki.Kontrol(YetkiIslemTurleri.Guncelleme);
                btnSil.Visible = IslemYetki.Kontrol(YetkiIslemTurleri.Silme);
            }
        }

        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            if (!IslemYetki.Kontrol(YetkiIslemTurleri.Guncelleme))
            {
                Mesaj.Ver(Mesajlar.YetkisizIslem, Mesaj.MesajTurleri.WARNING, Master);
                return;
            }
            if (!AlanlarUygunMu())
            {
                return;
            }

            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Makineler makineler = new Makineler(veritabaniIslemleri);
                makineler.Id = Convert.ToInt32(txtID.Text.Trim());
                makineler.ModelAd = txtModelAd.Text.Trim();
                makineler.EntegrasyonKod = txtEntegrasyonKod.Text.Trim();
                makineler.GgNo = txtGgNo.Text.Trim();
                makineler.MakineNo = txtMakineNo.Text.Trim();
                makineler.BandNo = txtBandNo.Text.Trim();
                makineler.Ip = txtIp.Text.Trim();
                makineler.Mfg = txtMfg.Text.Trim();
                makineler.GuncelleyenId = currentInfo.KullaniciId;
                makineler.GuncelleyenIp = currentInfo.Ip;

                if (makineler.Guncelle())
                {
                    Mesaj.Ver(Mesajlar.KayitGuncellemeBasarili, Mesaj.MesajTurleri.SUCCESS, Master);
                }
                else
                {
                    Mesaj.Ver(Mesajlar.KayitGuncellemeBasarisiz, Mesaj.MesajTurleri.FAIL, Master);
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

        protected void btnSil_Click(object sender, EventArgs e)
        {
            if (!IslemYetki.Kontrol(YetkiIslemTurleri.Silme))
            {
                Mesaj.Ver(Mesajlar.YetkisizIslem, Mesaj.MesajTurleri.WARNING, Master);
                return;
            }
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            try
            {
                Sessionlar sessionlar = new Sessionlar();
                CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Makineler makineler = new Makineler(veritabaniIslemleri);
                makineler.Id = Convert.ToInt32(id);

                if (!makineler.Doldur())
                {
                    Mesaj.Ver(Mesajlar.MakineBulunamadi, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                makineler.GuncelleyenId = currentInfo.KullaniciId;
                makineler.GuncelleyenIp = currentInfo.Ip;

                if (makineler.Sil())
                {
                    Response.Redirect("~/Pages/MakineListele.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                Mesaj.Ver(Mesajlar.KayitSilmeBasarisiz, Mesaj.MesajTurleri.FAIL, Master);
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

            if (txtModelAd.Text.Trim().Length == 0)
            {
                mesaj += " Model Adı";
                sonuc = false;
            }
            if (txtEntegrasyonKod.Text.Trim().Length == 0)
            {
                mesaj += " Entegrasyon Kodu";
                sonuc = false;
            }
            if (txtGgNo.Text.Trim().Length == 0)
            {
                mesaj += " GG Numarası";
                sonuc = false;
            }
            if (txtMakineNo.Text.Trim().Length == 0)
            {
                mesaj += " Makine Numarası";
                sonuc = false;
            }
            if (txtBandNo.Text.Trim().Length == 0)
            {
                mesaj += " Band Numarası";
                sonuc = false;
            }
            if (txtIp.Text.Trim().Length == 0)
            {
                mesaj += " IP";
                sonuc = false;
            }
            if (txtMfg.Text.Trim().Length == 0)
            {
                mesaj += " MFG";
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
