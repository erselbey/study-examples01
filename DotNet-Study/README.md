# Başlangıç Temelli .NET Eğitimi

Bu repo, .NET ekosistemine sıfırdan adım atanlar için 8 haftalık bir öğrenme yolu, haftalık kaynak dosyaları ve örnek uygulamalar sunar. Her modül kendi klasöründe detaylı hedefler, atölye adımları, ödevler ve kontrol listeleri içerir. Örnek projeler, kod içi yorumlarla yol göstererek eğitmen veya katılımcıların kolayca takip edebileceği hale getirildi.

## Depo Yapısı

```text
DotNet-Study
├── README.md                      # Genel bakış ve hızlı başlangıç rehberi
├── modules/                       # Haftalık detaylı içerikler (8 adet md dosyası)
│   ├── week-01-csharp-basics.md
│   ├── week-02-functions-collections.md
│   ├── week-03-oop.md
│   ├── week-04-error-handling-debugging.md
│   ├── week-05-web-intro.md
│   ├── week-06-mvc-rest.md
│   ├── week-07-data-access.md
│   └── week-08-testing-deployment.md
├── examples/                      # Canlı kodlama/ödev başlangıç projeleri
│   ├── week-01-number-guess/
│   │   └── Program.cs
│   ├── week-03-library-manager/
│   │   └── Program.cs
│   └── week-05-minimal-product-api/
│       └── Program.cs
└── .vscode/
    └── settings.json
```

> Not: Diğer haftalar için de örnek projeler eklemek istersen aynı yapıyı izleyebilirsin. README, katılımcılara hangi klasörü incelemeleri gerektiğini gösterir.

## Program Genel Yapısı
- **Süre:** 8 hafta (haftada ~6 saat aktif çalışma + ödev)
- **Ön Koşullar:** Temel bilgisayar kullanımı ve algoritmik düşünmeye giriş seviyesi
- **Araçlar:** .NET SDK 9+, Visual Studio Code, Git, Postman/Bruno, SQLite veya hafif bir veritabanı
- **Kazanımlar:** C# dil temelleri, nesne yönelimli programlama, ASP.NET Core (Minimal API + MVC), Entity Framework Core, test & yayınlama

## Haftalık Modüller

| Hafta | Konu | Dosya | İçerik başlıkları |
|------|------|-------|-------------------|
| 1 | C# Temelleri | [week-01-csharp-basics.md](modules/week-01-csharp-basics.md) | CLI, veri tipleri, döngüler, tahmin oyunu atölyesi |
| 2 | Fonksiyonlar ve Koleksiyonlar | [week-02-functions-collections.md](modules/week-02-functions-collections.md) | Metotlar, List/Dictionary, LINQ başlangıcı |
| 3 | Nesne Yönelimli Programlama | [week-03-oop.md](modules/week-03-oop.md) | Domain modelleme, SOLID, Library Manager örneği |
| 4 | Hata Yönetimi & Debugging | [week-04-error-handling-debugging.md](modules/week-04-error-handling-debugging.md) | try/catch, özel istisnalar, logging, VS Code debugger |
| 5 | Web'e Giriş & Minimal API | [week-05-web-intro.md](modules/week-05-web-intro.md) | Proje şablonları, dependency injection, minimal API |
| 6 | MVC & REST Tasarımı | [week-06-mvc-rest.md](modules/week-06-mvc-rest.md) | Controller/View, DTO, Swagger dokümantasyonu |
| 7 | Veri Erişimi & EF Core | [week-07-data-access.md](modules/week-07-data-access.md) | DbContext, migrasyon, repository + service katmanı |
| 8 | Test & Yayınlama | [week-08-testing-deployment.md](modules/week-08-testing-deployment.md) | xUnit, mocking, Docker, CI/CD, final sunum |

Her modül dosyasında:
- Oturum planı (dakika bazlı)
- Atölye yönergeleri (adım adım)
- Ödev tanımı ve teslim şartları
- Kontrol listesi ve ek kaynak önerisi

## Örnek Uygulamalar

| Yol | Açıklama | Çalıştırma |
|-----|----------|------------|
| `examples/week-01-number-guess/Program.cs` | Konsol tabanlı tahmin oyunu. Kod içi yorumlar CLI kullanımını ve döngü yapısını açıklar. | `dotnet run --project examples/week-01-number-guess` |
| `examples/week-03-library-manager/Program.cs` | OOP kavramlarını gösteren küçük kütüphane domain'i. Abstract sınıflar, interface ve özel istisna içerir. | `dotnet run --project examples/week-03-library-manager` |
| `examples/week-05-minimal-product-api/Program.cs` | Minimal API ile ürün servisinin çekirdeği. Repository desenine dair yönlendirici yorumlar barındırır. | `dotnet run --project examples/week-05-minimal-product-api` |

> Tüm örnekler tek `Program.cs` dosyası altında tutuldu. `dotnet new console` veya `dotnet new web` komutlarıyla tam projeler oluşturup bu dosyaları `Program.cs` yerine yapıştırarak hızlıca çalıştırabilirsin.

## Öğrenme Döngüsü
1. **Teori** – Modül dosyasındaki hedefler ve sunum slaytları.
2. **Canlı Kodlama** – `examples/` klasöründeki başlangıç kodları üzerinden ilerleyip yorum satırlarına göre eksikleri tamamlama.
3. **Atölye** – Katılımcıların kodu genişlettiği, eğitmenin ise kontrol listesine göre geri bildirim verdiği bölüm.
4. **Ödev** – Modül sonundaki gereksinimleri gerçek hayat senaryolarıyla pekiştirme.

## Örnek Ders Akışı (Hafta 2)
1. **Isınma (15 dk):** Önceki ödevlerin kod incelemeleri.
2. **Teori (45 dk):** Fonksiyonlar, parametreler, dönüş tipleri.
3. **Canlı Kodlama (45 dk):** Görev listesi uygulaması.
4. **Ara (15 dk)**
5. **Uygulama (60 dk):** LINQ ile filtreleme ve arama ekleme.
6. **Kapanış (15 dk):** Quiz + sonraki ödevin paylaşılması.

## Değerlendirme & Ödev Çerçevesi
- Haftalık ödevler, modül dosyalarında yer alan kriterlere göre puanlanır.
- 3. ve 6. haftalarda kısa quiz ile teorik bilgiyi test et.
- 8. hafta final projesi; API + veri + test kapsayacak şekilde demo edilir.

## Ek Kaynaklar
- Microsoft Learn modülleri (C#, ASP.NET Core, EF Core yolları)
- .NET resmi dokümantasyonu (learn.microsoft.com/dotnet)
- Örnek projeler: eShopOnWeb, Clean Architecture template
- Topluluklar: .NET Türkiye, Stack Overflow, dev.to/.NET

Bu yapı, eğitimi hızla başlatıp katmanlı şekilde ilerlemeni sağlar. Yeni içerikler veya ek örnekler eklemek istediğinde aynı klasör düzenini takip ederek programı zenginleştirebilirsin. İhtiyacın olursa her hafta için slayt taslakları ya da proje gereksinim belgeleri de oluşturabilirim.
