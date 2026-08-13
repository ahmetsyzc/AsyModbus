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
        private SqlTransaction sqlTransaction;

        #region Metotlar

        public enum IslemTip
        {
            BAGIMSIZ,
            BAGIMLI
        }

        public SqlConnection Baslat(IslemTip islemTip)
        {
            if (sqlConnection.State == ConnectionState.Closed)
            {
                sqlConnection.Open();
            }

            if (islemTip == IslemTip.BAGIMLI && sqlTransaction == null)
            {
                sqlTransaction = sqlConnection.BeginTransaction();
            }

            return sqlConnection;
        }

        public void Uygula()
        {
            if (sqlTransaction != null)
            {
                sqlTransaction.Commit();
                sqlTransaction = null;
            }
        }

        public void GeriAl()
        {
            if (sqlTransaction != null)
            {
                sqlTransaction.Rollback();
                sqlTransaction = null;
            }
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

                if (sqlTransaction != null)
                {
                    sqlCommand.Transaction = sqlTransaction;
                }

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
            try
            {
                SqlCommand sqlCommand = new SqlCommand(spAdi, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                if (sqlTransaction != null)
                {
                    sqlCommand.Transaction = sqlTransaction;
                }

                foreach (SqlParameter sqlParameter in parametreler)
                {
                    sqlCommand.Parameters.Add(sqlParameter);
                }

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);

                return dataTable;
            }
            finally
            {
                parametreler.Clear();
            }
            
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
            try
            {
                SqlCommand sqlCommand = new SqlCommand(spAdi, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                if (sqlTransaction != null)
                {
                    sqlCommand.Transaction = sqlTransaction;
                }

                foreach (SqlParameter sqlParameter in parametreler)
                {
                    sqlCommand.Parameters.Add(sqlParameter);
                }

                object deger = sqlCommand.ExecuteScalar();

                return deger;
            }
            finally
            {
                parametreler.Clear();
            }
        }



        #endregion
    }
}