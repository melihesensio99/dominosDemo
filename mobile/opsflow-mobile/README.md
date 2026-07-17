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

## API base URL örnekleri

- iPhone simülatör / macOS: `http://localhost:8000`
- Android emülatör: `http://10.0.2.2:8000`
- Gerçek telefon: bilgisayarının yerel IP adresi, örneğin `http://192.168.1.20:8000`

## Not

- Bu klasör web frontend'in yerini alır.
- Web uygulaması şu an dokunulmadan bırakıldı; istersen sonraki adımda onu kaldırıp mobil uygulamayı ana istemci yaparız.
