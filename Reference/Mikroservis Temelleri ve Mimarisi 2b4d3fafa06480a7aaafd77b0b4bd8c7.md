# Mikroservis Temelleri ve Mimarisi

---

### **41. Monolith vs Microservices kavramı**

**Monolith nedir?**

Tek parça, tek uygulama. Tüm modüller (ürün, sipariş, kullanıcı, ödeme…) *aynı kod tabanında, aynı deploy paketi içinde* yaşar. Tek yerde çalışır, genelde tek process ya da tek servis olarak düşünülür.

**Microservices nedir?**

Uygulamayı iş alanlarına göre parçalara bölersin: *Order Service, Payment Service, Catalog Service* gibi. Her parça kendi veritabanına, kendi koduna, hatta kendi teknolojisine sahip bağımsız bir servis olur.

**Günlük hayat benzetmesi**

- **Monolith**: Her şeyi yapan *dev bir AVM (alışveriş merkezi)* düşün. Elektronik, market, sinema, berber hepsi aynı bina, aynı yönetim. Elektrik kesilirse her yer kapanır.
- **Microservices**: Şehrin farklı yerlerine dağılmış *uzman dükkanlar*. Berber ayrı, market ayrı, sinema ayrı. Bir berber kapansa market açık kalır, ayrı ayrı büyüyebilirler.

**Teknik farkın özeti (sen anlatırken kullanabileceğin cümle)**

- Monolith: Tek kod tabanı, tek deploy, hızlı başlangıç ama zamanla *büyüdükçe yönetmesi ve ölçeklemesi zor*.
- Microservices: Küçük, bağımsız servisler, *bağımsız deploy ve ölçekleme*; karşılığında *dağıtık sistem karmaşıklığı* geliyor.

---

### **42. Microservice architecture design patterns**

**Nedir?**

Microservice dünyasında sık çözülen problemler için ortaya çıkan *tekrar kullanılabilir çözüm şablonlarıdır*. Örneğin:

- **API Gateway Pattern**
- **Saga Pattern (distributed transaction)**
- **CQRS Pattern**
- **Strangler Fig (monolith’ten microservice’e geçiş)**
    
    gibi.
    

**Günlük hayat benzetmesi**

Bir ev inşa ederken “mutfak hep böyle planlanır, banyoda su yalıtımı şöyle yapılır” gibi standart çözümler var ya; işte bu mimari pattern’ler de yazılımın “inşaat standartları”.

**Anlatırken kullanabileceğin teknik cümle**

> Microservice design pattern’ler, dağıtık sistemlerde sık yaşanan sorunlar için kanıtlanmış çözümler sunar. Transaction yönetimi için
> 
> 
> *Saga*
> 
> *API Gateway*
> 
> *CQRS*
> 

---

### **43. Bounded Context ve Domain-Driven Design (DDD)**

**DDD nedir? (çok kısaca)**

Eric Evans’ın ortaya koyduğu bir yaklaşım: *iş alanını (domain) iyi anlayıp*, yazılımı da bu iş diline göre modellemeyi öneriyor.

**Bounded Context nedir?**

DDD’nin içinde geçen *“bu model ve terimler sadece şu sınırlar içinde geçerlidir”* diyen kavram. Yani bir domain modelinin geçerli olduğu *mantıksal sınır*.

**Günlük hayat benzetmesi**

“Öğrenci” kelimesi:

- Üniversitenin **akademik** tarafında: ders alan kişi
- Üniversitenin **finans** tarafında: harç borcu olan müşteri
    
    Aynı kelime ama her departmanın kafasındaki model başka. İşte her departmanın kendi anlam dünyası bir **Bounded Context**.
    

**Teknik anlatım için kısa cümle**

> Bounded Context, belirli bir domain modelinin geçerli olduğu sınırdır. Her context içinde terimler ve kurallar tutarlı, dışarıyla entegrasyon ise açık kontratlarla yapılır. Böylece büyük sistemler yönetilebilir parçalara ayrılır.
> 

---

### **44. Service communication: HTTP/REST**

**Nedir?**

Microservice’lerin birbiriyle konuşma şekillerinden en yaygın olanı:

