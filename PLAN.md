# Gym Management System — Refactor Plan

## 🎯 الهدف
تحويل المشروع من تطبيق MVC عادي إلى نظام إدارة جيم احترافي باستخدام أفضل الممارسات (SOLID, Clean Code, OOP) مع حل كل المشاكل الموجودة وتنظيم الـ Repo بشكل مرتب مع Commits معبرة.

---

## 📁 الهيكل الجديد للمشروع (بعد الـ Refactor)

```
GymManagementSystem/
├── GymManagementSystem.DAL/                # Data Access Layer
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── Configurations/                 # Entity Type Configurations
│   ├── Entities/                           # Domain Models
│   ├── Repositories/                       # Generic + Specific Repositories
│   ├── Migrations/
│   └── Interceptors/                       # Audit & Soft Delete
│
├── GymManagementSystem.BLL/                # Business Logic Layer
│   ├── Services/                           # Application Services
│   ├── Interfaces/                         # Service Interfaces
│   ├── DTOs/                               # Data Transfer Objects
│   └── Mapping/                            # AutoMapper Profiles
│
├── GymManagementSystem.PL/                 # Presentation Layer (MVC)
│   ├── Controllers/
│   ├── ViewModels/
│   ├── Views/
│   ├── wwwroot/
│   ├── Infrastructure/                     # Filters, Middleware, Extensions
│   └── Seeders/
│
├── GymManagementSystem.BLL.Tests/
├── GymManagementSystem.PL.Tests/
└── GymManagementSystem.slnx
```

---

## 🔴 المشاكل الحالية (مأخوذة من Code Review)

### حرجة
| # | المشكلة | الحل |
|---|---------|------|
| 1 | مفيش Authorization — كل الـ endpoints مفتوحة | إضافة `[Authorize]` و Roles (Admin/User) |
| 2 | Secret Key في appsettings.json plaintext | نقلها إلى User Secrets / Environment Variables |
| 3 | AuditInterceptor دايمًا "System" مش المستخدم الحقيقي | Inject `IHttpContextAccessor` لجلب الـ User |
| 4 | BookingRepository بيعمل SaveChanges مباشر — بيبوصل UnitOfWork | إزالة SaveChanges من Repository واستخدام UnitOfWork |
| 5 | SoftDeleteInterceptor بيستخدم string literal fragile | استخدام `nameof` أو interface `ISoftDeletable` |

### متوسطة
| # | المشكلة | الحل |
|---|---------|------|
| 6 | Services فيها تكرار رهيب (10 Services نفس الشكل) | Generic Base Service + AutoMapper |
| 7 | ViewModel Mapping مكرر 3-4 مرات في كل Controller | AutoMapper Profiles |
| 8 | SplitName يتقسم على أول مسافة | تقسيم اسم (First/Last) في الـ Form نفسه |
| 9 | Health Records ملهاش Update | إضافة Edit للـ Health Record |
| 10 | Session ملهاش تعديل Category | إضافة CategoryId لـ UpdateSessionViewModel |
| 11 | TempData strings مكررة | Centralized Constants Class للمessages |
| 12 | PlanService.DeletePlanAsync بيسكت لو null | Return (bool, string) consistent |
| 13 | CleanupBackgroundService Raw SQL | استخدام EF Core Bulk Delete أو Soft Delete Query |
| 14 | AvailableSlots = Capacity مش Available | حساب الفرق Capacity - Booking Count |
| 15 | مفيش Pagination/Search/Filter | إضافة PagedResult<T> + Search/Sort في كل Index |
| 16 | مفيش Register page | إضافة Register + Email Confirmation |
| 17 | Phone regex مكرر 4 مرات | Custom Validation Attribute واحد |
| 18 | مفيش Error Handling للـ Exceptions | Global Exception Middleware |
| 19 | Login من غير Model Validation | LoginViewModel مع Data Annotations |

### بسيطة
| # | المشكلة | الحل |
|---|---------|------|
| 20 | Class1.cs فاضي | حذف |
| 21 | !important inline styles | CSS Classes موحدة |
| 22 | LifetimesDemo Teaching Tool | حذف أو وضع Behind Feature Flag |
| 23 | EndDateAfterStartDateAttribute Reflection | Custom Validation مع مقارنة مباشرة |
| 24 | PdfWriter License بيتعمل كل مرة | مرة واحدة في Program.cs |
| 25 | AttachmentService بياخد webRootPath string | Inject IWebHostEnvironment |
| 26 | مفيش Dashboard | إضافة Dashboard Controller مع Stats |

