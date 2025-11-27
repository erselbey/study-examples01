# Microservice Veri Yönetimi

---

### **53. Database per service yaklaşımı**

**Tanım (teknik)**

Microservice mimarisinde her servis, *kendi verisinden sadece kendisi sorumlu* olur ve bunun için **kendi veritabanına** sahip olur. Order Service kendi DB’sine, Payment Service kendi DB’sine yazar; başka servisin tablosuna direkt SELECT/UPDATE yapmaz.

**Neden?**

- Servisler **loosely coupled** olur: Bir servisin şemasını değiştirince diğerleri kırılmaz.
- Her servis ihtiyaç duyduğu **veritabanı tipini** seçebilir (SQL, NoSQL, graph, search vs.).

**Günlük hayat benzetmesi**

Şirkette her departmanın kendi dolabı var düşün:

- İnsan kaynakları, personel dosyalarını *kendi dolabında* tutuyor.
- Muhasebe, faturaları *kendi dolabında* tutuyor.
    
    Kimse gidip diğer departmanın dolabına kafasına göre dokunmuyor; ihtiyaç varsa **resmi bir talep** (API) ile istiyor.
    

---

### **54. Polyglot Persistence ve seçim kriterleri**

**Tanım (teknik)**

“Polyglot persistence”, tek bir projede veya microservice mimarisinde **farklı veri ihtiyaçları için farklı veritabanı teknolojileri** kullanma pratiği:

- Siparişler → ilişkisel DB (PostgreSQL, SQL Server)
- Ürün kataloğu → doküman DB (MongoDB)
- Log ve event’ler → stream/append-only (Kafka)
- Arama → ElasticSearch gibi search engine

**Seçim kriterleri**

Veritabanı seçerken şunlara bakarsın:

- **Erişim deseni:** Çok join’li, güçlü transaction mı lazım → SQL; esnek doküman, nested JSON → NoSQL.
- **Tutarlılık vs. ölçeklenebilirlik:** Finans gibi alanlarda güçlü ACID; analitik ve log için eventual consistency.
- **Query tipi:** Full-text search, geo-query, graph ilişkiler vs.
- **Takım bilgisi ve operasyon maliyeti:** 5 farklı DB türü koyarsan operasyonel karmaşıklık artar.

**Günlük hayat benzetmesi**

Evde “her işi tek bıçakla” yapmaya çalışmıyorsun:

- Ekmek için ekmek bıçağı,
- Sebze için şef bıçağı,
- Meyve için küçük bıçak.
    
    Polyglot persistence = “her iş için en uygun aracı seçmek”.
    

---

### **55. EF Core Context per microservice**

**Tanım (teknik)**

.NET dünyasında her microservice için **kendi EF Core DbContext sınıfını** tanımlarsın. Bu DbContext sadece o servisin tablolarını/map’lerini içerir ve o servisin kendi veritabanına bağlanır.

**Neden?**

- Her microservice’in kendi DbContexti olması, **Database per Service** ve **Bounded Context** yaklaşımıyla birebir uyumlu.
- Her servis, EF Core migration’larını ve schema değişikliklerini **bağımsız** yönetir.

**Günlük hayat benzetmesi**

Şirket içi sistemlerde:

- İnsan Kaynakları uygulamasının kendi EF HrDbContexti,
- Muhasebe uygulamasının kendi AccountingDbContexti var.
    
    Bu iki uygulamayı tek DbContext ile birleştirmeye çalışırsan, “her değişiklikte herkesi etkileme” kabusu başlar.
    

---

### **56. Shared Database anti-pattern**

**Tanım (teknik)**

Birden fazla microservice’in **aynı veritabanını / aynı şemayı doğrudan paylaşması**. Yani hem OrderService hem CustomerService aynı DB’ye bağlanıp aynı tabloları okuyor/yazıyor. Bu, microservices.io’da açıkça **anti-pattern** olarak geçiyor.

**Neden anti-pattern?**

