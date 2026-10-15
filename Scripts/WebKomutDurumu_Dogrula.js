const fs = require('node:fs');
const vm = require('node:vm');
const assert = require('node:assert/strict');
const listeners = {};

const buton = {disabled: false};
const etiket = {textContent: ''};
const kart = {querySelector(s) {assert.notEqual(s, '.komut-sonucu'); return s === '.makine-durum-etiketi' ? etiket : buton;}};
let kayit;
let fail = false;
let poll;
const context = {
    document: {
        hidden: false,
        addEventListener(n, fn) {listeners[n] = fn;},
        querySelector() {return kart;},

        createElement() {return {setAttribute() {}};}
    },
    window: {setInterval(fn) {poll = fn;}},
    fetch: async () => ({ok: !fail, json: async () => ({d: [kayit]})}),
};
vm.runInNewContext(fs.readFileSync(require('node:path').join(__dirname, '../AsyModbus/Scripts/Default.js'), 'utf8'), context);
async function kontrol(durum, aktif, mesaj) {
    kayit = {MakineId: 1, KomutId: 1, Durum: durum, DurdurAktif: aktif, Mesaj: mesaj,
        MakineDurum: aktif ? 'Çalışıyor' : 'Durduruldu', MakineCss: aktif ? 'bg-success' : 'bg-danger'};
    await poll();
    assert.equal(buton.disabled, !aktif);

}
(async () => {
    kayit = {MakineId: 1, KomutId: 0, Mesaj: ''};
    listeners.DOMContentLoaded();
    await new Promise(resolve => setImmediate(resolve));
    await kontrol('BEKLIYOR', false, 'Bekliyor');
    assert.equal(etiket.textContent, 'Bekliyor');
    await kontrol('ISLENIYOR', false, 'İşleniyor');
    assert.equal(etiket.textContent, 'İşleniyor');
    await kontrol('BASARILI', false, 'Durdurma komutu gönderildi.');
    assert.equal(etiket.textContent, 'Durduruldu');
    assert.equal(etiket.className, 'badge makine-durum-etiketi bg-danger');
    await kontrol('BASARISIZ', true, 'Başarısız');

    assert.equal(etiket.textContent, 'Çalışıyor');
    await kontrol('BELIRSIZ', false, 'Belirsiz');

    assert.equal(etiket.textContent, 'Sonuç belirsiz');
    fail = true;
    await poll();
    assert.equal(etiket.textContent, 'Sonuç belirsiz');
    assert.ok(!fs.readFileSync(require('node:path').join(__dirname, '../AsyModbus/Pages/Default.aspx'), 'utf8').includes('komut-sonucu'));
    console.log('GECTI: Kart bekleyen, islenen, basarili, basarisiz, belirsiz ve sorgu hatasi durumlari');
})().catch(e => {console.error(e); process.exitCode = 1;});
