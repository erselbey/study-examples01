# Hafta 8 – Test, Yayınlama ve DevOps'a Giriş

## Öğrenme Hedefleri
- xUnit veya MSTest ile birim testi yazmak, Arrange-Act-Assert akışını uygulamak
- Mocking araçları (Moq, NSubstitute) ile bağımlılıkları izole etmek
- `dotnet publish`, Docker ve temel CI/CD kavramlarıyla uygulamayı dağıtmak

## Oturum Planı
1. **Test Teorisi (30 dk)** – Test piramidi, birim/entegrasyon testi farkı.
2. **xUnit Demo (45 dk)** – Test projesi oluşturma, fixture kullanımı, veri odaklı testler.
3. **Mocking (30 dk)** – Moq ile repository/servisleri izole etme.
4. **Dağıtım (45 dk)** – `dotnet publish`, Dockerfile, GitHub Actions pipeline tanıtımı.
5. **Final Proje Sunumları (30 dk)** – Katılımcılar uygulamalarını gösterir.

## Atölye Adımları
1. `dotnet new xunit -n TaskBoard.Tests` komutu ile test projesi oluşturulur ve solution'a eklenir.
2. Servis katmanındaki kritik metotlar için pozitif/negatif senaryolar test edilir.
3. Dockerfile yazılarak API containerize edilir, `docker build` ile imaj alınır.
4. Örnek GitHub Actions workflow dosyası `ci-cd/dotnet.yml` altında paylaşılır.

## Ödev
- Tüm kritik servisler için %70+ code coverage hedefle.
- Docker imajını container registry'e gönder ve sürüm numarası ver.
- CI pipeline'ında test + publish adımlarını otomatikleştir.

## Kontrol Listesi
- [ ] Her servis için en az bir pozitif ve bir negatif test var.
- [ ] `dotnet publish -c Release` çıktısı doğrulandı.
- [ ] Docker imajı çalıştırılıp API sağlığı kontrol edildi.

## Ek Kaynaklar
- Microsoft Learn: *Test .NET applications with xUnit*
- GitHub Docs: *Set up a .NET workflow*
