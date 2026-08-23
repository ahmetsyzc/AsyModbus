document.addEventListener("DOMContentLoaded", function () {

    // Sayfadaki gerekli kontrolleri ID'leri üzerinden alıyoruz
    var txtBul = document.getElementById("txtBul");
    var grdMyGrid = document.getElementById("grdMyGrid");
    var ddlKayitSayisi = document.getElementById("ddlKayitSayisi");
    var gridSayfalama = document.getElementById("gridSayfalama");

    // Kontrollerden herhangi biri bulunamazsa kodu durduruyoruz
    if (txtBul == null || grdMyGrid == null || ddlKayitSayisi == null || gridSayfalama == null) {
        return;
    }

    // Kullanıcının şu anda bulunduğu sayfayı tutuyoruz
    var aktifSayfa = 1;


    // -------------------- ARAMA --------------------

    function GridAra() {

        // Arama kutusuna yazılan değeri alıyoruz
        var aranan = txtBul.value.toLowerCase().trim();

        // Grid içerisindeki bütün satırları alıyoruz
        var satirlar = grdMyGrid.getElementsByTagName("tr");

        // Başlık satırını atlayarak veri satırlarını geziyoruz
        for (var i = 1; i < satirlar.length; i++) {

            // O satırdaki bütün hücreleri alıyoruz
            var hucreler = satirlar[i].getElementsByTagName("td");

            // Arama yapılacak satır metnini burada oluşturacağız
            var satirMetni = "";

            // j = 1 ile başlıyoruz çünkü ilk hücre "Aç" butonu
            for (var j = 1; j < hucreler.length; j++) {

                // Hücrelerdeki yazıları birleştiriyoruz
                satirMetni += hucreler[j].textContent.toLowerCase() + " ";
            }

            // Aranan değer satırın içerisinde bulunuyorsa
            if (satirMetni.indexOf(aranan) > -1) {

                // Bu satırı eşleşen olarak işaretliyoruz
                satirlar[i].setAttribute("data-eslesiyor", "true");
            }
            else {

                // Eşleşmiyorsa false olarak işaretliyoruz
                satirlar[i].setAttribute("data-eslesiyor", "false");
            }
        }

        // Yeni aramada tekrar ilk sayfaya dönüyoruz
        aktifSayfa = 1;

        // Eşleşen kayıtlara kayıt sayısı ve sayfalama işlemini uyguluyoruz
        KayitSayisiUygula();
    }


    // -------------------- KAYIT SAYISI --------------------

    function KayitSayisiUygula() {

        // DropDown'dan seçilen 5, 10, 20... değerini alıyoruz
        var kayitSayisi = parseInt(ddlKayitSayisi.value);

        // Grid içerisindeki bütün satırları alıyoruz
        var satirlar = grdMyGrid.getElementsByTagName("tr");

        // Aramayla eşleşen satırları burada tutacağız
        var eslesenSatirlar = [];

        // Başlık hariç bütün satırları geziyoruz
        for (var i = 1; i < satirlar.length; i++) {

            // Aramayla eşleşen satırları diziye ekliyoruz
            if (satirlar[i].getAttribute("data-eslesiyor") != "false") {
                eslesenSatirlar.push(satirlar[i]);
            }

            // Önce bütün veri satırlarını gizliyoruz
            satirlar[i].style.display = "none";
        }

        // DropDown'da "Tümü" seçilmişse değeri -1 geliyor
        if (kayitSayisi == -1) {

            // Eşleşen bütün satırları gösteriyoruz
            for (var i = 0; i < eslesenSatirlar.length; i++) {
                eslesenSatirlar[i].style.display = "";
            }

            // Tümü gösterildiği için sayfa numaralarını kaldırıyoruz
            gridSayfalama.innerHTML = "";

            return;
        }

        // Aktif sayfanın başlayacağı kayıt indeksini hesaplıyoruz
        var baslangic = (aktifSayfa - 1) * kayitSayisi;

        // Aktif sayfanın biteceği kayıt indeksini hesaplıyoruz
        var bitis = baslangic + kayitSayisi;

        // Sadece aktif sayfaya ait kayıtları gösteriyoruz
        for (var i = baslangic; i < bitis && i < eslesenSatirlar.length; i++) {
            eslesenSatirlar[i].style.display = "";
        }

        // Kaç sayfa gerektiğini hesaplayıp butonları oluşturuyoruz
        SayfalamaOlustur(eslesenSatirlar.length, kayitSayisi);
    }


    // -------------------- SAYFALAMA --------------------

    function SayfalamaOlustur(toplamKayit, kayitSayisi) {

        // Eski sayfalama butonlarını temizliyoruz
        gridSayfalama.innerHTML = "";

        // Toplam sayfa sayısını hesaplıyoruz
        var toplamSayfa = Math.ceil(toplamKayit / kayitSayisi);

        // Hiç kayıt yoksa sayfalama oluşturmuyoruz
        if (toplamSayfa <= 0) {
            return;
        }

        // Aktif sayfa artık mevcut değilse son geçerli sayfaya alıyoruz
        if (aktifSayfa > toplamSayfa) {
            aktifSayfa = toplamSayfa;
        }


        // -------------------- GERİ BUTONU --------------------

        var geriButon = document.createElement("button");

        geriButon.type = "button";
        geriButon.textContent = "‹";
        geriButon.className = "btn btn-sm btn-outline-dark";

        // İlk sayfadaysak geri butonunu pasif yapıyoruz
        if (aktifSayfa == 1) {
            geriButon.disabled = true;
        }

        geriButon.addEventListener("click", function () {

            if (aktifSayfa > 1) {
                aktifSayfa--;
                KayitSayisiUygula();
            }

        });

        gridSayfalama.appendChild(geriButon);


        // -------------------- GÖRÜNECEK SAYFALAR --------------------

        var baslangicSayfa;
        var bitisSayfa;

        // İlk sayfadaysak 1 - 2 - 3 gösteriyoruz
        if (aktifSayfa == 1) {

            baslangicSayfa = 1;
            bitisSayfa = Math.min(3, toplamSayfa);

        }

        // Son sayfadaysak son 3 sayfayı gösteriyoruz
        else if (aktifSayfa == toplamSayfa) {

            baslangicSayfa = Math.max(1, toplamSayfa - 2);
            bitisSayfa = toplamSayfa;

        }

        // Ortadaki sayfalarda önceki - aktif - sonraki gösteriliyor
        else {

            baslangicSayfa = aktifSayfa - 1;
            bitisSayfa = aktifSayfa + 1;

        }


        // -------------------- SAYFA BUTONLARI --------------------

        for (var i = baslangicSayfa; i <= bitisSayfa; i++) {

            var buton = document.createElement("button");

            buton.type = "button";
            buton.textContent = i;

            // Aktif sayfa siyah
            if (i == aktifSayfa) {
                buton.className = "btn btn-sm btn-dark active";
            }

            // Diğer sayfalar outline
            else {
                buton.className = "btn btn-sm btn-outline-dark";
            }

            buton.addEventListener("click", function () {

                aktifSayfa = parseInt(this.textContent);

                KayitSayisiUygula();

            });

            gridSayfalama.appendChild(buton);
        }


        // -------------------- İLERİ BUTONU --------------------

        var ileriButon = document.createElement("button");

        ileriButon.type = "button";
        ileriButon.textContent = "›";
        ileriButon.className = "btn btn-sm btn-outline-dark";

        // Son sayfadaysak ileri butonunu pasif yapıyoruz
        if (aktifSayfa == toplamSayfa) {
            ileriButon.disabled = true;
        }

        ileriButon.addEventListener("click", function () {

            if (aktifSayfa < toplamSayfa) {
                aktifSayfa++;
                KayitSayisiUygula();
            }

        });

        gridSayfalama.appendChild(ileriButon);
    }


    // -------------------- EVENTLER --------------------

    // Arama kutusuna her yazıldığında arama yapıyoruz
    txtBul.addEventListener("input", function () {
        GridAra();
    });

    // Gösterilecek kayıt sayısı değiştirildiğinde listeyi yeniliyoruz
    ddlKayitSayisi.addEventListener("change", function () {

        // Yeni seçimde ilk sayfaya dönüyoruz
        aktifSayfa = 1;

        // Yeni kayıt sayısını uyguluyoruz
        KayitSayisiUygula();
    });


    // Sayfa ilk açıldığında Grid'i ilk kez hazırlıyoruz
    GridAra();

});