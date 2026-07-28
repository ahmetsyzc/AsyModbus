using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace AsyModbus.AppCode
{
    public class Kullanici
    {
        VeritabaniIslemleri veritabaniIslemleri;

        public short Id;
        public short RollerId;

        public string Ad;
        public string Soyad;
        public string Tckno;
        public string Mail;
        public string Sifre;
        public string CepNo;

        public DateTime DogumTarih;

        public bool AktifMi;

        public string ResimYol;

        public Kullanici(VeritabaniIslemleri veritabaniIslemleri)
        {
            this.veritabaniIslemleri = veritabaniIslemleri;
        }


        public bool Ekle()
        {

            try
            {
                SqlConnection sqlConnection = veritabaniIslemleri.BaglantiGetir();

                SqlCommand sqlCommand = new SqlCommand("SP_Kullanicilar_EKLE", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@roller_id", RollerId);
                sqlCommand.Parameters.AddWithValue("@ad", Ad);
                sqlCommand.Parameters.AddWithValue("@soyad", Soyad);
                sqlCommand.Parameters.AddWithValue("@tckno", Tckno);
                sqlCommand.Parameters.AddWithValue("@mail", Mail);
                sqlCommand.Parameters.AddWithValue("@sifre", Sifre);
                sqlCommand.Parameters.AddWithValue("@cep_no", CepNo);
                sqlCommand.Parameters.AddWithValue("@dogum_tarih", DogumTarih);
                sqlCommand.Parameters.AddWithValue("@aktif_mi", AktifMi);
                sqlCommand.Parameters.AddWithValue("@resim_yol", ResimYol);

                sqlCommand.ExecuteNonQuery();

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool KayitVarMi(string alan, string deger)
        {
            try
            {
                SqlConnection sqlConnection = veritabaniIslemleri.BaglantiGetir();

                SqlCommand sqlCommand = new SqlCommand("SP_Kullanicilar_KAYIT_VAR_MI", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@alan", alan);
                sqlCommand.Parameters.AddWithValue("@deger", deger);

                return Convert.ToInt32(sqlCommand.ExecuteScalar()) > 0;

            }
            catch
            {
                return false;
            }
        }

        public bool Guncelle()
        {
            try
            {
                SqlConnection sqlConnection = veritabaniIslemleri.BaglantiGetir();
                SqlCommand sqlCommand = new SqlCommand("SP_Kullanicilar_GUNCELLE", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@id", Id);
                sqlCommand.Parameters.AddWithValue("@roller_id", RollerId);
                sqlCommand.Parameters.AddWithValue("@ad", Ad);
                sqlCommand.Parameters.AddWithValue("@soyad", Soyad);
                sqlCommand.Parameters.AddWithValue("@tckno", Tckno);
                sqlCommand.Parameters.AddWithValue("@mail", Mail);
                sqlCommand.Parameters.AddWithValue("@sifre", Sifre);
                sqlCommand.Parameters.AddWithValue("@cep_no", CepNo);
                sqlCommand.Parameters.AddWithValue("@dogum_tarih", DogumTarih);
                sqlCommand.Parameters.AddWithValue("@resim_yol", ResimYol);

                sqlCommand.ExecuteNonQuery();

                return true;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Sil()
        {
            try
            {
                SqlConnection sqlConnection = veritabaniIslemleri.BaglantiGetir();
                SqlCommand sqlCommand = new SqlCommand("SP_Kullanicilar_SIL", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@id", Id);

                sqlCommand.ExecuteNonQuery();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public DataTable TumKayitGetir()
        {
            try
            {
                SqlConnection sqlConnection = veritabaniIslemleri.BaglantiGetir();
                SqlCommand sqlCommand = new SqlCommand("SP_Kullanicilar_TUM_KAYIT_GETIR", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                DataTable dataTable = new DataTable();

                sqlDataAdapter.Fill(dataTable);

                return dataTable;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool TekKayitGetir()
        {
            try
            {
                SqlConnection sqlConnection = veritabaniIslemleri.BaglantiGetir();
                SqlCommand sqlCommand = new SqlCommand("SP_Kullanicilar_TEK_KAYIT_GETIR", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@id", Id);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                if (sqlDataReader.Read())
                {
                    RollerId = Convert.ToInt16(sqlDataReader["roller_id"]);
                    Ad = sqlDataReader["ad"].ToString();
                    Soyad = sqlDataReader["soyad"].ToString();
                    Tckno = sqlDataReader["tckno"].ToString();
                    Mail = sqlDataReader["mail"].ToString();
                    Sifre = sqlDataReader["sifre"].ToString();
                    CepNo = sqlDataReader["cep_no"].ToString();
                    DogumTarih = Convert.ToDateTime(sqlDataReader["dogum_tarih"]);
                    AktifMi = Convert.ToBoolean(sqlDataReader["aktif_mi"]);
                    ResimYol = sqlDataReader["resim_yol"].ToString();

                    sqlDataReader.Close();
                    return true;
                }

                sqlDataReader.Close();
                return false;

            }
            catch (Exception)
            {
                return false;
            }

        }

        public SqlDataReader SifreKontrol()
        {
            try
            {
                SqlConnection sqlConnection = veritabaniIslemleri.BaglantiGetir();
                SqlCommand sqlCommand = new SqlCommand("SP_Kullanicilar_GIRIS", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@mail", Mail);
                sqlCommand.Parameters.AddWithValue("@sifre", Sifre);

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                return sqlDataReader;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SqlDataReader MailCepNoKontrol()
        {
            try
            {
                SqlConnection sqlConnection = veritabaniIslemleri.BaglantiGetir();
                SqlCommand sqlCommand = new SqlCommand("SP_Kullanicilar_MAIL_CEPNO_KONTROL",sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@mail",Mail);
                sqlCommand.Parameters.AddWithValue("@cep_no",CepNo);

                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                return sqlDataReader;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SifreGuncelle()
        {
            try
            {
                SqlConnection sqlConnection = veritabaniIslemleri.BaglantiGetir();
                SqlCommand sqlCommand = new SqlCommand("SP_Kullanicilar_SIFRE_GUNCELLE",sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@id", Id);
                sqlCommand.Parameters.AddWithValue("@sifre", Sifre);

                object sonuc = sqlCommand.ExecuteScalar();

                if (sonuc != null && Convert.ToInt32(sonuc) == 1)
                {
                    return true;
                }

                return false;

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }

        public string SifreOlustur(string ad, string soyad)
        {
            Random random = new Random();
            return ad.Substring(0, 2) + soyad.Substring(0, 2) + "@" + random.Next(10000, 100000);
        }
        
    }
}