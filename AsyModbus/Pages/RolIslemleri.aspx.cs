using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AsyModbus.Pages
{
    public partial class RolIslemleri : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {

                if (Session["RolBasariMesaji"] != null)
                {
                    Mesaj.Ver(Session["RolBasariMesaji"].ToString(), Mesaj.MesajTurleri.SUCCESS, Master);
                    Session.Remove("RolBasariMesaji");
                }

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

                btnEkle.Visible = IslemYetki.Kontrol(YetkiIslemTurleri.Ekleme);
                btnGuncelle.Visible = false;
                btnSil.Visible = false;
            }
        }

        protected void ddlRoller_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlRoller.SelectedIndex == 0)
            {
                txtId.Text = string.Empty;
                txtAd.Text = string.Empty;

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
            if (!IslemYetki.Kontrol(YetkiIslemTurleri.Ekleme))
            {
                Mesaj.Ver(Mesajlar.YetkisizIslem, Mesaj.MesajTurleri.WARNING, Master);
                return;
            }
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
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
                Roller roller = new Roller(veritabaniIslemleri);
                roller.Ad = txtAd.Text.Trim();
                roller.EkleyenId = currentInfo.KullaniciId;
                roller.EkleyenIp = currentInfo.Ip;
                if (roller.Ekle())
                {
                    int yeniRolId = roller.MaxIdGetir();
                    foreach (string sayfaAdi in Sayfalar.YetkilendirilenSayfalar)
                    {
                        RolYetkiler rolYetkiler = new RolYetkiler(veritabaniIslemleri);
                        rolYetkiler.RollerId = yeniRolId;
                        rolYetkiler.SayfaAdi = sayfaAdi;
                        rolYetkiler.Getirme = false;
                        rolYetkiler.Ekleme = false;
                        rolYetkiler.Guncelleme = false;
                        rolYetkiler.Silme = false;
                        rolYetkiler.AktifMi = true;
                        rolYetkiler.EkleyenId = currentInfo.KullaniciId;
                        rolYetkiler.EkleyenIp = currentInfo.Ip;

                        if (!rolYetkiler.Ekle())
                        {
                            veritabaniIslemleri.GeriAl();
                            Mesaj.Ver(Mesajlar.RolEklenemedi, Mesaj.MesajTurleri.FAIL, Master);
                            return;
                        }
                    }

                    veritabaniIslemleri.Uygula();
                    Session["RolBasariMesaji"] = Mesajlar.RolEklendi;
                    Response.Redirect(Request.RawUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;

                }
                else
                {
                    veritabaniIslemleri.GeriAl();
                    Mesaj.Ver(Mesajlar.RolEklenemedi, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }
            }
            catch (Exception ex)
            {
                veritabaniIslemleri.GeriAl();
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
            if (!IslemYetki.Kontrol(YetkiIslemTurleri.Silme))
            {
                Mesaj.Ver(Mesajlar.YetkisizIslem, Mesaj.MesajTurleri.WARNING, Master);
                return;
            }
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;

            int rolId = Convert.ToInt32(txtId.Text.Trim());
            if (rolId == 1)
            {
                Mesaj.Ver(Mesajlar.SuperAdminRoluSilinemez, Mesaj.MesajTurleri.WARNING, Master);
                return;
            }
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
                Roller roller = new Roller(veritabaniIslemleri);
                roller.Id = rolId;
                roller.GuncelleyenId = currentInfo.KullaniciId;
                roller.GuncelleyenIp = currentInfo.Ip;
                if (roller.Sil())
                {
                    RolYetkiler rolYetkiler = new RolYetkiler(veritabaniIslemleri);
                    DataTable dataTable = rolYetkiler.TumunuGetir();
                    DataView dataView = new DataView(dataTable);
                    dataView.RowFilter = RolYetkiler.C_Sutun_roller_id + " = " + rolId;

                    foreach (DataRowView dataRowView in dataView)
                    {
                        RolYetkiler rolYetkiler1 = new RolYetkiler(veritabaniIslemleri);
                        rolYetkiler1.Id = Convert.ToInt32(dataRowView[RolYetkiler.C_Sutun_id]);

                        if (!rolYetkiler1.Sil())
                        {
                            veritabaniIslemleri.GeriAl();
                            Mesaj.Ver(Mesajlar.RolSilinemedi, Mesaj.MesajTurleri.FAIL, Master);
                            return;
                        }
                    }
                    veritabaniIslemleri.Uygula();
                    Session["RolBasariMesaji"] = Mesajlar.RolSilindi;
                    Response.Redirect(Request.RawUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
                else
                {
                    veritabaniIslemleri.GeriAl();
                    Mesaj.Ver(Mesajlar.RolSilinemedi, Mesaj.MesajTurleri.FAIL, Master);
                }
            }
            catch (Exception ex)
            {
                veritabaniIslemleri.GeriAl();
                Mesaj.Ver(Mesajlar.SistemselHata(ex.Message), Mesaj.MesajTurleri.FAIL, Master);
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }

    }
}