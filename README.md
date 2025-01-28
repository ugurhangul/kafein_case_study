# Projelerin Kurulum ve Çalıştırma Kılavuzu

Bu kılavuz, Collector Service, Processor Service, Rule/Config Service ve Reporting/Alerting Service gibi mikroservislerin nasıl çalıştırılacağını adım adım açıklar. Ayrıca, bu servislerin Kafka, Elasticsearch gibi bağımlılıklarla nasıl entegre olduğunu ve Docker Compose kullanarak tüm sistemi nasıl başlatacağınızı detaylandırır.

---

## **Gereksinimler**
Projeyi çalıştırmadan önce aşağıdaki yazılımların sisteminizde kurulu olduğundan emin olun:

1. **Docker** ve **Docker Compose**
2. **.NET 8 SDK** (geliştirme/test için gerekli)
3. **cURL** veya Postman (API testleri için önerilir)

---

## **1. Projelerin Yapılandırması**
Projelerde kullanılan ana bağımlılıklar:

- **Kafka**: Event’lerin üretilmesi ve işlenmesi için.
- **Elasticsearch**: Event’lerin saklanması ve sorgulanması için.
- **PostgreSQL (opsiyonel)**: Rule/Config Service gibi veri tabanı gereksinimi olan servisler için.

### **Docker Compose Dosyası**
Projelerdeki tüm servisler ve bağımlılıkları bir `docker-compose.yml` dosyası üzerinden yönetilir.

---

## **2. Tüm Sistemi Başlatma**

1. **Docker Compose ile Servisleri Başlatın:**
   Proje dizininde aşağıdaki komutu çalıştırın:
   ```bash
   docker-compose up --build
   ```

2. **Servislerin Durumunu Kontrol Edin:**
   Tüm servislerin çalıştığını doğrulamak için:
   ```bash
   docker ps
   ```
   Beklenen servisler:
   - `collector-service`
   - `processor-service`
   - `rule-config-service`
   - `reporting-alerting-service`
   - `kafka`
   - `elasticsearch`

---

## **3. Servislerin Bağımsız Çalıştırılması**
Her bir mikroservisi bağımsız olarak çalıştırabilirsiniz:

### **a. Collector Service**
- Görevi: Event’leri Kafka’ya üretmek.

#### **Başlatma:**
   ```bash
   cd CollectorService
   dotnet run
   ```

#### **Test:**
Event üretmek için aşağıdaki API isteğini yapın:
   ```bash
   curl -X POST http://localhost:5000/produce \
   -H "Content-Type: application/json" \
   -d '{
     "EventType": "SELECT",
     "Username": "test_user",
     "DatabaseName": "test_db",
     "Statement": "SELECT * FROM Users;",
     "Severity": "High"
   }'
   ```
---

### **b. Processor Service**
- Görevi: Kafka’dan event’leri tüketip Elasticsearch’e kaydetmek.

#### **Başlatma:**
   ```bash
   cd ProcessorService
   dotnet run
   ```

#### **Test:**
Elasticsearch’e kaydedilen event’leri kontrol etmek için:
   ```bash
   curl -X GET "http://localhost:9200/audit-events/_search?pretty=true"
   ```
---

### **c. Rule/Config Service**
- Görevi: Event sınıflandırma kurallarını sağlamak.

#### **Başlatma:**
   ```bash
   cd RuleConfigService
   dotnet run
   ```

#### **Test:**
Kuralları kontrol etmek için:
   ```bash
   curl -X GET http://localhost:5001/api/rules
   ```
---

### **d. Reporting/Alerting Service**
- Görevi: Kritik olayları sorgulamak ve raporlamak.

#### **Başlatma:**
   ```bash
   cd ReportingAlertingService
   dotnet run
   ```

#### **Test:**
Belirli bir tarih aralığındaki kritik olayları sorgulamak için:
   ```bash
   curl -X GET "http://localhost:5005/api/reports/critical-events?startDate=2025-01-01&endDate=2025-01-27"
   ```

---

## **4. Endpoint Listesi**

### **Collector Service**
1. **POST /produce**: Event üretip Kafka’ya gönderir.
   - Örnek Kullanım:
     ```bash
     curl -X POST http://localhost:5000/produce -H "Content-Type: application/json" -d '{ "EventType": "SELECT", "Username": "user1", "DatabaseName": "test_db", "Statement": "SELECT * FROM users", "Severity": "High" }'
     ```

### **Processor Service**
1. **(Internal)**: Kafka’dan event tüketir ve Elasticsearch’e kaydeder. Public bir endpoint yoktur.

### **Rule/Config Service**
1. **GET /api/rules**: Tüm kuralları döner.
   - Örnek Kullanım:
     ```bash
     curl -X GET http://localhost:5001/api/rules
     ```

### **Reporting/Alerting Service**
1. **GET /api/reports/critical-events**: Kritik olayları sorgular.
   - Parametreler:
     - `startDate`: Başlangıç tarihi (ISO 8601 formatında).
     - `endDate`: Bitiş tarihi (ISO 8601 formatında).
   - Örnek Kullanım:
     ```bash
     curl -X GET "http://localhost:5005/api/reports/critical-events?startDate=2025-01-01&endDate=2025-01-27"
     ```
2. **GET /api/reports/config/rules**: Rule/Config Service’den kuralları getirir.
   - Örnek Kullanım:
     ```bash
     curl -X GET http://localhost:5005/api/reports/config/rules
     ```

---

## **5. Hata Giderme**
### **a. Kafka Bağlantı Sorunları**
- Eğer Kafka’ya bağlanamıyorsanız:
  1. Kafka konteynerinin çalıştığından emin olun:
     ```bash
     docker logs kafka
     ```
  2. Kafka bağlantısını test edin:
     ```bash
     docker exec -it kafka kafka-console-consumer --bootstrap-server kafka:9092 --topic audit-events --from-beginning
     ```

### **b. Elasticsearch Bağlantı Sorunları**
- Elasticsearch’e erişilemiyorsa:
  1. Elasticsearch konteynerinin çalıştığını doğrulayın:
     ```bash
     docker logs elasticsearch
     ```
  2. HTTP üzerinden erişim testi yapın:
     ```bash
     curl -X GET "http://localhost:9200"
     ```

### **c. API Sorunları**
- Servislerden birine erişilemiyorsa, port eşlemelerini kontrol edin ve logları inceleyin:
  ```bash
  docker logs [service-name]
  ```

---

## **6. Genel Test Senaryosu**
1. **Event Üret:**
   - Collector Service üzerinden Kafka’ya bir event gönder.

2. **Kafka’da Mesajı Tüket:**
   - Kafka consumer ile mesajın geldiğini doğrula.

3. **Elasticsearch’e Kaydet:**
   - Processor Service’in bu mesajı alıp Elasticsearch’e kaydettiğini kontrol et.

4. **Raporlama:**
   - Reporting/Alerting Service ile bu event’i sorgula.

---

## **7. Ek Notlar**
- Herhangi bir servis çalıştırılamazsa veya beklenmeyen bir hata oluşursa, logları kontrol edin.
- Geliştirme ortamında `docker-compose.override.yml` kullanarak servis ayarlarını özelleştirebilirsiniz.

Bu kılavuzu takip ederek tüm mikroservislerinizi kolayca çalıştırabilir ve test edebilirsiniz. 😊
