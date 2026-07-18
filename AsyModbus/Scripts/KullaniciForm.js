window.onload = function () {
    TelefonKontrol();
    TcKontrol();
};

function TelefonKontrol() {

    const txtTelefon = document.getElementById("txtCepNo");
    txtTelefon.addEventListener("input", function () {

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