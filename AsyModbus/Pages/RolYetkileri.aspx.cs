using System;
using System.Data;
using System.Web.UI.WebControls;

namespace AsyModbus.Pages
{
    public partial class RolYetkileri : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["RolYetkiBasariMesaji"] != null)
            {
                Mesaj.Ver(Session["RolYetkiBasariMesaji"].ToString(), Mesaj.MesajTurleri.SUCCESS, Master);
                Session.Remove("RolYetkiBasariMesaji");
            }
            if (!Page.IsPostBack)
            {
                VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
                try
                {
                    //Rol Listele
                    veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                    Roller rol = new Roller(veritabaniIslemleri);
                    rol.Listele(ddlRoller);
                    ddlRoller.Items.Insert(0, new ListItem("Rol Seçiniz...", "0"));
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
            if (ddlRoller.SelectedValue == "0")
            {
                rptYetkiler.DataSource = null;
                rptYetkiler.DataBind();
                btnKaydet.Visible = false;
                return;
            }

            int rolId = Convert.ToInt32(ddlRoller.SelectedValue);
            YetkileriDoldur(rolId);
        }

        private void YetkileriDoldur(int rolId)
        {
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                RolYetkiler rolYetkiler = new RolYetkiler(veritabaniIslemleri);
                DataTable dataTable = rolYetkiler.TumunuGetir();

                DataView dataView = new DataView(dataTable);
                dataView.RowFilter = RolYetkiler.C_Sutun_roller_id + " = " + rolId;

                rptYetkiler.DataSource = dataView;
                rptYetkiler.DataBind();

                btnKaydet.Visible = IslemYetki.Kontrol(YetkiIslemTurleri.Guncelleme);
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

        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            if (!IslemYetki.Kontrol(YetkiIslemTurleri.Guncelleme))
            {
                Mesaj.Ver(Mesajlar.YetkisizIslem, Mesaj.MesajTurleri.WARNING, Master);
                return;
            }

            if (ddlRoller.SelectedValue == "0")
            {
                Mesaj.Ver(Mesajlar.RolSeciniz, Mesaj.MesajTurleri.WARNING, Master);
                return;
            }

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;

            bool basariliMi = true;
            int rolId = Convert.ToInt32(ddlRoller.SelectedValue);

            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);

                foreach (RepeaterItem repeaterItem in rptYetkiler.Items)
                {
                    HiddenField hfYetkiId = (HiddenField)repeaterItem.FindControl("hfYetkiId");
                    HiddenField hfSayfaAdi = (HiddenField)repeaterItem.FindControl("hfSayfaAdi");
                    CheckBox chkGetirme = (CheckBox)repeaterItem.FindControl("chkGetirme");
                    CheckBox chkEkleme = (CheckBox)repeaterItem.FindControl("chkEkleme");
                    CheckBox chkGuncelleme = (CheckBox)repeaterItem.FindControl("chkGuncelleme");
                    CheckBox chkSilme = (CheckBox)repeaterItem.FindControl("chkSilme");

                    RolYetkiler rolYetkiler = new RolYetkiler(veritabaniIslemleri);
                    rolYetkiler.Id = Convert.ToInt32(hfYetkiId.Value);
                    rolYetkiler.RollerId = rolId;
                    rolYetkiler.SayfaAdi = hfSayfaAdi.Value;
                    rolYetkiler.Getirme = chkGetirme.Checked;
                    rolYetkiler.Ekleme = chkEkleme.Checked;
                    rolYetkiler.Guncelleme = chkGuncelleme.Checked;
                    rolYetkiler.Silme = chkSilme.Checked;
                    rolYetkiler.AktifMi = true;
                    rolYetkiler.GuncelleyenId = currentInfo.KullaniciId;
                    rolYetkiler.GuncelleyenIp = currentInfo.Ip;

                    if (!rolYetkiler.Guncelle())
                    {
                        basariliMi = false;
                        break;
                    }
                }

                if (basariliMi)
                {
                    veritabaniIslemleri.Uygula();
                    Session["RolYetkiBasariMesaji"] = Mesajlar.RolYetkileriGuncellendi;
                    Response.Redirect(Request.RawUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
                else
                {
                    veritabaniIslemleri.GeriAl();
                    Mesaj.Ver(Mesajlar.RolYetkileriGuncellenemedi, Mesaj.MesajTurleri.FAIL, Master);
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