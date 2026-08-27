using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AsyModbus.Pages
{
    public partial class RolIslemleri : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["RolBasariMesaji"] != null)
            {
                Mesaj.Ver(Session["RolBasariMesaji"].ToString(),Mesaj.MesajTurleri.SUCCESS,Master);
                Session.Remove("RolBasariMesaji");
            }

            if (Page.IsPostBack == false)
            {
                VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
                try
                {
                    //Rol Listele
                    veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                    Roller roller = new Roller(veritabaniIslemleri);
                    roller.Listele(ddlRoller);
                    ddlRoller.Items.Insert(0, new ListItem("Yeni Rol", "0"));
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

        protected void ddlRoller_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlRoller.SelectedIndex == 0)
            {
                txtId.Text = string.Empty;
                txtAd.Text = string.Empty;

                btnEkle.Visible = true;
                btnGuncelle.Visible = false;
                btnSil.Visible = false;
            }
            else
            {
                btnEkle.Visible = false;
                btnGuncelle.Visible = true;
                btnSil.Visible = true;

                VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
                try
                {
                    veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                    Roller roller = new Roller(veritabaniIslemleri);
                    roller.Id = Convert.ToInt32(ddlRoller.SelectedValue);
                    if (roller.Doldur())
                    {
                        txtId.Text = roller.Id.ToString();
                        txtAd.Text = roller.Ad.ToString();
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
            if (string.IsNullOrWhiteSpace(txtAd.Text))
            {
                Mesaj.Ver(Mesajlar.RolAdiBos, Mesaj.MesajTurleri.WARNING, Master);
                return;
            }

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Roller roller = new Roller(veritabaniIslemleri);
                roller.Ad = txtAd.Text.Trim();
                roller.EkleyenId = currentInfo.KullaniciId;
                roller.EkleyenIp = currentInfo.Ip;
                if (roller.Ekle())
                {
                    Session["RolBasariMesaji"] = Mesajlar.RolEklendi;
                    Response.Redirect(Request.RawUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
                else
                {
                    Mesaj.Ver(Mesajlar.RolEklenemedi, Mesaj.MesajTurleri.FAIL, Master);
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

        protected void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAd.Text))
            {
                Mesaj.Ver(Mesajlar.RolAdiBos, Mesaj.MesajTurleri.WARNING, Master);
                return;
            }

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Roller roller = new Roller(veritabaniIslemleri);
                roller.Id = Convert.ToInt32(txtId.Text.Trim());
                roller.Ad = txtAd.Text.Trim();
                roller.GuncelleyenId = currentInfo.KullaniciId;
                roller.GuncelleyenIp = currentInfo.Ip;
                if (roller.Guncelle())
                {
                    Session["RolBasariMesaji"] = Mesajlar.RolGuncellendi;
                    Response.Redirect(Request.RawUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
                else
                {
                    Mesaj.Ver(Mesajlar.RolGuncellenemedi, Mesaj.MesajTurleri.FAIL, Master);
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
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Roller roller = new Roller(veritabaniIslemleri);
                roller.Id = Convert.ToInt32(txtId.Text.Trim());
                roller.GuncelleyenId = currentInfo.KullaniciId;
                roller.GuncelleyenIp = currentInfo.Ip;
                if (roller.Sil())
                {
                    Session["RolBasariMesaji"] = Mesajlar.RolSilindi;
                    Response.Redirect(Request.RawUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
                else
                {
                    Mesaj.Ver(Mesajlar.RolSilinemedi, Mesaj.MesajTurleri.FAIL, Master);
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
}