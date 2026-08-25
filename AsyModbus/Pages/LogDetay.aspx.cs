using System;

namespace AsyModbus.Pages
{
    public partial class LogDetay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                int logId;

                if (!int.TryParse(Request.QueryString["log_id"], out logId))
                {
                    Mesaj.Ver(Mesajlar.LogBulunamadi, Mesaj.MesajTurleri.FAIL, Master);
                    return;
                }

                VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

                try
                {
                    veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);

                    Loglar loglar = new Loglar(veritabaniIslemleri);
                    loglar.Id = logId;

                    if (loglar.Doldur())
                    {
                        txtId.Text = loglar.Id.ToString();
                        txtEkleyenId.Text = loglar.EkleyenId.ToString();
                        txtEkleyenIp.Text = loglar.EkleyenIp;
                        txtEklenmeTarihi.Text = loglar.EklenmeTarih.ToString("dd.MM.yyyy HH:mm:ss");
                        txtTabloAd.Text = loglar.TabloAd;
                        txtIslemAd.Text = loglar.IslemAd;
                        txtIslemTip.Text = loglar.IslemTip;
                        txtUrl.Text = loglar.Url;
                        txtDetay.Text = loglar.Detay;
                    }
                    else
                    {
                        Mesaj.Ver(Mesajlar.LogBulunamadi, Mesaj.MesajTurleri.FAIL, Master);
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
}