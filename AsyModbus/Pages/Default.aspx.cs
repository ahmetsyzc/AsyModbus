using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AsyModbus
{
    public partial class Default : System.Web.UI.Page
    {
        private DataTable dataTable;
        private readonly Dictionary<int, string> makineKomutDurumlari = new Dictionary<int, string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }
            MakineleriGetir();
        }

        private void MakineleriGetir()
        {
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Makineler makineler = new Makineler(veritabaniIslemleri);
                dataTable = makineler.TumunuGetir();
                MakineKomutlar makineKomutlar = new MakineKomutlar(veritabaniIslemleri);
                DataTable aktifKomutlar = makineKomutlar.AktifleriGetir();
                makineKomutDurumlari.Clear();
                foreach (DataRow veriSatiri in aktifKomutlar.Rows)
                {
                    int makineId = Convert.ToInt32(veriSatiri[MakineKomutlar.C_Sutun_makineler_id]);
                    makineKomutDurumlari[makineId] = veriSatiri[MakineKomutlar.C_Sutun_durum_kod].ToString();
                }
                DataView dataView = new DataView(dataTable);
                // Bantların sıralanabilmesi için DataView oluşturulur.
                DataView bantGorunumu = new DataView(dataTable);
                // Bantlar küçük numaradan büyük numaraya sıralanır.
                bantGorunumu.Sort = Makineler.C_Sutun_band_no + " ASC";
                // Tekrar eden bant numaraları kaldırılır.
                DataTable bantlar = bantGorunumu.ToTable(true, Makineler.C_Sutun_band_no);
                // Sıralanmış bantlar dış Repeater'a bağlanır.
                rptBantlar.DataSource = bantlar;
                rptBantlar.DataBind();
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

        protected void rptBantlar_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item &&
                e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }
            string bandNo = DataBinder.Eval(e.Item.DataItem, Makineler.C_Sutun_band_no).ToString();
            Repeater rptMakineler = (Repeater)e.Item.FindControl("rptMakineler");

            rptMakineler.DataSource = BandMakineleriniSiraNoIleGetir(dataTable, bandNo);
            rptMakineler.DataBind();
        }

        private DataTable BandMakineleriniSiraNoIleGetir(DataTable kaynak, string bandNo)
        {
            var siraliSatirlar = kaynak.AsEnumerable()
                .Where(row => row[Makineler.C_Sutun_band_no].ToString() == bandNo)
                .OrderBy(row => row.Field<int?>(Makineler.C_Sutun_sira_no) ?? int.MaxValue);

            if (!siraliSatirlar.Any())
            {
                return kaynak.Clone();
            }

            return siraliSatirlar.CopyToDataTable();
        }

        protected string DurumMetniGetir(object durumAd, object durumKod)
        {
            if (durumAd != null && durumAd != DBNull.Value && !string.IsNullOrWhiteSpace(durumAd.ToString()))
            {
                return durumAd.ToString();
            }

            string durumDegeri = DurumKodNormalize(durumKod);

            switch (durumDegeri)
            {
                case SistemParametreKodlari.Calisiyor:
                    return "Çalışıyor";
                case SistemParametreKodlari.Durduruldu:
                    return "Durduruldu";
                case SistemParametreKodlari.BaglantiYok:
                    return "Bağlantı Yok";
                default:
                    return "Bağlantı Yok";
            }
        }

        protected string DurumCssGetir(object durumKod)
        {
            string durumDegeri = DurumKodNormalize(durumKod);

            switch (durumDegeri)
            {
                case SistemParametreKodlari.Calisiyor:
                    return "bg-success";
                case SistemParametreKodlari.Durduruldu:
                    return "bg-danger";
                case SistemParametreKodlari.BaglantiYok:
                    return "bg-secondary";
                default:
                    return "bg-secondary";
            }
        }

        protected string MakineEtiketMetniGetir(object makineId, object roleCihazlarId, object durumAd, object durumKod)
        {
            if (AktifKomutVarMi(makineId))
            {
                return KomutDurumMetniGetir(makineId);
            }

            if (!RoleSeciliMi(roleCihazlarId))
            {
                return "Röle seçilmedi";
            }

            return DurumMetniGetir(durumAd, durumKod);
        }

        protected string MakineEtiketCssGetir(object makineId, object roleCihazlarId, object durumKod)
        {
            if (AktifKomutVarMi(makineId))
            {
                return "bg-warning text-dark";
            }

            if (!RoleSeciliMi(roleCihazlarId))
            {
                return "badge-role-yok";
            }

            return DurumCssGetir(durumKod);
        }

        protected bool DurdurButonuAktifMi(object makineId, object roleCihazlarId, object durumKod)
        {
            if (!RoleSeciliMi(roleCihazlarId) || AktifKomutVarMi(makineId))
            {
                return false;
            }

            return DurumKodNormalize(durumKod) == SistemParametreKodlari.Calisiyor;
        }

        protected bool AktifKomutVarMi(object makineId)
        {
            return makineKomutDurumlari.ContainsKey(Convert.ToInt32(makineId));
        }

        protected string KomutDurumMetniGetir(object makineId)
        {
            string durumKod;
            if (!makineKomutDurumlari.TryGetValue(Convert.ToInt32(makineId), out durumKod))
            {
                return string.Empty;
            }
            return durumKod == SistemParametreKodlari.Isleniyor
                ? Mesajlar.MakineDurdurmaIslemeAlindi : Mesajlar.MakineDurdurmaBekleniyor;
        }

        private bool RoleSeciliMi(object roleCihazlarId)
        {
            return roleCihazlarId != null && roleCihazlarId != DBNull.Value;
        }

        private string DurumKodNormalize(object durumKod)
        {
            if (durumKod == null || durumKod == DBNull.Value)
            {
                return string.Empty;
            }

            return durumKod.ToString().Trim().ToUpper();
        }

        private string LogAlanGetir(DataRow satir, string kolon)
        {
            if (satir == null
                || string.IsNullOrEmpty(kolon)
                || !satir.Table.Columns.Contains(kolon)
                || satir[kolon] == DBNull.Value)
            {
                return string.Empty;
            }

            return satir[kolon].ToString();
        }

        protected void btnDurdur_Click(object sender, EventArgs e)
        {
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
            if (currentInfo == null || !currentInfo.LoginYapildiMi || currentInfo.KullaniciId <= 0)
            {
                Response.Redirect("~/Pages/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            Button btnDurdur = sender as Button;
            int makineId;
            if (btnDurdur == null || !int.TryParse(btnDurdur.CommandArgument, out makineId) || makineId <= 0)
            {
                Mesaj.Ver(Mesajlar.MakineBulunamadi, Mesaj.MesajTurleri.FAIL, Master);
                return;
            }

            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            try
            {
                // Talep ve otomatik uygulama logu birlikte kaydedilir.
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
                Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);
                kullanicilar.Id = currentInfo.KullaniciId;
                Roller roller = new Roller(veritabaniIslemleri);
                roller.Id = currentInfo.RolId;
                // Roller.DOLDUR yalnizca aktif rolleri getirir; oturumdaki rol de guncel olmali.
                if (!kullanicilar.Doldur() || !kullanicilar.AktifMi
                    || kullanicilar.RollerId != currentInfo.RolId || !roller.Doldur())
                {
                    Mesaj.Ver(Mesajlar.YetkisizIslem, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }

                Makineler makineler = new Makineler(veritabaniIslemleri);
                makineler.Id = makineId;
                if (!makineler.Doldur() || !makineler.AktifMi)
                {
                    Mesaj.Ver(Mesajlar.MakineBulunamadi, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                Parametreler parametreler = new Parametreler(veritabaniIslemleri);
                int? calisiyorId = parametreler.IdGetir(SistemParametreGruplari.MakineDurumu, SistemParametreKodlari.Calisiyor);
                int? durdurmaId = parametreler.IdGetir(SistemParametreGruplari.IslemTuru, SistemParametreKodlari.Durdurma);
                if (!calisiyorId.HasValue || !durdurmaId.HasValue)
                {
                    Mesaj.Ver(Mesajlar.SistemselHata(Mesajlar.KomutParametreleriEksik), Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }
                if (makineler.DurumParametreId != calisiyorId)
                {
                    Mesaj.Ver(Mesajlar.MakineCalismiyor, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }
                if (!makineler.RoleCihazlarId.HasValue || !makineler.RoleKanalNo.HasValue)
                {
                    Mesaj.Ver(Mesajlar.MakineRoleBaglantisiEksik, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }

                RoleCihazlar roleCihazlar = new RoleCihazlar(veritabaniIslemleri);
                roleCihazlar.Id = makineler.RoleCihazlarId.Value;
                // DOLDUR SP'si yalnizca aktif role cihazlarini getirir.
                if (!roleCihazlar.Doldur())
                {
                    Mesaj.Ver(Mesajlar.RoleCihaziBulunamadi, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }
                System.Net.IPAddress roleIp;
                if (!System.Net.IPAddress.TryParse(roleCihazlar.Ip, out roleIp)
                    || !roleCihazlar.Port.HasValue || roleCihazlar.Port.Value <= 0 || roleCihazlar.Port.Value > 65535)
                {
                    Mesaj.Ver(Mesajlar.RoleBaglantiBilgileriEksik, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }
                if (!roleCihazlar.KanalSayisi.HasValue || roleCihazlar.KanalSayisi.Value <= 0
                    || makineler.RoleKanalNo.Value <= 0 || makineler.RoleKanalNo.Value > roleCihazlar.KanalSayisi.Value)
                {
                    Mesaj.Ver(Mesajlar.RoleKanalGecersiz, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }

                MakineKomutlar makineKomutlar = new MakineKomutlar(veritabaniIslemleri);
                makineKomutlar.MakinelerId = makineId;
                makineKomutlar.IslemTurParametreId = durdurmaId.Value;
                if (makineKomutlar.AktifKomutVarMi())
                {
                    Mesaj.Ver(Mesajlar.MakineDurdurmaTalebiZatenVar, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }

                makineKomutlar.Detay = Mesajlar.MakineDurdurmaTalebiDetay;
                makineKomutlar.EkleyenId = currentInfo.KullaniciId;
                makineKomutlar.EkleyenIp = currentInfo.Ip;
                if (!makineKomutlar.Ekle())
                {
                    Mesaj.Ver(Mesajlar.MakineDurdurmaTalebiOlusturulamadi, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                veritabaniIslemleri.Uygula();
                Mesaj.Ver(Mesajlar.MakineDurdurmaTalebiOlusturuldu, Mesaj.MesajTurleri.SUCCESS, Master);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                // Iki eszamanli web isteginde son kontrolu EKLE SP'si yapar.
                if (ex.Number == 51104)
                {
                    Mesaj.Ver(Mesajlar.MakineDurdurmaTalebiZatenVar, Mesaj.MesajTurleri.WARNING, Master);
                }
                else
                {
                    Mesaj.Ver(Mesajlar.SistemselHata(ex.Message), Mesaj.MesajTurleri.FAIL, Master);
                }
            }
            catch (Exception ex)
            {
                Mesaj.Ver(Mesajlar.SistemselHata(ex.Message), Mesaj.MesajTurleri.FAIL, Master);
            }
            finally
            {
                try { veritabaniIslemleri.GeriAl(); }
                finally { veritabaniIslemleri.Bitir(); }
                MakineleriGetir();
            }
        }

        protected void btnDetay_Click(object sender, EventArgs e)
        {
            LinkButton btnDetay = (LinkButton)sender;
            int makineId = Convert.ToInt32(btnDetay.CommandArgument);
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Makineler makineler = new Makineler(veritabaniIslemleri);
                makineler.Id = makineId;
                if (!makineler.Doldur())
                {
                    Mesaj.Ver(Mesajlar.MakineBulunamadi, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                lblDetayMakineNo.Text = makineler.MakineNo;
                lblDetayGuncelDurum.Text = "Bağlantı Yok";
                if (makineler.DurumParametreId.HasValue)
                {
                    Parametreler durumParametre = new Parametreler(veritabaniIslemleri);
                    durumParametre.Id = makineler.DurumParametreId.Value;
                    if (durumParametre.Doldur())
                    {
                        lblDetayGuncelDurum.Text = DurumMetniGetir(durumParametre.Ad, durumParametre.Kod);
                    }
                }

                MakinelerLoglar makinelerLoglar = new MakinelerLoglar(veritabaniIslemleri);
                makinelerLoglar.MakinelerId = makineId;
                DataRow sonLog = makinelerLoglar.SonKayitGetir();
                if (sonLog == null)
                {
                    // Makineye ait henüz log yoksa boş alanlar gösterilir.
                    lblDetayKaynak.Text = "-";
                    lblDetayIslemiYapan.Text = "-";
                    lblDetayOncekiDurum.Text = "-";
                    lblDetayYeniDurum.Text = "-";
                    lblDetayIslemSonucu.Text = "-";
                    lblDetayIslemTarihi.Text = "-";
                    lblDetayAciklama.Text = "Bu makineye ait log bulunamadı.";
                }
                else
                {
                    // Son log kaydındaki bilgiler modal alanlarına yazılır.
                    int ekleyenId = 0;
                    if (sonLog[MakinelerLoglar.C_Sutun_ekleyen_id] != DBNull.Value)
                    {
                        ekleyenId = Convert.ToInt32(sonLog[MakinelerLoglar.C_Sutun_ekleyen_id]);
                    }

                    string kaynakKod = LogAlanGetir(sonLog, MakinelerLoglar.C_Sutun_kaynak_kod).Trim().ToUpper();
                    string kaynakAd = LogAlanGetir(sonLog, MakinelerLoglar.C_Sutun_kaynak_ad);
                    lblDetayKaynak.Text = string.IsNullOrEmpty(kaynakAd) ? "-" : kaynakAd;

                    if (kaynakKod == SistemParametreKodlari.Saha)
                    {
                        lblDetayIslemiYapan.Text = "Saha";
                    }
                    else if (kaynakKod == SistemParametreKodlari.Sistem)
                    {
                        lblDetayIslemiYapan.Text = "Sistem";
                    }
                    else if (kaynakKod == SistemParametreKodlari.Personel
                        || kaynakKod == SistemParametreKodlari.Servis)
                    {
                        // Servis işlemlerinde talep sahibinin kimliği operasyon logunda korunur.
                        lblDetayIslemiYapan.Text = kaynakKod == SistemParametreKodlari.Servis
                            ? Mesajlar.KomutTalepSahibiBulunamadi : "Personel";
                        if (ekleyenId > 0)
                        {
                            Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);
                            kullanicilar.Id = ekleyenId;
                            if (kullanicilar.Doldur())
                            {
                                string adSoyad = (kullanicilar.Ad + " " + kullanicilar.Soyad).Trim();
                                if (!string.IsNullOrWhiteSpace(adSoyad))
                                {
                                    lblDetayIslemiYapan.Text = Server.HtmlEncode(adSoyad);
                                }
                            }
                        }
                    }
                    else
                    {
                        lblDetayIslemiYapan.Text = "-";
                    }

                    lblDetayOncekiDurum.Text = DurumMetniGetir(
                        LogAlanGetir(sonLog, MakinelerLoglar.C_Sutun_onceki_durum_ad),
                        LogAlanGetir(sonLog, MakinelerLoglar.C_Sutun_onceki_durum_kod));
                    lblDetayYeniDurum.Text = DurumMetniGetir(
                        LogAlanGetir(sonLog, MakinelerLoglar.C_Sutun_yeni_durum_ad),
                        LogAlanGetir(sonLog, MakinelerLoglar.C_Sutun_yeni_durum_kod));
                    lblDetayIslemSonucu.Text = LogAlanGetir(sonLog, MakinelerLoglar.C_Sutun_islem_sonuc_ad);
                    if (string.IsNullOrEmpty(lblDetayIslemSonucu.Text))
                    {
                        lblDetayIslemSonucu.Text = "-";
                    }
                    lblDetayAciklama.Text = sonLog[MakinelerLoglar.C_Sutun_detay] == DBNull.Value
                        ? string.Empty
                        : sonLog[MakinelerLoglar.C_Sutun_detay].ToString();
                    if (sonLog[MakinelerLoglar.C_Sutun_eklenme_tarih] == DBNull.Value)
                    {
                        lblDetayIslemTarihi.Text = "-";
                    }
                    else
                    {
                        lblDetayIslemTarihi.Text = Convert.ToDateTime(sonLog[MakinelerLoglar.C_Sutun_eklenme_tarih]).ToString("dd.MM.yyyy HH:mm");
                    }
                }

                // Bilgiler doldurulduktan sonra Bootstrap modalı açılır.
                string modalScript = "var modal = new bootstrap.Modal(document.getElementById('makineDetayModal')); modal.show();";
                ClientScript.RegisterStartupScript(GetType(), "MakineDetayModal", modalScript, true);
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

        protected void btnBantDuzenle_Click(object sender, EventArgs e)
        {
            LinkButton btnBantDuzenle = (LinkButton)sender;
            string bandNo = btnBantDuzenle.CommandArgument;
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Makineler makineler = new Makineler(veritabaniIslemleri);
                // Bütün makineler veritabanından alınır.
                DataTable makinelerTablosu = makineler.TumunuGetir();
                // Seçilen banda ait makineleri bulmak için görünüm oluşturulur.
                DataView bantMakineleri = new DataView(makinelerTablosu);
                bantMakineleri.RowFilter = Makineler.C_Sutun_band_no + " = '" + bandNo + "'";
                // Makineler mevcut sıra numaralarına göre sıralanır.
                bantMakineleri.Sort = Makineler.C_Sutun_sira_no + " ASC";
                // Modalın hangi banda ait olduğu saklanır.
                hfSiralamaBantNo.Value = bandNo;
                lblSiralamaBantNo.Text = bandNo;
                rptSiralamaMakineler.DataSource = BandMakineleriniSiraNoIleGetir(makinelerTablosu, bandNo);
                rptSiralamaMakineler.DataBind();
                // Bootstrap modalı açılır.
                string modalScript = "var modal = new bootstrap.Modal(" + "document.getElementById('bantDuzenleModal'));" + "modal.show();";
                ClientScript.RegisterStartupScript(GetType(), "BantDuzenleModal", modalScript, true);
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

        protected void btnVarsayilanSirala_Click(object sender, EventArgs e)
        {
            // Modalın hangi bant için açıldığı alınır.
            string bandNo = hfSiralamaBantNo.Value;

            if (string.IsNullOrEmpty(bandNo))
            {
                Mesaj.Ver(Mesajlar.MakineSirasiBos, Mesaj.MesajTurleri.WARNING, Master);
                return;
            }
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
                Makineler makineler = new Makineler(veritabaniIslemleri);
                // Bütün makineler veritabanından alınır.
                DataTable makinelerTablosu = makineler.TumunuGetir();
                DataRow[] siraliMakineler = makinelerTablosu.AsEnumerable()
                    .Where(row => row[Makineler.C_Sutun_band_no].ToString() == bandNo)
                    .OrderBy(row =>
                    {
                        int makineNo;
                        return int.TryParse(row[Makineler.C_Sutun_makine_no].ToString(), out makineNo)
                            ? makineNo
                            : int.MaxValue;
                    })
                    .ToArray();

                for (int i = 0; i < siraliMakineler.Length; i++)
                {
                    makineler.Id = Convert.ToInt32(siraliMakineler[i][Makineler.C_Sutun_id]);
                    makineler.SiraNo = i + 1;
                    makineler.GuncelleyenId = currentInfo.KullaniciId;
                    makineler.GuncelleyenIp = currentInfo.Ip;
                    if (!makineler.SiraGuncelle())
                    {
                        veritabaniIslemleri.GeriAl();
                        Mesaj.Ver(Mesajlar.MakineSirasiGuncellenemedi, Mesaj.MesajTurleri.FAIL, Master);
                        return;
                    }
                }

                veritabaniIslemleri.Uygula();

                bool logBasarili = LogIslemleri.IslemKaydet(
                    "Makineler",
                    "Makine sıralama güncelleme",
                    LogIslemTipleri.Update,
                    "Bant " + bandNo + " makine sıralaması güncellendi.");

                if (logBasarili)
                {
                    Mesaj.Ver(Mesajlar.MakineSirasiGuncellendi, Mesaj.MesajTurleri.SUCCESS, Master);
                }
                else
                {
                    Mesaj.Ver(Mesajlar.MakineSirasiLoglamaHatasi, Mesaj.MesajTurleri.WARNING, Master);
                }

                DataTable guncelMakineler = makineler.TumunuGetir();
                rptSiralamaMakineler.DataSource = BandMakineleriniSiraNoIleGetir(guncelMakineler, bandNo);
                rptSiralamaMakineler.DataBind();
                string modalScript = "var modal = new bootstrap.Modal(" + "document.getElementById('bantDuzenleModal'));" + "modal.show();";
                ClientScript.RegisterStartupScript(GetType(), "BantDuzenleModal", modalScript, true);

                MakineleriGetir();
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

        protected void btnSiralamaKaydet_Click(object sender, EventArgs e)
        {
            // HiddenField içerisindeki sıralama alınır.
            string makineSirasi = hfMakineSirasi.Value;

            if (string.IsNullOrEmpty(makineSirasi))
            {
                Mesaj.Ver(Mesajlar.MakineSirasiBos, Mesaj.MesajTurleri.WARNING, Master);
                return;
            }
            // Örnek: "5,3,1,2" değeri ayrı makine ID'lerine dönüştürülür.
            string[] makineIdleri = makineSirasi.Split(',');

            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);
                Makineler makineler = new Makineler(veritabaniIslemleri);
                for (int i = 0; i < makineIdleri.Length; i++)
                {
                    int makineId;
                    if (!int.TryParse(makineIdleri[i], out makineId))
                    {
                        veritabaniIslemleri.GeriAl();
                        Mesaj.Ver(Mesajlar.MakineSirasiGecersiz, Mesaj.MesajTurleri.FAIL, Master);
                        return;
                    }
                    makineler.Id = makineId;
                    // Dizinin ilk elemanı 0 olduğu için sıra numarasına 1 eklenir.
                    makineler.SiraNo = i + 1;
                    makineler.GuncelleyenId = currentInfo.KullaniciId;
                    makineler.GuncelleyenIp = currentInfo.Ip;
                    if (!makineler.SiraGuncelle())
                    {
                        veritabaniIslemleri.GeriAl();
                        Mesaj.Ver(Mesajlar.MakineSirasiGuncellenemedi, Mesaj.MesajTurleri.FAIL, Master);
                        return;
                    }
                }

                veritabaniIslemleri.Uygula();

                string bandNo = hfSiralamaBantNo.Value;
                bool logBasarili = LogIslemleri.IslemKaydet(
                    "Makineler",
                    "Makine sıralama güncelleme",
                    LogIslemTipleri.Update,
                    "Bant " + bandNo + " makine sıralaması güncellendi.");
                if (logBasarili)
                {
                    Mesaj.Ver(Mesajlar.MakineSirasiGuncellendi, Mesaj.MesajTurleri.SUCCESS, Master);
                }
                else
                {
                    Mesaj.Ver(Mesajlar.MakineSirasiLoglamaHatasi, Mesaj.MesajTurleri.WARNING, Master);
                }

                // Ana ekrandaki makineler yeni sırasıyla tekrar yüklenir.
                MakineleriGetir();
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
