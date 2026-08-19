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
            lblAdSoyad.Text = currentInfo.Ad + " " + currentInfo.Soyad;
        }

        protected void btnCikis_Click(object sender, EventArgs e)
        {
            Sessionlar sessionlar = new Sessionlar();
            sessionlar.Current._CurrentInfo = null;
            Session.Clear();
            Session.Abandon();

            Response.Redirect("~/Pages/Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}