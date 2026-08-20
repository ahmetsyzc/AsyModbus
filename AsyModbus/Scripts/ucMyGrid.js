document.addEventListener("DOMContentLoaded", function () {

    var txtBul = document.getElementById("txtBul");
    var grdMyGrid = document.getElementById("grdMyGrid");

    if (txtBul == null || grdMyGrid == null) {
        return;
    }

    txtBul.addEventListener("input", function () {

        var aranan = txtBul.value.toLowerCase().trim();
        var satirlar = grdMyGrid.getElementsByTagName("tr");

        for (var i = 1; i < satirlar.length; i++) {

            var hucreler = satirlar[i].getElementsByTagName("td");
            var satirMetni = "";

            for (var j = 1; j < hucreler.length; j++) {
                satirMetni += hucreler[j].textContent.toLowerCase() + " ";
            }

            if (satirMetni.indexOf(aranan) > -1) {
                satirlar[i].style.display = "";
            }
            else {
                satirlar[i].style.display = "none";
            }
        }
    });
});