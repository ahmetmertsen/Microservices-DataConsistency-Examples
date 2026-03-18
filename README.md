# 1)Strong Consistency
Proje: 2PC-Example(Bank Transfer)

📌 Projenin Özellikleri

- Mimari: 3 servisli yapı (Coordinator API + Accounts.API + Ledger.API)

- Dağıtık İşlem Yaklaşımı: 2PC (Two-Phase Commit) – HTTP üzerinden simülasyon

- Teknolojiler: .NET 8, ASP.NET Core Minimal API, Entity Framework Core, PostgreSQL, Swagger, HttpClientFactory

Bu proje, “banka havalesi” senaryosu üzerinden tüm kaynaklarda(Accounts.API ve Ledger.API) bir işlemin atomik bir şekilde tamamlanıp, tamamlanmadığını pratik etmek için tasarlanmıştır.

- Coordinator API, transfer işlemini başlatır ve 2PC akışını yönetir:
- Phase 1 (Ready / Prepare): Accounts.API ve Ledger.API’ye “hazır mısın?” çağrısı yapılır. Servisler kendi DB’lerinde işlemi “pending/intent” olarak hazırlar.
- Phase 2 (Commit / Rollback): Tüm servisler “ready” dönerse commit edilir; aksi durumda rollback uygulanır.

# 2)Eventual Consistency
Proje 1) Saga-Example1 (Choreography) – Order / Stock / Payment

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
