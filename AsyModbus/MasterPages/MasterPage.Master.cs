using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AsyModbus.MasterPages
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Sessionlar sessionlar = new Sessionlar();
            CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;

            if (currentInfo == null || currentInfo.LoginYapildiMi == false)
            {
                Response.Redirect("~/Pages/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!RolAktifMi(currentInfo.RolId))
            {
                Session.Clear();
                Session["RolPasifMesaji"] = Mesajlar.RolPasifOturumSonlandirildi;
                Response.Redirect("~/Pages/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!KullaniciAktifMi(currentInfo.KullaniciId))
            {
                Session.Clear();
                Session["HesapPasifMesaji"] = Mesajlar.KullaniciPasifOturumSonlandirildi;
                Response.Redirect("~/Pages/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            hlAdSoyad.Text = currentInfo.Ad + " " + currentInfo.Soyad;

            if (Session["YetkiUyariMesaji"] != null)
            {
                Mesaj.Ver(Session["YetkiUyariMesaji"].ToString(), Mesaj.MesajTurleri.WARNING, this);
                Session.Remove("YetkiUyariMesaji");
            }

            if (Page.IsPostBack == false)
            {
                SayfaErisiminiKontrolEt();
                MenuYetkileriniAyarla();
            }
        }

        private void MenuYetkileriniAyarla()
        {
            menuKullaniciListele.Visible = IslemYetki.Kontrol(Sayfalar.KullaniciListele, YetkiIslemTurleri.Getirme);
            menuKullaniciEkle.Visible = IslemYetki.Kontrol(Sayfalar.KullaniciEkle, YetkiIslemTurleri.Getirme);
            menuMakineListele.Visible = IslemYetki.Kontrol(Sayfalar.MakineListele, YetkiIslemTurleri.Getirme);
            menuMakineEkle.Visible = IslemYetki.Kontrol(Sayfalar.MakineEkle, YetkiIslemTurleri.Getirme);
            menuLogListele.Visible = IslemYetki.Kontrol(Sayfalar.LogListele, YetkiIslemTurleri.Getirme);
            menuRolIslemleri.Visible = IslemYetki.Kontrol(Sayfalar.RolIslemleri, YetkiIslemTurleri.Getirme);
            menuRolYetkileri.Visible = IslemYetki.Kontrol(Sayfalar.RolYetkileri, YetkiIslemTurleri.Getirme);
        }

        private void SayfaErisiminiKontrolEt()
        {
            string sayfaAdi = System.IO.Path.GetFileName(Request.Url.AbsolutePath);

            if (sayfaAdi == Sayfalar.Default || sayfaAdi == Sayfalar.ProfilDuzenle)
            {
                return;
            }

            if (!IslemYetki.Kontrol(sayfaAdi, YetkiIslemTurleri.Getirme))
            {
                Session["YetkiUyariMesaji"] = Mesajlar.SayfaErisimYetkisiYok;
                Response.Redirect("~/Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        private bool RolAktifMi(int rolId)
        {
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Roller roller = new Roller(veritabaniIslemleri);
                roller.Id = rolId;
                return roller.Doldur();
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }

        private bool KullaniciAktifMi(int kullaniciId)
        {
            VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();
            try
            {
                veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMSIZ);
                Kullanicilar kullanicilar = new Kullanicilar(veritabaniIslemleri);
                kullanicilar.Id = kullaniciId;
                return kullanicilar.Doldur();
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }

        protected void btnCikis_Click(object sender, EventArgs e)
        {
            LogIslemleri.IslemKaydet(Mesajlar.LogSistem, Mesajlar.LogSistemdenCikis, LogIslemTipleri.Exit, Mesajlar.SistemdenBasariliCikisYapildi);

            Sessionlar sessionlar = new Sessionlar();
            sessionlar.Current._CurrentInfo = null;
            Session.Clear();
            Session.Abandon();

            Response.Redirect("~/Pages/Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}