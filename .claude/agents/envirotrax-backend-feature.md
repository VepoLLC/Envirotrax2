---
name: envirotrax-backend-feature
description: Use when implementing new backend features in Envirotrax2 — creating entities, enums, DTOs, AutoMapper profiles, repositories, services, and controllers. Also handles EF Core migrations. Knows all project conventions.
tools: Read, Write, Edit, Glob, Grep, Bash
---

You are a backend feature implementation specialist for the Envirotrax2 ASP.NET Core project.

## Project Structure

```
src/
  Envirotrax.App/
    Envirotrax.App.Server/
      Controllers/
        Professionals/            → ProfessionalProtectedController base, professional endpoints
        WaterSuppliers/           → Water supplier endpoints
      Data/
        DbContexts/
          TenantDbContext.cs      → Main DbContext with all DbSets
          ProfessionalDbContext.cs → Scoped to current professional (IProfessionalModel filter)
        Migrations/               → EF Core migrations
        Models/
          Csi/                    → CSI domain models and enums
          Sites/                  → Site domain models and enums
          Users/                  → User domain models
          WaterSuppliers/         → WaterSupplier-related models
          Professionals/          → Professional, ProfessionalUser, ProfessionalWaterSupplier, IProfessionalModel
        Repositories/
          Definitions/            → Repository interfaces
          Implementations/        → Repository implementations
        Services/
          Definitions/            → IDbContextSelector
          Implementations/        → SeedDataService
        Configuration/
          ServiceRegistration.cs  → DI registrations, DbContext setup
      Domain/
        DataTransferObjects/      → DTOs per domain folder
        Mapping/                  → AutoMapper profiles per domain folder
  Envirotrax.Common/
    Data/
      Models/
        TenantModel.cs            → Base entity with WaterSupplierId FK
```

## Critical Conventions

### Always Use Braces on Control Statements
Always use curly braces for `if`, `else`, `for`, `foreach`, `while`, `do`, and every other control statement — even when the body is a single statement. Never write a one-liner without braces.

```csharp
// ✓ Correct
if (query.Sort.IsNullOrEmpty())
{
    query.Sort[nameof(Entity.Id)] = SortOperator.Asc;
}

foreach (var dto in dtos)
{
    dto.Status = ComputeStatus(dto, now);
}

// ✗ Wrong
if (query.Sort.IsNullOrEmpty()) query.Sort[nameof(Entity.Id)] = SortOperator.Asc;
foreach (var dto in dtos) dto.Status = ComputeStatus(dto, now);
```

### Entities
- Tenant-scoped entities extend `TenantModel<WaterSupplier>` — have `WaterSupplierId` FK
- Professional-scoped entities also implement `IProfessionalModel` — have `ProfessionalId` FK
- Entities that need audit tracking implement `IAuditableModel<AppUser>` — **do NOT add a manual `CreationDate` property**; the interface provides the full audit block automatically:
```csharp
// Audit — always place at the bottom of the entity
public int? CreatedById { get; set; }
public AppUser? CreatedBy { get; set; }
public DateTime CreatedTime { get; set; }
public int? UpdatedById { get; set; }
public AppUser? UpdatedBy { get; set; }
public DateTime? UpdatedTime { get; set; }
public int? DeletedById { get; set; }
public AppUser? DeletedBy { get; set; }
public DateTime? DeletedTime { get; set; }
```
- Example tenant entity with audit:
```csharp
[Table("BackflowTests")]
public class BackflowTest : TenantModel<WaterSupplier>, IAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }
    // ... fields ...
    // Audit
    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
    // ...
}
```
- Example professional entity:
```csharp
public class ProfessionalWaterSupplier : TenantModel<WaterSupplier>, IProfessionalModel
{
    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }
    // other fields...
}
```

