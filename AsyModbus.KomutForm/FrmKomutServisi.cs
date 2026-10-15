using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsyModbus.KomutForm
{
    public partial class FrmKomutServisi : Form
    {
        private readonly MakineKomutServisi makineKomutServisi = new MakineKomutServisi();
        private bool kontrolAcik = false;
        private Task<bool> kontrolGorevi;
        private bool kapaniyor = false;
        private bool kapanisaIzin = false;

        public FrmKomutServisi()
        {
            InitializeComponent();
            Text = Mesajlar.KomutServisiPencereBasligi;
            lblBaslik.Text = Mesajlar.KomutServisiPencereBasligi;
            lblKayitlar.Text = Mesajlar.KomutServisiKayitBasligi;
            btnBaslat.Text = Mesajlar.KomutServisiBaslatButonu;
            btnDurdur.Text = Mesajlar.KomutServisiDurdurButonu;
            lblDurum.Text = Mesajlar.KomutServisiDurduruldu;
            makineKomutServisi.LogOlustu += MakineKomutServisi_LogOlustu;
        }

        private void MakineKomutServisi_LogOlustu( object sender, MakineKomutServisiLogEventArgs e)
        {
            if (kapaniyor || IsDisposed || !IsHandleCreated)
            {
                return;
            }

            try
            {
                // Ekran güncellemesini Formun çalıştığı iş parçacığına aktar.
                BeginInvoke(new Action(() =>
                {
                    if (kapaniyor || IsDisposed || Disposing)
                    {
                        return;
                    }

                    // Yeni kaydı listenin en başına (0. indekse) ekle
                    LogEkle(e.Tarih.ToString("HH:mm:ss") + " - " + e.Mesaj);


                    // Her zaman en üstü (yeni gelen mesajı) görünür tut.
                    lstLog.TopIndex = 0;
                }));
            }
            catch (InvalidOperationException)
            {
                // Form kapanırken ekran güncellemesi yapılamayabilir.
            }
        }

        private void LogEkle(string mesaj)
        {
            lstLog.Items.Insert(0, mesaj);
            while (lstLog.Items.Count > 10) lstLog.Items.RemoveAt(lstLog.Items.Count - 1);
            lstLog.TopIndex = 0;
        }

        private void btnBaslat_Click(object sender, EventArgs e)
        {
            if (kapaniyor || kontrolAcik || (kontrolGorevi != null && !kontrolGorevi.IsCompleted))
            {
                return;
            }
            btnBaslat.Enabled = false;
            btnDurdur.Enabled = true;
            kontrolAcik = true;
            timerKomut.Start();
            lblDurum.Text = Mesajlar.KomutServisiCalisiyor;
            LogEkle(Mesajlar.KomutServisiKontrolBaslatildi);
        }

        private void btnDurdur_Click(object sender, EventArgs e)
        {
            btnBaslat.Enabled = true;
            btnDurdur.Enabled = false;
            kontrolAcik = false;
            timerKomut.Stop();
            lblDurum.Text = Mesajlar.KomutServisiDurduruldu;
            LogEkle(Mesajlar.KomutServisiKontrolDurduruldu);
        }

        private async void timerKomut_Tick(object sender, EventArgs e)
        {

            // Kontrol kapalıysa, kapanış başladıysa
            // veya önceki işlem bitmediyse yeni işlem başlatma.
            if (!kontrolAcik || kapaniyor || (kontrolGorevi != null && !kontrolGorevi.IsCompleted))
            {
                return;
            }
            timerKomut.Stop();
            try
            {
                lblDurum.Text = Mesajlar.KomutServisiKontrolEdiliyor;
                kontrolGorevi = Task.Run( () => makineKomutServisi.KontrolEt());
                bool sonucKaydedildi = await kontrolGorevi;
                if (kapaniyor)
                {
                    return;
                }
                if (makineKomutServisi.AltyapiHatasiVar)
                {
                    kontrolAcik = false;
                    lblDurum.Text = Mesajlar.KomutServisiHataIleDurduruldu;
                    // Servisin hazırladığı hata detayını göster.
                    LogEkle(makineKomutServisi.SonHata);
                }
                else
                {
                    if (kontrolAcik)
                    {
                        lblDurum.Text = Mesajlar.KomutServisiCalisiyor;
                    }
                    else
                    {
                        lblDurum.Text = Mesajlar.KomutServisiDurduruldu;
                    }


                    if (sonucKaydedildi)
                    {
                        LogEkle(Mesajlar.KomutServisiSonucKaydedildi);
                    }
                    else
                    {
                        LogEkle(DateTime.Now.ToString("HH:mm:ss") + " - " + Mesajlar.KomutServisiKontrolTamamlandi);
                    }
                }
            }
            catch (Exception exception)
            {
                kontrolAcik = false;

                if (!kapaniyor)
                {
                    lblDurum.Text = Mesajlar.KomutServisiHataIleDurduruldu;
                    LogEkle(Mesajlar.SistemselHata(exception.Message));
                }
            }
            finally
            {
                // İş bitince kullanıcı hâlâ kontrol istiyorsa timerı yeniden başlat.
                if (kontrolAcik && !kapaniyor && !IsDisposed && !Disposing)
                {
                    timerKomut.Start();
                }
            }

        }

        private async void FrmKomutServisi_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (kapanisaIzin)
                return;

            // Şimdilik kapanmayı durdur.
            e.Cancel = true;

            if (kapaniyor)
                return;

            kapaniyor = true;
            kontrolAcik = false;
            timerKomut.Stop();

            btnBaslat.Enabled = false;
            btnDurdur.Enabled = false;
            lblDurum.Text = Mesajlar.KomutServisiKapaniyor;

            try
            {
                // Başlamış bir iş varsa bitmesini bekle.
                if (kontrolGorevi != null)
                    await kontrolGorevi;
            }
            catch (Exception ex)
            {
                LogEkle(Mesajlar.SistemselHata(ex.Message));
            }
            finally
            {
                makineKomutServisi.LogOlustu -= MakineKomutServisi_LogOlustu;
                makineKomutServisi.Dispose();

                kapanisaIzin = true;
                Close();
            }
        }

        private void FrmKomutServisi_Load(object sender, EventArgs e)
        {
            btnDurdur.Enabled = false;
        }
    }
}
