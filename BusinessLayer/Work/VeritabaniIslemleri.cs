using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using BusinessLayer.Work;

namespace BusinessLayer.Work
{
    public class VeritabaniIslemleri
    {

        SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlBaglanti"].ConnectionString);
        private List<SqlParameter> parametreler = new List<SqlParameter>();

        #region Metotlar

        public SqlConnection Baslat()
        {
            sqlConnection.Open();
            return sqlConnection;
        }

        public void Bitir()
        {
            if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
            {
                sqlConnection.Close();
            }
        }

        public int Calistir(string spAdi)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand(spAdi, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                foreach (SqlParameter sqlParameter in parametreler)
                {
                    sqlCommand.Parameters.Add(sqlParameter);
                }

                return sqlCommand.ExecuteNonQuery();
            }
            finally
            {
                parametreler.Clear();
            }
        }

        public void ParametreEkle(string parametreAdi,object parametreDegeri)
        {
            SqlParameter sqlParameter = new SqlParameter("@" + parametreAdi, parametreDegeri);
            parametreler.Add(sqlParameter);
        }

        public DataTable TabloGetir(string spAdi)
        {
            SqlCommand sqlCommand = new SqlCommand(spAdi, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            foreach (SqlParameter sqlParameter in parametreler)
            {
                sqlCommand.Parameters.Add(sqlParameter);
            }

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            DataTable dataTable = new DataTable();
            sqlDataAdapter.Fill(dataTable);

            parametreler.Clear();

            return dataTable;
        }

        public DataRow SatirGetir(string spAdi)
        {
            DataTable dataTable = TabloGetir(spAdi);

            if (dataTable.Rows.Count>0)
            {
                return dataTable.Rows[0];
            }

            return null;
        }

        public object DegerGetir(string spAdi)
        {
            SqlCommand sqlCommand = new SqlCommand(spAdi, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            foreach (SqlParameter sqlParameter in parametreler)
            {
                sqlCommand.Parameters.Add(sqlParameter);
            }

            object deger = sqlCommand.ExecuteScalar();

            parametreler.Clear();

            return deger;
        }

        #endregion
    }
}