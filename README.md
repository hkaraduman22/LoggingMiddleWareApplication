# Data Middleware (Ara Katman) - CENG302 Dönem Sonu Ödevi

## 1. Projenin Amacı ve Kapsamı

Bu proje, **CENG302 Dönem Sonu Ödevi** kapsamında geliştirilen yüksek performanslı bir **borsa ara katmanı (middleware) simülasyonudur**. Proje, gerçek bir borsacılık ortamında günlük (log) verilerinin işlenmesi, güvenlik standartlarının sağlanması ve farklı departman rollerine göre dinamik formatlandırılması gereksinimini karşılamak üzere tasarlanmıştır.

Sistem, iki adet bağımsız Docker konteyneri üzerinde çalışan, aşağıdaki modüllerden oluşur:

- **Data Generator**: Borsa işlemlerine ait rastgele log verilerini üreten konsol uygulaması
- **Middleware API**: Gelen log verilerini işleyen, güvenlik ve zenginleştirme işlemlerini gerçekleştiren ASP.NET Core Web API

Proje, modern yazılım mimarisi prensiplerini ve tasarım kalıplarını uygulaması ile birlikte, containerized ortamda yüksek performans ve skalabiliteyi göstermek amacındandır.

## 2. Kullanılan Teknolojiler

| Teknoloji | Versiyon | Kullanım Amacı |
|-----------|----------|----------------|
| **ASP.NET Core** | 8.0 | Web API ve REST uç noktaları |
| **C#** | 11.0+ | Temel programlama dili |
| **System.Text.Json** | - | JSON serialization ve deserialization |
| **Docker** | Latest | Containerization |
| **Docker Compose** | Latest | Multi-container orchestration |
| **.NET Runtime** | 8.0 | Uygulama çalıştırma ortamı |

## 3. Tasarım Kalıpları (Design Patterns)

### 3.1 Chain of Responsibility (Sorumluluk Zinciri)

**Chain of Responsibility** kalıbı, gelen log verilerinin sırayla birden fazla işlemci (handler) üzerinden geçmesi sağlanarak uygulanmıştır. Her handler, sorumlu olduğu işlemi gerçekleştirdikten sonra veriyi sonraki handler'a iletir.

#### İşlem Zincirine Dahil Edilen Adımlar:

1. **SecurityHandler (Güvenlik İşlemcisi)**
   - TC Kimlik numarası: Son 4 haneyi tutup, önceki haneleri maskeler
   - Kredi kartı: Son 4 hanesini görünür kılıp, geri kalanını gizler
   - E-posta: @ işaretinden önceki kısmı maskelenerek anonimleştirme sağlanır
   - Örnek: `john.doe@example.com` → `***@example.com`

2. **EnrichmentHandler (Zenginleştirme İşlemcisi)**
   - `IsCritical` bayrağı: Log seviyesi `CRITICAL` veya `WARNING` ise `true` olarak işaretlenir
   - `SummaryMessage`: İşlem detaylarından yararlanarak bir özet mesaj oluşturulur
   - Örnek: `"AAPL hissesi için 1500.50 TL'lik SATIN AL yapıldı (WARNING)"`

**Avantajları:**
- Her işlemci bağımsız ve değiştirilebilirdir
- Yeni işlemciler kolayca eklenebilir (Open/Closed Principle)
- Veri akışı sakin ve kontrollü bir şekilde gerçekleşir

### 3.2 Strategy (Strateji)

**Strategy** kalıbı, işlenmiş ve zenginleştirilmiş log verilerinin, hedef kullanıcı rollerine göre farklı formatlar için dinamik olarak dışarı aktarılmasını sağlar. Her strateji, aynı veriyi farklı bir biçimde temsil ederek departman ihtiyaçlarını karşılar.

#### Uygulanan Stratejiler:

1. **HtmlStrategy** (Sistem Yöneticisi İçin)
   - Log verilerini HTML tablosu formatında sunar
   - İnsan tarafından kolay okunabilir, web arayüzüne entegre için uygun
   - Oluşturulan HTML, anlık olarak konsola yazdırılır

2. **CsvStrategy** (Siber Güvenlik Uzmanı İçin)
   - Log verilerini virgülle ayrılmış değerler (CSV) formatında dışarı aktarır
   - Veri analiz araçları ve elektronik tablolara uyumlu
   - İstatistiksel analiz ve raporlama için ideal

3. **JsonStrategy** (Web Geliştirici İçin)
   - İşlenmiş veriyi JSON formatında sunarak API standardını sağlar
   - Diğer sistemlerle entegrasyon ve veri alışverişi için optimize
   - JavaScript ve JavaScript tabanlı araçlarla uyumlu