- Servisler **sıkı bağlı (tightly coupled)** olur. Bir tabloyu değiştirince bütün servisler etkilenir.
- Veriye kimin “sahip” olduğu belli olmaz → Ownership kaybı.
- Ölçekleme zorlaşır. Tek DB bottleneck olur.

**Günlük hayat benzetmesi**

3–4 kişi aynı Google Sheet’i API gibi kullanıyor düşün:

- Kolon isimlerini değiştiren biri, habersizce tüm raporları bozar.
- Kimse “bu kolona kim yetkili?” bilmiyor.
    
    Biri “tek DB kullanıyorsak microservice’e gerek yok, monolith yaz” diyor ya, tam o nokta.
    

---

### **57. Caching ve CQRS (Command Query Responsibility Segregation)**

**CQRS – Tanım (teknik)**

**CQRS**, okuma (Query) ve yazma (Command) operasyonlarını **farklı modellerle** yönetme pattern’idir:

- Command tarafı: “Sipariş oluştur”, “Ürünü güncelle” → domain kuralları, transaction’lar.
- Query tarafı: “Bu kullanıcının sipariş listesini getir” → okuma için optimize edilmiş read model.

**Caching ile ilişkisi**

- CQRS’te read tarafı çoğu zaman **cache gibi davranan**, sorgu için optimize edilmiş bir **view database / read model**’dir.
- Sık okunan verileri (ürün detayları, profil bilgisi, dashboard metrikleri vb.) Redis gibi bir cache’te veya read model DB’sinde tutarak performansı uçurursun.

**Günlük hayat benzetmesi**

- Ana kayıt defteri = yazma modeli (command-side, normal DB).
- Kasaya yakın fotokopi / özet liste = read modeli veya cache.
    
    Her seferinde ağır klasörü açmak yerine, sık sorulan sorular için hazır listeyi kullanıyorsun.
    

---

### **59. Event Sourcing kavramı**

**Tanım (teknik)**

Event Sourcing’de verinin **son halini** değil, o hale gelene kadar olan **tüm değişim olaylarını (events)** saklarsın:

- OrderCreated, ItemAddedToOrder, OrderShipped gibi event’ler append-only bir event store’a yazılır.
- Bir entity’nin güncel state’ini görmek istediğinde event’leri sırayla **replay** edersin.

**Avantajlar (kısaca)**

- Tam audit trail – ne, ne zaman, kim yüzünden oldu görebilirsin.
- Eski zamana “geri sarma” veya yeni read modeller üretme şansı.

**Günlük hayat benzetmesi**

Bankadaki hesabını düşün:

- Sadece “bakiye = 2.350 TL” bilmek (klasik model).
- Bir de “tüm hareket dökümü”nü bilmek (event sourcing).
    
    Event Sourcing, tam hareket dökümünü tutmak gibi; istersen dünü, geçen ayı, her şeyi tekrar hesaplayabilirsin.
    

---

### **60. Integration Events ve Event Handlers**

**Integration Event nedir?**

Farklı microservice’lerin veya bounded context’lerin **birbirinin durum güncellemelerinden haberdar olması** için yayınlanan olaylara *Integration Event* diyoruz.

Örnek: OrderPaidIntegrationEvent, UserRegisteredIntegrationEvent.

Amaç, state’i farklı sistemler arasında senkron tutmak.

**Event Handler nedir?**

Integration event’i **dinleyen ve işleyen** kod parçası.

Örnek:

- ShippingService’teki OrderPaidIntegrationEventHandler → “Bu sipariş ödenmiş, kargolamaya başla” der.

**Günlük hayat benzetmesi**

Online alışveriş:

- E-ticaret sitesi “Siparişiniz onaylandı” SMS’i/maile sebep olan event’i fırlatıyor.
- Kargo firması, faturalama sistemi vs. bu olayı dinleyip kendi tarafında aksiyon alıyor.
    
    Yani integration event, “benim tarafımda şu iş kesinleşti, ilgililer duysun” anonsu.
    

