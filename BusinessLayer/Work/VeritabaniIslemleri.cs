using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System;

public class VeritabaniIslemleri
{

    SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlBaglanti"].ConnectionString);
    private List<SqlParameter> parametreler = new List<SqlParameter>();
    private SqlTransaction sqlTransaction;
    public string SpAdi;

    #region Metotlar

    public enum IslemTip
    {
        BAGIMSIZ,
        BAGIMLI
    }

    private int etkilenenKayitSayisi;

    public int EtkilenenKayitSayisi
    {
        get { return etkilenenKayitSayisi; }
        set { etkilenenKayitSayisi = value; }
    }

    private void HatalariSil()
    {
        etkilenenKayitSayisi = 0;
    }

    public SqlConnection Baslat(IslemTip islemTip)
    {
        HatalariSil();

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
        HatalariSil();

        if (sqlTransaction != null)
        {
            sqlTransaction.Commit();
            sqlTransaction = null;
        }
    }

    public void GeriAl()
    {
        HatalariSil();

        if (sqlTransaction != null)
        {
            sqlTransaction.Rollback();
            sqlTransaction = null;
        }
    }

    public void Bitir()
    {
        HatalariSil();

        if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
        {
            sqlConnection.Close();
        }
    }

    public bool Calistir()
    {
        try
        {
            HatalariSil();
            SqlCommand sqlCommand = new SqlCommand(SpAdi, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            if (sqlTransaction != null)
            {
                sqlCommand.Transaction = sqlTransaction;
            }

            foreach (SqlParameter sqlParameter in parametreler)
            {
                sqlCommand.Parameters.Add(sqlParameter);
            }

            etkilenenKayitSayisi = sqlCommand.ExecuteNonQuery();
            if (etkilenenKayitSayisi > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        finally
        {
            parametreler.Clear();
        }
    }

    public void ParametreEkle(string parametreAdi, object parametreDegeri)
    {

        SqlParameter sqlParameter = new SqlParameter("@" + parametreAdi, parametreDegeri);
        parametreler.Add(sqlParameter);
    }

    public DataTable TabloGetir()
    {
        try
        {
            HatalariSil();

            SqlCommand sqlCommand = new SqlCommand(SpAdi, sqlConnection);
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

    public DataRow SatirGetir()
    {
        HatalariSil();
        DataTable dataTable = TabloGetir();

        if (dataTable.Rows.Count > 0)
        {
            return dataTable.Rows[0];
        }

        return null;
    }

    public int DegerGetir()
    {
        try
        {
            HatalariSil();
            SqlCommand sqlCommand = new SqlCommand(SpAdi, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;

            if (sqlTransaction != null)
            {
                sqlCommand.Transaction = sqlTransaction;
            }

            foreach (SqlParameter sqlParameter in parametreler)
            {
                sqlCommand.Parameters.Add(sqlParameter);
            }

            int deger = Convert.ToInt32(sqlCommand.ExecuteScalar());

            return deger;
        }
        finally
        {
            parametreler.Clear();
        }
    }



    #endregion
}
