# Hafta 4 – Hata Yönetimi, Dosya İşlemleri ve Debugging

## Öğrenme Hedefleri
- `try/catch/finally` bloklarını doğru senaryolarda kullanmak
- `IDisposable` ve `using` deseninin önemini kavramak
- VS Code debug araçlarıyla breakpoint, watch ve call stack incelemesi yapmak

## Oturum Planı
1. **Giriş (10 dk)** – Önceki haftaya ait hataların paylaşılması.
2. **Exception Mekanizması (40 dk)** – Standart istisnalar, özel istisna sınıfı oluşturma.
3. **Kaynak Yönetimi (30 dk)** – Dosya/stream işlemleri, JSON okumak/yazmak.
4. **Logging (20 dk)** – Console logger, Serilog gibi kütüphanelere giriş.
5. **Debugging Atölyesi (60 dk)** – `LibraryManager` üzerinde hata ayıklama turu.

## Atölye Adımları
1. `examples/week-03-library-manager` projesinde bilinçli olarak bazı hatalar bırakılır.
2. Katılımcılar `dotnet run` ile hatayla karşılaştıktan sonra breakpoint koyup değişkenleri izler.
3. Veri kaydetmek için `library-log.json` dosyası oluşturulur; `File.WriteAllText` kullanılırken `using` bloğu eklenir.
4. Özel istisna (`LoanLimitExceededException`) yazılarak kullanıcıya daha açıklayıcı mesaj gösterilir.

## Ödev
- Uygulamanın tüm giriş noktalarında try/catch kullanmak yerine merkezi hata yönetimi tasarla.
- `Serilog` veya benzeri bir kütüphane ekleyerek hataları dosyaya yaz.
- Debug oturumu sırasında konulan breakpoint ve watch listelerinin ekran görüntüsünü paylaş.

## Kontrol Listesi
- [ ] Her IO işlemi `using` bloğu veya `await using` ile güvence altına alındı.
- [ ] Özel istisnalar anlamlı isimler ve mesajlarla tanımlandı.
- [ ] Debugger kullanılarak en az bir karmaşık hata çözüldü.

## Ek Kaynaklar
- Microsoft Docs: *Exception Handling Basics*
- VS Code resmi rehberi: *Debug .NET Applications*
