# Containerization ve Orchestration

---

### **77. Docker temelleri ve Dockerfile oluşturma**

**Docker nedir?**

Docker, uygulamayı ve ihtiyaç duyduğu her şeyi (runtime, kütüphane, config vs.) **container** denilen hafif paketlere koyup çalıştırmamızı sağlayan bir platform. Aynı container, “bende çalıştı sende niye çalışmıyor” derdini azaltarak her ortamda aynı şekilde çalışır.

**Dockerfile nedir?**

Docker image oluşturmak için yazdığımız **talimat dosyası**. Basit bir metin dosyasıdır; içinde “hangi base image’i kullan, hangi dosyaları kopyala, hangi komutları çalıştır, container başlayınca hangi komutla ayağa kalk” gibi adımlar yazar.

**Günlük hayat benzetmesi**

- Docker image = Hazır dondurulmuş yemek paketi
- Container = O yemeği mikrodalgaya koyup ısıtıp hazır hale getirdiğin hal
- Dockerfile = Yemeğin arkasındaki “tarif”: hangi malzeme, hangi adım, kaç derece…

**Sunum cümlesi**

> Docker, uygulamayı container adı verilen izole paketlerde çalıştırmamızı sağlar; Dockerfile ise bu paketi nasıl üreteceğimizi adım adım tarif eden metin dosyasıdır.
> 

---

### **78. Docker Compose ile multi-container setup**

**Compose nedir?**

Docker Compose, birden fazla container’dan oluşan bir uygulamayı **tek bir YAML dosyası** ile tanımlayıp tek komutla (docker compose up) ayağa kaldırmaya yarayan araçtır. Aynı dosyada servisler, network’ler, volume’ler, env değişkenleri vs. tanımlanır.

Örneğin: web servisi (API) + db servisi (PostgreSQL) aynı docker-compose.yml içinde tanımlanır.

**Günlük hayat benzetmesi**

Tek tek cihazları açmak yerine, evde “tek tuşla tüm odaların ışığını, kombiyi, müziği açan bir akıllı ev senaryosu” kurmak gibi.

**Sunum cümlesi**

> Docker Compose, çoklu container’lı uygulamaları tek bir YAML dosyasında tanımlayıp tek komutla çalıştırmamızı sağlayan orkestrasyon aracıdır.
> 

---

### **79. Microservice containerization**

**Nedir?**

Microservice mimarisinde her servisi ayrı bir Docker image/container olarak paketlemeye **containerization** diyoruz.

- Her microservice’in kendi Dockerfile’ı, bağımsız build ve deploy pipeline’ı olur.

Avantaj:

- Versiyonlar bağımsız: order-service:v2, payment-service:v5 gibi.
- Farklı teknolojiler (Node, .NET, Go) aynı cluster’da yan yana çalışabilir.

**Günlük hayat benzetmesi**

Bir lojistik şirketinde her ürün, kendi kutusunda, kendi etiketleriyle gidiyor. Hepsini tek büyük kolide karışık taşımak yerine, her servis ayrı kutu.

**Sunum cümlesi**

> Microservice containerization, her microservice’i kendi Docker image’i ve container’ı olarak paketlememiz; böylece bağımsız sürümleyip ölçekleyebilmemiz anlamına gelir.
> 

---

### **80. Network, volume ve environment variables**

Üç temel Docker/Kubernetes konsepti:

**Network**

Container’ların **birbirini bulma ve konuşma** şekli. Docker’da bridge network’ler, Compose’de isimle erişim (db:5432 gibi), Kubernetes’te Service IP / DNS üzerinden iletişim.

**Volume**

Container silinse bile verinin kalmasını sağlayan **kalıcı depolama** mekanizması. Örneğin DB data klasörünü host’taki bir volume’e bağlamak.

**Environment variables**

Container’a dışarıdan geçirilen ENV değerleri. Örn: DB_HOST, DB_PASSWORD, ASPNETCORE_ENVIRONMENT. Kod aynı kalıyor, sadece env değişkenleriyle davranış değişiyor.

