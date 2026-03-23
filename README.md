# 1)Strong Consistency
### **Proje: 2PC-Example(Bank Transfer)**

📌 Projenin Özellikleri

- Mimari: 3 servisli yapı (Coordinator API + Accounts.API + Ledger.API)

- Dağıtık İşlem Yaklaşımı: 2PC (Two-Phase Commit) – HTTP üzerinden simülasyon

- Teknolojiler: .NET 8, ASP.NET Core Minimal API, Entity Framework Core, PostgreSQL, Swagger, HttpClientFactory

Bu proje, “banka havalesi” senaryosu üzerinden tüm kaynaklarda(Accounts.API ve Ledger.API) bir işlemin atomik bir şekilde tamamlanıp, tamamlanmadığını pratik etmek için tasarlanmıştır.

- Coordinator API, transfer işlemini başlatır ve 2PC akışını yönetir:
- Phase 1 (Ready / Prepare): Accounts.API ve Ledger.API’ye “hazır mısın?” çağrısı yapılır. Servisler kendi DB’lerinde işlemi “pending/intent” olarak hazırlar.
- Phase 2 (Commit / Rollback): Tüm servisler “ready” dönerse commit edilir; aksi durumda rollback uygulanır.

# 2)Eventual Consistency
### **Proje 1) Saga-Example1 (Choreography)**

📌 Projenin Özellikleri

- Mimari: 3 servisli yapı (Order.API + Stock.API + Payment.API)

- Saga Yaklaşımı: Choreography Saga (servisler event dinleyip event yayınlar)

- Mesajlaşma: RabbitMQ (MassTransit ile event publish/consume)

- Teknolojiler: .NET 8, ASP.NET Core Minimal API, MassTransit, RabbitMQ, Entity Framework Core, PostgreSQL, Swagger, Docker (RabbitMQ)

Bu proje, e-ticaret sipariş akışı üzerinden event-driven yapı ve choreography saga mantığını pratik etmek için hazırlanmıştır.

- Order.API, dış dünyaya REST endpoint sağlar. Sipariş kaydını DB’ye kaydettikten sonra OrderCreatedEvent yayınlar.
- Stock.API, sipariş event’ini tüketerek stok kontrolü yapar ve sonuca göre stok ayırır ya da başarısızlık event’i yayınlar.
- Payment.API, stok ayrıldı event’ini tüketerek ödeme işlemini simüle eder ve başarı/başarısızlık event’i yayınlar.
- Son durumda Order.API, ödeme veya stok sonucuna göre sipariş durumunu günceller.

### **Proje 2) Saga-Example2 (Orchestration)**

📌 Projenin Özellikleri

- Mimari: 4 servisli yapı (Order.API + Stock.API + Payment.API + SagaStateMachine.Service)

- Saga Yaklaşımı: Orchestration Saga (akış merkezi bir orchestrator/state machine tarafından yönetilir)

- Mesajlaşma: RabbitMQ (MassTransit + State Machine + Queue tabanlı yönlendirme)

- Teknolojiler: .NET 8, ASP.NET Core Minimal API, MassTransit, MassTransit State Machine, RabbitMQ, Entity Framework Core, PostgreSQL, Swagger, Docker (RabbitMQ)

Bu proje, e-ticaret sipariş akışını orchestrator yaklaşımıyla yöneterek, choreography’e kıyasla akış kontrolünü tek noktada toplamayı hedefler.

- Order.API siparişi oluşturur, DB’ye kaydeder ve OrderStartedEvent mesajını StateMachineQueue’ya gönderir
- SagaStateMachine.Service (Orchestrator) bu event’i alır, state’i oluşturur ve süreci yönetir:
  - Stock servisine OrderCreatedEvent gönderir
  - Stock cevabına göre Payment sürecini başlatır veya siparişi fail eder
- Stock.API stok kontrolü yapar:
  - başarılıysa StockReservedEvent → StateMachineQueue
  - başarısızsa StockNotReservedEvent → StateMachineQueue
- Payment.API ödeme simülasyonu yapar:
  - başarılıysa PaymentCompletedEvent → StateMachineQueue
  - başarısızsa PaymentFailedEvent → StateMachineQueue
- Orchestrator sonucu değerlendirir:
  - başarılıysa OrderCompletedEvent → Order.API
  - başarısızsa OrderFailedEvent → Order.API
  - ödeme başarısızsa ayrıca StockRollbackMessage → Stock.API (compensation)

- Telafi (Compensation)
  - PaymentFailed durumunda orchestrator, StockRollbackMessage göndererek stokların geri alınmasını sağlar (restock).

