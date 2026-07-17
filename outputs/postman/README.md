# OpsFlow Postman Pack

Bu klasör, projeyi Postman ile tek tek test etmek için hazırlanmıştır.

## Dosyalar

- `opsflow.postman_collection.json`
- `opsflow.postman_environment.json`

## Import sırası

1. Postman'da önce environment dosyasını import et.
2. Sonra collection dosyasını import et.
3. Environment olarak `OpsFlow Local` seç.

## Önerilen test sırası

1. `Gateway > Get Services`
2. `Auth > Register`
3. `Auth > Login`
4. `Catalog > Create Category`
5. `Catalog > Create Product`
6. `Inventory > Adjust Stock`
7. `Basket > Add Basket Item`
8. `Basket > Get Basket`
9. `Order > Create Order`
10. `Order > Get My Orders`
11. `Order > Get Order`
12. `Notification > Create Notification`
13. `Notification > Get Notifications`

## Order auth note

- `Order > Get My Orders` endpoint expects a bearer token.
- Login first, then let the collection store the returned `accessToken` into `token`.

## Notlar

- `Basket` servisi stok kontrolü için `Inventory` servisine gRPC ile gider, ama bunu Postman'dan doğrudan değil Basket endpoint'i üzerinden görmen daha kolaydır.
- `Order` tarafında outbox mantığı arka planda çalışır; sipariş oluşturunca event yayınlama işi otomatik tamamlanır.
- Eğer servisleri Docker ile çalıştırıyorsan URL'ler `docker-compose.yml` içindeki portlarla eşleşmelidir.