**Günlük hayat benzetmesi**

- Network = Ofis içi telefon santrali ve dahili numaralar
- Volume = Ofisteki kalıcı arşiv dolapları
- Env vars = Aynı programı farklı ayar dosyalarıyla çalıştırmak (TV’de “spor modu”, “sinema modu” profilleri gibi)

**Sunum cümlesi**

> Network, container’ların birbiriyle konuşmasını; volume, verinin kalıcı olmasını; environment variable’lar ise aynı image’i farklı konfigürasyonlarla çalıştırmamızı sağlar.
> 

---

### **81. Container health checks ve logging**

**Health check nedir?**

Container içindeki uygulamanın gerçekten **sağlıklı çalışıp çalışmadığını** periyodik kontrol etme mekanizması.

- Docker’da HEALTHCHECK talimatı veya Compose’de healthcheck.
- Kubernetes’te livenessProbe, readinessProbe.

Sağlıklı değilse orchestrator container’ı yeniden başlatır veya trafiği oradan keser.

**Logging nedir?**

Container içindeki uygulamanın log’larını (stdout/stderr) toplayıp merkezi bir yerde saklama ve analiz etme. Örn: docker logs, Kubernetes’te kubectl logs, ELK / Loki vs. ile merkezi logging.

**Günlük hayat benzetmesi**

- Health check = Doktorun rutinde tansiyon, nabız ölçmesi
- Logging = Hastane dosyanda tutulan tüm geçmiş ölçümler, teşhisler, notlar

**Sunum cümlesi**

> Health check’ler, container içindeki uygulamanın ayakta olup olmadığını otomatik izler; logging ise bu uygulamanın geçmişte neler yaptığını ve hata durumlarını kayıt altına alır.
> 

---

### **83. Kubernetes temelleri**

**Kubernetes nedir?**

Kubernetes, container’ları **otomatik olarak deploy eden, ölçekleyen ve yöneten** bir orkestrasyon platformudur. Cluster mantığıyla çalışır:

- Control plane (beyin)
- Worker node’lar (işi yapan makineler)
    
    üzerinde pod’ları planlar ve yönetir.
    

Çözdüğü problemler:

- Container’lar nerede çalışsın?
- Crash olursa yeniden başlat.
- Yük artınca kopya sayısını artır, azalınca düşür.

**Günlük hayat benzetmesi**

Bir fabrika düşün: Kubernetes = üretim bandını yöneten otomasyon sistemi.

- Hangi makine ne üretecek, fazla talep geldiğinde kaç makine daha devreye girecek, arızalı makine varsa yerine yenisi geçecek… hepsini organize ediyor.

**Sunum cümlesi**

> Kubernetes, container’ları tek tek değil, bir cluster içinde otomatik olarak planlayan, çalıştıran ve ölçekleyen bir orkestrasyon motorudur.
> 

---

### **84. Pod, Deployment, Service kavramları**

Bunlar Kubernetes’in *en temel üç objesi*.

**Pod**

Kubernetes’te **deploy edilebilen en küçük birim**. Bir veya birden fazla container’ı, aynı IP’yi ve aynı diskleri paylaşacak şekilde paketleyen nesne. Pod’lar kısa ömürlü; bozulursa yenisi yaratılır.

**Deployment**

Pod’ları yönetmek için kullanılan **kontrol nesnesi**.

- Kaç kopya (replica) olsun?
- Yeni versiyona rolling update nasıl yapılsın?
    
    gibi konuları tanımlarsın; controller, gerçek durumu buna göre ayarlar.
    

**Service**

Pod IP’leri sürekli değiştiği için, bunların önüne **sabit bir erişim noktası (virtual IP + DNS)** sağlayan nesne. Aynı label’e sahip pod’lara load balancing yapar.

**Günlük hayat benzetmesi**

- Pod = İçinde 1–2 çalışan olan küçük ofis odası
- Deployment = “Bu pozisyondan her zaman 3 kişi çalışsın” diyen İK politikası
- Service = “Müşteri hizmetleri hattı” telefonu; arayan kişi hangi temsilciye düştüğünü bilmez, sistem dağıtır

