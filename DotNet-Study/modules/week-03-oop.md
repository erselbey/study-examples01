# Hafta 3 – Nesne Yönelimli Programlama

## Öğrenme Hedefleri
- Sınıf ve nesne kavramlarını gerçek senaryoya uygulamak
- Kalıtım, polimorfizm, interface ve abstract sınıflar arasındaki farkı açıklamak
- SOLID prensiplerine uyumlu basit domain modeli tasarlamak

## Oturum Planı
1. **Giriş (15 dk)** – Önceki hafta görev uygulamasındaki kod tekrar değerlendirilir, sınıf tasarımına geçiş gerekçeleri anlatılır.
2. **Teori (45 dk)** – Erişim belirleyiciler, constructor, immutability, `record`.
3. **Alt Konular (30 dk)** – Interface vs abstract class örnekleri, dependency inversion.
4. **Canlı Kodlama (45 dk)** – `examples/week-03-library-manager` projesi üzerinden katmanlı yapı kurulması.
5. **Atölye (45 dk)** – Katılımcılar domain modeline yeni varlıklar ekler.

## Atölye Adımları
1. `LibraryItem` temel sınıfı oluşturulur; `Book` ve `Magazine` sınıfları bu sınıftan kalıtım alır.
2. `IBorrowable` interface'i tanımlanır ve ödünç verilebilen sınıflar tarafından uygulanır.
3. Katılımcılar, ödünç alma sürecini yönetmek için `LoanService` adında basit bir servis ekler.
4. Kod içerisine, neden bu yapının tercih edildiğini açıklayan yorumlar eklenir.

## Ödev
- `LibraryManager` uygulamasına kullanıcı kayıt sistemi ve ödünç alma limitleri ekle.
- Her yeni sınıf için sorumluluklarını açıklayan 1-2 satırlık yorumlar yaz.
- Yazdığın kodu UML sınıf diyagramı (elle çizilmiş olabilir) ile destekleyip paylaş.

## Kontrol Listesi
- [ ] Tüm domain sınıfları tek sorumluluk prensibine uygun.
- [ ] Arayüz ve abstract sınıfların kullanım nedeni kod içinde belgelenmiş.
- [ ] `LoanService` temel validasyonları yapıyor ve başarısız durumlarda anlamlı mesajlar dönüyor.

## Ek Kaynaklar
- Robert C. Martin – *SOLID Principles* makaleleri
- Pluralsight: *C# Interfaces, Abstract Classes and Inheritance*
