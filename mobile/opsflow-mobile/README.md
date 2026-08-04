# OpsFlow Mobile

Bu klasör, OpsFlow projesinin React Native + Expo tabanlı mobil uygulamasıdır.

## Odak

- Pizza ve yemek siparişi hissi veren mobil arayüz
- Menü, sepet, siparişler, bildirimler ve hesap ekranları
- Backend ile gateway üzerinden iletişim

## Çalıştırma

1. `package.json` içindeki bağımlılıkları kur.
2. `npm start` ile Expo başlat.
3. `EXPO_PUBLIC_API_BASE_URL` ortam değişkenini gateway adresine ayarla.

## Siparis durumlari icin SignalR

Mobil uygulama, giris yapan kullanicinin siparis durumlarini Order API'deki SignalR hub'ina baglanarak dinler. Hub adresi `EXPO_PUBLIC_ORDER_HUB_URL` ile verilir.

Masaustu web testinde varsayilan adres kullanilir:

```text
http://localhost:5093/hubs/orders
```

Fiziksel telefonda `localhost` telefonun kendisini ifade eder. Bu nedenle bilgisayarin yerel IP adresini kullan:

```powershell
$env:EXPO_PUBLIC_API_BASE_URL="http://192.168.1.10:5022"
$env:EXPO_PUBLIC_ORDER_HUB_URL="http://192.168.1.10:5093/hubs/orders"
npm run start
```

Buradaki IP adresini API'lerin calistigi bilgisayarin LAN IP adresiyle degistir. Telefon ve bilgisayar ayni Wi-Fi aginda olmali, Windows guvenlik duvari da `5022` ve `5093` portlarina izin vermelidir.

SignalR baglantisi kullanicinin JWT'sini `accessTokenFactory` ile otomatik olarak gonderir. `OrderStatusChanged` mesaji geldiginde `useOrders` React Query onbellegini gunceller. Baglanti gecici olarak kullanilamazsa 30 saniyelik polling yedegi calismaya devam eder.

## API base URL örnekleri

- iPhone simülatör / macOS: `http://localhost:5022`
- Android emülatör: `http://10.0.2.2:8000`
- Gerçek telefon: bilgisayarının yerel IP adresi, örneğin `http://192.168.1.20:5022`

## Not

- Bu klasör web frontend'in yerini alır.
- Web uygulaması şu an dokunulmadan bırakıldı; istersen sonraki adımda onu kaldırıp mobil uygulamayı ana istemci yaparız.
