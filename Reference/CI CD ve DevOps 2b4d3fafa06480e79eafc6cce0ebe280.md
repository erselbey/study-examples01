# CI/CD ve DevOps

---

### **101. Git ve branch stratejileri ✅**

**Teknik:**

Git, kaynak kodunun geçmişini tutan versiyon kontrol sistemidir. *Branch* ise ana koddaki (genelde main) hattan ayrılıp ayrı bir kolda geliştirme yapmana izin veren “bağımsız değişiklik hattı”dır. GitFlow, GitHub Flow, Trunk-based gibi stratejiler, “hangi branch ne için, ne zaman açılır/kapanır” sorusuna cevap veren yöntemlerdir.

**Benzetme:**

Ana branch = şirketin resmi ürün versiyonu, feature branch = o ürünü bozmayıp kenarda deneme yaptığın sandbox gibi.

---

### **102. Azure DevOps / GitHub Actions ile pipeline ✅**

**Teknik:**

Azure Pipelines ve GitHub Actions, kodu her commit / pull request’te **otomatik derleyip test eden ve istersen deployment yapan** CI/CD platformlarıdır. YAML dosyalarıyla build–test–deploy adımlarını “pipeline as code” olarak tarif edersin.

**Benzetme:**

Her push’tan sonra projeyi elle derleyip test etmek yerine, “butona bastığında otomatik çalışan üretim bandı” kurmak gibi.

---

### **103. Build ve release pipeline setup**

**Teknik:**

- **Build pipeline:** Kodu çekip restore, build, test, artifact üretme işlerini otomatik yapar (CI kısmı).
- **Release / deploy pipeline:** Oluşan artifact’ı **test, staging, prod** gibi ortamlara sırayla deploy eder, genelde onay, manuel gate, vs. içerir (CD kısmı).

**Benzetme:**

Build pipeline = fabrikanın ürün üretim hattı; release pipeline = üretilen ürünlerin hangi mağazaya, hangi sırayla gönderileceğini belirleyen dağıtım hattı.

---

### **104. Docker image build & push**

**Teknik:**

- docker build ile Dockerfile’dan bir **image** üretirsin.
- Sonra bu image’i bir container registry’e (docker push ile Docker Hub, ACR, ECR vs.) gönderirsin ki Kubernetes ya da diğer ortamlardan çekip çalıştırabilesin.

**Benzetme:**

Yemeği evde pişirip (build), sonra marketin soğuk dolabına koymak (registry). Müşteri yani Kubernetes, oradan alıp ısıtıp servis ediyor (container).

---

### **105. Kubernetes deploy pipeline**

**Teknik:**

Kubernetes deploy pipeline, build & push sonrası **otomatik olarak cluster’a deploy eden** CI/CD adımıdır. Genelde şu sırayla gider:

1. Kod → build → test
2. Docker image build & push
3. kubectl apply / Helm chart / Kustomize ile Kubernetes’e deploy
4. Gerekirse rollout strategy, canary/blue-green vs. uygular.

**Benzetme:**

Yeni ürün kutularını depoya gönderdikten sonra (registry), otomatik çalışan bir sistemin bunları raflara yerleştirmesi (Kubernetes cluster’a rollout).

---

### **107. Monitoring ve alerting setup (Prometheus/Grafana)**

**Teknik:**

- **Prometheus**, uygulama ve altyapıdan **metrik toplayan ve zaman serisi olarak saklayan** monitoring/alerting aracıdır.
- **Grafana**, bu metrikleri görselleştirmek için dashboard’lar ve aynı zamanda alarm kurma imkânı veren bir arayüzdür.

**Benzetme:**

Prometheus = fabrikadaki sensörlerden veri toplayan cihaz; Grafana = bu verileri ekranda gösteren kontrol paneli ve bir şey ters giderse alarm çaldıran sistem.

---

### **108. Logging aggregation (ELK stack / Azure Monitor)**

**Teknik:**

- **ELK Stack** = Elasticsearch (arama + storage) + Logstash (log işleme) + Kibana (görselleştirme). Farklı sunucu ve uygulamalardan gelen log’ları **tek yerde toplayıp arayabileceğin** bir logging çözümüdür.
- **Azure Monitor**, Azure ortamındaki log, metrik ve trace’leri toplayıp analiz edebildiğin managed bir observability platformudur.

**Benzetme:**

Tüm şubelerdeki kamera kayıtlarını ve güvenlik loglarını tek bir merkezde toplayıp, “şu saat, şu olay ne olmuş” diye arayabildiğin bir güvenlik merkezi gibi.

---

### **109. Health check endpoints**

**Teknik:**

Health check endpoint, uygulamanın **“sağlıklı mı, hazır mı”** olduğunu kontrol eden özel URL veya komuttur (ör. /health, /ready). Kubernetes tarafında bunlar liveness/readiness/startup probe’lara bağlanır:

- Liveness → Uygulama kilitlendi mi, yeniden başlatmam gerekir mi?
- Readiness → Trafik almaya hazır mı?

**Benzetme:**

Doktorun belli aralıklarla tansiyon, nabız ölçmesi gibi: Uygulama nabız veriyorsa yaşıyor, ama hazır değilse “hastayı sahaya çıkarmıyoruz”.

---

### **110. Distributed tracing (OpenTelemetry)**

**Teknik:**

Distributed tracing, bir isteğin **frontend → API Gateway → microservice’ler → database** boyunca geçtiği tüm adımları “trace” ve “span”’ler halinde kaydetme tekniğidir. OpenTelemetry, bu trace/log/metric verilerini toplamak için kullanılan açık standart ve SDK setidir.

**Benzetme:**

Kargoya verilen bir paketin “nerede, hangi aktarma merkezinde, ne kadar beklemiş” bilgisini kargo takip ekranında adım adım görmek gibi. Trace, paketin tüm yolculuğu; span’ler, o yolculuğun tek tek durakları.

---

### **111. Performance tuning ve scaling**

**Teknik:**

Performance tuning, uygulamanın **daha hızlı, daha az kaynakla** çalışması için kod, veritabanı, cache, konfigürasyon ve altyapı seviyesinde yapılan optimizasyonlardır. Scaling ise yük arttığında sistemi **daha fazla kaynakla büyütme** işidir:

- Vertical scaling → Daha güçlü makine
- Horizontal scaling → Daha fazla instance/pod (Kubernetes HPA vb.).

**Benzetme:**

- Tuning = Arabayı bakıma sokup yağını, lastiğini değiştirip daha az yakıtla daha hızlı gitmesini sağlamak.
- Scaling = Aynı anda daha çok yolcu taşımak için otobüs sayısını artırmak.

---