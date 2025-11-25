# Hafta 6 – ASP.NET Core MVC ve RESTful Tasarım

## Öğrenme Hedefleri
- MVC mimarisinin Controller-View-Model ayrımını uygulamak
- REST ilkelerine uygun endpoint tasarlamak, HTTP status kodlarını doğru kullanmak
- Swagger/Swashbuckle ile API dokümantasyonu ve testini yapmak

## Oturum Planı
1. **Teori (45 dk)** – MVC yaşam döngüsü, routing, model binding.
2. **View Tarafı (30 dk)** – Razor view söz dizimi, layout, partial view, tag helpers.
3. **REST Prensipleri (30 dk)** – Kaynak isimlendirme, idempotent istekler, DTO kullanımı.
4. **Atölye (60 dk)** – Görev yönetim API'sine MVC arayüzü ekleme.

## Atölye Adımları
1. `dotnet new mvc -n TaskBoard` komutu ile yeni proje oluşturulur.
2. `TaskItem` modeli API katmanından paylaşılan bir class library'ye taşınır.
3. Controller içinde `ILogger<TaskController>` ve repository DI üzerinden alınır.
4. Endpoint ve view'lerde kullanılan DTO'lar, veri doğrulama (DataAnnotations) ile desteklenir.
5. Swagger paketini (Swashbuckle.AspNetCore) ekleyip `Startup/Program` dosyasında etkinleştir.

## Ödev
- MVC uygulamasına filtre/sort seçenekleri ekle ve sonuçları partial view içinde göster.
- API tarafında `ProblemDetails` dönecek hata sonuçlarını ayarla.
- Swagger UI üzerinde örnek istek gövdeleri ve açıklamalarını güncelle.

## Kontrol Listesi
- [ ] Controller'lar yalnızca orchestration içeriyor, iş mantığı servislere taşındı.
- [ ] View modelleri domain modellerinden ayrıldı.
- [ ] REST endpointleri doğru HTTP kodları ve aksiyon isimleriyle hizalı.

## Ek Kaynaklar
- Microsoft Docs: *Introduction to ASP.NET Core MVC*
- Swagger resmi sitesi: *OpenAPI Specification*