**Avantajları:**
- Aynı veri, farklı ihtiyaçlar için çeşitli formatlarda sunulur
- Formatlandırma mantığı merkezi yerden yönetilir
- Yeni formatlar kolayca eklenebilir (Strategy Principle)

## 4. Sistem Mimarisi ve Veri Akışı

### 4.1 Mimari Diyagramı

```
┌─────────────────────┐
│  Data Generator     │
│  (Console App)      │
└──────────┬──────────┘
           │
           │ HTTP POST
           │ (50ms delay)
           │
           ▼
┌─────────────────────────────────────┐
│    Middleware API (ASP.NET Core)    │
│   POST /api/logs                    │
└──────────┬────────────────────────┬─┘
           │                        │
           ▼                        ▼
    ┌────────────────┐   ┌─────────────────┐
    │  LogHandler    │   │  LogHandler     │
    │  Security      │──▶│  Enrichment     │
    │  (Maskeleme)   │   │  (Zenginleştirme)
    └────────────────┘   └────────┬────────┘
                                  │
                 ┌────────────────┼────────────────┐
                 │                │                │
                 ▼                ▼                ▼
          ┌────────────────┐ ┌─────────┐ ┌──────────────┐
          │ HtmlStrategy   │ │CsvStrat.│ │ JsonStrategy │
          └────────┬───────┘ └────┬────┘ └──────┬───────┘
                   │              │             │
                   └──────────────┼─────────────┘
                                  │
                                  ▼
                         ┌─────────────────┐
                         │ Console Output  │
                         │ (Formatted Logs)│
                         └─────────────────┘
```

### 4.2 Veri Akışının Detayı

#### **Aşama 1: Veri Ürretimi (Data Generator)**

Data Generator modülü, her 50 milisaniye aralıkla aşağıdaki yapıda rastgele log verileri üretir:

```json
{
  "timestamp": "2026-06-03T14:30:45.1234567Z",
  "sender_id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "transaction_no": "TXN20260603001",
  "sensitive_data": {
    "tc_kimlik": "12345678901",
    "credit_card": "4532123456789012",
    "email": "user@example.com"
  },
  "transaction_details": {
    "symbol": "AAPL",
    "action": "BUY",
    "amount": 1500.50
  },
  "log_level": "INFO"
}
```

**Rasageliştirilerek Üretilen Değerler:**
- **Hisse Sembolleri**: AAPL, THYAO, MSFT, GOOGL, AMZN, TSLA
- **İşlem Türleri**: BUY, SELL
- **Log Seviyeleri**: INFO, WARNING, DEBUG, CRITICAL

#### **Aşama 2: HTTP İletişimi**

- Data Generator, üretilen veriyi HTTP POST isteği olarak `http://middleware-api:8080/api/logs` adresine gönderir
- Docker Compose içerisinde `middleware-api` servisinin adı, DNS çözümlemesi ile otomatik olarak tanınır
- Her istekte bağlantı zaman aşımı 10 saniye olarak ayarlanmıştır

#### **Aşama 3: Ara Katmanda İşleme (Chain of Responsibility)**

Middleware API'ye ulaşan veri, sırasıyla iki handler'dan geçer:

1. **SecurityHandler**: Hassas verileri maskeleme
   - TC Kimlik: `12345678901` → `1234****901`
   - Kredi Kartı: `4532123456789012` → `****6789****9012`
   - E-posta: `user@example.com` → `****@example.com`

2. **EnrichmentHandler**: Veri zenginleştirme
   - `IsCritical = (LogLevel == "CRITICAL" || LogLevel == "WARNING")`
   - `SummaryMessage = "AAPL hissesi için 1500.50 TL'lik SATIN AL işlemi, INFO seviyesi"`

#### **Aşama 4: Dinamik Formatlandırma (Strategy Pattern)**

İşlenmiş veriler, üç stratejiye serileştirilir ve konsola yazdırılır:

- **HTML Tablosu**: İnsan tarafından doğrudan okunabilir
- **CSV Satırı**: Veri analiz araçlarına uyumlu
- **JSON Objesi**: Diğer API'lere aktarılabilir

## 5. Kurulum ve Çalıştırma

### 5.1 Ön Koşullar

Sistemde aşağıdakiler kurulu olmalıdır:
- **Docker Desktop** (Windows/Mac) veya **Docker Engine + Docker Compose** (Linux)
- **Git** (klonlama için isteğe bağlı)

### 5.2 Adım Adım Kurulum

#### **Adım 1: Projeyi Hazırlama**

```bash
# Proje dizinine gidin
cd C:\YMODEVİ
# veya
cd /path/to/YMODEVİ
```

