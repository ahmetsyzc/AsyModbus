using System;

namespace AsyModbus.Pages
{
    public partial class MakineEkle : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                btnKaydet.Visible = IslemYetki.Kontrol(YetkiIslemTurleri.Ekleme);
            }
        }

        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            if (!IslemYetki.Kontrol(YetkiIslemTurleri.Ekleme))
            {
                Mesaj.Ver(Mesajlar.YetkisizIslem, Mesaj.MesajTurleri.WARNING, Master);
                return;
            }
            if (!AlanlarUygunMu())
            {
                return;
            }

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;

            try
            {
                Makineler makineler = new Makineler(veritabaniIslemleri);
                makineler.MakineAd = txtMakineAd.Text.Trim();
                makineler.ModelAd = txtModelAd.Text.Trim();
                makineler.EntegrasyonKod = txtEntegrasyonKod.Text.Trim();
                makineler.GgNo = txtGgNo.Text.Trim();
                makineler.MakineNo = txtMakineNo.Text.Trim();
                makineler.BandNo = txtBandNo.Text.Trim();
                makineler.Ip = txtIp.Text.Trim();
                makineler.Mfg = txtMfg.Text.Trim();
                makineler.SiraNo = makineler.MaxSiraGetir() + 1;
                makineler.AktifMi = true;
                makineler.EkleyenId = currentInfo.KullaniciId;
                makineler.EkleyenIp = currentInfo.Ip;

                if (makineler.Ekle())
                {
                    Response.Redirect("~/Pages/MakineListele.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
                Mesaj.Ver(Mesajlar.KayitEklemeIslemiBasarisiz, Mesaj.MesajTurleri.FAIL, Master);
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

            if (txtMakineAd.Text.Trim().Length == 0)
            {
                mesaj += " Makine Adı";
                sonuc = false;
            }
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
