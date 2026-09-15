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

        protected string DurumMetniGetir(object durum)
        {
            if (durum == null || durum == DBNull.Value)
            {
                return "Bağlantı Yok";
            }

            string durumDegeri = durum.ToString().Trim().ToUpper();

            switch (durumDegeri)
            {
                case "CALISIYOR":
                    return "Çalışıyor";
                case "DURDURULDU":
                    return "Durduruldu";
                case "BAGLANTI_YOK":
                    return "Bağlantı Yok";
                default:
                    return "Bağlantı Yok";
            }
        }

        protected string DurumCssGetir(object durum)
        {
            if (durum == null || durum == DBNull.Value)
            {
                return "bg-secondary";
            }

            string durumDegeri = durum.ToString().Trim().ToUpper();

            switch (durumDegeri)
            {
                case "CALISIYOR":
                    return "bg-success";
                case "DURDURULDU":
                    return "bg-danger";
                case "BAGLANTI_YOK":
                    return "bg-secondary";
                default:
                    return "bg-secondary";
            }
        }

        protected bool DurdurButonuAktifMi(object durum)
        {
            if (durum == null || durum == DBNull.Value)
            {
                return false;
            }

            string durumDegeri = durum.ToString().Trim().ToUpper();

            return durumDegeri == "CALISIYOR";
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
                if (!string.Equals(makineler.Durum, "CALISIYOR", StringComparison.OrdinalIgnoreCase))
                {
                    Mesaj.Ver(Mesajlar.MakineCalismiyor, Mesaj.MesajTurleri.WARNING, Master);
                    return;
                }

                string oncekiDurum = makineler.Durum;
                makineler.Durum = "DURDURULDU";
                makineler.GuncelleyenId = currentInfo.KullaniciId;
                makineler.GuncelleyenIp = currentInfo.Ip;

                if (!makineler.DurumGuncelle())
                {
                    Mesaj.Ver(Mesajlar.MakineDurdurmaHatasi, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                MakinelerLoglar makinelerLoglar = new MakinelerLoglar(veritabaniIslemleri);
                makinelerLoglar.MakinelerId = makineId;
                makinelerLoglar.Kaynak = "PERSONEL";
                makinelerLoglar.OncekiDurum = oncekiDurum;
                makinelerLoglar.YeniDurum = "DURDURULDU";
                makinelerLoglar.IslemTur = "DURDURMA";
                makinelerLoglar.IslemSonuc = "BASARILI";
                makinelerLoglar.Detay = "Makine web paneli üzerinden durduruldu.";
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
                lblDetayGuncelDurum.Text = DurumMetniGetir(makineler.Durum);

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
                    string kaynak = sonLog[MakinelerLoglar.C_Sutun_kaynak].ToString().Trim().ToUpper();
                    lblDetayKaynak.Text = kaynak;
                    if (kaynak == "SAHA")
                    {
                        lblDetayIslemiYapan.Text = "Saha";
                    }
                    else if (kaynak == "SISTEM")
                    {
                        lblDetayIslemiYapan.Text = "Sistem";
                    }
                    else if (kaynak == "PERSONEL")
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

                    lblDetayOncekiDurum.Text = DurumMetniGetir(sonLog[MakinelerLoglar.C_Sutun_onceki_durum]);
                    lblDetayYeniDurum.Text = DurumMetniGetir(sonLog[MakinelerLoglar.C_Sutun_yeni_durum]);
                    lblDetayIslemSonucu.Text = sonLog[MakinelerLoglar.C_Sutun_islem_sonuc].ToString();
                    lblDetayAciklama.Text = sonLog[MakinelerLoglar.C_Sutun_detay].ToString();
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