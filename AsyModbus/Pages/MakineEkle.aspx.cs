using System;
using System.Data;
using System.Web.UI.WebControls;

namespace AsyModbus.Pages
{
    public partial class MakineEkle : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
                try
                {
                    veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                    RoleCihazlar roleCihazlar = new RoleCihazlar(veritabaniIslemleri);
                    RoleCihazlariniDoldur(roleCihazlar);
                    RoleKanallariniTemizle();
                }
                catch (Exception ex)
                {
                    Mesaj.Ver(Mesajlar.SistemselHata(ex.Message), Mesaj.MesajTurleri.FAIL, Master);
                }
                finally
                {
                    veritabaniIslemleri.Bitir();
                }

                btnKaydet.Visible = IslemYetki.Kontrol(YetkiIslemTurleri.Ekleme);
            }
        }

        protected void ddlRoleCihaz_SelectedIndexChanged(object sender, EventArgs e)
        {
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                RoleKanallariniDoldur(veritabaniIslemleri, 0, null);
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
                makineler.RoleCihazlarId = SeciliRoleCihazlarIdGetir();
                makineler.RoleKanalNo = makineler.RoleCihazlarId.HasValue ? SeciliRoleKanalNoGetir() : null;
                makineler.SiraNo = makineler.MaxSiraGetir() + 1;
                makineler.AktifMi = true;
                makineler.EkleyenId = currentInfo.KullaniciId;
                makineler.EkleyenIp = currentInfo.Ip;

                if (makineler.KanalKullanimdaMi(makineler.RoleCihazlarId, makineler.RoleKanalNo, null))
                {
                    Mesaj.Ver(Mesajlar.RoleKanalKullanimda, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }

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
                return false;
            }

            if (SeciliRoleCihazlarIdGetir().HasValue && !SeciliRoleKanalNoGetir().HasValue)
            {
                Mesaj.Ver(Mesajlar.RoleKanalSecilmedi, Mesaj.MesajTurleri.WARNING, Master);
                return false;
            }

            return sonuc;
        }

        private void RoleCihazlariniDoldur(RoleCihazlar roleCihazlar)
        {
            DataTable tablo = roleCihazlar.TumunuGetir();
            if (tablo == null)
            {
                throw new Exception("Role cihaz kayıtları getirilemedi.");
            }

            ddlRoleCihaz.Items.Clear();
            ddlRoleCihaz.Items.Add(new ListItem("Seçiniz", "0"));

            foreach (DataRow satir in tablo.Rows)
            {
                string ad = satir[RoleCihazlar.C_Sutun_ad] == DBNull.Value
                    ? string.Empty
                    : satir[RoleCihazlar.C_Sutun_ad].ToString();
                string ip = satir[RoleCihazlar.C_Sutun_ip] == DBNull.Value
                    ? string.Empty
                    : satir[RoleCihazlar.C_Sutun_ip].ToString();
                ddlRoleCihaz.Items.Add(new ListItem(ad + " - " + ip, satir[RoleCihazlar.C_Sutun_id].ToString()));
            }
        }

        private void RoleKanallariniDoldur(VeritabaniIslemleri veritabaniIslemleri, int makineId, int? seciliKanalNo)
        {
            int? roleCihazlarId = SeciliRoleCihazlarIdGetir();
            if (!roleCihazlarId.HasValue)
            {
                RoleKanallariniTemizle();
                return;
            }

            RoleCihazlar roleCihazlar = new RoleCihazlar(veritabaniIslemleri);
            roleCihazlar.Id = roleCihazlarId.Value;
            if (!roleCihazlar.Doldur() || !roleCihazlar.KanalSayisi.HasValue)
            {
                RoleKanallariniTemizle();
                return;
            }

            Makineler makineler = new Makineler(veritabaniIslemleri);
            makineler.Id = makineId;
            makineler.RoleCihazlarId = roleCihazlarId;
            makineler.BosKanallariListele(ddlRoleKanal, roleCihazlar.KanalSayisi.Value);

            if (seciliKanalNo.HasValue)
            {
                ListItem kanalItem = ddlRoleKanal.Items.FindByValue(seciliKanalNo.Value.ToString());
                if (kanalItem != null)
                {
                    ddlRoleKanal.SelectedValue = kanalItem.Value;
                }
            }

            if (ddlRoleKanal.Items.Count <= 1)
            {
                Mesaj.Ver(Mesajlar.RoleBosKanalKalmadi, Mesaj.MesajTurleri.WARNING, Master);
            }
        }

        private void RoleKanallariniTemizle()
        {
            ddlRoleKanal.Items.Clear();
            ddlRoleKanal.Items.Add(new ListItem("Seçiniz", "0"));
        }

        private int? SeciliRoleCihazlarIdGetir()
        {
            int deger;
            if (!int.TryParse(ddlRoleCihaz.SelectedValue, out deger) || deger <= 0)
            {
                return null;
            }

            return deger;
        }

        private int? SeciliRoleKanalNoGetir()
        {
            int deger;
            if (!int.TryParse(ddlRoleKanal.SelectedValue, out deger) || deger <= 0)
            {
                return null;
            }

            return deger;
        }
    }
}
