using System;
using System.Data;
using System.Web.UI.WebControls;

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

                    RoleCihazlar roleCihazlar = new RoleCihazlar(veritabaniIslemleri);
                    RoleCihazlariniDoldur(roleCihazlar);

                    if (makineler.Doldur())
                    {
                        txtID.Text = makineler.Id.ToString();
                        txtMakineAd.Text = makineler.MakineAd;
                        txtModelAd.Text = makineler.ModelAd;
                        txtEntegrasyonKod.Text = makineler.EntegrasyonKod;
                        txtGgNo.Text = makineler.GgNo;
                        txtMakineNo.Text = makineler.MakineNo;
                        txtBandNo.Text = makineler.BandNo;
                        txtIp.Text = makineler.Ip;
                        txtMfg.Text = makineler.Mfg;

                        if (makineler.RoleCihazlarId.HasValue)
                        {
                            ListItem cihazItem = ddlRoleCihaz.Items.FindByValue(makineler.RoleCihazlarId.Value.ToString());
                            if (cihazItem != null)
                            {
                                ddlRoleCihaz.SelectedValue = cihazItem.Value;
                            }
                            RoleKanallariniDoldur(veritabaniIslemleri, makineler.Id, makineler.RoleKanalNo);
                        }
                        else
                        {
                            RoleKanallariniTemizle();
                        }
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

        protected void ddlRoleCihaz_SelectedIndexChanged(object sender, EventArgs e)
        {
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                int makineId;
                if (!int.TryParse(txtID.Text.Trim(), out makineId))
                {
                    makineId = 0;
                }
                RoleKanallariniDoldur(veritabaniIslemleri, makineId, null);
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
            if (!AlanlarUygunMu())
            {
                return;
            }

            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
                Makineler makineler = new Makineler(veritabaniIslemleri);
                makineler.Id = Convert.ToInt32(txtID.Text.Trim());

                if (!makineler.Doldur())
                {
                    veritabaniIslemleri.GeriAl();
                    Mesaj.Ver(Mesajlar.MakineBulunamadi, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                string eskiBand = makineler.BandNo;
                int? eskiSira = makineler.SiraNo;

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
                makineler.GuncelleyenId = currentInfo.KullaniciId;
                makineler.GuncelleyenIp = currentInfo.Ip;

                if (makineler.KanalKullanimdaMi(makineler.RoleCihazlarId, makineler.RoleKanalNo, makineler.Id))
                {
                    veritabaniIslemleri.GeriAl();
                    Mesaj.Ver(Mesajlar.RoleKanalKullanimda, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }

                bool bandDegisti = eskiBand != makineler.BandNo;

                if (bandDegisti)
                {
                    makineler.SiraNo = makineler.MaxSiraGetir() + 1;
                }
                else if (eskiSira.HasValue)
                {
                    makineler.SiraNo = eskiSira;
                }
                else
                {
                    makineler.SiraNo = makineler.MaxSiraGetir() + 1;
                }

                if (!makineler.Guncelle())
                {
                    veritabaniIslemleri.GeriAl();
                    Mesaj.Ver(Mesajlar.KayitGuncellemeBasarisiz, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                if (bandDegisti)
                {
                    makineler.BandNo = eskiBand;
                    makineler.GuncelleyenId = currentInfo.KullaniciId;
                    makineler.GuncelleyenIp = currentInfo.Ip;
                    if (!makineler.BandSiralariniSikistir())
                    {
                        veritabaniIslemleri.GeriAl();
                        Mesaj.Ver(Mesajlar.KayitGuncellemeBasarisiz, Mesaj.MesajTurleri.FAIL, Master);
                        return;
                    }
                }

                veritabaniIslemleri.Uygula();
                Mesaj.Ver(Mesajlar.KayitGuncellemeBasarili, Mesaj.MesajTurleri.SUCCESS, Master);
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
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
                Makineler makineler = new Makineler(veritabaniIslemleri);
                makineler.Id = Convert.ToInt32(id);

                if (!makineler.Doldur())
                {
                    veritabaniIslemleri.GeriAl();
                    Mesaj.Ver(Mesajlar.MakineBulunamadi, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                string eskiBand = makineler.BandNo;
                makineler.GuncelleyenId = currentInfo.KullaniciId;
                makineler.GuncelleyenIp = currentInfo.Ip;

                if (!makineler.Sil())
                {
                    veritabaniIslemleri.GeriAl();
                    Mesaj.Ver(Mesajlar.KayitSilmeBasarisiz, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                makineler.BandNo = eskiBand;
                makineler.GuncelleyenId = currentInfo.KullaniciId;
                makineler.GuncelleyenIp = currentInfo.Ip;
                if (!makineler.BandSiralariniSikistir())
                {
                    veritabaniIslemleri.GeriAl();
                    Mesaj.Ver(Mesajlar.KayitSilmeBasarisiz, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                veritabaniIslemleri.Uygula();
                Response.Redirect("~/Pages/MakineListele.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
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
