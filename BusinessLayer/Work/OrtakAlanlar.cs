using System;
using BusinessLayer.Work;

namespace BusinessLayer.Work
{
    public class OrtakAlanlar
    {

        #region Sabitler

        public const string C_Sutun_id = "id";
        public const string C_Sutun_ekleyen_id = "ekleyen_id";
        public const string C_Sutun_ekleyen_ip = "ekleyen_ip";
        public const string C_Sutun_eklenme_tarih = "eklenme_tarih";
        public const string C_Sutun_guncelleyen_id = "guncelleyen_id";
        public const string C_Sutun_guncelleyen_ip = "guncelleyen_ip";
        public const string C_Sutun_guncellenme_tarih = "guncellenme_tarih";
        public const string C_Sutun_aktif_mi = "aktif_mi";

        #endregion

        #region Nesneler

        public int Id { get; set; }

        public int EkleyenId { get; set; }
        public string EkleyenIp { get; set; }
        public DateTime EklenmeTarih { get; set; }

        public int GuncelleyenId { get; set; }
        public string GuncelleyenIp { get; set; }
        public DateTime GuncellenmeTarih { get; set; }

        public bool AktifMi { get; set; }

        #endregion

    }
}