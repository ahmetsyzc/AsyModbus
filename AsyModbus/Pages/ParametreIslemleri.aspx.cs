using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AsyModbus.Pages
{
    public partial class ParametreIslemleri : System.Web.UI.Page
    {
        private bool SeciliGrupSistemMi
        {
            get { return ViewState["SeciliGrupSistemMi"] != null && Convert.ToBoolean(ViewState["SeciliGrupSistemMi"]); }
            set { ViewState["SeciliGrupSistemMi"] = value; }
        }

        private bool SeciliParametreSistemMi
        {
            get { return ViewState["SeciliParametreSistemMi"] != null && Convert.ToBoolean(ViewState["SeciliParametreSistemMi"]); }
            set { ViewState["SeciliParametreSistemMi"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                return;
            }

            if (Session["ParametreIslemBasariMesaji"] != null)
            {
                Mesaj.Ver(Session["ParametreIslemBasariMesaji"].ToString(), Mesaj.MesajTurleri.SUCCESS, Master);
                Session.Remove("ParametreIslemBasariMesaji");
            }

            GruplariDoldur();
            ParametreBolumunuHazirla(false);
            GrupYeniModunaAl();
        }

        protected void ddlParametreGruplari_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlParametreGruplari.SelectedValue == "0")
            {
                GrupYeniModunaAl();
                return;
            }

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                ParametreGruplari parametreGruplari = new ParametreGruplari(veritabaniIslemleri);
                parametreGruplari.Id = Convert.ToInt32(ddlParametreGruplari.SelectedValue);
                if (!parametreGruplari.Doldur())
                {
                    Mesaj.Ver(Mesajlar.ParametreGrupBulunamadi, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                txtGrupId.Text = parametreGruplari.Id.ToString();
                txtGrupKod.Text = parametreGruplari.Kod;
                txtGrupAd.Text = parametreGruplari.Ad;
                txtGrupAciklama.Text = parametreGruplari.Aciklama;
                SeciliGrupSistemMi = parametreGruplari.SistemMi;
                txtGrupKod.Enabled = !parametreGruplari.SistemMi;

                btnGrupEkle.Visible = false;
                btnGrupGuncelle.Visible = IslemYetki.Kontrol(YetkiIslemTurleri.Guncelleme);
                btnGrupSil.Visible = IslemYetki.Kontrol(YetkiIslemTurleri.Silme) && !parametreGruplari.SistemMi;

                ParametreBolumunuHazirla(true);
                ParametreleriDoldur(veritabaniIslemleri, parametreGruplari.Id);
                ParametreYeniModunaAl();
                MevcutGrupModunaAl();
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

        protected void ddlParametreler_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlParametreler.SelectedValue == "0")
            {
                ParametreYeniModunaAl();
                return;
            }

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Parametreler parametreler = new Parametreler(veritabaniIslemleri);
                parametreler.Id = Convert.ToInt32(ddlParametreler.SelectedValue);
                if (!parametreler.Doldur())
                {
                    Mesaj.Ver(Mesajlar.ParametreBulunamadi, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                txtParametreId.Text = parametreler.Id.ToString();
                txtParametreKod.Text = parametreler.Kod;
                txtParametreAd.Text = parametreler.Ad;
                txtParametreAciklama.Text = parametreler.Aciklama;
                SeciliParametreSistemMi = parametreler.SistemMi;
                txtParametreKod.Enabled = !parametreler.SistemMi;

                btnParametreEkle.Visible = false;
                btnParametreGuncelle.Visible = IslemYetki.Kontrol(YetkiIslemTurleri.Guncelleme);
                btnParametreSil.Visible = IslemYetki.Kontrol(YetkiIslemTurleri.Silme) && !parametreler.SistemMi;
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

        protected void btnGrupEkle_Click(object sender, EventArgs e)
        {
            if (!IslemYetki.Kontrol(YetkiIslemTurleri.Ekleme))
            {
                Mesaj.Ver(Mesajlar.YetkisizIslem, Mesaj.MesajTurleri.WARNING, Master);
                return;
            }
            if (!GrupAlanlariUygunMu())
            {
                return;
            }

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                ParametreGruplari parametreGruplari = new ParametreGruplari(veritabaniIslemleri);
                if (GrupKodKullanimdaMi(veritabaniIslemleri, txtGrupKod.Text.Trim(), null))
                {
                    Mesaj.Ver(Mesajlar.ParametreGrupKodZatenVar, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }

                GrupFormdanDoldur(parametreGruplari);
                parametreGruplari.SistemMi = false;
                parametreGruplari.EkleyenId = currentInfo.KullaniciId;
                parametreGruplari.EkleyenIp = currentInfo.Ip;

                if (parametreGruplari.Ekle())
                {
                    Session["ParametreIslemBasariMesaji"] = Mesajlar.ParametreGrupBasariylaEklendi;
                    Response.Redirect(Request.RawUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                Mesaj.Ver(Mesajlar.ParametreGrupEklenemedi, Mesaj.MesajTurleri.FAIL, Master);
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

        protected void btnGrupGuncelle_Click(object sender, EventArgs e)
        {
            if (!IslemYetki.Kontrol(YetkiIslemTurleri.Guncelleme))
            {
                Mesaj.Ver(Mesajlar.YetkisizIslem, Mesaj.MesajTurleri.WARNING, Master);
                return;
            }
            if (!GrupAlanlariUygunMu())
            {
                return;
            }

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                ParametreGruplari parametreGruplari = new ParametreGruplari(veritabaniIslemleri);
                parametreGruplari.Id = Convert.ToInt32(txtGrupId.Text.Trim());
                if (!parametreGruplari.Doldur())
                {
                    Mesaj.Ver(Mesajlar.ParametreGrupBulunamadi, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                string yeniKod = parametreGruplari.SistemMi ? parametreGruplari.Kod : txtGrupKod.Text.Trim();
                if (!parametreGruplari.SistemMi && GrupKodKullanimdaMi(veritabaniIslemleri, yeniKod, parametreGruplari.Id))
                {
                    Mesaj.Ver(Mesajlar.ParametreGrupKodZatenVar, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }

                parametreGruplari.Kod = yeniKod;
                parametreGruplari.Ad = txtGrupAd.Text.Trim();
                parametreGruplari.Aciklama = txtGrupAciklama.Text.Trim();
                parametreGruplari.GuncelleyenId = currentInfo.KullaniciId;
                parametreGruplari.GuncelleyenIp = currentInfo.Ip;

                if (parametreGruplari.Guncelle())
                {
                    Session["ParametreIslemBasariMesaji"] = Mesajlar.ParametreGrupBasariylaGuncellendi;
                    Response.Redirect(Request.RawUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                Mesaj.Ver(Mesajlar.ParametreGrupGuncellenemedi, Mesaj.MesajTurleri.FAIL, Master);
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

        protected void btnGrupSil_Click(object sender, EventArgs e)
        {
            if (!IslemYetki.Kontrol(YetkiIslemTurleri.Silme))
            {
                Mesaj.Ver(Mesajlar.YetkisizIslem, Mesaj.MesajTurleri.WARNING, Master);
                return;
            }

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                ParametreGruplari parametreGruplari = new ParametreGruplari(veritabaniIslemleri);
                parametreGruplari.Id = Convert.ToInt32(txtGrupId.Text.Trim());
                if (!parametreGruplari.Doldur())
                {
                    Mesaj.Ver(Mesajlar.ParametreGrupBulunamadi, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                if (parametreGruplari.SistemMi)
                {
                    Mesaj.Ver(Mesajlar.ParametreGrupSistemKaydiSilinemez, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }

                Parametreler parametreler = new Parametreler(veritabaniIslemleri);
                DataTable aktifParametreler = parametreler.GrubaGoreGetir(parametreGruplari.Id);
                if (aktifParametreler != null && aktifParametreler.Rows.Count > 0)
                {
                    Mesaj.Ver(Mesajlar.ParametreGrupAktifParametreVar, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }

                parametreGruplari.GuncelleyenId = currentInfo.KullaniciId;
                parametreGruplari.GuncelleyenIp = currentInfo.Ip;
                if (parametreGruplari.Sil())
                {
                    Session["ParametreIslemBasariMesaji"] = Mesajlar.ParametreGrupBasariylaSilindi;
                    Response.Redirect(Request.RawUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                Mesaj.Ver(Mesajlar.ParametreGrupSilinemedi, Mesaj.MesajTurleri.FAIL, Master);
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

        protected void btnParametreEkle_Click(object sender, EventArgs e)
        {
            if (!IslemYetki.Kontrol(YetkiIslemTurleri.Ekleme))
            {
                Mesaj.Ver(Mesajlar.YetkisizIslem, Mesaj.MesajTurleri.WARNING, Master);
                return;
            }

            int grupId;
            if (!int.TryParse(ddlParametreGruplari.SelectedValue, out grupId) || grupId <= 0)
            {
                Mesaj.Ver(Mesajlar.ParametreGrupSecilmedi, Mesaj.MesajTurleri.WARNING, Master);
                return;
            }
            if (!ParametreAlanlariUygunMu())
            {
                return;
            }

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Parametreler parametreler = new Parametreler(veritabaniIslemleri);
                if (parametreler.KodKullanimdaMi(grupId, txtParametreKod.Text.Trim(), null))
                {
                    Mesaj.Ver(Mesajlar.ParametreKodZatenVar, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }

                ParametreFormdanDoldur(parametreler, grupId);
                parametreler.SistemMi = false;
                parametreler.EkleyenId = currentInfo.KullaniciId;
                parametreler.EkleyenIp = currentInfo.Ip;

                if (parametreler.Ekle())
                {
                    Session["ParametreIslemBasariMesaji"] = Mesajlar.ParametreBasariylaEklendi;
                    Response.Redirect(Request.RawUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                Mesaj.Ver(Mesajlar.ParametreEklenemedi, Mesaj.MesajTurleri.FAIL, Master);
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

        protected void btnParametreGuncelle_Click(object sender, EventArgs e)
        {
            if (!IslemYetki.Kontrol(YetkiIslemTurleri.Guncelleme))
            {
                Mesaj.Ver(Mesajlar.YetkisizIslem, Mesaj.MesajTurleri.WARNING, Master);
                return;
            }
            if (!ParametreAlanlariUygunMu())
            {
                return;
            }

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Parametreler parametreler = new Parametreler(veritabaniIslemleri);
                parametreler.Id = Convert.ToInt32(txtParametreId.Text.Trim());
                if (!parametreler.Doldur())
                {
                    Mesaj.Ver(Mesajlar.ParametreBulunamadi, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                string yeniKod = parametreler.SistemMi ? parametreler.Kod : txtParametreKod.Text.Trim();
                if (!parametreler.SistemMi && parametreler.KodKullanimdaMi(parametreler.ParametreGrupId, yeniKod, parametreler.Id))
                {
                    Mesaj.Ver(Mesajlar.ParametreKodZatenVar, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }

                parametreler.Kod = yeniKod;
                parametreler.Ad = txtParametreAd.Text.Trim();
                parametreler.Aciklama = txtParametreAciklama.Text.Trim();
                parametreler.GuncelleyenId = currentInfo.KullaniciId;
                parametreler.GuncelleyenIp = currentInfo.Ip;

                if (parametreler.Guncelle())
                {
                    Session["ParametreIslemBasariMesaji"] = Mesajlar.ParametreBasariylaGuncellendi;
                    Response.Redirect(Request.RawUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                Mesaj.Ver(Mesajlar.ParametreGuncellenemedi, Mesaj.MesajTurleri.FAIL, Master);
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

        protected void btnParametreSil_Click(object sender, EventArgs e)
        {
            if (!IslemYetki.Kontrol(YetkiIslemTurleri.Silme))
            {
                Mesaj.Ver(Mesajlar.YetkisizIslem, Mesaj.MesajTurleri.WARNING, Master);
                return;
            }

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Parametreler parametreler = new Parametreler(veritabaniIslemleri);
                parametreler.Id = Convert.ToInt32(txtParametreId.Text.Trim());
                if (!parametreler.Doldur())
                {
                    Mesaj.Ver(Mesajlar.ParametreBulunamadi, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                if (parametreler.SistemMi)
                {
                    Mesaj.Ver(Mesajlar.ParametreSistemKaydiSilinemez, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }

                parametreler.GuncelleyenId = currentInfo.KullaniciId;
                parametreler.GuncelleyenIp = currentInfo.Ip;
                if (parametreler.Sil())
                {
                    Session["ParametreIslemBasariMesaji"] = Mesajlar.ParametreBasariylaSilindi;
                    Response.Redirect(Request.RawUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                Mesaj.Ver(Mesajlar.ParametreSilinemedi, Mesaj.MesajTurleri.FAIL, Master);
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

        private void GruplariDoldur()
        {
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                ParametreGruplari parametreGruplari = new ParametreGruplari(veritabaniIslemleri);
                parametreGruplari.Listele(ddlParametreGruplari);
                ddlParametreGruplari.Items.Insert(0, new ListItem("Yeni Grup", "0"));
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

        private void ParametreleriDoldur(VeritabaniIslemleri veritabaniIslemleri, int grupId)
        {
            Parametreler parametreler = new Parametreler(veritabaniIslemleri);
            parametreler.GrubaGoreListele(ddlParametreler, grupId);
            ddlParametreler.Items.Insert(0, new ListItem("Yeni Parametre", "0"));
        }

        private void GrupYeniModunaAl()
        {
            SeciliGrupSistemMi = false;
            txtGrupId.Text = string.Empty;
            txtGrupKod.Text = string.Empty;
            txtGrupAd.Text = string.Empty;
            txtGrupAciklama.Text = string.Empty;
            txtGrupKod.Enabled = true;

            btnGrupEkle.Visible = IslemYetki.Kontrol(YetkiIslemTurleri.Ekleme);
            btnGrupGuncelle.Visible = false;
            btnGrupSil.Visible = false;

            pnlGrupAlanlari.Visible = true;
            pnlGrupAlanlari.CssClass = string.Empty;
            ParametreBolumunuHazirla(false);
        }

        private void ParametreYeniModunaAl()
        {
            SeciliParametreSistemMi = false;
            txtParametreId.Text = string.Empty;
            txtParametreKod.Text = string.Empty;
            txtParametreAd.Text = string.Empty;
            txtParametreAciklama.Text = string.Empty;
            txtParametreKod.Enabled = true;

            btnParametreEkle.Visible = IslemYetki.Kontrol(YetkiIslemTurleri.Ekleme);
            btnParametreGuncelle.Visible = false;
            btnParametreSil.Visible = false;

            if (ddlParametreler.Items.Count > 0)
            {
                ddlParametreler.SelectedValue = "0";
            }
        }

        private void ParametreBolumunuHazirla(bool aktif)
        {
            pnlParametreBolumu.Visible = aktif;
            if (!aktif)
            {
                ddlParametreler.Items.Clear();
                ddlParametreler.Items.Add(new ListItem("Yeni Parametre", "0"));
                ParametreYeniModunaAl();
            }
        }

        private void MevcutGrupModunaAl()
        {
            pnlGrupAlanlari.Visible = true;
            pnlGrupAlanlari.CssClass = "bolum-ayirici";
            btnGrupEkle.Visible = false;
        }

        private void GrupFormdanDoldur(ParametreGruplari parametreGruplari)
        {
            parametreGruplari.Kod = txtGrupKod.Text.Trim();
            parametreGruplari.Ad = txtGrupAd.Text.Trim();
            parametreGruplari.Aciklama = txtGrupAciklama.Text.Trim();
        }

        private void ParametreFormdanDoldur(Parametreler parametreler, int grupId)
        {
            parametreler.ParametreGrupId = grupId;
            parametreler.Kod = txtParametreKod.Text.Trim();
            parametreler.Ad = txtParametreAd.Text.Trim();
            parametreler.Aciklama = txtParametreAciklama.Text.Trim();
        }

        private bool GrupAlanlariUygunMu()
        {
            if (string.IsNullOrWhiteSpace(txtGrupKod.Text) || string.IsNullOrWhiteSpace(txtGrupAd.Text))
            {
                Mesaj.Ver(Mesajlar.ParametreGrupAlanlariBos, Mesaj.MesajTurleri.WARNING, Master);
                return false;
            }

            return true;
        }

        private bool ParametreAlanlariUygunMu()
        {
            if (string.IsNullOrWhiteSpace(txtParametreKod.Text) || string.IsNullOrWhiteSpace(txtParametreAd.Text))
            {
                Mesaj.Ver(Mesajlar.ParametreAlanlariBos, Mesaj.MesajTurleri.WARNING, Master);
                return false;
            }

            return true;
        }

        private bool GrupKodKullanimdaMi(VeritabaniIslemleri veritabaniIslemleri, string kod, int? haricId)
        {
            ParametreGruplari parametreGruplari = new ParametreGruplari(veritabaniIslemleri);
            DataTable gruplar = parametreGruplari.TumunuGetir();
            if (gruplar == null)
            {
                return false;
            }

            foreach (DataRow satir in gruplar.Rows)
            {
                if (satir[ParametreGruplari.C_Sutun_kod] == DBNull.Value)
                {
                    continue;
                }

                if (!string.Equals(satir[ParametreGruplari.C_Sutun_kod].ToString(), kod, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                int id = Convert.ToInt32(satir[ParametreGruplari.C_Sutun_id]);
                if (haricId.HasValue && haricId.Value == id)
                {
                    continue;
                }

                return true;
            }

            return false;
        }
    }
}
