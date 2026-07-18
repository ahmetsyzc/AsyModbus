using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using AsyModbus.AppCode;

namespace AsyModbus.Pages
{
    public partial class SifreSifirlama : System.Web.UI.Page
    {
        SqlBaglanti sqlBaglanti = new SqlBaglanti();
        string yeniSifre;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSifreSifirla_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMail.Text) ||
                string.IsNullOrWhiteSpace(txtCepNo.Text))
            {
                lblUyari.Text = "Mail ve Cep No boş bırakılamaz.";
                return;
            }
            try
            {
                SqlCommand sqlCommand = new SqlCommand("select kullanici_cep_no , kullanici_ad , kullanici_soyad from Kullanicilar where kullanici_mail=@p1", sqlBaglanti.SqlBaglan());
                sqlCommand.Parameters.AddWithValue("@p1", txtMail.Text);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.Read())
                {
                    if (sqlDataReader["kullanici_cep_no"].ToString() == txtCepNo.Text)
                    {
                        Random random = new Random();
                        yeniSifre= sqlDataReader["kullanici_ad"].ToString().Substring(0, 2) +
                   sqlDataReader["kullanici_soyad"].ToString().Substring(0, 2) +
                   "@" +
                   random.Next(10000, 100000);

                        SqlCommand sifreGüncelle = new SqlCommand("update kullanicilar set kullanici_sifre=@p1 where kullanici_mail=@p2", sqlBaglanti.SqlBaglan());
                        sifreGüncelle.Parameters.AddWithValue("@p1", yeniSifre);
                        sifreGüncelle.Parameters.AddWithValue("@p2", txtMail.Text);
                        sifreGüncelle.ExecuteNonQuery();
                        sqlBaglanti.SqlBaglan().Close();

                        lblUyari.Text = "Yeni şifreniz = "+yeniSifre;
                    }
                    else
                    {
                        lblUyari.Text = "Kullanıcı Bulunamadı !";
                    }
                }
                else
                {
                    lblUyari.Text = "Hatalı Mail !";
                }

                sqlDataReader.Close();
                sqlBaglanti.SqlBaglan().Close();
            }
            catch (Exception ex)
            {

                lblUyari.Text="Sistemsel Hata "+ex.Message;
            }
        }
    }
}