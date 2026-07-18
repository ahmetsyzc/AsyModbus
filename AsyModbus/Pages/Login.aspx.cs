using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using AsyModbus.AppCode;


namespace AsyModbus.Pages
{
    public partial class Login : System.Web.UI.Page
    {
        SqlBaglanti sqlBaglanti = new SqlBaglanti();

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        protected void btnGiris_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMail.Text) ||
                string.IsNullOrWhiteSpace(txtSifre.Text))
            {
                lblUyari.Text = "Mail ve Şifre boş bırakılamaz.";
                return;
            }

            try
            {
                SqlCommand sqlCommand = new SqlCommand("select kullanici_id , kullanici_ad , kullanici_soyad , kullanici_sifre from kullanicilar where kullanici_mail=@p1", sqlBaglanti.SqlBaglan() );
                sqlCommand.Parameters.AddWithValue("@p1", txtMail.Text); 
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.Read())
                {
                    if (sqlDataReader["kullanici_sifre"].ToString()==txtSifre.Text)
                    {
                        Session["KullaniciID"] = sqlDataReader["kullanici_id"].ToString();

                        Session["AktifKullanici"] = sqlDataReader["kullanici_ad"].ToString() + " "
                            + sqlDataReader["kullanici_soyad"].ToString();

                        Response.Redirect("~/Default.aspx",false);
                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    }
                    else
                    {
                        lblUyari.Text = "Hatalı Şifre !";
                    }
                }
                else
                {
                    lblUyari.Text = "Kullanıcı Bulunamadı !";
                }

                sqlDataReader.Close();
                sqlBaglanti.SqlBaglan().Close();
            }
            catch (Exception ex)
            {
               lblUyari.Text = "Sistemsel Hata ! " + ex.Message;
            }
        }
    }
}