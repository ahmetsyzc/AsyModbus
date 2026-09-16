using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AsyModbus.Pages
{
    public partial class RoleCihazIslemleri : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                if (Session["RoleCihazBasariMesaji"] != null)
                {
                    Mesaj.Ver(Session["RoleCihazBasariMesaji"].ToString(), Mesaj.MesajTurleri.SUCCESS, Master);
                    Session.Remove("RoleCihazBasariMesaji");
                }

                VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
                try
                {
                    veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                    RoleCihazlar roleCihazlar = new RoleCihazlar(veritabaniIslemleri);
                    roleCihazlar.Listele(ddlCihazlar);
                    ddlCihazlar.Items.Insert(0, new ListItem("Yeni Cihaz", "0"));
                }
                catch (Exception ex)
                {
                    Mesaj.Ver(Mesajlar.SistemselHata(ex.Message), Mesaj.MesajTurleri.FAIL, Master);
                }
                finally
                {
                    veritabaniIslemleri.Bitir();
                }

                btnEkle.Visible = IslemYetki.Kontrol(YetkiIslemTurleri.Ekleme);
                btnGuncelle.Visible = false;
                btnSil.Visible = false;
            }
        }

        protected void ddlCihazlar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCihazlar.SelectedIndex == 0)
            {
                FormuTemizle();
                btnEkle.Visible = IslemYetki.Kontrol(YetkiIslemTurleri.Ekleme);
                btnGuncelle.Visible = false;
                btnSil.Visible = false;
            }
            else
            {
                btnEkle.Visible = false;
                btnGuncelle.Visible = IslemYetki.Kontrol(YetkiIslemTurleri.Guncelleme);
                btnSil.Visible = IslemYetki.Kontrol(YetkiIslemTurleri.Silme);

                VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
                try
                {
                    veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                    RoleCihazlar roleCihazlar = new RoleCihazlar(veritabaniIslemleri);
                    roleCihazlar.Id = Convert.ToInt32(ddlCihazlar.SelectedValue);
                    if (roleCihazlar.Doldur())
                    {
                        txtId.Text = roleCihazlar.Id.ToString();
                        txtAd.Text = roleCihazlar.Ad;
                        txtIp.Text = roleCihazlar.Ip;
                        txtPort.Text = roleCihazlar.Port.HasValue ? roleCihazlar.Port.Value.ToString() : string.Empty;
                        txtKanalSayisi.Text = roleCihazlar.KanalSayisi.HasValue ? roleCihazlar.KanalSayisi.Value.ToString() : string.Empty;
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

        protected void btnEkle_Click(object sender, EventArgs e)
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
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                RoleCihazlar roleCihazlar = new RoleCihazlar(veritabaniIslemleri);
                FormdanDoldur(roleCihazlar);
                roleCihazlar.EkleyenId = currentInfo.KullaniciId;
                roleCihazlar.EkleyenIp = currentInfo.Ip;

                if (roleCihazlar.Ekle())
                {
                    Session["RoleCihazBasariMesaji"] = Mesajlar.RoleCihazEklendi;
                    Response.Redirect(Request.RawUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                Mesaj.Ver(Mesajlar.RoleCihazEklenemedi, Mesaj.MesajTurleri.FAIL, Master);
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

        protected void btnGuncelle_Click(object sender, EventArgs e)
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

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                RoleCihazlar roleCihazlar = new RoleCihazlar(veritabaniIslemleri);
                roleCihazlar.Id = Convert.ToInt32(txtId.Text.Trim());
                FormdanDoldur(roleCihazlar);
                roleCihazlar.GuncelleyenId = currentInfo.KullaniciId;
                roleCihazlar.GuncelleyenIp = currentInfo.Ip;

                if (roleCihazlar.Guncelle())
                {
                    Session["RoleCihazBasariMesaji"] = Mesajlar.RoleCihazGuncellendi;
                    Response.Redirect(Request.RawUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                Mesaj.Ver(Mesajlar.RoleCihazGuncellenemedi, Mesaj.MesajTurleri.FAIL, Master);
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
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                RoleCihazlar roleCihazlar = new RoleCihazlar(veritabaniIslemleri);
                roleCihazlar.Id = Convert.ToInt32(txtId.Text.Trim());
                roleCihazlar.GuncelleyenId = currentInfo.KullaniciId;
                roleCihazlar.GuncelleyenIp = currentInfo.Ip;

                if (roleCihazlar.Sil())
                {
                    Session["RoleCihazBasariMesaji"] = Mesajlar.RoleCihazSilindi;
                    Response.Redirect(Request.RawUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                Mesaj.Ver(Mesajlar.RoleCihazSilinemedi, Mesaj.MesajTurleri.FAIL, Master);
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

        private void FormuTemizle()
        {
            txtId.Text = string.Empty;
            txtAd.Text = string.Empty;
            txtIp.Text = string.Empty;
            txtPort.Text = string.Empty;
            txtKanalSayisi.Text = string.Empty;
        }

        private void FormdanDoldur(RoleCihazlar roleCihazlar)
        {
            roleCihazlar.Ad = txtAd.Text.Trim();
            roleCihazlar.Ip = txtIp.Text.Trim();
            roleCihazlar.Port = Convert.ToInt32(txtPort.Text.Trim());
            roleCihazlar.KanalSayisi = Convert.ToInt32(txtKanalSayisi.Text.Trim());
        }

        private bool AlanlarUygunMu()
        {
            if (string.IsNullOrWhiteSpace(txtAd.Text))
            {
                Mesaj.Ver(Mesajlar.RoleCihazAdiBos, Mesaj.MesajTurleri.WARNING, Master);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtIp.Text))
            {
                Mesaj.Ver(Mesajlar.RoleCihazIpBos, Mesaj.MesajTurleri.WARNING, Master);
                return false;
            }

            int port;
            if (!int.TryParse(txtPort.Text.Trim(), out port))
            {
                Mesaj.Ver(Mesajlar.RoleCihazPortGecersiz, Mesaj.MesajTurleri.WARNING, Master);
                return false;
            }

            int kanalSayisi;
            if (!int.TryParse(txtKanalSayisi.Text.Trim(), out kanalSayisi))
            {
                Mesaj.Ver(Mesajlar.RoleCihazKanalSayisiGecersiz, Mesaj.MesajTurleri.WARNING, Master);
                return false;
            }

            return true;
        }
    }
}
