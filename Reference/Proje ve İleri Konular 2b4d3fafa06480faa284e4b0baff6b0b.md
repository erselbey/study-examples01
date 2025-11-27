# Proje ve İleri Konular

---

## **7. Gün – Proje ve İleri Konular (Genel İçerik)**

### **113. Microservice orchestration ve choreography**

- **Amaç:** Dağıtık iş akışlarını yönetmenin iki ana yaklaşımını (orchestration vs choreography) kavratmak.
- **İçerik:**
    - Orchestration: Merkezi bir “orkestratör” servisin diğer microservice’lere hangi sırada ne yapacaklarını komutla söylemesi (command-driven workflow).
    - Choreography: Merkezi beyin yok; servisler yayınlanan event’leri dinleyip kendi rollerine göre tepki veriyor (event-driven, gevşek bağlı yapı).
    - Hangi senaryoda hangisi? Örneğin ödeme–sipariş–stok süreçlerinde Saga orchestrator kullanımı vs tamamen event tabanlı koreografi.

---

### **114. API Gateway advanced routing (Ocelot)**

- **Amaç:** Basit “/api/order → OrderService” yönlendirmesinin ötesine geçip Ocelot ile gelişmiş routing ve cross-cutting yeteneklerini göstermek.
- **İçerik:**
    - Path/host bazlı gelişmiş route tanımları, upstream / downstream mapping.
    - Request aggregation (birden fazla microservice çağrısını tek endpoint’te birleştirme).
    - Ocelot ile load balancing, caching, authentication/authorization ve rate limiting gibi özellikleri konfigürasyonla ekleme.
- **Senaryo:** Client sadece API Gateway’i görüyor; ürün listesi + stok + fiyatı tek endpoint’ten alırken gateway arkada 2–3 servise gidip sonucu birleştiriyor.

---

### **115. Circuit Breaker pattern (Polly)**

- **Amaç:** “Bozuk servise ısrarla istek atıp tüm sistemi gömmemek” için Circuit Breaker mantığını ve Polly ile implementasyonunu göstermek.
- **İçerik:**
    - Circuit Breaker pattern: Belirli sayıda hata sonrası hattı “açık” konuma getirip çağrıları geçici olarak kesme, sonra “yarı açık” moda dönüp kontrol etme.
    - Polly ile Circuit Breaker + Retry + Timeout kombinasyonları ve .NET’te kullanım örnekleri.
- **Senaryo:** PaymentService down olduğunda OrderService’in 50 kez deneyip CPU yakması yerine belli sayıda denemeden sonra devreyi kesmesi ve hatayı kontrollü yönetmesi.

---

### **116. Caching stratejileri (Redis / MemoryCache)**

- **Amaç:** Hem basit MemoryCache hem de Redis ile microservice’lerde performanslı cache mimarilerini anlatmak.
- **İçerik:**
    - Caching temel kavramı, cold/hot cache, TTL, eviction.
    - In-memory cache (MemoryCache) vs distributed cache (Redis) karşılaştırması.
    - Mikroservis bağlamında:
        - Her servisin yanında kendi Redis cache’i (query caching, per-service cache).
        - Ortak paylaşılan Redis cluster ile distributed cache topolojileri.
- **Senaryo:** Ürün kataloğunun sık okunan verilerini Redis’te cache’leyip veritabanı yükünü düşürme, OrderService’in kendi in-memory cache’ini sadece kısa süreli lookup için kullanması.

---

### **117. Rate limiting ve throttling**

- **Amaç:** API’leri hem kötü niyetli hem de yanlış konfigürasyonlu client’lardan korumayı öğretmek.
- **İçerik:**
    - Rate limiting: Belirli zaman aralığında kullanıcı/IP/token başına maksimum istek sayısı.
    - Throttling: Gelen trafiği yavaşlatma veya kademeli kısıtlama (örneğin arka uç baskılanmasın diye).
    - Ocelot ve API Gateway üzerinden rate limiting konfigürasyonu (route bazlı limitler, client bazlı kotalar).
- **Senaryo:** Ücretsiz plandaki kullanıcılar için saniyede 5 istek, premium kullanıcılar için 50 istek; brute force ve DDoS benzeri davranışların gateway seviyesinde kesilmesi.

---

### **118. Mini proje: full microservice uygulaması**

- **Amaç:** İlk 6 günde öğrendiğin her şeyi kullanarak uçtan uca çalışan küçük ama “gerçekçi” bir microservice sistemi kurmak.
- **İçerik:**
    - Domain seçimi: Küçük e-ticaret / sipariş sistemi (Catalog, Basket, Order, Payment, Notification gibi 3–5 servis).
    - Her servis için bağımsız API, DB, Docker image, Kubernetes deployment.
    - API Gateway, auth, logging, messaging, caching, CI/CD entegrasyonu.
- **Çıktı:**
    - Git repo + docs + diyagram (topology / sequence).
    - Local veya dev cluster’da çalışan, uçtan uca test edilebilir bir sistem.

