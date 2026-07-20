using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AsyModbus.AppCode;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace AsyModbus.Pages
{
    public partial class KullaniciEkle : System.Web.UI.Page
    {
        SqlBaglanti sqlBaglanti = new SqlBaglanti();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                txtDogumTarihi.Attributes["max"] = DateTime.Now.ToString("yyyy-MM-dd");

                try
                {
                    //Rol Listele
                    SqlCommand sqlCommand = new SqlCommand("select * from Roller", sqlBaglanti.SqlBaglan());
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                    DropDownList1.DataTextField = "ad";
                    DropDownList1.DataValueField = "id";

                    DropDownList1.DataSource = sqlDataReader;
                    DropDownList1.DataBind();

                    sqlBaglanti.SqlBaglan().Close();
                }
                catch (Exception ex)
                {
                    lblUyari.Text = "Sistemsel Hata " + ex.Message;
                }
            }
        }

        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAd.Text) ||
   string.IsNullOrWhiteSpace(txtSoyad.Text) ||
   string.IsNullOrWhiteSpace(txtTckno.Text) ||
   string.IsNullOrWhiteSpace(txtMail.Text) ||
   string.IsNullOrWhiteSpace(txtCepNo.Text) ||
   string.IsNullOrWhiteSpace(txtDogumTarihi.Text) ||
   !FileUpload1.HasFile)
            {
                lblUyari.Text = "Lütfen tüm alanları doldurunuz.";
                return;
            }

            if (txtTckno.Text.Trim().Length!=11)
            {
                lblUyari.Text = "Tc Kimlik No 11 Hane Olmalıdır!";
                return;
            }

            //trim() sadece baştaki ve sondaki boşlukları siler
            // Telefon numarasındaki rakam olmayan tüm karakterleri siler.
            // Örnek:
            // (532)-555-1234  →  5325551234
            // \D = Rakam olmayan karakterler
            // "" = Bulduğu rakam olmayan karakterleri siler.
            string telefon = Regex.Replace(txtCepNo.Text, @"\D", "");

            // Telefon numarası sadece rakamlardan oluşacağı için
            // uzunluğu tam 10 hane olmalıdır.
            if (telefon.Length != 10)
            {
                lblUyari.Text = "Telefon numarası 10 haneli olmalıdır!";
                return;
            }


            //Şifre Belirleme
            Random random = new Random();
            string ad = txtAd.Text.Trim();
            string soyad = txtSoyad.Text.Trim();
            if (ad.Length >= 2 && soyad.Length >= 2)
            {
                string sifre =
                    ad.Substring(0, 2) +
                    soyad.Substring(0, 2) +
                    "@" +
                    random.Next(10000, 100000);
                txtSifre.Text = sifre;
            }
            else
            {
                lblUyari.Text = "Ad ve Soyad en az 2 karakter olmalıdır.";
                return;
            }

            try
            {
                SqlConnection sqlConnection = sqlBaglanti.SqlBaglan();

                //personel var mı kontrol ediyoruz
                if (KayitVarMi(sqlConnection,"mail",txtMail.Text))
                {
                    lblUyari.Text = "Bu Mail Hesabı Sistemde Kayıtlı !";
                    sqlConnection.Close();
                    return;
                }
                if (KayitVarMi(sqlConnection, "cep_no", txtMail.Text))
                {
                    lblUyari.Text = "Bu Telefon Numarası Hesabı Sistemde Kayıtlı !";
                    sqlConnection.Close();
                    return;
                }

                // Resmin adını al - Resmi proje klasörüne kaydet - Veritabanına kaydedilecek yol
                string dosyaAdi = FileUpload1.FileName;
                FileUpload1.SaveAs(Server.MapPath("~/Files/Images/Kullanicilar/") + dosyaAdi);
                string resimYolu = "Files/Images/Kullanicilar/" + dosyaAdi;

                SqlCommand komut = new SqlCommand("insert into kullanicilar (sifre,ad,soyad,tckno,mail,cep_no,dogum_tarih,roller_id, resim_yol) values (@t1,@t2,@t3,@t4,@t5,@t6,@t7,@t8,@t9)", sqlConnection);
                komut.Parameters.AddWithValue("@t1", txtSifre.Text);
                komut.Parameters.AddWithValue("@t2", txtAd.Text);
                komut.Parameters.AddWithValue("@t3", txtSoyad.Text);
                komut.Parameters.AddWithValue("@t4", txtTckno.Text);
                komut.Parameters.AddWithValue("@t5", txtMail.Text);
                komut.Parameters.AddWithValue("@t6", txtCepNo.Text);
                komut.Parameters.AddWithValue("@t7", txtDogumTarihi.Text);
                komut.Parameters.AddWithValue("@t8", DropDownList1.SelectedValue);
                komut.Parameters.AddWithValue("@t9", resimYolu);
                komut.ExecuteNonQuery();
                sqlConnection.Close();

                Response.Redirect("~/Pages/KullaniciListele.aspx");
            }
            catch (Exception ex)
            {
                lblUyari.Text = "Hata: " + ex.Message;
            }
        }

        bool KayitVarMi(SqlConnection sqlConnection, string alan, string deger)
        {
            SqlCommand sqlCommand = new SqlCommand("select COUNT(*) from kullanicilar where " + alan + " = @deger ", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@deger", deger);
            return Convert.ToInt32(sqlCommand.ExecuteScalar()) > 0;
        }
    }
}