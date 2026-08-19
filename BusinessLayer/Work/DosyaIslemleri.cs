using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class DosyaIslemleri
{

    #region Sabitler

    public const string C_Klasor_Kullanicilar = "~/Files/Images/Kullanicilar/";


    #endregion

    #region Metotlar

    public bool ResimUzantisiGecerliMi(string dosyaAdi)
    {
        string uzanti = System.IO.Path.GetExtension(dosyaAdi).ToLower();

        if (uzanti == ".jpg" ||
            uzanti == ".jpeg" ||
            uzanti == ".png")
        {
            return true;
        }

        return false;
    }

    public string ResimKaydet(string klasorYolu, System.Web.HttpPostedFile dosya)
    {
        string uzanti = System.IO.Path.GetExtension(dosya.FileName).ToLower();
        string dosyaAdi = Guid.NewGuid().ToString() + uzanti;

        string fizikselKlasor =  System.Web.HttpContext.Current.Server.MapPath(klasorYolu);
        string fizikselYol = System.IO.Path.Combine(fizikselKlasor, dosyaAdi);

        dosya.SaveAs(fizikselYol);

        if (!System.IO.File.Exists(fizikselYol))
        {
            throw new Exception("Resim sisteme yüklenemedi.");
        }

        return klasorYolu.Replace("~/", "") + dosyaAdi;
    }

    public bool ResimSil(string resimYolu)
    {
        if (string.IsNullOrEmpty(resimYolu))
        {
            return false;
        }

        string fizikselYol = System.Web.HttpContext.Current.Server.MapPath("~/" + resimYolu);

        if (!System.IO.File.Exists(fizikselYol))
        {
            return false;
        }

        System.IO.File.Delete(fizikselYol);
        return true;
    }

    #endregion
}

