# ⁠⁠Authentication ve Security

---

### **65. JWT Authentication**

**Nedir?**

JWT (JSON Web Token), client ile server arasında **kullanıcı bilgilerini ve yetkilerini taşıyan, dijital olarak imzalı bir “bilet”** gibi düşünebileceğin token formatıdır. Genelde kullanıcı login olduktan sonra backend bir JWT üretir, client bu token’ı her istekte Authorization: Bearer <token> header’ı ile gönderir.

**Öne çıkarabileceğin teknik detaylar**

- İçinde **claims** (örn: sub, email, role) taşır.
- İmzalıdır (HMAC veya RS256 vs.), böylece token değiştirildiyse backend anlar.
- “Stateless auth” sağlar: Server tarafında session tutmak yerine token içeriğine bakarsın.

**Günlük hayat benzetmesi**

Sinema bileti gibi:

- Üzerinde hangi koltuk, hangi seans yazıyor (claims).
- Üzerindeki hologram/imza sahte olup olmadığını anlamaya yarıyor (signature).

---

### **66. OAuth2 ve OpenID Connect**

**OAuth2 nedir?**

OAuth 2.0, bir uygulamanın (client) **başka bir uygulamadaki kaynaklara (API) sınırlı ve kontrollü erişim almasını** sağlayan yetkilendirme (authorization) protokolüdür. Örneğin bir uygulamanın senin Google takvimine erişmesi.

**OpenID Connect (OIDC) nedir?**

OpenID Connect, OAuth2’nin üzerine eklenen **kimlik katmanı**dır. Kullanıcının kim olduğunu doğrulamak (authentication) ve kullanıcı profili bilgilerini almak için kullanılır; ID Token adında genelde JWT olan bir token üretir.

**Sahnede şöyle diyebilirsin**

> OAuth2 temelde “bir uygulamaya kaynaklara erişim izni verme” protokolü, OpenID Connect ise bunun üzerine “kullanıcı kimliğini doğrulama” katmanı ekliyor.
> 

**Günlük hayat benzetmesi**

- OAuth2: Bankadan “otomatik ödeme talimatı” vermek → elektrik şirketine hesabından fatura çekme izni.
- OIDC: Bankaya gidip kimlik göstererek “gerçekten ben miyim?” diye kimlik doğrulatmak.

---

### **67. IdentityServer / Keycloak integration**

**IdentityServer / Keycloak nedir?**

- **Keycloak**: Red Hat tarafından desteklenen, open-source bir **kimlik ve erişim yönetimi (IdP)** sunucusu. OAuth2, OIDC, SAML, SSO, kullanıcı yönetimi, social login gibi işleri merkezileştirir.
- **IdentityServer**: .NET ekosisteminde kullanılan, benzer şekilde OAuth2/OIDC sağlayan bir framework/server (geçmişte OSS, şimdi duende lisanslı).

**Integration ne demek?**

Uygulamalarında auth işini tek tek her serviste yazmak yerine:

- API’lerini, SPA’larını, mobil app’lerini **Keycloak/IdentityServer’a “client” olarak kaydedersin**,
- Login, token üretme, refresh token, social login, MFA gibi şeyleri bu merkezi IdP yapar,
- Sen uygulamalarda gelen JWT’yi validate edip claim’lere göre yetki verirsin.

**Günlük hayat benzetmesi**

Şirkette tek bir **turnike sistemi** var:

- Çalışan kartları, ziyaretçi geçişleri, yetkiler hep turnike sisteminde tutuluyor.
- Ofisteki her kapıya ayrı ayrı kart okuyucu sistemi yazmıyorsun; sadece “bu kart geçerli mi?” diye turnike sistemine soruyorsun.

---

### **68. Role-based ve Claims-based authorization**

**Role-based authorization (RBA)**

Kullanıcının sahip olduğu **rol’e** göre yetki vermek: Admin, Editor, User gibi. “Admin her şeyi yapar, User sadece kendi verisine erişir” mantığı.