---

### **61. Outbox Pattern**

**Problem: dual-write sorunu**

Bir servis, bir iş yaptığında genelde iki şey ister:

1. Kendi veritabanına yazmak (örneğin Order tablosuna kayıt eklemek),
2. Diğer servisleri haberdar etmek için message broker’a event göndermek.

Bu iki yazma işlemi ayrı sistemlerde olduğu için “DB yazıldı ama mesaj gönderilemedi” ya da tam tersi gibi **tutarsızlık** riski doğar – buna *dual write problem* deniyor.

**Outbox Pattern çözümü (teknik)**

- DB transaction içinde **hem iş verisini hem de gönderilecek mesajı/outbox kaydını** aynı veritabanına yazıyorsun (ör. Outbox tablosu).
- Transaction commit olursa outbox kaydı garanti oluşuyor.
- Ayrı bir **Outbox Processor/Message Relay**, bu tabloyu tarayıp mesajları message broker’a gönderiyor.

**Günlük hayat benzetmesi**

Kargo şubesinde:

- Görevli, yapılan işlemleri önce **deftere (outbox)** yazar.
- Başka bir görevli bu defteri belirli aralıklarla okuyup, ilgili yerlere (merkez, diğer şubeler) bilgi/mektup yollar.
    
    Böylece “işlem deftere yazıldı ama haber verilmedi” gibi tutarsızlıklar minimize edilir.
    

---

### **62. Transactional messaging**

**Tanım (teknik)**

**Transactional messaging**, veritabanı güncellemesiyle mesaj yayınlama işinin **tek bir atomik işlem** gibi çalışmasını sağlayan yaklaşımdır. Yani:

> “DB’ye yazıldıysa mesaj da
> 
> 
> *kesin*
> 
> *kesin*
> 

Pratikte bu genellikle **Outbox Pattern + message broker (Kafka/RabbitMQ)** kombinasyonuyla uygulanır.

**Günlük hayat benzetmesi**

Online alışverişte:

- Siparişin sisteme kaydedilip **sana mail/SMS gitmesi** aynı “iş”in parçası.
    
    Transactional messaging, “sipariş gerçekten oluşmadıysa, asla onay maili gitmesin” garantisini sağlamaya çalışır.
    

---

### **63. Saga Pattern örnekleri**

**Kısa hatırlatma**

Saga Pattern, tek bir büyük dağıtık transaction yerine, **birbirini takip eden lokal transaction’lar ve gerektiğinde geri alan (compensating) adımlar** kullanır.

**Örnek 1 – E-ticaret sipariş süreci**

Adımlar:

1. **Order Service:** Siparişi PENDING durumda oluşturur.
2. **Inventory Service:** Stok ayırır.
3. **Payment Service:** Ödemeyi çeker.
4. **Order Service:** Sonuca göre APPROVED ya da REJECTED yapar.

Hata senaryosu (Saga’nın gücü):

- Ödeme başarısız → Payment Service “payment failed” event’i atar.
- Inventory Service stok ayırmayı geri alır (compensating transaction).
- Order Service siparişi CANCELLED yapar.

**Örnek 2 – Uçak + Otel + Araç kiralama**

1. FlightService: Bileti rezerve et.
2. HotelService: Oteli rezerve et.
3. CarRentalService: Aracı rezerve et.

Eğer 3. adımda araç bulunamazsa:

- Hotel rezervasyonunu iptal eden adım,
- Uçak biletini iptal eden adım devreye girer.

**Günlük hayat benzetmesi**

“Tur paketi” alıyorsun; tur şirketi senin adına uçak, otel, tur rehberi, araç kiralama gibi bir sürü işi arka arkaya yapıyor. Ortalarda bir şey patlarsa, daha önce yapılan rezervasyonları iptal eden geri adımları var. İşte Saga, yazılımda bu orkestrasyonu tarif ediyor.

---