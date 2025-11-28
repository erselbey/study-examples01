# Messaging ve Event Bus

---

### **89. RabbitMQ / Kafka setup ve configuration**

**RabbitMQ / Kafka nedir?**

İkisi de **message broker**: servisler arası mesajları güvenli ve kontrollü taşımak için kullanıyoruz.

- **RabbitMQ**: Queue temelli, AMQP protokolü, klasik iş kuyruğu ve routing senaryoları için çok kullanılıyor.
- **Kafka**: Yüksek hacimli **event stream** işlemek için tasarlanmış; topic/partition yapısıyla log akışlarını çok hızlı taşıyor.

**Setup & configuration’da ne yapıyoruz? (özet)**

- Broker kurulumu (Docker, Kubernetes, managed service vs.)
- **RabbitMQ**: Exchange, queue, binding, routing key ayarları
- **Kafka**: Topic, partition sayısı, replication factor, consumer group ayarları

**Günlük hayat benzetmesi**

- RabbitMQ = Şehir içi klasik **postane ve posta kutusu** sistemi
- Kafka = Sürekli akan **haber ajansı veri hattı**; TV kanallarına canlı veri akıyor

---

### **90. Publish/Subscribe pattern**

**Tanım (teknik)**

Publish/Subscribe (Pub/Sub), mesaj gönderenlerin (publisher) **kime gittiğini bilmeden** bir “topic”e mesaj attığı; alıcıların (subscriber) da ilgi duydukları topic’lere abone olup mesaj aldığı modeldir. Gönderici ile alıcılar birbirini tanımaz, arada message broker vardır.

Özellikler:

- **Gevşek bağlılık (decoupling)**
- Asenkron, **çoktan çoğa** iletişim
- Yeni subscriber eklemek göndericiyi etkilemez

**Günlük hayat benzetmesi**

YouTube kanalı gibi:

- Youtuber (publisher) video yükler, tek tek kim izliyor bilmez.
- Sen kanala abone (subscriber) olursun, ilgilendiğin videolar sana düşer.

---

### **91. Queue ve Topic yönetimi**

**Queue nedir?**

Mesajların **ilk giren ilk çıkar (FIFO)** yapıda tutulduğu liste. RabbitMQ, SQS, Service Bus vb. sistemlerde kullanıyoruz. Genelde “iş kuyruğu” senaryolarında: her mesaj bir tüketici tarafından işlenir.

**Topic nedir?**

Kafka, RabbitMQ topic exchange, SNS/SB gibi sistemlerde **“kategori başlığı”** gibi düşünebilirsin. Publisher bir topic’e yazar; birden fazla subscriber aynı topic’ten mesaj alabilir. Kafka’da topic, partition’lara bölünerek ölçeklenir.

**Yönetirken dikkat edilenler**

- İsimlendirme (örn: orders.created, payments.completed)
- Retention süreleri (Kafka’da ne kadar süre saklanacak)
- Queue/Topic başına consumer sayısı, throughput
- DLQ/Retry yapılarını bağlama (aşağıda)

**Günlük hayat benzetmesi**

- Queue = Tek sıra halinde banko kuyruğu
- Topic = “Duyurular panosu”; isteyen herkes gelip o panodan değişiklikleri görür

---

### **92. Event-driven microservice implementation**

**Event-driven microservice ne demek?**

Microservice’lerin birbirlerine **“şu olay oldu” diye event fırlatıp** tepki verdiği mimari:

- OrderService → OrderCreated event yayınlar
- PaymentService → bu event’i dinleyip ödeme başlatır
- NotificationService → aynı event’ten mail/SMS yollar

Özellikler:

- Servisler birbirini doğrudan çağırmak zorunda değil (gevşek bağlı)
- Event’ler log gibi tutulabildiği için sonradan tekrar işlenebilir

**Günlük hayat benzetmesi**

Apartman WhatsApp grubu:

- “Kargo kapıya bırakıldı” mesajını atıyorsun (event).
- Kargoyu bekleyen kimse o mesajı görüp aksiyon alıyor (event handler).
    
    Mesajı atan kişinin “kim alacak, ne yapacak” diye düşünmesine gerek yok.
    

---

### **93. Retry ve Dead-letter queue**

**Retry nedir?**

Consumer bir mesajı işlerken hata aldıysa, **belirli sayıda yeniden deneme** (retry) yapar. Örneğin:

- 3 kez dene, aralarında 5 sn bekle
- Hâlâ hata varsa “başka yere” gönder

**Dead-letter queue (DLQ) nedir?**

Bir mesaj **defalarca denenip yine de işlenemiyorsa** veya belirli kuralları bozuyorsa, normal kuyruğu tıkamasın diye özel bir kuyruğa, yani **DLQ’ya** gönderilir.

- Kafka’da bu genelde özel bir DLQ topic’idir
- Azure Service Bus, Pulsar vb. sistemlerde de benzer mantık var

DLQ’daki mesajlar:

- Manuel incelenir,
- Gerekirse düzeltilip tekrar işlenir.

**Günlük hayat benzetmesi**

Kargoda teslim edilemeyen paketler için “kaybolan eşya ofisi”:

- Adres hatalı, alıcı yok, paket defalarca götürülmüş ama verilememiş.
- Bu paketler normal dağıtımı bloklamasın diye ayrı depoya (DLQ) kaldırılıyor.

---

### **95. Integration Event design**

**Integration Event nedir?**

Farklı microservice’lerin veya bounded context’lerin **birbirini haberdar etmek** için kullandığı olaylar. Örnek:

- OrderPaidIntegrationEvent
- UserRegisteredIntegrationEvent