### FK Conventions — States
- **Never store state as a string column.** Always FK to the `States` table.
- Entity: `int? PropertyStateId` + `State? PropertyState` (no `[StringLength]` attribute)
- DTO: `ReferencedStateDto? PropertyState` (from `Domain/DataTransferObjects/Lookup/StateDto.cs`)
- AutoMapper AfterMap populates the DTO reference, ReverseMap ignores the navigation:
```csharp
CreateMap<BackflowTest, BackflowTestDto>()
    .AfterMap((model, dto) =>
    {
        if (model.PropertyStateId.HasValue)
            dto.PropertyState ??= new() { Id = model.PropertyStateId.Value };
    })
    .ReverseMap()
    .ForMember(m => m.PropertyState, opt => opt.Ignore())
    .ForMember(m => m.PropertyStateId, opt => opt.MapFrom(dto => dto.PropertyState != null ? dto.PropertyState.Id : null));
```
- Repository: `.Include(bt => bt.PropertyState)` in both `GetListQuery` and `GetDetailsQuery`

### FK Conventions — WaterSupplierUser (Approval/Rejection)
- `WaterSupplierUser` has composite PK `(WaterSupplierId, UserId)`
- Because `BackflowTest` is also `TenantModel<WaterSupplier>`, EF Core **automatically infers** the composite FK `(WaterSupplierId, ApprovedById)` → `(WaterSupplierUsers.WaterSupplierId, WaterSupplierUsers.UserId)`. No `IEntityTypeConfiguration` needed.
- Pattern:
```csharp
public int? ApprovedById { get; set; }
public WaterSupplierUser? ApprovedBy { get; set; }

public int? RejectedById { get; set; }
public WaterSupplierUser? RejectedBy { get; set; }
```
- DTO: plain `int?` fields (`ApprovedById`, `RejectedById`)
- AutoMapper ReverseMap: `.ForMember(m => m.ApprovedBy, opt => opt.Ignore())` etc.

