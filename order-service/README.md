# Sipariş Servisi

Bu proje, basit bir sipariş yönetimi servisi sağlar ve ASP.NET Core kullanılarak geliştirilmiştir. Kullanıcıların ürünleri listelemelerine, yeni ürün eklemelerine ve sipariş oluşturmalarına olanak tanır.

## Özellikler

- Ürün Yönetimi
  - Tüm ürünleri listeleme
  - Belirli bir ürünü ID ile görüntüleme
  - Yeni ürün ekleme

- Sipariş Yönetimi
  - Tüm siparişleri listeleme
  - Belirli bir siparişi ID ile görüntüleme
  - Yeni sipariş ekleme

- Swagger Entegrasyonu
  - API uç noktalarını test etmek için kullanıcı ara yüzü

## Teknolojiler

- ASP.NET Core
- C#
- In-Memory Database (Bellek içi veri saklama)

## Gereksinimler

- .NET 6.0 veya üstü

## Kurulum

1. Bu projeyi klonlayın:

    bash     git clone https://github.com/erselbey/study-examples01.git     

2. Gerekli bağımlılıkları yükleyin:

    bash     dotnet restore     

3. Uygulamayı çalıştırın:

    bash     dotnet run     

4. Tarayıcınızı açın ve http://localhost:5000/swagger adresine gidin. Burada API uç noktalarını test edebilirsiniz.

## Kullanım

- Ürünler API Uç Noktaları
  - GET /api/products: Tüm ürünleri listele
  - GET /api/products/{id:int}: Belirli bir ürünü getir (ID ile)
  - POST /api/products: Yeni ürün ekle (JSON formatında)

- Siparişler API Uç Noktaları
  - GET /api/orders: Tüm siparişleri listele
  - GET /api/orders/{id:int}: Belirli bir siparişi getir (ID ile)
  - POST /api/orders: Yeni sipariş ekle (JSON formatında)

## Örnek İstekler

### Yeni Ürün Ekleme

```http
POST /api/products
Content-Type: application/json

{
  "name": "Game Controller",
  "price": 59.99,
  "stock": 30
}


### Yeni Sipariş Oluşturma

POST /api/orders
Content-Type: application/json

{
  "productId": 1,
  "quantity": 2
}
