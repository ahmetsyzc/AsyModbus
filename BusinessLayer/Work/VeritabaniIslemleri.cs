using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System;
using System.Diagnostics;
using System.Web;
using System.Reflection;
using System.Text;

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

    private void TransactionAta(SqlCommand sqlCommand)
    {
        if (sqlTransaction != null)
        {
            sqlCommand.Transaction = sqlTransaction;
        }
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

            // Calistir metodunu çağıran entity ve metodu bulunur.
            StackFrame stackFrame = new StackFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();

            string sinifAdi = string.Empty;
            string metotAdi = string.Empty;
            bool loglanacakMi = false;
            LogIslemTipleri islemTipi = LogIslemTipleri.Insert;

            if (methodBase != null && methodBase.DeclaringType != null)
            {
                sinifAdi = methodBase.DeclaringType.Name;
                metotAdi = methodBase.Name;

                string metotAdiKucuk = metotAdi.ToLower();

                if (metotAdiKucuk.Contains("guncelle"))
                {
                    islemTipi = LogIslemTipleri.Update;
                }
                else if (metotAdiKucuk.Contains("sil"))
                {
                    islemTipi = LogIslemTipleri.Delete;
                }
                else if (!metotAdiKucuk.Contains("ekle"))
                {
                    // Ekle / Guncelle / Sil dışındaki metotlar loglanmaz.
                    metotAdi = string.Empty;
                }

                // Log ekleme işlemi tekrar loglanmaz.
                loglanacakMi = sinifAdi.ToLower() != "loglar" && metotAdi != string.Empty;
            }

            DataTable eskiKayitlar = null;
            DataTable yeniKayitlar = null;

            if (loglanacakMi)
            {
                if (islemTipi == LogIslemTipleri.Insert)
                {
                    // Insert işleminde kayıt henüz oluşmadığı için parametrelerden üretilir.
                    yeniKayitlar = LogIcinKayitGetir(parametreler);
                }
                else
                {
                    eskiKayitlar = LogIcinKayitGetir(parametreler);
                }
            }

            SqlCommand sqlCommand = new SqlCommand(SpAdi, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            TransactionAta(sqlCommand);
            foreach (SqlParameter sqlParameter in parametreler)
            {
                sqlCommand.Parameters.Add(sqlParameter);
            }

            etkilenenKayitSayisi = sqlCommand.ExecuteNonQuery();
            if (etkilenenKayitSayisi < 0)
            {
                return false;
            }

            if (!loglanacakMi)
            {
                return true;
            }

            if (islemTipi == LogIslemTipleri.Update)
            {
                yeniKayitlar = LogIcinKayitGetir(parametreler);
            }

            // Ana işlemin parametreleri temizlenir.
            // Loglar.Ekle() aynı VeritabaniIslemleri nesnesini kullanacağı için gereklidir.
            parametreler.Clear();

            string detay = Mesajlar.OtomatikLogDetay(sinifAdi, metotAdi) + Environment.NewLine + Environment.NewLine; 
            if (islemTipi == LogIslemTipleri.Update)
            {
                detay += LogKayitMetniOlustur(eskiKayitlar, Mesajlar.LogEskiKayit);
                detay += Environment.NewLine + Environment.NewLine;
                detay += LogKayitMetniOlustur(yeniKayitlar, Mesajlar.LogYeniKayit);
            }
            else if (islemTipi == LogIslemTipleri.Delete)
            {
                detay += LogKayitMetniOlustur(eskiKayitlar, Mesajlar.LogEskiKayit);
            }
            else
            {
                detay += LogKayitMetniOlustur(yeniKayitlar, Mesajlar.LogYeniKayit);
            }

            int ekleyenId = 0;
            string ekleyenIp = string.Empty;
            string url = string.Empty;

            try
            {
                if (HttpContext.Current != null)
                {
                    url = HttpContext.Current.Request.Url.ToString();
                    ekleyenIp = HttpContext.Current.Request.UserHostAddress;
                }

                Sessionlar sessionlar = new Sessionlar();
                CurrentInfo currentInfo = sessionlar.Current._CurrentInfo;

                if (currentInfo != null && currentInfo.LoginYapildiMi)
                {
                    ekleyenId = currentInfo.KullaniciId;

                    if (!string.IsNullOrEmpty(currentInfo.Ip))
                    {
                        ekleyenIp = currentInfo.Ip;
                    }
                }
            }
            catch
            {
            }
            try
            {
                Loglar loglar = new Loglar(this);

                loglar.Url = url;
                loglar.TabloAd = sinifAdi;
                loglar.IslemAd = Mesajlar.OtomatikLogIslemAdi(sinifAdi, metotAdi);
                loglar.IslemTip = islemTipi.ToString();
                loglar.Detay = detay;
                loglar.AktifMi = true;
                loglar.EkleyenId = ekleyenId;
                loglar.EkleyenIp = ekleyenIp;

                return loglar.Ekle();
            }
            catch
            {
                return true;
            }
        }
        finally
        {
            parametreler.Clear();
        }
    }

    private DataTable LogIcinKayitGetir(List<SqlParameter> parametreListesi)
    {
        try
        {
            string[] procedureParcalari = SpAdi.Replace("dbo.", "").Split('_');

            if (procedureParcalari.Length < 3)
            {
                return null;
            }

            string tabloAdi = procedureParcalari[1];
            string islemAdi = procedureParcalari[2].ToLower();

            DataTable dataTable = new DataTable();

            if (islemAdi.Contains("ekle"))
            {
                foreach (SqlParameter sqlParameter in parametreListesi)
                {
                    dataTable.Columns.Add(sqlParameter.ParameterName.Replace("@", ""));
                }

                DataRow dataRow = dataTable.NewRow();

                foreach (SqlParameter sqlParameter in parametreListesi)
                {
                    dataRow[sqlParameter.ParameterName.Replace("@", "")] = Convert.ToString(sqlParameter.Value);
                }

                dataTable.Rows.Add(dataRow);
                return dataTable;
            }

            SqlParameter idParametresi = null;

            foreach (SqlParameter sqlParameter in parametreListesi)
            {
                if (sqlParameter.ParameterName.ToLower() == "@id")
                {
                    idParametresi = sqlParameter;
                    break;
                }
            }

            if (idParametresi == null)
            {
                return null;
            }

            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM dbo." + tabloAdi + " WHERE id = @id", sqlConnection);
            TransactionAta(sqlCommand);

            sqlCommand.Parameters.AddWithValue("@id", Convert.ToInt32(idParametresi.Value));

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            sqlDataAdapter.Fill(dataTable);
            return dataTable;
        }
        catch
        {
            return null;
        }
    }

    private string LogKayitMetniOlustur(DataTable dataTable, string baslik)
    {
        StringBuilder metin = new StringBuilder();
        metin.Append(baslik).Append(Environment.NewLine);

        if (dataTable == null || dataTable.Rows.Count == 0)
        {
            metin.Append(Mesajlar.LogKayitBulunamadi);
            return metin.ToString();
        }

        foreach (DataRow dataRow in dataTable.Rows)
        {
            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                if (dataColumn.ColumnName.ToLower() == "sifre")
                {
                    continue;
                }

                metin.Append(dataColumn.ColumnName).Append(": ")
                     .Append(dataRow[dataColumn].ToString())
                     .Append(Environment.NewLine);
            }
        }

        return metin.ToString();
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
            TransactionAta(sqlCommand);

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
            TransactionAta(sqlCommand);

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