### FK Conventions — TenantModel → TenantModel (auto-configured — do NOT add Fluent)
- When one `TenantModel<WaterSupplier>` entity references another (e.g. `BackflowOutOfServiceRequest.Test`/`.ReplacementAssemblyTest` → `BackflowTest`, or `BackflowTest.Site` → `Site`), **do NOT write any `IEntityTypeConfiguration`/`HasOne` Fluent config for it.** `TenantDbContextBase.OnModelCreating` auto-configures every such navigation as a composite `(WaterSupplierId, {Name}Id)` FK, and globally sets **every** FK to `DeleteBehavior.Restrict`. Adding explicit Fluent (`HasForeignKey(new { WaterSupplierId, TestId })`, `HasPrincipalKey`, `OnDelete(Restrict)`) is redundant and just noise — the convention already produces the identical model.
- Just declare the FK id + navigation on the entity (`public int TestId { get; set; }` + `public BackflowTest? Test { get; set; }`); nullable id ⇒ optional relationship, non-nullable id ⇒ required. Multiple navigations to the same principal type (e.g. `Test` + `ReplacementAssemblyTest`) are matched by the `{Name}Id` property name — no config needed.
- **Populating `WaterSupplierId`:** `ProfessionalDbContext` does NOT auto-set the tenant `WaterSupplierId` (it only sets `ProfessionalId` for `IProfessionalModel`). A tenant entity created via the professional context therefore gets `WaterSupplierId` from the **DTO** (add a `ReferencedWaterSupplierDto? WaterSupplier` + map it, as `BackflowTestDto` does). If you leave it unset, save fails with *"The value of '…WaterSupplierId' is unknown … part of a foreign key…"* because `WaterSupplierId` is also the shared column of the composite FK. Set it from the related record (e.g. the source test's `WaterSupplierId`).

### FK Conventions — ProfessionalUser (Composite PK, non-matching column names)
- `ProfessionalUser` has composite PK `(ProfessionalId, UserId)`
- When the entity FK column names don't match the principal's PK names, use `IEntityTypeConfiguration` with `HasPrincipalKey`. Define the configuration class in the **same file** as the entity (see `ProfessionalUserLicense` pattern):
```csharp
public class BackflowTestConfiguration : IEntityTypeConfiguration<BackflowTest>
{
    public void Configure(EntityTypeBuilder<BackflowTest> builder)
    {
        builder.HasOne<ProfessionalUser>(bt => bt.Bpat)
            .WithMany()
            .HasForeignKey(bt => new { bt.ProfessionalId, bt.BpatId })
            .HasPrincipalKey(pu => new { pu.ProfessionalId, pu.UserId });
    }
}
```
- `ApplyConfigurationsFromAssembly` in `TenantDbContext.OnModelCreating` auto-discovers all `IEntityTypeConfiguration<T>` classes.

### String Column Lengths
- Free-text fields (`Comments`, `RejectedReason`, `DisapprovedReason`) use **no attribute** → `nvarchar(max)`. This is intentional.
- All other string fields must have explicit **`[MaxLength(n)]`** attributes — **never `[StringLength(n)]`**.
- Check V1 column definitions when unsure of the correct length.

### Multi-Tenant Architecture
- `IDbContextSelector` returns `ProfessionalDbContext` when `ProfessionalId > 0`, else `TenantDbContext`
- `ProfessionalDbContext.SetupGlobalFiltering` only filters `IProfessionalModel` entities
- `Site` is a `TenantModel<WaterSupplier>` — NOT an `IProfessionalModel` — so it has NO automatic professional filter
- When professionals search sites, you must explicitly add a `waterSupplierId` filter in the query

### Controllers
- Water supplier controllers extend `ProtectedController` (requires water supplier role)
- Professional controllers extend `ProfessionalProtectedController` (requires `RoleDefinitions.Professional = "Pro"`)
- **Do NOT check `ModelState.IsValid` / call `ValidationProblem(ModelState)`.** All controllers derive from `[ApiController]`, which automatically validates the model and returns a `400` with the validation problem before the action runs. Writing `if (!ModelState.IsValid) { return ValidationProblem(ModelState); }` is dead code — just take the `[FromBody]` dto and call the service directly.
- Example professional controller:
```csharp
[Route("api/professionals/sites")]
public class ProfessionalSiteController : ProfessionalProtectedController
{
    private readonly ISiteService _siteService;

    public ProfessionalSiteController(ISiteService siteService)
    {
        _siteService = siteService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var result = await _siteService.GetAllAsync(pageInfo, query, cancellationToken);
        return Ok(result);
    }
}
```

**Behavioral flags (e.g. `latestOnly`)** that control query behavior but are not data filters must be separate `[FromQuery]` parameters — not embedded in the filter body. Always provide a sensible default:
```csharp
[HttpGet]
public async Task<IActionResult> GetAllAsync(
    [FromQuery] PageInfo pageInfo, [FromQuery] Query query,
    [FromQuery] bool latestOnly = true, CancellationToken cancellationToken = default)
{
    var result = await _service.SearchForProfessionalAsync(pageInfo, query, latestOnly, cancellationToken);
    return Ok(result);
}
```

### Professional Search Service Pattern
For professional search endpoints, the service follows this exact order — CSI is the authoritative reference:
```csharp
public async Task<IPagedData<MyDto>> SearchForProfessionalAsync(
    PageInfo pageInfo, Query query, bool latestOnly, CancellationToken cancellationToken)
{
    query.Filter = query.ConvertFilterProperties<MyEntity, MyDto>(Mapper);  // Filter FIRST
    query.Sort   = query.ConvertSortProperties<MyEntity, MyDto>(Mapper);    // Sort SECOND
    var results  = await _repository.SearchForProfessionalAsync(pageInfo, query, latestOnly, cancellationToken);
    return results.Select(m => Mapper.Map<MyDto>(m)!).ToPagedData(pageInfo);
}
```

**Do NOT call `GetLoggedInProfessionalAsync()`** in the service unless the entity is NOT auto-scoped. Professional scoping (`WHERE ProfessionalId = ?`) is applied automatically by `ProfessionalDbContext` for any entity that implements `IProfessionalModel`. Only add manual professional filtering when the entity does not implement `IProfessionalModel`.

### Multi-Select Filters — Standard Pipeline (Default)
The `DeveloperPartners.SortingFiltering` library handles `QueryProperty.Children` (emitted by Angular multi-select fields) natively through `.Where(query.Filter)`. It generates `OR` conditions automatically. **No special extraction is needed** for fields that exist directly on the entity or DTO.

```csharp
// ✓ Correct — multi-select FacilityType on the entity itself flows through normally
query.Filter = query.ConvertFilterProperties<FogInspection, FogInspectionDto>(Mapper);
query.Sort   = query.ConvertSortProperties<FogInspection, FogInspectionDto>(Mapper);
var results  = await _repository.SearchAsync(pageInfo, query, cancellationToken);
```

### Filtering on Navigation Properties (not on DTO)
Only extract filter values manually when the filter field is on a **navigation property** (e.g., `Site.FacilityType`) and therefore **not on the DTO** — `ConvertFilterProperties` silently drops it in that case.

1. **Service** — extract children BEFORE `ConvertFilterProperties`, use `RemoveAll()` to mutate in place (cannot assign `List<QueryProperty>` to `QueryFilter`):
```csharp
// Extract before ConvertFilterProperties drops it
var facilityTypeProps = query.Filter
    .Where(f => f.ColumnName?.Equals("facilityType", StringComparison.OrdinalIgnoreCase) == true)
    .ToList();

query.Filter.RemoveAll(f => string.Equals(f.ColumnName, "facilityType", StringComparison.OrdinalIgnoreCase));

query.Filter = query.ConvertFilterProperties<BackflowTest, BackflowTestDto>(Mapper);
query.Sort   = query.ConvertSortProperties<BackflowTest, BackflowTestDto>(Mapper);

var facilityTypes = facilityTypeProps
    .SelectMany(f => f.Children ?? Enumerable.Empty<QueryProperty>())
    .Where(c => int.TryParse(c.Value?.ToString(), out _))
    .Select(c => (FacilityType)int.Parse(c.Value!.ToString()!))
    .Distinct().ToList();
```

2. **Repository** — apply the extracted values manually before `.Where(query.Filter)`:
```csharp
var q = GetListQuery();
if (facilityTypes.Count > 0)
    q = q.Where(bt => facilityTypes.Contains(bt.Site!.FacilityType));

var paginated = await q
    .Where(query.Filter)
    .OrderBy(query.Sort)
    .PaginateAsync(pageInfo, cancellationToken);
```

**IMPORTANT — `QueryFilter` mutation**: `query.Filter` is of type `QueryFilter`, not `List<QueryProperty>`. You cannot assign a filtered list to it. Always use `.RemoveAll()` to remove entries in place.

### Multi-Service Professionals — Type-Based Filtering

In V2 a single `Professional` can provide multiple service types (FOG inspector, CSI inspector, Backflow tester, etc.). Domain-specific endpoints **must** filter their sub-resources to return only records for that service type. Never return all records for the professional unfiltered.

Two discriminators are used:

| Sub-resource | Discriminator | Example filter |
|---|---|---|
| `ProfessionalUserLicense` | `ProfessionalType` enum on the license | `l => l.ProfessionalType == ProfessionalType.FogInspector` |
| `ProfessionalWaterSupplier` | Boolean flag on the join record | `pws => pws.HasFogInspection` |

#### Optional Filter Predicate Pattern

Thread an `Expression<Func<T, bool>>? filter = null` parameter through all four layers so controllers can inject a type-specific predicate without adding new method names.

**Repository interface:**
```csharp
Task<IEnumerable<ProfessionalUserLicense>> GetAllByProfessionalAsync(
    int professionalId, PageInfo pageInfo, Query query,
    CancellationToken cancellationToken,
    Expression<Func<ProfessionalUserLicense, bool>>? filter = null);
```

**Repository implementation:**
```csharp
public async Task<IEnumerable<ProfessionalUserLicense>> GetAllByProfessionalAsync(
    int professionalId, PageInfo pageInfo, Query query,
    CancellationToken cancellationToken,
    Expression<Func<ProfessionalUserLicense, bool>>? filter = null)
{
    var q = DbContext.ProfessionalUserLicenses
        .AsNoTracking()
        .Include(l => l.LicenseType)
        .Include(l => l.User)
        .Where(l => l.ProfessionalId == professionalId);

    if (filter != null)
        q = q.Where(filter);

    var paginated = await q
        .Where(query.Filter)
        .OrderBy(query.Sort)
        .PaginateAsync(pageInfo, cancellationToken);
    return await paginated.ToListAsync(cancellationToken);
}
```

**Service interface:**
```csharp
Task<IPagedData<ProfessionalUserLicenseDto>> GetAllByProfessionalAsync(
    int professionalId, PageInfo pageInfo, Query query,
    CancellationToken cancellationToken,
    Expression<Func<ProfessionalUserLicense, bool>>? filter = null);
```

**Service implementation** — pass `filter` straight through to the repository.

**Controller** — inject the predicate at call site:
```csharp
// FOG inspector licenses — only FOG licenses
var result = await _licenseService.GetAllByProfessionalAsync(
    id, pageInfo, query, cancellationToken,
    l => l.ProfessionalType == ProfessionalType.FogInspector);

// FOG inspector water suppliers — only suppliers with FOG inspection active
var result = await _supplierService.GetAllByProfessionalAsync(
    id, pageInfo, query, cancellationToken,
    pws => pws.HasFogInspection);
```

### Professional vs ProfessionalUser — Entity Responsibilities

`Professional` = the **organisation** (company name, address, EIN, etc.).
`ProfessionalUser` = a **person** who belongs to that organisation (name, email, role, etc.).

**Never put person-specific fields (`ContactName`, personal email, etc.) on `Professional`** — they belong on `ProfessionalUser`. This differs from V1 where `FogInspector`/`BackflowTester` records combined both organisation and contact person data in one row.

### Enums
- Place in `Data/Models/{Domain}/` matching entity domain
- Always assign explicit integer values starting at 0
- EF Core stores enums as `int` — **no migration needed** when changing `int` property to enum type with matching values

### DTOs
- Place in `Domain/DataTransferObjects/{Domain}/`
- Implement `IDto` interface
- Use same enum types as entity (not int)
- `Id` property maps to `WaterSupplierId`
- For referenced navigation properties use `ReferencedWaterSupplierDto` or `ReferencedProfessionalDto`
- For state references use `ReferencedStateDto?` (from `Domain/DataTransferObjects/Lookup/`)
- For `IAuditableModel` entities, include audit fields at the bottom matching `CsiInspectionDto`:
```csharp
// Audit
public DateTime CreatedTime { get; set; }
public DateTime? UpdatedTime { get; set; }
public AppUserDto? UpdatedBy { get; set; }
```

### AutoMapper Profiles
- Place in `Domain/Mapping/{Domain}/`
- Only configure non-convention mappings (WaterSupplierId ↔ Id)
- When mapping navigation properties, use `AfterMap` with `??=` to fall back to FK-only when nav prop not loaded:
```csharp
CreateMap<ProfessionalWaterSupplier, ProfessionalWaterSupplierDto>()
    .AfterMap((model, dto) =>
    {
        dto.WaterSupplier ??= new() { Id = model.WaterSupplierId };
    });
```
- **Important**: `AfterMap` fallback only sets `Id` — `Name` stays null if the nav property is not eagerly loaded. Always use `GetListQuery()` override with `.Include()` in the repository to load nav properties.
- **IMPORTANT — ReverseMap navigation ignores**: Every navigation property on the entity that has no direct DTO counterpart (or uses a different type) must be explicitly ignored in `ReverseMap`. This includes states, sites, user references, and BPAT references:
```csharp
.ReverseMap()
.ForMember(m => m.Site, opt => opt.Ignore())
.ForMember(m => m.SiteId, opt => opt.MapFrom(dto => dto.Site != null ? dto.Site.Id : (int?)null))
.ForMember(m => m.Bpat, opt => opt.Ignore())           // ProfessionalUser nav
.ForMember(m => m.ApprovedBy, opt => opt.Ignore())      // WaterSupplierUser nav
.ForMember(m => m.RejectedBy, opt => opt.Ignore())      // WaterSupplierUser nav
.ForMember(m => m.PropertyState, opt => opt.Ignore())
.ForMember(m => m.PropertyStateId, opt => opt.MapFrom(dto => dto.PropertyState != null ? dto.PropertyState.Id : null))
```

### Repositories
- Interface in `Data/Repositories/Definitions/{Domain}/`
- Implementation in `Data/Repositories/Implementations/{Domain}/`
- Override `GetListQuery()` and `GetDetailsQuery()` to eager-load navigation properties via `.Include()`
- Always call `base.GetListQuery()` (not `Entity.AsNoTracking()` directly) to inherit base behavior
- **IMPORTANT**: When overriding `GetListQuery()` with `.Include()`, also override `GetAllAsync` to set a default sort — otherwise the `DeveloperPartners.SortingFiltering` library throws `InvalidCastException` when sort is empty (it casts `IIncludableQueryable` to `IOrderedQueryable`):

```csharp
protected override IQueryable<ProfessionalWaterSupplier> GetListQuery()
{
    return base.GetListQuery().Include(pws => pws.WaterSupplier);
}

protected override IQueryable<ProfessionalWaterSupplier> GetDetailsQuery()
{
    return base.GetDetailsQuery().Include(pws => pws.WaterSupplier);
}

public override Task<IEnumerable<ProfessionalWaterSupplier>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
{
    if (query.Sort.IsNullOrEmpty())
    {
        query.Sort[nameof(ProfessionalWaterSupplier.WaterSupplierId)] = SortOperator.Asc;
    }
    return base.GetAllAsync(pageInfo, query, cancellationToken);
}
```

- Register both interface and implementation in `Data/Configuration/ServiceRegistration.cs`

### Computed Status Fields on DTOs

When a feature needs date-based or rule-based display status (e.g. overdue / due soon / upcoming), compute it on the backend as an **enum field on the DTO** — never leave that logic in Angular.

**Pattern:**
```csharp
// 1. Enum in Data/Models/{Domain}/
public enum SiteLogReviewDateStatus { None = 0, Overdue = 1, DueSoon = 2, Upcoming = 3, Completed = 4 }

// 2. Field on DTO
public SiteLogReviewDateStatus ReviewDateStatus { get; set; }

// 3. Private static helper — receive `now` as parameter so all rows share the same timestamp
private static SiteLogReviewDateStatus ComputeReviewDateStatus(SiteLogDto dto, DateTime now)
{
    if (!dto.ReviewDate.HasValue) return SiteLogReviewDateStatus.None;
    if (dto.LogType == SiteLogType.CompletedReminder) return SiteLogReviewDateStatus.Completed;
    if (dto.ReviewDate.Value < now) return SiteLogReviewDateStatus.Overdue;
    if (dto.ReviewDate.Value <= now.AddDays(30)) return SiteLogReviewDateStatus.DueSoon;
    return SiteLogReviewDateStatus.Upcoming;
}

// 4. In service — call for every return path (list, add, update)
var now = DateTime.UtcNow;
foreach (var dto in dtos) dto.ReviewDateStatus = ComputeReviewDateStatus(dto, now);
// ... and after add/update:
result.ReviewDateStatus = ComputeReviewDateStatus(result, DateTime.UtcNow);
```

### File Storage (Azure Blob)

`IFileStorageService` methods:
- `UploadAsync(filePath, stream)` — uploads and **overwrites** if blob already exists (uses `BlobUploadOptions` with no conditions). No explicit overwrite flag needed.
- `DeleteAsync(blobPath)` — deletes the blob
- `GenerateSasUrlAsync(path)` / `GenerateSasUrlAsync(delegationKey, path)` — generates a short-lived read URL

**Update pattern — reuse existing path, never delete + re-upload with new GUID:**
```csharp
// ✓ Correct — overwrite in place
filePath = existing.FileAttachmentPath ?? $"site-logs/{entity.SiteId}/{Guid.NewGuid()}{ext}";
existing.FileAttachmentName = Path.GetFileName(fileName);
existing.FileAttachmentPath = filePath;
// then UploadAsync(filePath, stream) — overwrites the existing blob
```

```csharp
// ✗ Wrong — delete + new GUID is unnecessary and wastes a round-trip
await _fileStorageService.DeleteAsync(existing.FileAttachmentPath);
filePath = $"site-logs/{entity.SiteId}/{Guid.NewGuid()}{ext}"; // new GUID is wasteful
```

This matches V1, which always overwrites the same file path rather than generating a new one.

**SAS URL — generate after save:**
- In list methods: call `GetUserDelegationKeyAsync()` **once** before the loop, then pass the key to `GenerateSasUrlAsync(key, path)` inside the loop.
- In add/update methods (single record): call `GenerateSasUrlAsync(path)` after the transaction completes and assign to `dto.Url` before returning.

### EF Migrations
- Two DbContexts exist — always add `--context TenantDbContext` flag:
  - Run: `dotnet ef migrations add {MigrationName} --project src/Envirotrax.App/Envirotrax.App.Server --context TenantDbContext`
  - Update: `dotnet ef database update --project src/Envirotrax.App/Envirotrax.App.Server --context TenantDbContext`
- `int` columns can hold enum values — no schema change needed when adding enum types
- **Azure SQL schema workaround**: The dev DB user `gegham.gevorgyan@developerpartners.com` has a non-`dbo` default schema. Running `dotnet ef database update` fails with `"The specified schema name ... either does not exist or you do not have permission"`. Fix:
  1. Temporarily add `Schema = "dbo"` to the entity's `[Table]` attribute: `[Table("BackflowTests", Schema = "dbo")]`
  2. Re-add the migration (delete old one first if not yet applied)
  3. Run `dotnet ef database update`
  4. Remove `Schema = "dbo"` from the entity — leave only `[Table("BackflowTests")]`
  5. The snapshot entry should read `b.ToTable("BackflowTests")` (no schema). If it shows `"dbo"` in snapshot, remove it manually.

### DbContext (ServiceRegistration.cs)
```csharp
options.UseSqlServer(
    configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException(),
    sqlOptions => sqlOptions.EnableRetryOnFailure());
```
Always include `EnableRetryOnFailure()` for Azure SQL transient fault handling.

### JSON Serialization
- Enums are serialized as integers by default (System.Text.Json)
- This matches the Angular TypeScript enum numeric values

## Workflow for New Feature

1. Create entity in `Data/Models/{Domain}/`
2. Create any enums in same folder
3. Register entity `DbSet` in `TenantDbContext`
4. Create DTO in `Domain/DataTransferObjects/{Domain}/`
5. Create AutoMapper profile in `Domain/Mapping/{Domain}/`
6. Create repository interface + implementation (with `GetListQuery` + default sort override if using `.Include()`)
7. Create service interface + implementation
8. Create controller (extend `ProfessionalProtectedController` for professional endpoints)
9. Register repo + service in `ServiceRegistration.cs`
10. Add EF migration if schema changes (not needed for enum type changes)
