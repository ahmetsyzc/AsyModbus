using System;
using System.Web;

namespace AsyModbus
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RolYetkiSeeder.Calistir();
        }
    }
}
