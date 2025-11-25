# Hafta 7 – Veri Erişimi ve Entity Framework Core

## Öğrenme Hedefleri
- EF Core kullanarak DbContext, DbSet ve migrasyon kavramlarını uygulamak
- İlişkili tablolarla çalışma, include/theninclude ile eager loading yapmak
- Repository + Service katmanı oluşturarak veri erişimini soyutlamak

## Oturum Planı
1. **Intro (30 dk)** – ORM nedir, EF Core mimarisi, alternatifler (Dapper).
2. **DbContext Tasarımı (30 dk)** – Konvansiyonlar, Fluent API, veri tohumlama.
3. **Migrasyonlar (30 dk)** – `dotnet ef migrations add`, `dotnet ef database update`.
4. **Atölye (60 dk)** – Görev yönetim API'sini EF Core ve SQLite ile entegre etmek.

## Atölye Adımları
1. `TaskBoard.Infrastructure` adında class library oluşturulup EF Core paketleri eklenir.
2. `TaskDbContext` sınıfı tanımlanır, `DbSet<TaskItem>` ve `DbSet<Category>` eklenir.
3. `TaskRepository` sınıfı EF Core üzerinden CRUD işlemlerini gerçekleştirir.
4. `SeedData` sınıfı ile başlangıç görevleri eklenir; uygulama başlarken `Database.Migrate()` çağrılır.
5. Performans için sorgular `AsNoTracking()` ile optimize edilir.

## Ödev
- Repository katmanına pagination ve arama metodları ekle.
- `TaskDbContext` için integre test yaz (InMemory provider kullanarak).
- Dapper kullanarak bir raporlama endpointi yaz ve EF ile farklarını dokümante et.

## Kontrol Listesi
- [ ] DbContext ve migrasyonlar başarıyla oluşturuldu.
- [ ] Transaction gerektiren operasyonlar `await using var transaction = await context.Database.BeginTransactionAsync();` desenini kullanıyor.
- [ ] Repository metotları async/await ile yazıldı ve servis katmanında kullanılıyor.

## Ek Kaynaklar
- Microsoft Learn: *Build data-driven apps with EF Core*
- Julie Lerman: *EF Core Getting Started* makaleleri
