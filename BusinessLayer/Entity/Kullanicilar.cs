using System;
using System.Data;
using System.Text.RegularExpressions;
using BusinessLayer.Interface;
using BusinessLayer.Work;

namespace BusinessLayer.Entity
{
    public class Kullanicilar : OrtakAlanlar, IOrtakMetotlar
    {
        VeritabaniIslemleri veritabaniIslemleri;

        #region Sabitler

        public const string C_Tablo = "dbo.Kullanicilar";

        public const string C_Sp_Ekle = "dbo.SP_Kullanicilar_EKLE";
        public const string C_Sp_Sil = "dbo.SP_Kullanicilar_SIL";
        public const string C_Sp_Guncelle = "dbo.SP_Kullanicilar_GUNCELLE";
        public const string C_Sp_TumKayitGetir = "dbo.SP_Kullanicilar_TUM_KAYIT_GETIR";
        public const string C_Sp_TekKayitGetir = "dbo.SP_Kullanicilar_TEK_KAYIT_GETIR";
        public const string C_Sp_SifreKontrol = "dbo.SP_Kullanicilar_SIFRE_KONTROL";
        public const string C_Sp_SifreGuncelle = "dbo.SP_Kullanicilar_SIFRE_GUNCELLE";
        public const string C_Sp_MailCepNoKontrol = "dbo.SP_Kullanicilar_MAIL_CEPNO_KONTROL";
        public const string C_Sp_TcknoVarMi = "dbo.SP_Kullanicilar_TCKNO_VAR_MI";
        public const string C_Sp_MailVarMi = "dbo.SP_Kullanicilar_MAIL_VAR_MI";
        public const string C_Sp_CepNoVarMi = "dbo.SP_Kullanicilar_CEPNO_VAR_MI";



        public const string C_Sutun_roller_id = "roller_id";
        public const string C_Sutun_ad = "ad";
        public const string C_Sutun_soyad = "soyad";
        public const string C_Sutun_tckno = "tckno";
        public const string C_Sutun_mail = "mail";
        public const string C_Sutun_sifre = "sifre";
        public const string C_Sutun_cep_no = "cep_no";
        public const string C_Sutun_dogum_tarih = "dogum_tarih";
        public const string C_Sutun_resim_yol = "resim_yol";


        #endregion


        #region Nesneler

