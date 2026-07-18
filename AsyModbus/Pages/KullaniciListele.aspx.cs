using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using AsyModbus.AppCode;
using System.Data.SqlClient;

namespace AsyModbus.Pages
{
    public partial class KullaniciListele : System.Web.UI.Page
    {
        SqlBaglanti sqlBaglanti = new SqlBaglanti();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack == false)
            {
                //Kullanıcıları Listele
                SqlCommand sqlCommand = new SqlCommand("select * from kullanicilar", sqlBaglanti.SqlBaglan());
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                Repeater1.DataSource = dataTable;
                Repeater1.DataBind();
                sqlBaglanti.SqlBaglan().Close();
            }
        }

        protected void btnGuncelle_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            string id = button.CommandArgument;

            Response.Redirect("~/Pages/KullaniciDüzenle.aspx?kullanici_id=" + id);
        }
    }
}