**Claims-based authorization**

Karar verirken sadece role değil, token içindeki **her türlü bilgiye (claim)** bakmak:

- department = HR,
- subscription = premium,
- age >= 18,
    
    gibi özellikler.
    

Bu model, daha esnek “policy based” yetkilendirmeye imkân verir (örn: AgeOver18Policy).

**Günlük hayat benzetmesi**

- Role-based: “Bu kişi **müdür** mü, değil mi?”
- Claims-based: “Bu kişi satış departmanında mı? Çalışma süresi 1 yılı geçti mi? Eğitimini tamamladı mı?” → Daha detaylı filtre.

---

### **69. Secure API communication (HTTPS, CORS, Rate Limiting)**

**HTTPS**

HTTP’nin TLS ile şifrelenmiş hâli. Aradaki trafiği dinleyen biri, istek ve cevap içeriğini okuyamaz. API konuşurken **mutlaka** HTTPS kullanıyoruz.

**CORS (Cross-Origin Resource Sharing)**

Bir domain’de çalışan JS kodunun, başka bir domain’deki API’ye istek atarken **hangi origin’lere izin verildiğini** belirleyen mekanizma. Tarayıcı, gelen response header’larına bakarak buna karar verir. Yanlış ayarlanırsa ya çok kısıtlayıcı olur ya da güvenlik açığına sebep olabilir.

**Rate Limiting**

Bir IP ya da kullanıcı belli bir süre içinde **en fazla kaç istek atabilir** sorusunun cevabı. DoS / brute force / abuse gibi durumlara karşı API’yi korur. OWASP API Top 10’da da kritik bir madde olarak geçiyor.

**Günlük hayat benzetmesi**

- HTTPS: Postayı şeffaf zarfta değil, kapalı mühürlü zarfta göndermek.
- CORS: “Bu kapıdan sadece şu binadan gelen kuryeler girebilir” demek.
- Rate limiting: Banka gişesinde “kişi başı işlem süresi ve sıra limiti” koymak, birinin sistemi kilitlemesini engellemek.

---

### **71. API Gateway ile authentication ve authorization**

**API Gateway ne yapıyordu?**

Tüm client’ların microservice’lere ulaşmadan önce geçtiği **tek giriş kapısı**. Routing, logging, header manipulation, vs. yanında **security** işleri de burada toplanabilir.

**Auth işini Gateway’e almanın faydası**

- JWT doğrulama, token expiry kontrolü, role/claim check gibi işlerin **her serviste tekrar tekrar yazılmasını engeller**.
- Rate limiting, IP kısıtlama, mTLS, WAF entegrasyonu gibi şeyler de gateway’de merkezi yönetilir.

**Örnek akış**

1. Client → API Gateway’e istek atar (Authorization: Bearer ...).
2. Gateway token’ı doğrular, gerekirse IdentityServer/Keycloak ile konuşur.
3. Yetkisi yoksa 401/403 döner; yetkisi varsa isteği ilgili microservice’e yollar.

**Günlük hayat benzetmesi**

Ofise girerken **resepsiyon**:

- Giriş kartını (JWT) kontrol eder, ziyaretçi kartı verir, hangi kata çıkacağını belirler.
- Katlardaki kapılara “bu işi sen yap” demiyorsun, ilk kapıda çözüyorsun.

---

### **72. Security headers ve OWASP temel önlemler**

**Security headers nedir?**

HTTP response’larında gönderilen ve tarayıcıya **“şunlara izin ver, şunlara verme”** diye güvenlik politikaları tanımlayan header’lar:

- Content-Security-Policy (CSP) → Hangi kaynaklardan script/style yüklenebilir.
- X-Frame-Options → Clickjacking’e karşı iframe içi gösterimi kısıtlama.
- X-Content-Type-Options → MIME type sniffing engelleme.
- Strict-Transport-Security (HSTS) → Her zaman HTTPS kullan.

