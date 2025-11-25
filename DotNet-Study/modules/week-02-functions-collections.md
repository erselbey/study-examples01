# Hafta 2 – Fonksiyonlar ve Koleksiyonlar

## Öğrenme Hedefleri
- Metot tanımlama, parametre geçme ve değer döndürme mekanizmalarını kavramak
- `List<T>`, `Dictionary<TKey, TValue>` gibi koleksiyonları kullanmak
- Basit LINQ sorguları yazarak koleksiyonları filtrelemek

## Oturum Planı
1. **Isınma (15 dk)** – Önceki haftanın tahmin oyunu ödevinin değerlendirilmesi.
2. **Teori (45 dk)** – Metot imzaları, overloading, default parametreler.
3. **Koleksiyonlar (30 dk)** – List/Dictionary karşılaştırması, pratik ekleme/çıkarma örnekleri.
4. **LINQ (30 dk)** – `Where`, `Select`, `OrderBy` operatörlerine kısa demo.
5. **Atölye (60 dk)** – Görev takip uygulamasının fonksiyonel hale getirilmesi.

## Atölye Akışı
1. `examples/week-01-number-guess` projesi `examples/week-03-library-manager` klasörüne taşınmaz; bunun yerine yeni proje kurulur.
2. Veri modeli olarak `TaskItem` sınıfı oluşturulup görevler `List<TaskItem>` içinde tutulur.
3. Kullanıcının görev ekleyip filtreleyebileceği menü hazırlanır.
4. Filtreleme için LINQ kullanımı zorunludur (örneğin `tasks.Where(t => t.IsCompleted)`).

## Ödev
- Görev yönetim uygulamasına kategorilere göre filtreleme, tarih bazlı sıralama ve arama ekle.
- Metot isimlerini, parametre listesini ve dönüş tiplerini açıklayan kısa yorumlar yaz.

## Kontrol Listesi
- [ ] Metotları tek sorumluluk prensibine göre organize ettim.
- [ ] LINQ ile en az iki farklı filtreleme/sıralama gerçekleştirdim.
- [ ] Uygulamadaki tüm kullanıcı aksiyonları ayrı metotlara taşındı.

## Ek Kaynaklar
- Microsoft Learn: *Metotlar ve diziler*
- Dotnet resmi örnekleri: *Language Integrated Query (LINQ) 101 Samples*
