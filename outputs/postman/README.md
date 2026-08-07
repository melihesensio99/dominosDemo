# OpsFlow Postman MVP Flow

Bu koleksiyon, alisveris akisinin Gateway uzerinden tekrar edilebilir sekilde test edilmesi icin hazirlanmistir.

## Import

1. `opsflow.postman_environment.json` dosyasini Postman'a import et.
2. `opsflow.postman_collection.json` dosyasini import et.
3. Environment olarak `OpsFlow Local` sec.

## Calistirma Sirasi

1. Gateway servis haritasini kontrol et.
2. Deneme kullanicisi olustur.
3. Deneme kullanicisi ile giris yap.
4. Kategori olustur.
5. Urun olustur.
6. Otomatik olusan stok kaydini kontrol et.
7. Urunu sepete ekle.
8. Sepeti goruntule.
9. Siparis olustur.
10. Siparislerimi goruntule.
11. Bildirimleri goruntule.

## Degiskenler

- `test_password`: Deneme kullanicisinin sifresi.
- Register isteginde `confirmPassword`, `password` ile ayni gonderilir.
- `test_email`: Bos birakilirsa benzersiz bir e-posta uretilir.
- `token`: Login cevabindan otomatik doldurulur.
- `category_id`: Kategori cevabindan otomatik doldurulur.
- `product_id`: Urun cevabindan otomatik doldurulur.
- `order_id`: Siparis cevabindan otomatik doldurulur.

Basket ve Order isteklerinde `customerId` body veya URL icinde gonderilmez. Backend kullanici kimligini JWT icindeki `sub` claim'inden okur.

Tum istekler Gateway uzerinden gider:

`http://localhost:5022/proxy/{service}/{endpoint}`