        public int RollerId { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Tckno { get; set; }
        public string Mail { get; set; }
        public string Sifre { get; set; }

        private string cepNo;
        public string CepNo
        {
            get
            {
                return cepNo;
            }
            set
            {
                cepNo = Regex.Replace(value, @"\D", "");
            }
        }

        public DateTime DogumTarih { get; set; }
        public string ResimYol { get; set; }

        #endregion


        #region Metotlar

        public Kullanicilar(VeritabaniIslemleri veritabaniIslemleri)
        {
            this.veritabaniIslemleri = veritabaniIslemleri;
        }


        public bool Ekle()
        {
            try
            {
                veritabaniIslemleri.Baslat();
                veritabaniIslemleri.ParametreEkle(C_Sutun_roller_id, RollerId);
                veritabaniIslemleri.ParametreEkle(C_Sutun_ad, Ad);
                veritabaniIslemleri.ParametreEkle(C_Sutun_soyad, Soyad);
                veritabaniIslemleri.ParametreEkle(C_Sutun_tckno, Tckno);
                veritabaniIslemleri.ParametreEkle(C_Sutun_mail, Mail);
                veritabaniIslemleri.ParametreEkle(C_Sutun_sifre, Sifre);
                veritabaniIslemleri.ParametreEkle(C_Sutun_cep_no, CepNo);
                veritabaniIslemleri.ParametreEkle(C_Sutun_dogum_tarih, DogumTarih);
                veritabaniIslemleri.ParametreEkle(C_Sutun_resim_yol, ResimYol);
                veritabaniIslemleri.ParametreEkle(C_Sutun_aktif_mi, AktifMi);
                veritabaniIslemleri.ParametreEkle(C_Sutun_ekleyen_id, EkleyenId);
                veritabaniIslemleri.ParametreEkle(C_Sutun_ekleyen_ip, EkleyenIp);
                return veritabaniIslemleri.Calistir(C_Sp_Ekle) > 0;
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


        public bool Guncelle()
        {
            try
            {
                veritabaniIslemleri.Baslat();
                veritabaniIslemleri.ParametreEkle(C_Sutun_id, Id);
                veritabaniIslemleri.ParametreEkle(C_Sutun_roller_id, RollerId);
                veritabaniIslemleri.ParametreEkle(C_Sutun_ad, Ad);
                veritabaniIslemleri.ParametreEkle(C_Sutun_soyad, Soyad);
                veritabaniIslemleri.ParametreEkle(C_Sutun_tckno, Tckno);
                veritabaniIslemleri.ParametreEkle(C_Sutun_mail, Mail);
                veritabaniIslemleri.ParametreEkle(C_Sutun_sifre, Sifre);
                veritabaniIslemleri.ParametreEkle(C_Sutun_cep_no, CepNo);
                veritabaniIslemleri.ParametreEkle(C_Sutun_dogum_tarih, DogumTarih);
                veritabaniIslemleri.ParametreEkle(C_Sutun_resim_yol, ResimYol);
                veritabaniIslemleri.ParametreEkle(C_Sutun_guncelleyen_id, GuncelleyenId);
                veritabaniIslemleri.ParametreEkle(C_Sutun_guncelleyen_ip, GuncelleyenIp);
                return veritabaniIslemleri.Calistir(C_Sp_Guncelle) > 0;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }

        public bool Sil()
        {
            try
            {
                veritabaniIslemleri.Baslat();
                veritabaniIslemleri.ParametreEkle(C_Sutun_id, Id);
                return veritabaniIslemleri.Calistir(C_Sp_Sil) > 0;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }

        public DataTable TumKayitGetir()
        {
            try
            {
                veritabaniIslemleri.Baslat();
                return veritabaniIslemleri.TabloGetir(C_Sp_TumKayitGetir);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }

        public bool TekKayitGetir()
        {
            try
            {
                veritabaniIslemleri.Baslat();
                veritabaniIslemleri.ParametreEkle(C_Sutun_id, Id);
                DataRow dataRow = veritabaniIslemleri.SatirGetir(C_Sp_TekKayitGetir);

                if (dataRow == null)
                {
                    return false;
                }

                Id = Convert.ToInt32(dataRow[C_Sutun_id]);

                RollerId = Convert.ToInt32(dataRow[C_Sutun_roller_id]);

                Ad = dataRow[C_Sutun_ad].ToString();
                Soyad = dataRow[C_Sutun_soyad].ToString();
                Tckno = dataRow[C_Sutun_tckno].ToString();
                Mail = dataRow[C_Sutun_mail].ToString();
                Sifre = dataRow[C_Sutun_sifre].ToString();
                CepNo = dataRow[C_Sutun_cep_no].ToString();
                DogumTarih = Convert.ToDateTime(dataRow[C_Sutun_dogum_tarih]);
                AktifMi = Convert.ToBoolean(dataRow[C_Sutun_aktif_mi]);
                ResimYol = dataRow[C_Sutun_resim_yol].ToString();

                EkleyenId = Convert.ToInt32(dataRow[C_Sutun_ekleyen_id]);
                EkleyenIp = dataRow[C_Sutun_ekleyen_ip].ToString();
                EklenmeTarih = Convert.ToDateTime(dataRow[C_Sutun_eklenme_tarih]);

                if (dataRow[C_Sutun_guncelleyen_id] == DBNull.Value)
                {
                    GuncelleyenId = 0;
                }
                else
                {
                    GuncelleyenId =
                        Convert.ToInt32(dataRow[C_Sutun_guncelleyen_id]);
                }

                if (dataRow[C_Sutun_guncelleyen_ip] == DBNull.Value)
                {
                    GuncelleyenIp = "";
                }
                else
                {
                    GuncelleyenIp = dataRow[C_Sutun_guncelleyen_ip].ToString();
                }

                if (dataRow[C_Sutun_guncellenme_tarih] == DBNull.Value)
                {
                    GuncellenmeTarih = DateTime.MinValue;
                }
                else
                {
                    GuncellenmeTarih =
                        Convert.ToDateTime(dataRow[C_Sutun_guncellenme_tarih]);
                }


                return true;
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

        public DataRow SifreKontrol()
        {
            try
            {
                veritabaniIslemleri.Baslat();
                veritabaniIslemleri.ParametreEkle(C_Sutun_mail, Mail);
                veritabaniIslemleri.ParametreEkle(C_Sutun_sifre, Sifre);
                DataRow dataRow = veritabaniIslemleri.SatirGetir(C_Sp_SifreKontrol);
                return dataRow;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }

        public DataRow MailCepNoKontrol()
        {
            try
            {
                veritabaniIslemleri.Baslat();
                veritabaniIslemleri.ParametreEkle(C_Sutun_mail, Mail);
                veritabaniIslemleri.ParametreEkle(C_Sutun_cep_no, CepNo);
                DataRow dataRow = veritabaniIslemleri.SatirGetir(C_Sp_MailCepNoKontrol);
                return dataRow;
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

        public bool SifreGuncelle()
        {
            try
            {
                veritabaniIslemleri.Baslat();
                veritabaniIslemleri.ParametreEkle(C_Sutun_id, Id);
                veritabaniIslemleri.ParametreEkle(C_Sutun_sifre, Sifre);

                return veritabaniIslemleri.Calistir(C_Sp_SifreGuncelle) > 0;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                veritabaniIslemleri.Bitir();
            }
        }

        public int MailVarMi()
        {
            try
            {
                veritabaniIslemleri.Baslat();
                veritabaniIslemleri.ParametreEkle(C_Sutun_mail, Mail);
                object sonuc = veritabaniIslemleri.DegerGetir(C_Sp_MailVarMi);
                return Convert.ToInt32(sonuc);
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

        public int TcknoVarMi()
        {
            try
            {
                veritabaniIslemleri.Baslat();
                veritabaniIslemleri.ParametreEkle(C_Sutun_tckno, Tckno);
                object sonuc = veritabaniIslemleri.DegerGetir(C_Sp_TcknoVarMi);
                return Convert.ToInt32(sonuc);
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

        public int CepNoVarMi()
        {
            try
            {
                veritabaniIslemleri.Baslat();
                veritabaniIslemleri.ParametreEkle(C_Sutun_cep_no, CepNo);
                object sonuc = veritabaniIslemleri.DegerGetir(C_Sp_CepNoVarMi);
                return Convert.ToInt32(sonuc);
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

        #endregion
    }
}