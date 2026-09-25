using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AsyModbus
{
    public partial class Default : System.Web.UI.Page
    {
        private DataTable dataTable;

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

        protected string MakineEtiketMetniGetir(object roleCihazlarId, object durumAd, object durumKod)
        {
            if (!RoleSeciliMi(roleCihazlarId))
            {
                return "Röle seçilmedi";
            }

            return DurumMetniGetir(durumAd, durumKod);
        }

        protected string MakineEtiketCssGetir(object roleCihazlarId, object durumKod)
        {
            if (!RoleSeciliMi(roleCihazlarId))
            {
                return "badge-role-yok";
            }

            return DurumCssGetir(durumKod);
        }

        protected bool DurdurButonuAktifMi(object roleCihazlarId, object durumKod)
        {
            if (!RoleSeciliMi(roleCihazlarId))
            {
                return false;
            }

            return DurumKodNormalize(durumKod) == SistemParametreKodlari.Calisiyor;
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
            Button btnDurdur = (Button)sender;
            int makineId = Convert.ToInt32(btnDurdur.CommandArgument);
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;
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

                Parametreler parametreler = new Parametreler(veritabaniIslemleri);
                int? calisiyorId = parametreler.IdGetir(SistemParametreGruplari.MakineDurumu, SistemParametreKodlari.Calisiyor);
                int? durdurulduId = parametreler.IdGetir(SistemParametreGruplari.MakineDurumu, SistemParametreKodlari.Durduruldu);
                int? personelId = parametreler.IdGetir(SistemParametreGruplari.IslemKaynagi, SistemParametreKodlari.Personel);
                int? durdurmaId = parametreler.IdGetir(SistemParametreGruplari.IslemTuru, SistemParametreKodlari.Durdurma);
                int? basariliId = parametreler.IdGetir(SistemParametreGruplari.IslemSonucu, SistemParametreKodlari.Basarili);
                int? basarisizId = parametreler.IdGetir(SistemParametreGruplari.IslemSonucu, SistemParametreKodlari.Basarisiz);

                if (!calisiyorId.HasValue || !durdurulduId.HasValue || !personelId.HasValue ||
                    !durdurmaId.HasValue || !basariliId.HasValue || !basarisizId.HasValue)
                {
                    Mesaj.Ver(Mesajlar.SistemselHata("Gerekli parametre kayıtları bulunamadı."), Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                if (!makineler.DurumParametreId.HasValue || makineler.DurumParametreId.Value != calisiyorId.Value)
                {
                    Mesaj.Ver(Mesajlar.MakineCalismiyor, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }
                // Makineye röle cihazı ve kanal atanmış mı kontrol edilir.
                if (!makineler.RoleCihazlarId.HasValue || !makineler.RoleKanalNo.HasValue)
                {
                    Mesaj.Ver(Mesajlar.MakineRoleBaglantisiEksik, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }

                // Makinenin bağlı olduğu röle cihazı veritabanından alınır.
                RoleCihazlar roleCihazlar = new RoleCihazlar(veritabaniIslemleri);
                roleCihazlar.Id = makineler.RoleCihazlarId.Value;

                if (!roleCihazlar.Doldur())
                {
                    Mesaj.Ver(Mesajlar.RoleCihaziBulunamadi, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                // Röle cihazının IP ve port bilgileri kontrol edilir.
                if (string.IsNullOrEmpty(roleCihazlar.Ip) || !roleCihazlar.Port.HasValue || roleCihazlar.Port.Value <= 0)
                {
                    Mesaj.Ver(Mesajlar.RoleBaglantiBilgileriEksik, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                // HW-584 üzerindeki makineye atanmış kanal 2 saniye tetiklenir.
                RoleKontrol roleKontrol = new RoleKontrol();

                bool roleBasarili = roleKontrol.KanalTetikle( roleCihazlar.Ip, roleCihazlar.Port.Value, makineler.RoleKanalNo.Value );

                // Röle işlemi başarısızsa makinenin durumu değiştirilmez; deneme MakinelerLoglar'a yazılır.
                if (!roleBasarili)
                {
                    MakinelerLoglar hataLog = new MakinelerLoglar(veritabaniIslemleri);
                    hataLog.MakinelerId = makineId;
                    hataLog.KaynakParametreId = personelId;
                    hataLog.OncekiDurumParametreId = makineler.DurumParametreId;
                    hataLog.YeniDurumParametreId = makineler.DurumParametreId;
                    hataLog.IslemTurParametreId = durdurmaId;
                    hataLog.IslemSonucParametreId = basarisizId;
                    hataLog.Detay = "Röle cihazına durdurma komutu gönderilemedi. Röle cihazı ID: "
                        + roleCihazlar.Id
                        + ", IP: " + roleCihazlar.Ip
                        + ", Port: " + roleCihazlar.Port.Value
                        + ", Kanal: " + makineler.RoleKanalNo.Value
                        + ". Teknik hata: " + roleKontrol.SonHata;
                    hataLog.AktifMi = true;
                    hataLog.EkleyenId = currentInfo.KullaniciId;
                    hataLog.EkleyenIp = currentInfo.Ip;
                    hataLog.Ekle();

                    Mesaj.Ver(Mesajlar.RoleTetiklemeHatasi, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                int? oncekiDurumParametreId = makineler.DurumParametreId;
                makineler.DurumParametreId = durdurulduId;
                makineler.GuncelleyenId = currentInfo.KullaniciId;
                makineler.GuncelleyenIp = currentInfo.Ip;

                if (!makineler.DurumGuncelle())
                {
                    Mesaj.Ver(Mesajlar.MakineDurdurmaHatasi, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                MakinelerLoglar makinelerLoglar = new MakinelerLoglar(veritabaniIslemleri);
                makinelerLoglar.MakinelerId = makineId;
                makinelerLoglar.KaynakParametreId = personelId;
                makinelerLoglar.OncekiDurumParametreId = oncekiDurumParametreId;
                makinelerLoglar.YeniDurumParametreId = durdurulduId;
                makinelerLoglar.IslemTurParametreId = durdurmaId;
                makinelerLoglar.IslemSonucParametreId = basariliId;
                makinelerLoglar.Detay = "Makine web paneli üzerinden durdurma komutu aldı. Röle cihazı ID: "
                    + roleCihazlar.Id
                    + ", IP: " + roleCihazlar.Ip
                    + ", Port: " + roleCihazlar.Port.Value
                    + ", Kanal: " + makineler.RoleKanalNo.Value + ".";
                makinelerLoglar.AktifMi = true;
                makinelerLoglar.EkleyenId = currentInfo.KullaniciId;
                makinelerLoglar.EkleyenIp = currentInfo.Ip;

                if (!makinelerLoglar.Ekle())
                {
                    Mesaj.Ver(Mesajlar.MakineLoglamaHatasi, Mesaj.MesajTurleri.FAIL, Master);
                    MakineleriGetir();
                    return;
                }

                Mesaj.Ver(Mesajlar.MakineBasariylaDurduruldu, Mesaj.MesajTurleri.SUCCESS, Master);
                MakineleriGetir();
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
                    else if (kaynakKod == SistemParametreKodlari.Personel)
                    {
                        if (ekleyenId > 0)
                        {
                            Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);
                            kullanicilar.Id = ekleyenId;
                            if (kullanicilar.Doldur())
                            {
                                lblDetayIslemiYapan.Text = kullanicilar.Ad + " " + kullanicilar.Soyad;
                            }
                            else
                            {
                                lblDetayIslemiYapan.Text = "Personel";
                            }
                        }
                        else
                        {
                            lblDetayIslemiYapan.Text = "Personel";
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