- **HTTP**: İnternet’te kullandığımız protokol.
- **REST**: HTTP üzerinde kaynak (resource) odaklı tasarım stili: GET /orders/1, POST /orders gibi.

Servisler arası “istek–cevap” (request–response) iletişimi için kullanılır.

**Günlük hayat benzetmesi**

Restoran uygulaması:

- Sen uygulamadan **“/menu”** isteği atıyorsun → Garson menüyü getiriyor (GET).
- Sipariş verince **“/orders”** endpoint’ine POST atıyorsun → Yeni sipariş oluşturuluyor.

**Teknik cümle**

> Service-to-service iletişimde HTTP/REST, senkron request/response modelini kullanır. Bir servis diğerine HTTP isteği gönderir, karşı taraf işlem yapıp hemen cevap döner.
> 

---

### **45. Service discovery ve API Gateway (Ocelot)**

**Service discovery nedir?**

Microservice ortamında servisler sabit IP’de durmaz; scale olur, pod yeniden başlar, adresi değişir. *Service Discovery*, “Order Service şu anda nerede, hangi adreste çalışıyor?” sorusunun cevabını veren mekanizmadır. Registry (kayıt defteri) gibi düşünebilirsin.

**API Gateway nedir? (Ocelot örneği)**

Tüm client’ların (web, mobil) microservice’lere *tek bir kapı* üzerinden erişmesini sağlayan katman.

- URL yönlendirme (routing)
- Auth, rate limit, logging, header ekleme vb. cross-cutting işler
    
    burada toplanır.
    

Ocelot, .NET dünyasında kullanılan hafif bir API Gateway kütüphanesi.

**Günlük hayat benzetmesi**

- **Service Discovery**: AVM’deki *dijital rehber ekranı* gibi: “Berber hangi katta, tam olarak nerede?”
- **API Gateway**: AVM’ye tek ana giriş kapısı. İçeride hangi dükkâna gideceğine göre seni yönlendiriyor; güvenlik kontrolü de bu kapıda yapılıyor.

---

### **47. Event-driven architecture (EDA)**

**Nedir?**

Sistem bileşenlerinin birbirleriyle **“olay” (event)** fırlatarak haberleştiği mimari stil. Bir şey olduğunda, “şu olay gerçekleşti” diye yayınlanır; ilgilenen servisler bunu dinleyip kendi işini yapar.

Örnek event’ler:

- OrderCreated
- PaymentCompleted
- UserRegistered

**Günlük hayat benzetmesi**

Apartman WhatsApp grubu:

- Sen “Ben kargo bıraktım, kapıda.” diye mesaja atıyorsun (**event publish**).
- Kargoyu bekleyen kişi bu mesajı görüp kapıya gidiyor (**event subscribe & handle**).
    
    Sen mesajı kime gitti, kim okudu, ne zaman kapıya indi bilmiyorsun – sadece olayı duyurdun.
    

**Teknik anlatım cümlesi**

> Event-driven architecture’da bileşenler loosely-coupled’dır; event üreticileri (producers) ile event tüketicileri (consumers) arasında genellikle bir event broker bulunur ve iletişim çoğunlukla asenkron gerçekleşir.
> 

---

### **48. Message Brokers (RabbitMQ / Kafka)**

**Message broker nedir?**

Servisler arasında mesajları güvenli ve kontrollü şekilde taşıyan *aracı sistem*.

- Mesajları kuyruğa alır (queue/topic).
- Bir servis gönderir, başka bir servis uygun olduğunda alır.
- Retry, sıraya sokma, kalıcılık (durability) gibi özellikler sunar.

**RabbitMQ** → Genelde *mesaj kuyruğu (queue)*, iş sıralama;

**Kafka** → *yüksek hacimli, akış (stream) ve event log* odaklı.

**Günlük hayat benzetmesi**

Postane:

- Gönderici mektubu posta kutusuna bırakır (queue).
- Posta dağıtıcıları mektupları sırasıyla alıp dağıtır.
    
    Gönderici mektubu tam ne zaman alındı, kim dağıttı, o sırada alıcı evde miydi bilmez – sadece postaya bırakır.
    

