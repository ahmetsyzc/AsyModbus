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

            if (Session["KullaniciId"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx");
            }


            if (Session["AktifKullanici"]!=null)
            {
                lblAdSoyad.Text = Session["AktifKullanici"].ToString();
            }
            else
            {
                Response.Redirect("~/Pages/Login.aspx",false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
        }

        protected void btnCikis_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            Response.Redirect("~/Pages/Login.aspx");
        }
    }
}