---

### **119. Event-driven microservice uygulama final**

- **Amaç:** Projeyi özellikle event-driven tarafa odaklayıp Integration Event, Outbox, Saga gibi kavramları gerçek senaryoda kullanmak.
- **İçerik:**
    - Sipariş akışının event’lerle yönetilmesi (OrderCreated, PaymentCompleted, StockReserved, OrderFailed…).
    - Event Bus (RabbitMQ / Kafka) kullanımı, consumer’lar, event handler’lar.
    - Gerekirse Saga orchestrator veya choreography yaklaşımıyla iş akışını bağlamak.
- **Çıktı:**
    - “Monolith değil, gerçekten event-driven microservice çalışıyor” diyebileceğin final akışı.

---

### **120. Logging, metrics, monitoring integration**

- **Amaç:** Sadece “çalışıyor” değil, “gözlemlenebilir (observable)” bir sistem kurmak.
- **İçerik:**
    - Uygulama log’larını merkezi bir yere toplama (ELK, Azure Monitor, vs.).
    - Prometheus ile metrik toplamak (request sayısı, latency, error rate, queue length).
    - Grafana ile dashboard ve alert kurgulama (p95 latency, 5xx oranı, CPU, memory vs.).
- **Senaryo:**
    - “Sipariş yaratma akışı şu an yavaşladı mı?”,
    - “PaymentService hata oranı arttı mı?”,
    - “Mesaj kuyruklarında birikme var mı?” sorularına dashboard üzerinden cevap verebilmek.

---

### **121. Security review ve audit**

- **Amaç:** Projeyi güvenlik açısından tarayıp temel açıkları bulmak ve düzeltmek.
- **İçerik:**
    - Endpoint’lerde auth / authz kontrolleri, rol/claim bazlı yetki.
    - Güvenli headers, OWASP’in önerdiği temel kontroller (input validation, security misconfiguration vs.).
    - Secrets yönetimi (connection string, API key’lerin environment / KeyVault üzerindeki durumu).
    - Log’larda hassas veri (şifre, kart bilgisi) olup olmadığının kontrolü.
- **Senaryo:**
    - Küçük “security checklist” üzerinden projeyi item item gözden geçirmek.
    - Örnek küçük zafiyetler bırakıp bunları birlikte tespit etmek (eğitim için).

---

### **122. Performance testing**

- **Amaç:** Sistemin sadece “tek kullanıcıda hızlı” değil, yük altında nasıl davrandığını görmek.
- **İçerik:**
    - Load / stress test kavramları, p95/p99 latency, throughput, error rate gibi temel metrikler.
    - Örnek araçlar: k6, JMeter, Azure Load Testing vs. (hangi aracı kullanacağın ortama göre seçilebilir).
    - Test senaryosu: “10 dk boyunca saniyede 100 istekle sipariş oluştur” gibi script’ler ile bottleneck tespiti.
- **Bağlantı:**
    - Buradaki sonuçlar, bir önceki adımda kurduğun monitoring ve alerting ile beraber anlamlı hâle gelecek (grafikler üzerinden analizi).

---

### **123. Disaster recovery ve high availability**

- **Amaç:** “Node çöktü, region gitti, DB bozuldu” gibi gerçek hayattaki kötü senaryolara hazırlıklı olmak.
- **İçerik:**
    - High Availability (HA): redundant servis kopyaları, load balancing, auto-scaling, health check, failover stratejileri.
    - Disaster Recovery (DR): yedekleme, restore planları; RTO (ne kadar sürede ayağa kalkmalıyım?) ve RPO (ne kadar veri kaybına tahammülüm var?).
    - Microservice özelinde: state tutan servisler (DB, queue, file storage) için DR senaryoları, tüm sistemi aynı anda yedeklemenin zorlukları.
- **Senaryo:**
    - “OrderService pod’larını kasıtlı olarak öldürüp” sistemin nasıl davrandığını gözlemlemek (chaos / failover testi).

---

### **124. Genel tekrar ve proje sunumu**

- **Amaç:** Bütün haftayı toparlamak, kavramları projeyle bağlayıp “gerçek bir mimari sunum” deneyimi kazandırmak.
- **İçerik:**
    - Her grubun/kişinin mini projesini tanıtması:
        - Mimari diyagram (servisler, DB’ler, Gateway, message broker, cache, observability).
        - Kullandığı design pattern’ler (Saga, Outbox, Circuit Breaker, Caching, Rate Limiting…).
        - Seçilen teknolojilerin kısa gerekçeleri.
    - Eğitmen / ekip olarak Soru-Cevap ve “şöyle yapsan daha iyi olurdu” tarzı feedback.
    - Son olarak: 1–2 tane “gerçek hayatta neler farklı olacak?” örneği (organizasyon, takım yapısı, DevOps süreçleri, cloud maliyeti vs.).

---