---

## ✨ الميزات الجديدة المطلوبة

### 1. Authentication & Authorization (Identity)
- [ ] Register (Email Confirmation)
- [ ] Login / Logout
- [ ] Forgot Password / Reset Password
- [ ] Roles: Admin, User (بصلاحيات مختلفة)
- [ ] `[Authorize(Roles = "Admin")]` على الـ CRUD

### 2. Dashboard
- [ ] عدد الأعضاء (Total Members)
- [ ] عدد المدربين (Total Trainers)
- [ ] الخطط النشطة (Active Plans)
- [ ] الجلسات النهاردة (Today's Sessions)
- [ ] الحضور النهاردة (Today's Attendance)
- [ ] الاشتراكات المنتهية (Expiring Memberships)
- [ ] آخر العمليات (Recent Activity Log)

### 3. CRUD كامل مع Pagination, Search, Sorting, Filtering
- [ ] Members (with Pagination, Search by Name/Phone)
- [ ] Trainers (with Pagination, Search by Name/Specialty)
- [ ] Plans (with Pagination, Active/Inactive Filter)
- [ ] Class Sessions (with Pagination, Filter by Date/Trainer/Category)
- [ ] Bookings (with Pagination, Filter by Date/Member/Session)
- [ ] Categories
- [ ] Health Records

### 4. Validation
- [ ] Client-side (jQuery Validation)
- [ ] Server-side (Data Annotations + Custom Attributes)
- [ ] Custom Validation Attributes (Egyptian Phone, EndDate > StartDate)

### 5. File Upload
- [ ] رفع صور الأعضاء
- [ ] رفع صور المدربين (optionally)
- [ ] حفظ في wwwroot/uploads/
- [ ] Validation (حجم الملف، نوع الملف)

### 6. Error Handling
- [ ] Global Exception Middleware
- [ ] Custom Error Pages (404, 500, 403)
- [ ] Logging كل الأخطاء

### 7. Logging (Serilog)
- [ ] Login Attempts (ناجح/فاشل)
- [ ] Important Actions (Create/Update/Delete)
- [ ] Errors & Exceptions
- [ ] Seq Sink (already configured)

### 8. Soft Delete
- [ ] تحسين الـ Interceptor ليكون type-safe
- [ ] Query Filters على كل الـ Entities
- [ ] CRUD يحترم Soft Delete

### 9. Unit Tests
- [ ] Services Tests (MemberService, PlanService, AttendanceService, ...)
- [ ] Repositories Tests
- [ ] Use InMemory Database
- [ ] xUnit + Shouldly + NSubstitute

### 10. Generic Repository + Unit of Work
- [ ] `IRepository<T>` مع CRUD أساسي
- [ ] Specific Repositories للإضافات
- [ ] `IUnitOfWork` مع Transaction Support (حديثًا)
- [ ] تحسين الـ Repository Module

### 11. Audit Trail
- [ ] CreatedAt / CreatedBy
- [ ] UpdatedAt / UpdatedBy
- [ ] SoftDelete: DeletedAt / DeletedBy
- [ ] ربط فعلي بالمستخدم مش "System"

### 12. العلاقات (Relationships)
- [ ] **One to Many**: Trainer → ClassSessions, Member → Bookings, Category → ClassSessions
- [ ] **Many to Many**: Member ↔ ClassSession (through Booking)
- [ ] **One to One**: Member → HealthRecord

---

## 🧱 تسلسل التنفيذ (Commits)

> كل Commit هيكون focused على حاجة واحدة ومعبر عن نفسه

### Phase 1: Foundation (الإعدادات الأساسية)

| Commit | المحتوى |
|--------|---------|
| `1-clean-up-project-structure` | حذف Class1.cs, تنظيم الملفات, ترتيب المجلدات |
| `2-add-automapper-profiles` | إضافة AutoMapper وتجهيز Mapping Profiles |
| `3-improve-generic-repository` | تحسين Generic Repository (Pagination, Search, Filter) |
| `4-fix-unit-of-work` | حل مشكلة SaveChanges في BookingRepository |
| `5-fix-audit-interceptor` | ربط AuditInterceptor بالمستخدم الحقيقي |
| `6-fix-soft-delete-interceptor` | تحسين SoftDeleteInterceptor Type-Safe |

### Phase 2: Identity & Auth (الأمان)

| Commit | المحتوى |
|--------|---------|
| `7-add-identity-with-roles` | Register, Login, Logout, Roles (Admin/User) |
| `8-add-authorization-to-controllers` | [Authorize] على كل Controllers + Role-based Access |
| `9-add-email-confirmation` | Email Confirmation + Forgot Password |
| `10-move-secret-key-to-user-secrets` | نقل Secret Key والـ Connection String |

### Phase 3: Core Features (الوظائف الأساسية)

| Commit | المحتوى |
|--------|---------|
| `11-add-dashboard-controller` | Dashboard مع Stats Cards + Charts |
| `12-add-pagination-search-filter` | PagedResult, Search, Sort, Filter لكل الـ Indexes |
| `13-add-validation-attributes` | Custom Validation Attributes (Phone, EndDate) |
| `14-add-constants-for-messages` | SuccessMessages / ErrorMessages Constants Class |
| `15-fix-health-record-edit` | إضافة Edit للـ Health Records |
| `16-fix-session-category-edit` | إضافة CategoryId لـ UpdateSession |
| `17-fix-available-slots` | حساب Available Slots الحقيقي |
| `18-fix-name-splitting` | فصل First/Last Name في الـ Form |

### Phase 4: Error Handling & Logging

| Commit | المحتوى |
|--------|---------|
| `19-add-global-exception-middleware` | Middleware للـ Exception Handling |
| `20-add-custom-error-pages` | صفحات 404, 500, 403 مخصصة |
| `21-enhance-serilog-logging` | تحسين Serilog مع Structured Logging |

### Phase 5: File Upload & Attachment

| Commit | المحتوى |
|--------|---------|
| `22-improve-attachment-service` | Refactor AttachmentService (IWebHostEnvironment) |
| `23-add-file-upload-validation` | حجم/نوع الملف validation |

### Phase 6: Tests

| Commit | المحتوى |
|--------|---------|
| `24-add-service-unit-tests` | MemberService, PlanService, AttendanceService Tests |
| `25-add-repository-unit-tests` | Repository Tests with InMemory |

### Phase 7: Polish

| Commit | المحتوى |
|--------|---------|
| `26-remove-lifetimes-demo` | حذف LifetimesDemo |
| `27-fix-pdfwriter-license` | مرة واحدة في Startup |
| `28-clean-up-inline-styles` | CSS Classes بدل !important |
| `29-add-documentation` | README + Code Documentation |
| `30-final-cleanup-and-optimization` | مراجعة نهائية |

---

## 🛠️ التقنيات المستخدمة

| التقنية | الاستخدام |
|---------|-----------|
| ASP.NET Core MVC 10.0 | الإطار الرئيسي |
| Entity Framework Core 10.0 | ORM |
| SQL Server | قاعدة البيانات |
| ASP.NET Core Identity | Authentication & Authorization |
| AutoMapper | Mapping Entities ↔ DTOs/ViewModels |
| Serilog | Logging |
| Autofac | Dependency Injection Container |
| xUnit + Shouldly + NSubstitute | Unit Testing |
| ClosedXML | Export to Excel |
| QuestPDF | Export to PDF |
| QRCoder | QR Code Generation |

---

## 📐 المبادئ المطبقة

- **SOLID**: كل class له responsibility واحدة
- **DRY**: Generic Repository + Base Service + AutoMapper
- **KISS**: كود بسيط ومباشر
- **Clean Code**: مفيش comments في الكود
- **Separation of Concerns**: 3 Layers واضحة
- **Convention over Configuration**: EF Core Conventions

---

## 📝 قواعد الـ Commits

1. مفيش `--` ولا رموز غريبة في الـ commit message
2. كل commit يعبر عن feature واحدة
3. commit message بالانجليزي — واضح ومختصر
4. مفيش commit واحد يجمع حاجتين مختلفين
5. مفيش comments جوه الكود خالص
6. مفيش ملفات زيادة (Class1.cs / unnecessary files)
