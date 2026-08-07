window.onload = function () {
    TelefonKontrol();
    TcKontrol();
};

function TelefonFormatla(txtTelefon) {

    let telNo = txtTelefon.value;
    telNo = telNo.replace(/\D/g, "");

    let alanKodu = telNo.substring(0, 3);
    let ilkUc = telNo.substring(3, 6);
    let sonDort = telNo.substring(6, 10);

    if (telNo.length <= 3) {

    }
    else if (telNo.length <= 6) {

        telNo = "(" + alanKodu + ")-" + ilkUc;
    }
    else {

        telNo = "(" + alanKodu + ")-" + ilkUc + "-" + sonDort;
    }

    txtTelefon.value = telNo;
}

function TelefonKontrol() {

    const txtTelefon = document.getElementById("txtCepNo");

    TelefonFormatla(txtTelefon);

    txtTelefon.addEventListener("input", function () {

        TelefonFormatla(txtTelefon);

    });


}


function TcKontrol() {

    //Tc Kontrol
    const txtTcNo = document.getElementById("txtTckno");

    txtTcNo.addEventListener("input", function () {

        let tcNo = txtTcNo.value;
        tcNo = tcNo.replace(/\D/g, "");

        txtTcNo.value = tcNo;
    });

};

function KullaniciDogrula() {

    const txtTelefon = document.getElementById("txtCepNo");
    const txtTcNo = document.getElementById("txtTckno");

    let telNo = txtTelefon.value.replace(/\D/g, "");
    let tcNo = txtTcNo.value.replace(/\D/g, "");

    if (tcNo.length !== 11) {
        alert("TC Kimlik Numarası 11 haneli olmalıdır.");
        txtTcNo.focus();
        return false;
    }

    if (tcNo.charAt(0) == "0") {
        alert("TC Kimlik Numarası 0 ile başlayamaz.");
        txtTcNo.focus();
        return false;
    }

    // TC algoritma kontrolü
    if (!TcKimlikDogrula(tcNo)) {
        alert("Geçersiz TC Kimlik Numarası.");
        txtTcNo.focus();
        return false;
    }

    if (telNo.length !== 10) {
        alert("Telefon numarası 10 haneli olmalıdır.");
        txtTelefon.focus();
        return false;
    }

    if (telNo.charAt(0) !== "5") {
        alert("Telefon numarası 5 ile başlamalıdır.");
        txtTelefon.focus();
        return false;
    }

    return true;
};

function TcKimlikDogrula(tcNo) {


    let toplam = 0;
    let toplamTek = 0;
    let toplamCift = 0;

    // İlk 10 hanenin toplamı:
    // Bu toplam 11. haneyi hesaplamak için 
    for (let i = 0; i < 10; i++) {

        let rakam = parseInt(tcNo.charAt(i));

        toplam += rakam;
    }

    // İlk 9 hanedeki tek ve çift sıralı rakamların toplamı:
    // Bu toplamlar 10. haneyi hesaplamak için 
    for (let i = 0; i < 9; i++) {

        let rakam = parseInt(tcNo.charAt(i));

        if (i % 2 === 0) {
            toplamTek += rakam;
        }
        else {
            toplamCift += rakam;
        }
    }

    let onuncuHaneIslemi =
        (toplamTek * 7) - toplamCift;

    let hesaplananOnuncuHane =
        ((onuncuHaneIslemi % 10) + 10) % 10;

    let hesaplananOnBirinciHane =
        toplam % 10;

    let gercekOnuncuHane =
        parseInt(tcNo.charAt(9));

    let gercekOnBirinciHane =
        parseInt(tcNo.charAt(10));

    if (
        hesaplananOnuncuHane === gercekOnuncuHane &&
        hesaplananOnBirinciHane === gercekOnBirinciHane
    ) {
        return true;
    }

    return false;
}

function SifreSifirlamaDogrulama() {

    const txtTelefon = document.getElementById("txtCepNo");
    let telNo = txtTelefon.value.replace(/\D/g, "");

    if (telNo.length !== 10) {
        alert("Telefon numarası 10 haneli olmalıdır.");
        txtTelefon.focus();
        return false;
    }

    if (telNo.charAt(0) !== "5") {
        alert("Telefon numarası 5 ile başlamalıdır.");
        txtTelefon.focus();
        return false;
    }

    return true;
}