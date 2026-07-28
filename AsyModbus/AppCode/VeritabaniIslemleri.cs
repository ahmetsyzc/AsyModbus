using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace AsyModbus.AppCode
{
    public class VeritabaniIslemleri
    {
        SqlConnection sqlConnection;
        SqlBaglanti sqlBaglanti = new SqlBaglanti();

        public void Baslat()
        {
            sqlConnection = sqlBaglanti.SqlBaglan();
        }
        public SqlConnection BaglantiGetir()
        {
            return sqlConnection;
        }
        public void Bitir()
        {
            if (sqlConnection != null)
            {
                sqlConnection.Close();
            }
        }
    }
}