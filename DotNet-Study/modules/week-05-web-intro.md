# Hafta 5 – .NET Proje Tipleri ve ASP.NET Core'a Giriş

## Öğrenme Hedefleri
- .NET SDK yapısı, proje şablonları ve bağımlılık yönetimini anlamak
- ASP.NET Core pipeline'ını tanıyıp minimal API kurmak
- Konfigürasyon yönetimi ve dependency injection temelini kullanmak

## Oturum Planı
1. **Teorik Giriş (30 dk)** – Proje şablonları (`console`, `classlib`, `web`, `mvc`), `csproj` dosyasının yapısı.
2. **CLI Demo (30 dk)** – `dotnet new webapi` ve `dotnet new mvc` farkları.
3. **Minimal API (60 dk)** – `MapGet`, `MapPost`, model binding, `appsettings.json`.
4. **Atölye (45 dk)** – `examples/week-05-minimal-product-api` üzerinde endpoint geliştirme.

## Atölye Adımları
1. `examples/week-05-minimal-product-api` klasörüne girip `dotnet run` ile API'yi ayağa kaldır.
2. `Program.cs` içindeki yorumları takip ederek doğrulama, logging ve DI örneklerini tamamla.
3. Postman/Bruno kullanarak GET/POST isteklerini test et, çıkan sonuçları `docs/` klasörüne kaydet.

## Ödev
- Minimal API'yi `Product` dışında `Category` varlığıyla genişlet.
- `IProductRepository` arayüzünü oluşturup DI ile uygulama içine ekle.
- Swagger dokümantasyonunu özelleştirip örnek istek/yanıtları tanımla.

## Kontrol Listesi
- [ ] `dotnet new` komutlarıyla farklı proje tiplerini deneyimledim.
- [ ] Minimal API'de dependency injection kullanarak veri sağlayıcısı enjekte ettim.
- [ ] Konfigürasyon değerlerini `IConfiguration` üzerinden okudum.

## Ek Kaynaklar
- Microsoft Learn: *Build a minimal API with ASP.NET Core*
- Steve Smith blog: *Minimal APIs best practices*
