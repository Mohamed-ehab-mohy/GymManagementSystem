# GymPro — Gym Management System

A complete gym management web application built on .NET 10.0 with Clean Architecture, featuring a professional UI with dark/light mode support.

## Tech Stack

| Technology | Version |
|------------|---------|
| .NET | 10.0 |
| ASP.NET Core MVC | 10.0 |
| Entity Framework Core | 10.0.8 |
| SQL Server | 2022 |
| PostgreSQL | 16 Alpine |
| Redis (via IDistributedCache) | - |
| Autofac | 10.0.0 |
| Mapster | 7.4.0 |
| Serilog | 9.0.0 |
| Quartz.NET | 3.13.1 |
| SignalR | - |
| Tailwind CSS | 3.4.4 |
| Alpine.js | 3.x |
| Chart.js | - |
| xUnit | 2.9.3 |

### Packages

| Category | Packages |
|----------|----------|
| Mapping | Mapster, Mapster.DependencyInjection |
| DI | Autofac.Extensions.DependencyInjection |
| Logging | Serilog.AspNetCore, Serilog.Sinks.Seq, Serilog.Sinks.Postgresql.Alternative, Serilog.Enrichers.Environment, Serilog.Enrichers.Thread |
| Export | ClosedXML (Excel), QuestPDF (PDF) |
| AI | OpenAI (GPT-4o-mini) |
| Images / QR | CloudinaryDotNet, QRCoder, html5-qrcode@2.3.8 |
| Auth | BCrypt.Net-Next, Microsoft.AspNetCore.Authentication.Google |
| Email | MailKit |
| Scheduling | Quartz, Quartz.Extensions.Hosting |
| Monitoring | prometheus-net.AspNetCore, AspNetCore.HealthChecks.SqlServer, AspNetCore.HealthChecks.NpgSql, AspNetCore.HealthChecks.UI |
| Testing | xUnit, Shouldly, NSubstitute, NetArchTest.Rules, Microsoft.AspNetCore.Mvc.Testing, coverlet.collector |

## Architecture

Clean Architecture with 4 layers:

```
GymManagementSystem.Domain    - Entities, interfaces, enums (no dependencies)
GymManagementSystem.DAL       - DbContext, repositories, migrations, interceptors
GymManagementSystem.BLL       - Services, DTOs, mapping, export logic
GymManagementSystem.PL        - Controllers, Views, Hubs, Jobs, Seeders, wwwroot
```

### Domain Layer

Entities: `BaseEntity`, `GymUser` (abstract), `Member`, `Trainer`, `Plan`, `Membership`, `Booking`, `ClassSession`, `Category`, `Payment`, `HealthRecord`, `Notification`, `PasswordResetToken`, `Address` (value object), `TrainerSpecialty` (enum).

Key relationships:
- Member -> Memberships -> Plan
- Member -> Bookings -> ClassSession -> Trainer + Category
- Member -> HealthRecord (1:1)
- Member -> Payments -> Membership

### DAL Layer

- `GymDbContext` with Fluent API configurations (10 entity configurations)
- Generic `Repository<T>` and specific repositories (Member, Booking, Plan, etc.)
- `UnitOfWork` pattern
- `AuditInterceptor` (tracks CreatedBy/UpdatedBy)
- `SoftDeleteInterceptor` (auto-filters IsDeleted=false)
- 8 EF Core migrations

### BLL Layer

- 17 service classes implementing 18 interfaces
- 12 DTOs for data transfer
- Mapster `MappingProfile`
- Export system: Excel (ClosedXML), PDF (QuestPDF)
- Attendance system: HMAC-signed QR payloads, `CheckInPayloadHelper`
- Result pattern: `Result<T>` for error handling
- Caching: `ICacheService` with Memory Cache

### Presentation Layer

- 10 MVC Controllers
- 15 View folders with professional blue/slate design system
- Sidebar-based layout (`_LayoutSidebar.cshtml`) with collapsible sidebar and topbar
- Auth pages use simplified `_Layout.cshtml` (centered card layout)
- Dark/Light mode via Alpine.js with localStorage persistence
- Consistent component classes: `page-card`, `stat-card`, `data-table`, `badge-*`, `form-*`, `btn-*`
- DataTables integration with Tailwind-styled pagination
- SignalR Hub (`/hubs/notifications`) with real-time toast notifications
- AI Chatbot widget on all pages
- 2 Quartz jobs: `PurgeDeletedRecordsJob` (daily 3 AM), `RenewalReminderJob` (daily 8 AM)
- Database seeder with 4 plans from JSON
- Tailwind CSS with PostCSS build pipeline (`npm run build:css`)

## Features

