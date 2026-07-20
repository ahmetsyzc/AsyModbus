using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AsyModbus.AppCode;
using System.Data.SqlClient;

namespace AsyModbus.Pages
{
    public partial class KullaniciDüzenle : System.Web.UI.Page
    {
        SqlBaglanti sqlBaglanti = new SqlBaglanti();
        string id;

        protected void Page_Load(object sender, EventArgs e)
        {
            id = Request.QueryString["kullanici_id"];

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

                    //Verileri Getirme
                    SqlCommand sqlCommand1 = new SqlCommand("select * from kullanicilar where id=@p1", sqlBaglanti.SqlBaglan());
                    sqlCommand1.Parameters.AddWithValue("@p1", id);
                    SqlDataReader sqlDataReader1 = sqlCommand1.ExecuteReader();
                    while (sqlDataReader1.Read())
                    {
                        txtID.Text = sqlDataReader1["id"].ToString();
                        txtAd.Text = sqlDataReader1["ad"].ToString();
                        txtSoyad.Text = sqlDataReader1["soyad"].ToString();
                        txtTckno.Text = sqlDataReader1["tckno"].ToString();
                        txtMail.Text = sqlDataReader1["mail"].ToString();
                        txtSifre.Text = sqlDataReader1["sifre"].ToString();
                        txtCepNo.Text = sqlDataReader1["cep_no"].ToString();
                        txtDogumTarihi.Text = Convert.ToDateTime(sqlDataReader1["dogum_tarih"]).ToString("yyyy-MM-dd");
                        imgProfil.ImageUrl = "~/" + sqlDataReader1["resim_yol"].ToString();
                        DropDownList1.SelectedValue = sqlDataReader1["roller_id"].ToString();
                    }
                    sqlBaglanti.SqlBaglan().Close();
                }
                catch (Exception ex)
                {
                    lblUyari.Text = "Veriler Yüklenemedi " + ex.Message;
                }
            }
        }

        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAd.Text) ||
 string.IsNullOrWhiteSpace(txtSoyad.Text) ||
 string.IsNullOrWhiteSpace(txtTckno.Text) ||
 string.IsNullOrWhiteSpace(txtMail.Text) ||
 string.IsNullOrWhiteSpace(txtCepNo.Text))
            {
                lblUyari.Text = "Lütfen tüm alanları doldurunuz.";
                return;
            }

            if (txtAd.Text.Trim().Length <= 2 || txtSoyad.Text.Trim().Length <= 2)
            {
                lblUyari.Text = "Ad ve Soyad en az 2 karakter olmalıdır.";
                return;
            }

            try
            {

                // Eski resmi korumak için mevcut ImageUrl'i alıyoruz
                string resimYolu = imgProfil.ImageUrl.Replace("~/", "");

                // Eğer kullanıcı yeni fotoğraf seçtiyse
                if (FileUpload1.HasFile)
                {
                    string eskiResimYolu = resimYolu;

                    string dosyaAdi = FileUpload1.FileName;

                    FileUpload1.SaveAs(Server.MapPath("~/Files/Images/Kullanicilar/") + dosyaAdi);

                    resimYolu = "Files/Images/Kullanicilar/" + dosyaAdi;

                    imgProfil.ImageUrl = "~/" + resimYolu;

                    if (!string.IsNullOrEmpty(resimYolu))
                    {
                        string fizikselYol = Server.MapPath("~/" + eskiResimYolu);

                        if (System.IO.File.Exists(fizikselYol))
                        {
                            System.IO.File.Delete(fizikselYol);
                        }
                    }

                }

                SqlCommand sqlCommand = new SqlCommand(
                    "Update Kullanicilar set " +
                    "ad=@p1, " +
                    "soyad=@p2, " +
                    "tckno=@p3, " +
                    "mail=@p4, " +
                    "cep_no=@p5, " +
                    "dogum_tarih=@p6, " +
                    "roller_id=@p7, " +
                    "resim_yol=@p8 " +
                    "where id=@p9", sqlBaglanti.SqlBaglan());

                sqlCommand.Parameters.AddWithValue("@p1", txtAd.Text);
                sqlCommand.Parameters.AddWithValue("@p2", txtSoyad.Text);
                sqlCommand.Parameters.AddWithValue("@p3", txtTckno.Text);
                sqlCommand.Parameters.AddWithValue("@p4", txtMail.Text);
                sqlCommand.Parameters.AddWithValue("@p5", txtCepNo.Text);
                sqlCommand.Parameters.AddWithValue("@p6", Convert.ToDateTime(txtDogumTarihi.Text));
                sqlCommand.Parameters.AddWithValue("@p7", DropDownList1.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@p8", resimYolu);
                sqlCommand.Parameters.AddWithValue("@p9", id);

                sqlCommand.ExecuteNonQuery();
                sqlBaglanti.SqlBaglan().Close();

                lblUyari.Text = "Personel bilgileri güncellendi.";
            }
            catch (Exception ex)
            {
                lblUyari.Text = "Hata: " + ex.Message;
            }
        }

        protected void btnSil_Click(object sender, EventArgs e)
        {


            try
            {

                // resim yolunu al
                SqlCommand sqlCommand1 = new SqlCommand(
                    "SELECT resim_yol FROM kullanicilar WHERE id=@p1",
                    sqlBaglanti.SqlBaglan());

                sqlCommand1.Parameters.AddWithValue("@p1", id);

                string resimYolu = sqlCommand1.ExecuteScalar().ToString();

                sqlBaglanti.SqlBaglan().Close();

                // klasörden sil
                if (!string.IsNullOrEmpty(resimYolu))
                {
                    string fizikselYol = Server.MapPath("~/" + resimYolu);

                    if (System.IO.File.Exists(fizikselYol))
                    {
                        System.IO.File.Delete(fizikselYol);
                    }
                }

                SqlCommand sqlCommand = new SqlCommand("delete from kullanicilar where id=@p1", sqlBaglanti.SqlBaglan());
                sqlCommand.Parameters.AddWithValue("@p1", id);
                sqlCommand.ExecuteNonQuery();
                sqlBaglanti.SqlBaglan().Close();

                Response.Redirect("~/Pages/KullaniciListele.aspx");
            }
            catch (Exception ex)
            {
                lblUyari.Text = "Hata: " + ex.Message;
            }
        }
    }
}