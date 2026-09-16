using System;
using System.Collections.Generic;
using System.Data;

public static class RolYetkiSeeder
{
    public static void Calistir()
    {
        VeritabaniIslemleri veritabaniIslemleri = new VeritabaniIslemleri();

        try
        {
            veritabaniIslemleri.Baslat(VeritabaniIslemleri.IslemTip.BAGIMLI);

            Roller roller = new Roller(veritabaniIslemleri);
            DataTable rollerTablosu = roller.TumunuGetir();

            RolYetkiler rolYetkiler = new RolYetkiler(veritabaniIslemleri);
            DataTable yetkilerTablosu = rolYetkiler.TumunuGetir();

            HashSet<string> mevcutYetkiler = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (yetkilerTablosu != null)
            {
                foreach (DataRow satir in yetkilerTablosu.Rows)
                {
                    string anahtar = satir[RolYetkiler.C_Sutun_roller_id].ToString()
                        + "|"
                        + satir[RolYetkiler.C_Sutun_sayfa_adi].ToString();
                    mevcutYetkiler.Add(anahtar);
                }
            }

            if (rollerTablosu == null)
            {
                veritabaniIslemleri.Uygula();
                return;
            }

            foreach (DataRow rolSatiri in rollerTablosu.Rows)
            {
                int rolId = Convert.ToInt32(rolSatiri[Roller.C_Sutun_id]);

                foreach (string sayfaAdi in Sayfalar.YetkilendirilenSayfalar)
                {
                    string anahtar = rolId + "|" + sayfaAdi;
                    if (mevcutYetkiler.Contains(anahtar))
                    {
                        continue;
                    }

                    RolYetkiler yeniYetki = new RolYetkiler(veritabaniIslemleri);
                    yeniYetki.RollerId = rolId;
                    yeniYetki.SayfaAdi = sayfaAdi;
                    yeniYetki.Getirme = false;
                    yeniYetki.Ekleme = false;
                    yeniYetki.Guncelleme = false;
                    yeniYetki.Silme = false;
                    yeniYetki.AktifMi = true;
                    yeniYetki.EkleyenId = 0;
                    yeniYetki.EkleyenIp = "SYSTEM";

                    if (!yeniYetki.Ekle())
                    {
                        veritabaniIslemleri.GeriAl();
                        return;
                    }

                    mevcutYetkiler.Add(anahtar);
                }
            }

            veritabaniIslemleri.Uygula();
        }
        catch
        {
            try
            {
                veritabaniIslemleri.GeriAl();
            }
            catch
            {
            }
        }
        finally
        {
            try
            {
                veritabaniIslemleri.Bitir();
            }
            catch
            {
            }
        }
    }
}