### UI Design
- Professional blue/slate color palette (primary: #2563eb, neutrals: slate)
- Collapsible sidebar navigation with role-based menu items
- Top bar with theme toggle, notifications dropdown, and profile menu
- Dark/Light mode with system preference detection and localStorage persistence
- Responsive design for desktop and mobile
- AI chatbot widget floating on all pages

### Members
- Full CRUD with soft delete
- Photo upload to Cloudinary
- Health records (blood type, weight, height, conditions)
- DataTables integration with search, sort, pagination
- Export to Excel and PDF

### Trainers
- Full CRUD
- Specialties: GeneralFitness, Yoga, Boxing, CrossFit
- DataTables view with contact info

### Plans
- 4 plans: Basic (30d/300 EGP), Standard (60d/500 EGP), Premium (90d/900 EGP), Annual (365d/3000 EGP)
- Toggle active/inactive (blocks deactivation if active memberships exist)
- Memory cache for active plans
- Export to Excel/PDF

### Memberships
- Purchase plan for member with auto date calculation
- Renewal with end date extension
- Auto reminders before expiry (Quartz job)
- Track active/expired status

### Class Sessions & Bookings
- Sessions with trainers and categories
- Booking with capacity validation
- Duplicate booking prevention
- Active membership requirement
- Booking cancellation with ownership check
- Real-time SignalR notifications

### Attendance
- QR code generation (QRCoder) for member check-in
- HMAC-signed payload format: `GYMYCHECKIN:{bookingId}:{signature}`
- Signature verification prevents tampering
- Duplicate attendance prevention
- Session date validation (today only)
- Admin scan page with html5-qrcode camera scanner
- Member QR code display page

### Payments
- Paymob gateway integration
- Payment status tracking (Pending, Completed, Failed)
- Multi-currency support (EGP default)

### AI Assistant
- OpenAI GPT-4o-mini integration
- Chat interface for admin questions

### Dashboard & Analytics
- Real-time stats: members, bookings, revenue, attendance
- Revenue analytics, popular sessions, trainer performance
- Chart.js visualizations

### Auth & Security
- Cookie authentication with 8h sliding expiration
- Google OAuth 2.0
- BCrypt password hashing (12 rounds)
- OTP-based password reset via email
- Roles: Admin, SuperAdmin, Member, Trainer
- Anti-CSRF on all POST requests
- Soft delete with audit fields

### Notifications
- Real-time via SignalR Hub
- Booking confirmations
- Renewal reminders
- Read/unread tracking

## Database Schema

```
Members ──> Memberships ──> Plans
  │             │
  │             └──> Payments
  │
  ├──> Bookings ──> ClassSessions ──> Trainers
  │                                └──> Categories
  ├──> HealthRecords
  └──> Notifications
```

### Entity Details

**Member:** Id, FirstName, LastName, Email, PhoneNumber, PasswordHash, Role, Photo, JoinDate, EmergencyContact, Address (Street, City, State, ZipCode)

**Membership:** Id, MemberId, PlanId, StartDate, EndDate, IsActive, ReminderSentAt, ReminderDaysSent

**Plan:** Id, Name, Description, DurationDays, Price, IsActive

**Booking:** Id, MemberId, ClassSessionId, BookingDate, IsAttended, CheckedInAt

**ClassSession:** Id, Name, ScheduleTime, StartTime, EndTime, Capacity, TrainerId, CategoryId

**Payment:** Id, MemberId, MembershipId, Amount, Currency, Status, PaymobOrderId, PaymobTransactionId

All entities inherit from `BaseEntity`: Id, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted (soft delete), DeletedAt.

## Default Credentials

After running the app (Docker or manual), the seeder creates these accounts:

| Email | Password | Role |
|-------|----------|------|
| admin@gymy.com | Admin@123 | Admin |
| superadmin@gymy.com | Super@123 | SuperAdmin |
| member1@gmail.com – member100@gmail.com | Member@123 | Member |
| trainer1@gymy.com – trainer100@gymy.com | Trainer@123 | Trainer |

> **Note:** `AttendanceSettings:SecretKey` must be configured in `appsettings.json` (or via environment variable `AttendanceSettings__SecretKey`) for the QR attendance system to work.

## How to Run

### Prerequisites
- .NET 10.0 SDK
- Docker Desktop (for Docker setup)
- SQL Server (for manual setup)
- Node.js (for Tailwind CSS)
- Git

### Option 1: Docker (Recommended)

```bash
git clone https://github.com/Mohamed-ehab-mohy/GymManagementSystem.git
cd GymManagementSystem
cp .env.example .env
# Edit .env - set OPENAI_API_KEY
docker-compose up -d
```

**Services:**

| Service | Port | Purpose |
|---------|------|---------|
| App | 5000 | Web application |
| SQL Server | 1433 | Main database |
| PostgreSQL | 5432 | Log storage |
| Seq | 5341 / 8081 | Log viewer |
| Prometheus | 9090 | Metrics |
| Grafana | 3000 | Dashboards |

Open: `http://localhost:5000`

### Option 2: Manual Setup

```bash
# 1. Clone
git clone https://github.com/Mohamed-ehab-mohy/GymManagementSystem.git
cd GymManagementSystem

# 2. Configure connection string in appsettings.Local.json
# "DefaultConnection": "Server=.;Database=GymManagementDb;Trusted_Connection=true;TrustServerCertificate=true"
# "AttendanceSettings:SecretKey": "your-secret-key"

# 3. Apply migrations
cd GymManagementSystem.PL
dotnet ef database update

# 4. Build Tailwind CSS
npm install
npm run build:css

# 5. Run
dotnet run
```

Open: `http://localhost:5000`

## Tests

3 test projects, 27 tests total.

### Architecture Tests (GymManagementSystem.ArchTests)
| Test | Description |
|------|-------------|
| BLL_Must_Not_Reference_DAL | Ensures BLL has no DAL dependency |
| Controllers_Must_Not_Import_EF_Directly | Controllers cannot import EF |
| All_Service_Classes_Must_Implement_Corresponding_Interface | Every service implements an interface |
| All_Repositories_Must_Live_In_DataAccess | Repositories are only in DAL |

### Unit Tests (GymManagementSystem.BLL.Tests)
| Test Suite | Count | What it tests |
|-----------|-------|---------------|
| AttendanceServiceTests | 7 | Check-in flow, tampered signatures, already attended, not found, QR generation |
| AuthServiceTests | 4 | Register, login, wrong password, role claims |
| BookingServiceTests | 5 | Full capacity, no membership, duplicate booking, valid booking, wrong owner cancel |
| ExportServiceTests | 4 | Excel output, PDF magic bytes, empty data, custom headers |
| PlanServiceTests | 4 | Toggle active with/without memberships, cache hit/miss |

### Integration Tests (GymManagementSystem.PL.Tests)
| Test Suite | Count | What it tests |
|-----------|-------|---------------|
| AccountControllerIntegrationTests | 3 | Login, Register, ForgotPassword page loads |
| AttendanceControllerIntegrationTests | 2 | Valid/invalid QR check-in with DB verification |
| BookingsControllerIntegrationTests | 3 | Authorization, past session, role check |
| HomeControllerIntegrationTests | 2 | Home page, error page |
| MembersControllerIntegrationTests | 4 | Index, DataTable JSON, Excel export, PDF export |
| RenewalReminderJobTests | 2 | Sends 2 emails, prevents duplicates |

### Run Tests

```bash
dotnet test                                    # All tests
dotnet test GymManagementSystem.BLL.Tests      # Unit tests only
dotnet test GymManagementSystem.PL.Tests       # Integration tests only
dotnet test GymManagementSystem.ArchTests      # Architecture tests only
dotnet test --collect:"XPlat Code Coverage"    # With coverage
```

**Test tools:** xUnit, Shouldly (assertions), NSubstitute (mocking), NetArchTest.Rules (architecture), Microsoft.AspNetCore.Mvc.Testing (integration)

## Routes

| Path | Description |
|------|-------------|
| `/` | Home page |
| `/Account/Login` | Login |
| `/Account/Register` | Register |
| `/Account/Logout` | Logout |
| `/Account/ForgotPassword` | Forgot password |
| `/Account/ResetPassword` | Reset password with OTP |
| `/Dashboard` | Dashboard |
| `/Members` | Member list |
| `/Members/DataTableData` | DataTable JSON |
| `/Members/Create` | Create member |
| `/Members/Edit/{id}` | Edit member |
| `/Members/Details/{id}` | Member details |
| `/Members/Delete/{id}` | Delete member |
| `/Members/ExportExcel` | Export to Excel |
| `/Members/ExportPdf` | Export to PDF |
| `/Trainers` | Trainer list |
| `/Plans` | Plan management |
| `/Bookings` | Book a session (Member) |
| `/Bookings/Mine` | My bookings (Member) |
| `/AdminBooking` | Manage bookings (Admin) |
| `/AdminBooking/Create` | Create booking (Admin) |
| `/ClassSessions` | Class sessions |
| `/Attendance/Scan` | QR scanner page (Admin) |
| `/Attendance/CheckIn` | Process check-in |
| `/Attendance/MyQr` | My QR code (Member) |
| `/Analytics` | Analytics page |
| `/Notifications` | Notifications |
| `/hubs/notifications` | SignalR hub |
| `/health` | Health check |
| `/health/ready` | Readiness check |
| `/health-ui` | Health check UI |
| `/metrics` | Prometheus metrics |

## Monitoring

### Serilog
- Console (JSON format)
- File (daily rolling: `logs/gymy-.log`)
- Seq (http://localhost:5341)
- PostgreSQL (table: Logs)

### Health Checks
- Endpoints: `/health`, `/health/ready`, `/health-ui`
- Checks: SQL Server, PostgreSQL

### Prometheus
- UI: http://localhost:9090
- Metrics: http://localhost:5000/metrics

### Grafana
- UI: http://localhost:3000

## Contributing

1. Fork the repo
2. Create branch: `git checkout -b feature/my-feature`
3. Commit: `git commit -m 'Add new feature'`
4. Push: `git push origin feature/my-feature`
5. Open a Pull Request

## License

MIT

## Developer

**Mohamed Ehab Mohy** - [GitHub](https://github.com/Mohamed-ehab-mohy)