**OWASP ne diyor?**

OWASP, web güvenliği için en kritik riskleri ve best practice’leri listeleyen bir topluluk/döküman seti. Security headers kullanımı, **Security Misconfiguration** başlığı altında da özellikle vurgulanıyor.

**Günlük hayat benzetmesi**

Apartman girişindeki uyarı tabelaları gibi:

- “Ziyaretçiler için giriş bu kapıdan”
- “Yetkisiz giriş yasaktır”
- “Bu kapı sadece yangında kullanılır”
    
    Tarayıcıya, sayfayı nasıl çalıştırması gerektiğini söylüyorsun.
    

---

### **73. Secrets management (Azure KeyVault / AWS Secrets Manager)**

**Secrets nedir?**

Uygulamalar için hassas bilgiler:

- DB connection string
- API keys, client secrets
- SSH key’ler, sertifikalar

Bunları .env dosyasında, repo’da veya düz text konfiglerde tutmak büyük risk.

**Azure Key Vault / AWS Secrets Manager ne sağlar?**

- Şifreleri, anahtarları, sertifikaları **şifreli ve merkezi** bir şekilde saklar.
- Erişim, IAM / RBAC ile sıkı şekilde kontrol edilir.
- Pek çok senaryoda **otomatik secret rotation** yapabilir (örn. RDS password).

**Günlük hayat benzetmesi**

Evdeki değerli eşyaları (altın, pasaport) çekmeceye değil, **bankadaki kasaya** koymak gibi:

- Kasa şifreli, giriş yetkisi kontrollü.
- Kimin ne zaman kasayı açtığı log’lanıyor.

---

### **74. SSL/TLS implementasyonu**

**TLS nedir?**

Eskiden SSL dendi, artık modern karşılığı **TLS** (Transport Layer Security). Client ile server arasındaki trafiği şifreleyip veri gizliliği ve bütünlüğü sağlar. HTTPS, HTTP + TLS demek.

**Implementasyon’da dikkat edeceğin şeyler**

- Doğru sertifikayı almak (Let’s Encrypt, ACM, vs.).
- Güncel protokol versiyonlarını ve güçlü cipher suite’leri kullanmak.
- HSTS gibi header’larla sadece HTTPS kullanımını zorunlu kılmak.

**Günlük hayat benzetmesi**

Telefonla konuşurken uçtan uca şifreleme kullanmak gibi:

- Ortadaki biri konuşmayı duysa bile anlamıyor.
- Aynı zamanda konuşmanın değiştirilmediğinden de emin oluyorsun.

---

### **75. Logging and auditing**

**Logging nedir?**

Sistem içinde olan olayların (istekler, hatalar, uyarılar, iş süreçleri) **log’lanması**, yani kayıt altına alınması. Örn: “User 123 /orders endpoint’ine 10:23’te GET attı, 200 döndü.”

**Auditing nedir?**

Özellikle güvenlik, yetki ve kritik veri değişimleri için **“kim, ne zaman, ne yaptı?”** sorusuna cevap verecek şekilde daha kontrollü loglama:

- Kim hangi kaydı oluşturdu/güncelledi/sildi?
- Kim hangi admin işlemlerini yaptı?
- Kim hangi kaynağa erişti?

**Güvenlik açısından neden kritik?**

- İhlal oldu mu, nasıl olmuş, nereden sızılmış anlamak için.
- Regülasyon ve compliance (KVKK, GDPR vb.) için zorunlu.
- Anomali tespiti (beklenmeyen erişim desenleri) için.

**Günlük hayat benzetmesi**

Şirket binasında:

- Giriş-çıkış kartı kayıtları (audit trail).
- Güvenlik kameraları (log).
    
    Bir olay olduğunda kayıtları geriye sarıp, kim ne zaman neredeymiş görebiliyorsun.
    

---