**Teknik cümle**

> Message broker’lar, asenkron iletişim için queue veya topic yapıları üzerinden mesajlaşmayı sağlar; sistemleri decouple eder ve ölçeklenebilir, fault-tolerant mimariler kurulmasına yardımcı olur.
> 

---

### **49. Asynchronous communication**

**Nedir?**

Gönderen ve alanın *aynı anda hazır olmasının gerekmediği* iletişim şekli.

- Gönderen mesajı veya isteği bırakır.
- Karşı taraf uygun olduğunda işler.
- Arada genelde message broker, job queue vb. vardır.

**Günlük hayat benzetmesi**

- WhatsApp’ta *ses kaydı bırakmak*: Karşı taraf online olmasa da mesajı atarsın, o sonra dinler.
- E-posta: Hemen cevap almayı beklemezsin.

**Teknik cümle**

> Asenkron iletişim; yüksek gecikmeyi tolere eder, sistemleri birbirinden bağımsız çalıştırır ve yoğun yük altında bile dayanıklı (resilient) kalmalarını sağlar.
> 

---

### **50. Synchronous vs Asynchronous communication**

**Synchronous (senkron) nedir?**

Gönderen, cevap gelene kadar *bekler*. HTTP request-response bunun tipik örneği.

- Avantaj: Basit, akışı takip etmesi kolay.
- Dezavantaj: Karşı taraf yavaşsa tüm zincir yavaşlar, dependency zinciri oluşur.

**Asynchronous (asenkron) nedir?**

Gönderen, mesajı bırakır ve işine devam eder; cevap “sonra bir şekilde” gelir veya gelmesi gerekmeyebilir.

- Avantaj: Performans, ölçeklenebilirlik, gevşek bağlılık.
- Dezavantaj: Akışı anlamak ve debug etmek daha zor, eventual consistency vs.

**Günlük hayat benzetmesi**

- **Senkron**: Banko kuyruğunda vezneyle yüz yüze işlem yapmak; sıran gelene kadar bekliyorsun.
- **Asenkron**: Banka mobil uygulamasında EFT talimatı vermek; ekranı kapatsan da EFT arkada işleniyor.

Sunumda şöyle özetleyebilirsin:

> Senkron iletişimde “bekleyen bir telefon görüşmesi” vardır; asenkron iletişimde ise “mesaj bırakıp karşı tarafın uygun olduğunda işlemesini” beklersin.
> 

---

### **51. Saga Pattern ve Distributed Transactions**

**Problem ne?**

Microservice’lerde tek bir “global” veritabanı transaction’ı yapamıyoruz; çünkü her servis kendi DB’sine sahip. Buna rağmen “ya hepsi başarıyla tamamlansın ya da hepsi geri dönsün” dediğimiz iş senaryoları var (örn. sipariş–ödeme–stok).

**Saga Pattern nedir?**

Büyük bir iş adımını, birden fazla serviste çalışan *küçük lokal transaction’lara* bölüyor.

- Her servis kendi DB’sinde **lokal transaction** çalıştırır.
- İş başarılıysa bir sonraki adımı tetikleyen event/mesaj yayınlar.
- Bir yerde hata olursa, o ana kadar yapılan işleri geri alacak **compensating transaction**’lar çalıştırılır (iade, stok geri yükleme vs.).

**Günlük hayat benzetmesi (online alışveriş)**

1. Sipariş oluştur (Order Service)
2. Stok düş (Inventory Service)
3. Ödeme al (Payment Service)

Ödeme başarısız olursa:

- Stok tekrar artırılır (compensating action).
- Sipariş iptal edilir.

Bir banka şubesinde tek transaction yapamıyorsun ama her işlem için “iş ters giderse geri alma prosedürü” var gibi düşünebilirsin.

**Teknik cümle**

> Saga Pattern, dağıtık sistemlerde uzun süren ve birden fazla servise yayılan işlemleri, birbirini tetikleyen lokal transaction’lar dizisi olarak modelleyip; hata durumunda compensating transaction’lar ile eventual consistency sağlayan bir pattern’dir.
> 

---