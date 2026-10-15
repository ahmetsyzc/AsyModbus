
// Şu anda sürüklenen makine kartını tutar.
var suruklenenKart = null;

// Sürükleme işlemi başladığında çalışır.
document.addEventListener("dragstart", function (event) {

    // Sürüklenen elemanın makine kartı olup olmadığı bulunur.
    var kart = event.target.closest(".siralama-karti");

    if (kart == null) {
        return;
    }

    suruklenenKart = kart;

    // Sürüklenen kartı hafif saydam gösterir.
    kart.style.opacity = "0.5";
});

// Kart başka bir kartın üzerinde hareket ederken çalışır.
document.addEventListener("dragover", function (event) {

    var hedefKart = event.target.closest(".siralama-karti");

    if (hedefKart == null || suruklenenKart == null) {
        return;
    }

    // Tarayıcının varsayılan sürükleme davranışını engeller.
    event.preventDefault();

    if (hedefKart == suruklenenKart) {
        return;
    }

    var alan = document.getElementById("siralamaAlani");
    var hedefKonum = hedefKart.getBoundingClientRect();

    // Farenin hedef kartın hangi tarafında olduğu bulunur.
    var hedefinYarisi = hedefKonum.left + hedefKonum.width / 2;

    if (event.clientX < hedefinYarisi) {

        // Hedef kartın önüne yerleştirir.
        alan.insertBefore(suruklenenKart, hedefKart);
    }
    else {

        // Hedef kartın arkasına yerleştirir.
        alan.insertBefore(suruklenenKart, hedefKart.nextSibling);
    }
});

// Sürükleme işlemi tamamlandığında çalışır.
document.addEventListener("dragend", function () {

    if (suruklenenKart == null) {
        return;
    }

    // Kartın görünümünü normale döndürür.
    suruklenenKart.style.opacity = "1";
    suruklenenKart = null;

    // Yeni makine sırasını HiddenField içerisine yazar.
    makineSirasiniHazirla();
});

function makineSirasiniHazirla() {

    var kartlar = document.querySelectorAll( "#siralamaAlani .siralama-karti" );
    var makineIdleri = [];
    kartlar.forEach(function (kart) {
        makineIdleri.push(kart.getAttribute("data-makine-id"));
    });
    // ASP.NET tarafından oluşturulan HiddenField bulunur.
    var hiddenField = document.querySelector( "[id$='hfMakineSirasi']");
    if (hiddenField != null) {
        hiddenField.value = makineIdleri.join(",");
    }
}

// Salt okunur durum sorgusu; cihaz adresleri tarayiciya tasinmaz.
(function () {
    var sorguSuruyor = false;
    async function komutDurumlariniYenile() {
        if (sorguSuruyor || document.hidden) return;
        sorguSuruyor = true;
        try {
            var yanit = await fetch('Default.aspx/KomutDurumlariniGetir', {
                method: 'POST', credentials: 'same-origin',
                headers: { 'Content-Type': 'application/json; charset=utf-8' }, body: '{}'
            });
            if (!yanit.ok) throw new Error('Durum sorgulanamadı');
            var veri = await yanit.json();
            if (!Array.isArray(veri.d)) throw new Error('Geçersiz durum yanıtı');
            veri.d.forEach(function (kayit) {
                var kart = document.querySelector('[data-komut-makine-id="' + kayit.MakineId + '"]');
                if (!kart) return;
                var bekliyor = kayit.Durum === 'BEKLIYOR' || kayit.Durum === 'ISLENIYOR';
                var buton = kart.querySelector('input[id$="btnDurdur"]');
                if (buton) buton.disabled = !kayit.DurdurAktif;
                var etiket = kart.querySelector('.makine-durum-etiketi');
                if (etiket && kayit.KomutId) {
                    var belirsiz = kayit.Durum === 'BELIRSIZ';
                    etiket.textContent = bekliyor ? (kayit.Durum === 'BEKLIYOR' ? 'Bekliyor' : 'İşleniyor')
                        : belirsiz ? 'Sonuç belirsiz'
                        : kayit.MakineCss === 'badge-role-yok' ? 'Röle seçilmedi' : kayit.MakineDurum;
                    etiket.className = 'badge makine-durum-etiketi ' +
                        (bekliyor || belirsiz ? 'bg-warning text-dark' : kayit.MakineCss);
                    etiket.title = etiket.textContent;
                }
            });
        } catch (hata) {
            // Gecici sorgu hatasinda mevcut kart gorunumu korunur.
        } finally { sorguSuruyor = false; }
    }
    document.addEventListener('DOMContentLoaded', function () {
        komutDurumlariniYenile();
        window.setInterval(komutDurumlariniYenile, 5000);
    });
})();