**Sunum cümlesi**

> Pod, container’ların çalıştığı temel birim; Deployment bu pod’ların sayısını ve versiyonunu yönetir; Service ise bu pod’lara sabit bir adres üzerinden load balancing yapar.
> 

---

### **85. ConfigMap ve Secret kullanımı**

**ConfigMap nedir?**

Uygulama içinde ihtiyaç duyulan **konfigürasyon verilerini (non-sensitive)** key–value olarak tutan Kubernetes objesi. Örn: APP_THEME=dark, LOG_LEVEL=Debug. Container’lara environment variable veya dosya olarak mount edilir.

**Secret nedir?**

Şifre, token, anahtar gibi **gizli verileri** saklamak için kullanılan objedir. Temel formda base64 ile encode edilse de, genelde ek olarak KMS / Vault gibi şifreleme çözümleriyle beraber kullanılır.

**Günlük hayat benzetmesi**

- ConfigMap = şirkette herkesin görebileceği genel duyuru panosu
- Secret = kilitli kasadaki şifreler

**Sunum cümlesi**

> ConfigMap, şifre içermeyen ayarları; Secret ise şifreli veya hassas değerleri Kubernetes içinde merkezi ve kontrollü bir şekilde saklamamızı sağlar.
> 

---

### **86. Ingress ve Load Balancer**

**Service + LoadBalancer**

Kubernetes içinde çalışan servisleri **dış dünyaya açmak** için kullanılan yöntemlerden biri Service type=LoadBalancer. Cloud sağlayıcı (AWS, GCP vs.) sizin için bir dış load balancer oluşturur ve trafiği Service’e yönlendirir.

**Ingress nedir?**

Ingress, HTTP/HTTPS trafiğini domain/path bazlı **routing ve load balancing** ile Kubernetes içindeki Service’lere yöneten üst seviye nesne. Genelde bir **Ingress Controller** (NGINX, Traefik vb.) ile birlikte çalışır ve SSL termination, host bazlı routing, path bazlı routing gibi özellikler sunar.

**Günlük hayat benzetmesi**

- LoadBalancer Service = Her uygulama için ayrı ayrı bina girişi yapmak
- Ingress = Tek bir büyük bina girişi, içeride “Bu domain/URL şu kata gitsin” diye yönlendiren resepsiyon

**Sunum cümlesi**

> LoadBalancer Service dış dünyaya direkt IP/port açarken, Ingress HTTP/HTTPS trafiğini domain ve path bazlı olarak birden fazla Service’e yönlendiren daha esnek bir giriş katmanıdır.
> 

---

### **87. Horizontal scaling ve auto-scaling**

**Horizontal scaling (yatay ölçekleme)**

Artan yükü kaldırmak için **aynı uygulamadan daha fazla kopya (pod) çalıştırmak**. Kubernetes’te replicas sayısını artırmak veya Horizontal Pod Autoscaler (HPA) kullanmak bu iş için kullanılıyor.

- Vertical scaling: Mevcut pod’a daha fazla CPU/RAM vermek
- Horizontal scaling: Pod sayısını artırmak

**Auto-scaling (HPA)**

Kubernetes’te **HorizontalPodAutoscaler**, CPU/memory veya custom metriklere bakarak replica sayısını **otomatik** arttırıp azaltır. Örn: CPU %70 üzerine çıkınca 3 pod’dan 6 pod’a.

**Günlük hayat benzetmesi**

- Horizontal scaling = Yoğun saatlerde markette kasiyer sayısını artırmak, sakin saatlerde azaltmak
- Vertical scaling = Tek kasiyeri daha hızlı çalışan süper kasiyerle değiştirmek :)

**Sunum cümlesi**

> Yatay ölçekleme, pod kopya sayısını artırıp azaltmak demek; auto-scaling ise bu işi CPU, bellek gibi metriklere bakarak Kubernetes’in otomatik yapmasıdır.
> 

---