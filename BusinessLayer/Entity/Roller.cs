using System;
using System.Data;
using System.Web.UI.WebControls;
using BusinessLayer.Interface;
using BusinessLayer.Work;

namespace BusinessLayer.Entity
{
    public class Roller : OrtakAlanlar, IOrtakMetotlar
    {
        VeritabaniIslemleri veritabaniIslemleri;


        #region Sabitler

        public const string C_Tablo = "dbo.Roller";

        public const string C_Sp_TumKayitGetir = "dbo.SP_Roller_TUM_KAYIT_GETIR";

        public const string C_Sutun_ad = "ad";

        #endregion


        #region Nesneler

        public string Ad { get; set; }

        #endregion


        #region Metotlar


        public Roller(VeritabaniIslemleri veritabaniIslemleri)
        {
            this.veritabaniIslemleri = veritabaniIslemleri;
        }

        public bool Ekle()
        {
            return false;
        }
        public bool Sil()
        {
            return false;
        }
        public bool Guncelle()
        {
            return false;
        }

        public DataTable TumKayitGetir()
        {
            return veritabaniIslemleri.TabloGetir(C_Sp_TumKayitGetir);
        }

        public void Listele(DropDownList dropDownList)
        {
            DataTable dataTable = TumKayitGetir();

            if (dataTable == null)
            {
                throw new Exception("Rol kayıtları getirilemedi.");
            }

            dropDownList.DataTextField = C_Sutun_ad;
            dropDownList.DataValueField = C_Sutun_id;
            dropDownList.DataSource = dataTable;
            dropDownList.DataBind();
        }

        #endregion

    }
}