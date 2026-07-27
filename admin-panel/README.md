# Melo's Pizza Admin Panel

Bu klasor, siparis durumlarini tek bir admin panelinden denemek icin hazirlanan basit HTML panelidir.

1. Gateway, Order API ve Auth API'yi calistirin.
2. Bu klasoru bir yerel web sunucusuyla acin. Ornek: `python -m http.server 7070 --directory admin-panel`
3. Tarayicida `http://localhost:7070` adresini acin.
4. Admin hesabiyla giris yapin.

Siparis durum akisi:

`pending` -> `confirmed` -> `preparing` -> `shipped` -> `delivered`

Paneldeki butonlar bu kurala gore otomatik gorunur. Yeni siparis ve durum degisiklikleri SignalR ile geldiginde liste yenilenir; SignalR baglanamazsa `Yenile` butonu ve REST akisi calismaya devam eder.
