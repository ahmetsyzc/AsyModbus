using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace AsyModbus.AppCode
{
    public class Rol
    {
        VeritabaniIslemleri veritabaniIslemleri;

        public short Id;
        public short Ad;

        public Rol(VeritabaniIslemleri veritabaniIslemleri)
        {
            this.veritabaniIslemleri = veritabaniIslemleri;
        }

        public DataTable TumKayitGetir()
        {
            try
            {
                SqlConnection sqlConnection = veritabaniIslemleri.BaglantiGetir();
                SqlCommand sqlCommand = new SqlCommand("SP_Roller_TUM_KAYIT_GETIR", sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);

                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Listele(DropDownList dropDownList)
        {
            DataTable dataTable = TumKayitGetir();

            if (dataTable == null)
            {
                throw new Exception("Rol kayıtları getirilemedi.");
            }

            dropDownList.DataTextField = "ad";
            dropDownList.DataValueField = "id";
            dropDownList.DataSource = dataTable;
            dropDownList.DataBind();
        }

    }
}