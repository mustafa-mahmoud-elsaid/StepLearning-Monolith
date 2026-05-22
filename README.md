# StepLearning — Modular Monolith API

An online learning platform built as a **Modular Monolith** using **ASP.NET Core**, organized into independently structured modules that share a single deployment unit (`Host.Api`).

---

## 🏗️ Architecture

The project follows a **Modular Monolith** architecture where each module is fully self-contained with its own Domain, Application, and Infrastructure layers, but all modules are composed and hosted together by a single API host.

```
StepLearningSolution/
├── Host/
│   └── Host.Api/               # Single API entry point — composes all modules
├── Courses/
│   ├── Courses.Domain/
│   ├── Courses.Application/
│   └── Courses.Infrastructure/ # Has its own DbContext (CoursesDbContext)
├── Identity/
│   ├── Identity.Domain/
│   ├── Identity.Application/
│   └── Identity.Infrastructure/
├── Enrollments/
│   ├── Enrollment.Domain/
│   ├── Enrollment.Application/
│   └── Enrollment.Infrastructure/ # Has its own DbContext (EnrollmentDbContext)
├── Commerce/
│   ├── Commerce.Domain/
│   ├── Commerce.Application/
│   └── Commerce.Infrastructure/ # Has its own DbContext (CommerceDbContext)
├── Notifications/
│   ├── Notifications.Application/
│   └── Notifications.Infrastructure/
└── Shared/
    └── StepLearning.Shared/     # Shared abstractions (integration events, etc.)
```

---

## 📦 Modules

| Module | Responsibility | Database |
|---|---|---|
| **Identity** | User registration, login, JWT authentication, roles | `StepLearning.UsersDb` |
| **Courses** | Course catalog, sections, section items, instructors | `StepLearning.CoursesDb` |
| **Enrollments** | Student enrollments, enrollment status | `StepLearning.EnrollmentsDb` |
| **Commerce** | Payments via Stripe, shopping cart, payment records | `StepLearning.CommerceDb` |
| **Notifications** | Event-driven notifications triggered via RabbitMQ | — |

---

## 🛠️ Tech Stack

- **ASP.NET Core 10** — Web API
- **Entity Framework Core** — ORM (one DbContext per module)
- **SQL Server** — Relational database
- **MassTransit + RabbitMQ** — Async messaging and integration events
- **Stripe** — Payment processing
- **JWT** — Authentication and authorization
- **Docker & Docker Compose** — Containerized local development

---

## 🚀 Running with Docker Compose

The project includes a `docker-compose.yml` at the root that spins up all required infrastructure with a single command.

### Services

| Service | Image | Ports |
|---|---|---|
| `host.api` | Built from `Dockerfile` | `8080` (HTTP), `8081` (HTTPS) |
| `sqlserver` | `mcr.microsoft.com/mssql/server:2022-latest` | `1433` |
| `rabbitmq` | `rabbitmq:3-management` | `5672` (AMQP), `15672` (Management UI) |

All services are connected via a shared custom bridge network: `steplearning-net`.

### Start

```bash
docker compose up -d --build
```

- `--build` forces Docker to recompile your latest code changes into a fresh image.
- `-d` runs containers in detached (background) mode.

### Stop

```bash
docker compose down
```

### Useful URLs (once running)

| Resource | URL |
|---|---|
| Swagger UI | http://localhost:8080/swagger |
| RabbitMQ Management UI | http://localhost:15672 (user: `guest` / pass: `guest`) |

---

## 🌱 Database Seeding

The API exposes a dedicated seeding endpoint that populates all databases with realistic fake data using the **Bogus** library. Seeding runs in 4 ordered steps:

### Trigger Seeding

```http
POST http://localhost:8080/api/seed
```

You can call this from Swagger UI, Postman, or curl after the containers are running.

### Seeding Steps

| Step | What Gets Seeded | Volume |
|---|---|---|
| **1 — Roles** | Admin, Student, and Instructor roles | — |
| **2 — Users** | Students and Instructors with fake profiles | 4,000 students, 500 instructors |
| **3 — Courses** | Courses with sections and section items assigned to instructors | Based on instructor count |
| **4 — Payments & Enrollments** | Payment records (80% success rate) and active enrollments for successful payments | Per student |

### Seeding Notes

- Each student is randomly enrolled in **0, 1, 3, 5, or 8** courses.
- **80%** of payment attempts are marked as `Succeeded`, which triggers an `Active` enrollment.
- **20%** of payment attempts are marked as `Failed` with no enrollment created.
- Seeding uses **EFCore.BulkExtensions** with batches of 500 students for efficient bulk inserts.
- Re-running the endpoint will insert additional records (idempotency guard is configurable in the seeders).

### Seeding Response

```json
{
  "message": "Database seeded successfully!",
  "elapsedMilliseconds": 12345,
  "studentsSeeded": 4000,
  "instructorsSeeded": 500,
  "coursesSeeded": 150
}
```

---

## ⚙️ Configuration

Key settings are in `Host/Host.Api/appsettings.json`. When running via Docker Compose, all connection strings and infrastructure settings are **automatically overridden** via environment variables.

### Connection Strings

| Key | Database |
|---|---|
| `CoursesDbConnection` | `StepLearning.CoursesDb` |
| `StepLearning.UsersDb` | `StepLearning.UsersDb` |
| `StepLearning.EnrollmentsDb` | `StepLearning.EnrollmentsDb` |
| `StepLearning.CommerceDb` | `StepLearning.CommerceDb` |

### Other Settings

| Key | Description |
|---|---|
| `JWT:Key` | Secret key used to sign JWT tokens |
| `JWT:Issuer` | JWT issuer |
| `JWT:Audience` | JWT audience |
| `JWT:DurationInMinutes` | Access token lifetime |
| `JWT:RefreshTokenDurationInDays` | Refresh token lifetime |
| `RabbitMq:Host` | RabbitMQ host (`rabbitmq` in Docker, `localhost` locally) |
| `Stripe:SecretKey` | Stripe secret key for payment processing |
| `Stripe:WebhookSecret` | Stripe webhook secret for event verification |

---

## 🗃️ Migrations

Each module manages its own EF Core migrations. To run migrations for a specific module, target its `Infrastructure` project and DbContext:

```bash
# Example: Courses module
dotnet ef migrations add <MigrationName> \
  --project Courses/Courses.Infrastructure \
  --startup-project Host/Host.Api \
  --context CoursesDbContext

# Apply migrations
dotnet ef database update \
  --project Courses/Courses.Infrastructure \
  --startup-project Host/Host.Api \
  --context CoursesDbContext
```

> **Note:** Migrations are applied automatically on startup via `context.Database.Migrate()` when the application starts.

---

## 🌐 Messaging (RabbitMQ + MassTransit)

The system uses **MassTransit** over **RabbitMQ** for asynchronous, event-driven communication between modules.

### Consumers

| Consumer | Module | Triggered By |
|---|---|---|
| `PaymentSucceededConsumer` | Enrollment | `PaymentSucceeded` integration event |
| `EnrollmentCompletedNotificationConsumer` | Notifications | `EnrollmentCompleted` integration event |

---

## 🔐 Authentication

The API uses **JWT Bearer** authentication.

1. Register a user via `POST /api/identity/register/student` or `/instructor`.
2. Login via `POST /api/identity/login` to receive an access token and refresh token.
3. Pass the token in the `Authorization` header: `Bearer <token>`.