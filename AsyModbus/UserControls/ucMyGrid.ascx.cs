using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AsyModbus.UserControls
{
    public partial class ucMyGrid : System.Web.UI.UserControl
    {
        public const string C_Command_Ac = "AC";
        private string detayURL;
        public string DetayURL
        {
            get { return detayURL; }
            set { detayURL = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void grdMyGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == C_Command_Ac )
            {
                string id = e.CommandArgument.ToString();
                Response.Redirect(DetayURL + id,false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        public void VeriBagla(DataTable dataTable)
        {
            grdMyGrid.DataSource = dataTable;
            grdMyGrid.DataBind();

            lblKayitSayisi.Text = "Toplam " + dataTable.Rows.Count.ToString() + " Adet Kayıt Listelendi"; 
        }

        public void Temizle()
        {
            grdMyGrid.DataSource = null;
            grdMyGrid.DataBind();
        }

        public void KolonEkle(string veriAlani, string baslik)
        {
            foreach (DataControlField dataControlField in grdMyGrid.Columns)
            {
                BoundField mevcutBoundField = dataControlField as BoundField;

                if (mevcutBoundField != null && mevcutBoundField.DataField == veriAlani)
                {
                    return;
                }
            }

            BoundField boundField = new BoundField();

            boundField.DataField = veriAlani;
            boundField.HeaderText = baslik;

            grdMyGrid.Columns.Add(boundField);
        }
    }

}