using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AsyModbus.UserControls
{
    public partial class ucCepNo : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string CssClass
        {
            get
            {
                return txtCepNo.CssClass;
            }
            set
            {
                txtCepNo.CssClass = value;
            }
        }

        public string Text
        {
            get { return txtCepNo.Text; }
            set { txtCepNo.Text = value; }
        }

        public string CepNoAl()
        {
            return Regex.Replace(txtCepNo.Text, @"\D", "");
        }

        public bool CepNoUygunMu()
        {
            if (string.IsNullOrWhiteSpace(txtCepNo.Text))
            {
                return false;
            }

            if (CepNoAl().Length != 10)
            {
                return false;
            }

            return true;
        }
    }
}