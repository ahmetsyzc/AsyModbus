using System.Web.UI;
using System.Web.UI.WebControls;

public class Mesaj
{
    public enum MesajTurleri
    {
        SUCCESS,
        FAIL,
        INFO,
        WARNING
    }

    public static void Ver(string mesajMetni, MesajTurleri mesajTuru, MasterPage master)
    {
        try
        {
            Label lblSuccess = (Label)master.FindControl("lbl_success");
            Label lblWarning = (Label)master.FindControl("lbl_warning");
            Label lblInfo = (Label)master.FindControl("lbl_info");
            Label lblError = (Label)master.FindControl("lbl_error");

            lblSuccess.Text = "";
            lblWarning.Text = "";
            lblInfo.Text = "";
            lblError.Text = "";

            if (mesajTuru == MesajTurleri.SUCCESS)
            {
                lblSuccess.Text = mesajMetni;
            }
            else if (mesajTuru == MesajTurleri.FAIL)
            {
                lblError.Text = mesajMetni;
            }
            else if (mesajTuru == MesajTurleri.INFO)
            {
                lblInfo.Text = mesajMetni;
            }
            else if (mesajTuru == MesajTurleri.WARNING)
            {
                lblWarning.Text = mesajMetni;
            }
        }
        catch
        {

        }
    }

    public static void Ver(string mesajMetni, MesajTurleri mesajTuru, Page page)
    {
        try
        {
            Label lblSuccess = (Label)page.FindControl("lbl_success");
            Label lblWarning = (Label)page.FindControl("lbl_warning");
            Label lblInfo = (Label)page.FindControl("lbl_info");
            Label lblError = (Label)page.FindControl("lbl_error");

            lblSuccess.Text = "";
            lblWarning.Text = "";
            lblInfo.Text = "";
            lblError.Text = "";

            if (mesajTuru == MesajTurleri.SUCCESS)
            {
                lblSuccess.Text = mesajMetni;
            }
            else if (mesajTuru == MesajTurleri.FAIL)
            {
                lblError.Text = mesajMetni;
            }
            else if (mesajTuru == MesajTurleri.INFO)
            {
                lblInfo.Text = mesajMetni;
            }
            else if (mesajTuru == MesajTurleri.WARNING)
            {
                lblWarning.Text = mesajMetni;
            }
        }
        catch
        {

        }
    }
}