Bunlar “başka sistemler de bunu bilsin” diye tasarlanan “dışa dönük” event’ler.

**Design’de nelere dikkat edilir?**

- Event isimleri **domen diline uygun ve geçmiş zaman**: OrderCreated, PaymentFailed
- Minimal ama yeterli veri: ID, zaman damgası, gerekli alanlar
- Schema versiyonlama: Event formatı değişirse eski consumer’lar bozulmasın

**Günlük hayat benzetmesi**

Resmî bir bildirim yazısı gibi:

- “Şu tarihte, şu kişi, şu işlemi yaptı”
- İmza, tarih, ID gibi sabit alanlar hep var; formatı değiştirmek ciddi iş.

---

### **96. Saga Pattern implementasyonu**

**Kısaca hatırlatma**

Saga Pattern, dağıtık bir iş akışını **birden fazla yerel transaction + gerektiğinde geri alma (compensating action)** olarak modelleyen pattern.

**Implementasyon stilleri**

- **Orchestration**: Tek bir “Saga orchestrator” servisi var, adımları sırayla tetikliyor; her adım başarılı/başarısız diye cevap veriyor.
- **Choreography**: Ortada merkez yok; her servis dinlediği event’e göre kendi adımını atıp yeni event fırlatıyor (event chain).

**Örnek akış (e-ticaret)**

1. OrderService → OrderCreated
2. InventoryService → stok ayır, başarılıysa StockReserved
3. PaymentService → para çek, başarılıysa PaymentCompleted
4. Bir adım fail olursa: PaymentFailed → Stok geri bırak, siparişi iptal et vb.

**Günlük hayat benzetmesi**

Tur şirketi paket tur satıyor:

- Uçak, otel, araç kiralama, gezi turları ayrı ayrı rezervasyon.
- Son adımda bir şey patlarsa, firma önceki rezervasyonları iptal edip para iadesi yapıyor. Bütün senaryo “ya komple başarılı, ya da her şey geri alınmış” gibi ele alınıyor.

---

### **97. Distributed transaction management**

**Problem ne?**

Monolith’te tek veritabanı üzerinde BEGIN TRANSACTION diyorsun, her şey ya commit ya rollback. Microservice dünyasında ise:

- Her servis **kendi DB’sine** sahip,
- Arada message broker’lar var,
    
    Dolayısıyla klasik 2PC (two-phase commit) hem pratik değil hem de çoğu ortamda önerilmiyor.
    

**Distributed transaction management ne yapıyor?**

Dağıtık ortamda “işin tamamı ya başarılı olsun ya tutarlı bir hale gelsin” hedefi için:

- **Saga Pattern** (işi parçalara bölüp compensating actions ile yönetme)
- **Transactional Outbox** (DB + mesaj tutarlılığı)
- Idempotent consumer gibi tekniklerle
    
    tutarlılığı (consistency) sağlıyoruz.
    

**Günlük hayat benzetmesi**

Birden fazla banka ve kurumu içeren karmaşık bir finans işlemi düşün:

- Tüm adımlar tek banka ekranından “atomik” olamıyor.
- Onun yerine iyi tanımlanmış adımlar, geri alma prosedürleri, makbuzlar ve kayıtlar ile yönetiliyor.

---

### **98. Outbox pattern kullanımı**

**Kısaca tanım**

Outbox Pattern, aynı işlem içinde:

1. Kendi veritabanına yazmayı,
2. Dışarıya göndereceğin event/mesajı **outbox tablosuna** yazmayı
    
    birleştirerek **dual-write problemini** çözen pattern’dir.
    

**Nasıl kullanılır? (özet flow)**

- OrderService, siparişi DB’ye kaydederken aynı transaction içinde Outbox tablosuna da “OrderCreatedEvent” kaydını ekler.
- Transaction commit olursa hem sipariş hem outbox kaydı kesinleşir.
- Ayrı bir **Outbox Processor**, periyodik olarak bu tabloyu okuyup event’leri RabbitMQ/Kafka’ya gönderir.
- Gönderilenler işaretlenir veya silinir.

Böylece **“DB yazıldı ama event gönderilemedi”** veya tam tersi durumları minimize etmiş olursun.

**Günlük hayat benzetmesi**

Kargo şubesinde memur önce her işlemi deftere (outbox) yazıyor:

- Defter + kasa işlemi aynı anda tamamlanıyor.
- Gün sonunda başka bir görevli bu deftere bakıp hangi müşterilere SMS/mail gönderileceğini işliyor.

---

### **99. Logging, monitoring ve metrics**

**Logging**

Uygulama ve consumer’ların **“ne yaptığını” satır satır kaydetmek**:

- İşlenen mesaj ID’leri
- Hata stack trace’leri
- İş akışının önemli adımları (info log)

**Monitoring**

Sistemin **o anki sağlığını izlemek**:

- Queue uzunlukları, consumer lag’i
- Broker’ın CPU/RAM, disk kullanımı
- Hata oranları, throughput (mesaj/saniye)

**Metrics**

Sayısal göstergeler:

- processed_messages_total
- processing_duration_seconds
- dlq_messages_total vs.

Bu metrikler Prometheus, Grafana, CloudWatch gibi araçlarla görselleştirilir.

**Günlük hayat benzetmesi**

- Logging = Günlük tutmak: “Saat 10:15 şu işi yaptım, 10:30’da şu hata oldu”
- Monitoring = Arabanın gösterge paneli: hız, devir, motor uyarı ışıkları
- Metrics = Yakıt tüketimi istatistiği, ortalama hız, aylık yol toplamı gibi özet rakamlar

---