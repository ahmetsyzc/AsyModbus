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

        // Önceden oluşturulmuş sayfa butonlarını temizliyoruz
        gridSayfalama.innerHTML = "";

        // Toplam kaç sayfa gerektiğini hesaplıyoruz
        var toplamSayfa = Math.ceil(toplamKayit / kayitSayisi);

        // Aktif sayfa artık mevcut değilse ilk sayfaya dönüyoruz
        if (aktifSayfa > toplamSayfa) {
            aktifSayfa = 1;
        }

        // Her sayfa için bir buton oluşturuyoruz
        for (var i = 1; i <= toplamSayfa; i++) {

            // Yeni HTML button oluşturuyoruz
            var buton = document.createElement("button");

            // Formu submit etmesini engelliyoruz
            buton.type = "button";

            // Butonun üzerinde sayfa numarasını gösteriyoruz
            buton.textContent = i;

            // Bulunduğumuz sayfanın butonuna "aktif" class'ı veriyoruz
            if (i == aktifSayfa) {
                buton.classList.add("aktif");
            }

            // Sayfa butonuna tıklandığında çalışacak event
            buton.addEventListener("click", function () {

                // Tıklanan butonun numarasını aktif sayfa yapıyoruz
                aktifSayfa = parseInt(this.textContent);

                // Yeni sayfanın kayıtlarını gösteriyoruz
                KayitSayisiUygula();
            });

            // Oluşturulan butonu sayfalama alanına ekliyoruz
            gridSayfalama.appendChild(buton);
        }
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