#### **Adım 2: Docker Compose Konfigürasyonunu Doğrulama**

```bash
# Syntax kontrolü ve compose dosyasını görüntüle
docker-compose config
```

Eğer hata görmüyorsanız, konfigürasyon doğrudur.

#### **Adım 3: Konteynerları İnşa Etme ve Çalıştırma**

```bash
# Konteynerleri inşa et ve başlat
docker-compose up --build
```

Bu komut:
1. `DataMiddleware` ve `DataGenerator` için Docker image'ları inşa eder
2. Her iki servisi başlatır
3. Data Generator'ün, Middleware API'yi bulmasını ve bağlanmasını sağlar
4. Canlı log çıktısını ekranda gösterir

#### **Adım 4: Çalıştırma Çıktısı**

Başarılı bir başlatma sonrası, terminal ekranında şu şekilde bir çıktı görüntülenecektir:

```
middleware-api     | info: DataMiddleware.Controllers.LogsController[0]
middleware-api     |       === HTML (System Admin) ===
middleware-api     |       <table border='1'>
middleware-api     |       <tr><th>Timestamp</th><th>Sender ID</th>..
middleware-api     |
middleware-api     |       === CSV (Cybersec) ===
middleware-api     |       2026-06-03T14:30:45.1234567Z;a1b2c3d4-e5f6...
middleware-api     |
middleware-api     |       === JSON (Web Dev) ===
middleware-api     |       {"timestamp":"2026-06-03T14:30:45.1234567Z"...
middleware-api     |
data-generator     | SUCCESS: 200
data-generator     | SUCCESS: 200
data-generator     | SUCCESS: 200
```

#### **Adım 5: Durumu Durdurma**

```bash
# Konteynerları durdur
docker-compose down

# Yalnızca durdurmak (silmek değil)
docker-compose stop

# Yeniden başlat
docker-compose restart
```

### 5.3 Sorun Giderme

**Problem**: `Error: Cannot connect to middleware-api:8080`
- **Çözüm**: `docker-compose up --build` komutunun tamamen bitmesini bekleyin (Middleware API'nin başlaması biraz zaman alabilir)

**Problem**: `error CS8802: Only one compilation unit can have top-level statements`
- **Çözüm**: `DataGenerator\` ve `DataMiddleware\` dizinlerinin ayrı olduğundan emin olun ve iki `Program.cs` dosyası bulunmadığından kontrol edin

**Problem**: Port `8080` zaten kullanımda
- **Çözüm**: `docker-compose.yml` içinde port numarasını değiştirin: `"9000:8080"` gibi

## 6. Proje Yapısı

```
YMODEVİ/
├── docker-compose.yml           # Konteyner orkestrasyonu
├── DataMiddleware/
│   ├── Dockerfile               # Middleware API container imajı
│   ├── DataMiddleware.csproj    # Proje dosyası
│   ├── Program.cs               # Uygulama entry point
│   ├── Controllers/
│   │   └── LogsController.cs    # POST /api/logs endpoint
│   ├── Models/
│   │   └── LogData.cs           # Veri modelleri
│   └── Patterns/
│       ├── ChainOfResponsibility/
│       │   ├── LogHandler.cs
│       │   ├── SecurityHandler.cs
│       │   └── EnrichmentHandler.cs
│       └── Strategy/
│           ├── IOutputStrategy.cs
│           ├── HtmlStrategy.cs
│           ├── CsvStrategy.cs
│           └── JsonStrategy.cs
└── DataGenerator/
    ├── Dockerfile               # Generator container imajı
    ├── DataGenerator.csproj     # Proje dosyası
    └── Program.cs               # Veri üreten konsol uygulaması
```

## 7. Performans ve Ölçeklenebilirlik

- **İstek Hızı**: 50 ms aralıklar (saniye başına ~20 istek)
- **Load Testing**: Aralık değeri `const int DelayMilliseconds = 50;` düzenleyerek ayarlanabilir
- **Docker Compose**: Çoklu Generator örneği eklemek için `docker-compose.yml` kopyalanabilir
- **Timeout Ayarlanması**: HttpClient timeout'u 10 saniye olarak konfigüre edilmiştir

## 8. Kaynaklar ve Referanslar

- [Microsoft - ASP.NET Core Documentation](https://learn.microsoft.com/aspnet/core)
- [Design Patterns - Gang of Four](https://en.wikipedia.org/wiki/Design_Patterns)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [System.Text.Json - JSON Serialization](https://learn.microsoft.com/dotnet/standard/serialization/system-text-json)

---

**Projeyi Yapan**:Hakan Tarık Karaduman  
**Tarihi**: Haziran 2026  

