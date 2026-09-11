---
name: envirotrax-angular-feature
description: Use when implementing new Angular features in Envirotrax2 — components, modules, routing, forms, services, and models. Knows all UI component conventions, the settings tab pattern, vp-input usage, and toast notifications.
tools: Read, Write, Edit, Glob, Grep, Bash
---

You are an Angular feature implementation specialist for the Envirotrax2 client app.

## Project Structure

```
src/app/
  admin/
    home/                        → Admin home (menu tiles)
    settings/
      general/                   → General Settings & Fees component
      csi-settings/              → CSI Settings component
      settings.module.ts         → Declares all settings components
      settings-routing.module.ts → Routes for /admin/settings/*
    water-suppliers/
    users/
    admin.module.ts
    admin-routing.module.ts      → Lazy-loads feature modules
  professionals/
    sites/                       → Professional site search component
    professional.module.ts
    professional-routing.module.ts
  shared/
    components/                  → vp-input, vp-section, vp-filter-panel, vp-filter-panel-field, vp-validation-summary, etc.
    models/
      settings/                  → CsiSettings, GeneralSettings interfaces + enums
    services/
      settings/                  → CsiSettingsService, GeneralSettingsService
      professionals/             → ProfessionalSupplierService (getMyAsOptions for water supplier dropdowns)
      toast.service.ts
      auth/auth.service.ts
    guards/
```

## Critical Conventions

### Adding a New Settings Page
1. Create component folder: `admin/settings/{feature-name}/`
2. Create `{feature-name}.component.ts` and `{feature-name}.component.html`
3. Declare component in `settings/settings.module.ts`
4. Add route to `settings/settings-routing.module.ts`
5. Add tile to `admin/home/home.component.ts` with `routerLink: ['settings', '{feature-name}']`
6. **Do NOT create a separate Angular module** — all settings share `SettingsModule`

### Routing Pattern
```typescript
// settings-routing.module.ts
{
    path: 'csi-settings',
    title: 'CSI Settings',
    component: CsiSettingsComponent
}
// Route resolves to: /admin/settings/csi-settings
```

### Home Tile Pattern
```typescript
// home.component.ts
{
    title: 'CSI Settings',
    iconCss: 'fa-solid fa-magnifying-glass-chart',
    routerLink: ['settings', 'csi-settings'],
    hasPermission: true,  // or: await this._authService.hasAnyPermisison(...)
    description: 'Description text here.'
}
```

### Navigation Tabs (Settings Pages)
Always add nav tabs at the top of settings pages to allow navigation between them:
```html
<ul class="nav nav-tabs mb-3">
    <li class="nav-item">
        <a class="nav-link" [routerLink]="['/admin/settings/general']">General Settings & Fees</a>
    </li>
    <li class="nav-item">
        <a class="nav-link active" [routerLink]="['/admin/settings/csi-settings']">CSI Settings</a>
    </li>
</ul>
```
Add this component style to fix the global nav-link color override:
```typescript
styles: [`
    .nav-tabs .nav-link { color: var(--bs-body-color) !important; }
    .nav-tabs .nav-link.active { color: var(--bs-body-color) !important; }
`]
```

### Label and Header Casing Convention
Section headers (`header=`) use **title case**. Field labels (`label=`) use **sentence case** — only the first word and acronyms are capitalised:
```html
<!-- ✓ Correct — title case for section headers -->
<vp-section header="Search Criteria">
<vp-section header="Recent CSI Inspections">

<!-- ✓ Correct — sentence case for labels -->
<vp-filter-panel-field label="Account number" ...>
<vp-filter-panel-field label="BPAT license number" ...>   <!-- acronym keeps caps -->
<vp-filter-panel-field label="OSSF" ...>                  <!-- acronym keeps caps -->

<!-- ✗ Wrong — sentence case on section headers -->
<vp-section header="Search criteria">
<!-- ✗ Wrong — title case on labels -->
<vp-filter-panel-field label="Account Number" ...>
```

### vp-input Component
The `vp-input` component replaces raw `<input>` and `<select>` elements for consistent styling.

**Text/number input:**
```html
<vp-input [(ngModel)]="model.field"
          name="field"
          label="Field Label"
          [required]="true"
          [form]="myForm" />
```

**Select/dropdown (options must use `{ id, text }` format):**
```html
<vp-input type="select"
          label="My Field"
          [(ngModel)]="model.field"
          name="field"
          [options]="myOptions" />
```
```typescript
// Options array must use { id, text } — NOT { value, label }
public readonly myOptions = [
    { id: MyEnum.Off, text: 'Off' },
    { id: MyEnum.Option1, text: 'Option 1' },
];
```

**Supported types:** `text`, `number`, `date`, `datetime`, `daterange`, `textarea`, `select`, `multi-select`, `email`

### vp-filter-panel and vp-filter-panel-field Components
Use these for search/filter forms instead of raw `vp-input`. They automatically emit `QueryProperty[]` on change via `(filterChange)`.

```html
<vp-filter-panel (filterChange)="onFilterChange($event)">
    <vp-section header="Search criteria">
        <div class="row">
            <div class="col-md-6 mb-3">
                <vp-filter-panel-field fieldName="accountNumber"
                                       label="Account number"
                                       [form]="searchForm" />
            </div>
            <div class="col-md-6 mb-3">
                <vp-filter-panel-field fieldName="propertyType"
                                       label="Property type"
                                       type="select"
                                       [options]="propertyTypes"
                                       [form]="searchForm" />
            </div>
        </div>
    </vp-section>
</vp-filter-panel>
```

**Supported inputs on `vp-filter-panel-field`:**
- `fieldName: string` — maps to backend column name (camelCase)
- `label: string` — display label
- `type: 'text' | 'number' | 'date' | 'daterange' | 'select' | 'multi-select'` — defaults to `'text'`
- `[options]` — required when `type="select"` or `type="multi-select"`, use `{ id, text }` format with string IDs
- `[required]="true"` — marks field as required, shows validation asterisk
- `[form]="searchForm"` — enables validation display
- `borderColor` — colors the dropdown border to visually group status filters. Conventions:
  - `borderColor="#00a000"` — green, for pass/active/current status
  - `borderColor="orange"` — out-of-service / warning status
  - `borderColor="cyan"` — approval status
  - `borderColor="red"` — rejected / failed status

**IMPORTANT — select option IDs must be strings:**
```typescript
// For boolean yes/no dropdowns:
public yesNoOptions: InputOption[] = [
    { id: "", text: "Any Value" },
    { id: "true", text: "Yes" },
    { id: "false", text: "No" }
];

// For enum dropdowns (single-select — include empty first option):
// ✓ Always use the existing PropertyType enum — never hardcode '0' / '1'
public propertyTypeOptions: InputOption[] = [
    { id: "", text: "Any Value" },
    { id: PropertyType.Residential.toString(), text: "Residential" },
    { id: PropertyType.Commercial.toString(), text: "Commercial" }
];

// For enum dropdowns (multi-select — NO empty first option, selecting nothing means "any"):
public facilityTypeOptions: InputOption[] = [
    { id: FacilityType.Other.toString(), text: "Other" },
    { id: FacilityType.Restaurant.toString(), text: "Restaurant" },
    // ...
];
```
IDs must be strings (not numbers or booleans) because `convertFilterPanelFields` in `QueryHelperService` checks `typeof value === 'string'` and silently drops non-string values. For numeric enum dropdowns use `.toString()`. For string-valued fields (like interceptor types stored as strings in the DB), create a **string enum** in `shared/enums/` and use the enum value directly as the id (no `.toString()` needed):
```typescript
// shared/enums/interceptor-type.enum.ts
export enum InterceptorType {
    GreaseTrap = 'Grease Trap',
    GritTrap = 'Grit Trap',
    SepticTank = 'Septic Tank',
    ChemicalToilet = 'Chemical Toilet',
    Other = 'Other'
}

// In component:
public readonly interceptorTypeOptions: InputOption[] = [
    { id: '', text: 'Any type' },
    { id: InterceptorType.GreaseTrap, text: 'Grease Trap' },
    { id: InterceptorType.GritTrap, text: 'Grit Trap' },
    // ...
];
```

**Multi-select emits OR children to the backend.** `QueryHelperService` converts a `string[]` value into a single `QueryProperty` with `children` (each child has `logicalOperator: 'Or'`). The `DeveloperPartners.SortingFiltering` library handles these children natively through `.Where(query.Filter)` — **no special backend extraction is needed** for fields that exist directly on the entity/DTO. Special extraction is only needed when the field is on a navigation property not present on the DTO (see backend conventions).

**Component TypeScript:**
```typescript
public onFilterChange(queryProperties: QueryProperty[]): void {
    this.table.query.filter = queryProperties;
}
```

### Water Supplier Dropdown for Professionals
When a professional needs to select a water supplier, use `ProfessionalSupplierService.getMyAsOptions()`. This returns `InputOption[]` with string IDs, ready for use in `vp-filter-panel-field` or `vp-input`.

```typescript
// In ngOnInit:
this.waterSupplierOptions = await this._proSupplierService.getMyAsOptions();
```

```html
<vp-filter-panel-field fieldName="waterSupplierId"
                       label="Water supplier"
                       type="select"
                       [form]="searchForm"
                       [options]="waterSupplierOptions"
                       [required]="true" />
```

Do NOT convert `ProfessionalWaterSupplier[]` to `InputOption[]` in the component — always use `getMyAsOptions()` from the service so the logic is reusable.

### vp-section Component
```html
<vp-section header="Section title">
    <!-- content -->
</vp-section>
```

**Header action buttons** — project them through the **`<vp-section-actions>`** component (a direct child of `<vp-section>`). The old `vpSectionHeaderActions` attribute slot was removed — do NOT use it. Use `[collapsible]="false"` to hide the collapse chevron (e.g. for card-style sections, one per list row):
```html
<!-- ✓ Header action + non-collapsible card section -->
<vp-section [header]="request.header" [collapsible]="false" [noPadding]="true">
    <vp-section-actions>
        <button type="button" class="btn btn-danger btn-sm" (click)="clear(request)">Clear</button>
    </vp-section-actions>
    <!-- body content -->
</vp-section>
```
For a plain "Add" button on a normal collapsible section, placing it at the top of the section body is also fine (keeps a cramped header clean on mobile):
```html
<vp-section header="Property Log History">
    <div *ngIf="canModify" class="mb-2">
        <button type="button" class="btn btn-primary btn-sm" (click)="openAddModal()">Add Item</button>
    </div>
    <!-- table -->
</vp-section>
```

**Section spacing** — consecutive `vp-section` blocks must be wrapped in `<div class="mt-3">` to create vertical spacing. For two sections placed side-by-side, use `<div class="row mt-3">` with `col-md-6` on each:
```html
<!-- Stacked sections -->
<vp-section header="Search criteria"> ... </vp-section>

<div class="mt-3">
    <vp-section header="BPAT criteria"> ... </vp-section>
</div>

<!-- Side-by-side sections -->
<div class="row mt-3">
    <div class="col-md-6">
        <vp-section header="Location / property criteria"> ... </vp-section>
    </div>
    <div class="col-md-6 mt-3 mt-md-0">
        <vp-section header="Contact / mailing criteria"> ... </vp-section>
    </div>
</div>
```

### vp-validation-summary
```html
<vp-validation-summary [form]="myForm" [validationErrors]="validationErrors" />
```

### Template-Driven Forms Pattern
```typescript
public myModel: MyModel = { /* defaults */ };
public isLoading: boolean = false;
public validationErrors: string[] = [];

public async save(form: NgForm): Promise<void> {
    if (form.valid) {
        try {
            this.isLoading = true;
            let result;
            if (this.myModel.id) {
                result = await this._myService.update(this.myModel);
            } else {
                result = await this._myService.add(this.myModel);
            }
            if (result) { this.myModel = result; }
            this._toastService.successfullySaved('My Feature');
        } catch (error) {
            if (!this._helper.parseValidationErrors(error, this.validationErrors)) {
                throw error;
            }
            this._toastService.failedToSave('My Feature');
        } finally {
            this.isLoading = false;
        }
    }
}
```

### Loading Spinner Pattern
```html
<div>
    <dp-loading-spinner *ngIf="isLoading" />
    <div dp-spinner-parent>
        <!-- content -->
    </div>
</div>
```

### Toast Notifications
Always inject `ToastService` and call after save:
```typescript
constructor(private readonly _toastService: ToastService) {}

// On success:
this._toastService.successfullySaved('Feature Name');
// On failure:
this._toastService.failedToSave('Feature Name');
```

### TypeScript Enums for Select Options
Create enums in `shared/models/{domain}/{feature}-enums.ts`:
```typescript
export enum CsiPastDueType {
    Off = 0,
    LastMonth = 1,
    TwoMonthsAgo = 2,
}
```
Update model interface to use enum type:
```typescript
import { CsiPastDueType } from './my-enums';
export interface MyModel {
    pastDueNotice1?: CsiPastDueType;
}
```
Use enum values in options array:
```typescript
public readonly pastDueOptions = [
    { id: CsiPastDueType.Off, text: 'Off' },
    { id: CsiPastDueType.LastMonth, text: 'Past Due Last Month' },
];
```

### Always Use Braces on Control Statements

Always use curly braces for `if`, `else`, `for`, `foreach`, `while`, `do`, and every other control statement — even when the body is a single statement. Never write a one-liner without braces; they can hide bugs (accidental fall-through, misread indentation).

```typescript
// ✓ Correct
if (deviceInfo) {
    label += ` - ${deviceInfo}`;
}

for (const item of items) {
    total += item.value;
}

// ✗ Wrong
if (deviceInfo) label += ` - ${deviceInfo}`;
for (const item of items) total += item.value;
```

This applies to TypeScript and C# alike (`if`, `foreach`, `for`, `while`, …).

### Backend-Computed Status → Static CSS Class Map

When a backend DTO includes a status enum (e.g. `ReviewDateStatus`, overdue/due-soon/upcoming), map it to CSS classes using a `readonly Record` on the component — never recompute dates or business logic in Angular.

```typescript
// ✓ Correct — static readonly Record, declared once
public readonly reviewDateStatusClasses: Record<SiteLogReviewDateStatus, string> = {
    [SiteLogReviewDateStatus.None]: '',
    [SiteLogReviewDateStatus.Overdue]: 'badge bg-danger',
    [SiteLogReviewDateStatus.DueSoon]: 'badge bg-warning text-dark',
    [SiteLogReviewDateStatus.Upcoming]: 'badge bg-success',
    [SiteLogReviewDateStatus.Completed]: 'badge bg-secondary'
};
```

```html
<!-- ✓ Correct — simple property access, no method call -->
<span [class]="reviewDateStatusClasses[log.reviewDateStatus!]">...</span>

<!-- ✗ Wrong — date logic in Angular, called every change-detection cycle -->
<span [class]="getReviewDateClass(log)">...</span>
```

Also create the matching TypeScript enum in `shared/models/{domain}/` with values matching the C# enum.

### Professional Details Page Pattern
Detail pages that load data by route `id` must subscribe to `paramMap` (not use `snapshot`) so they react when the ID changes programmatically. Implement `OnDestroy` to unsubscribe.

```typescript
export class FogInspectorDetailsComponent implements OnInit, OnDestroy {
    public id: number | null = null;
    public accountInfo: Professional | null = null;
    public isAccountLoading: boolean = false;
    public formattedAddress: string = '';          // ← static field, NOT a method

    private _routeSub?: Subscription;

    public ngOnInit(): void {
        this._routeSub = this._activatedRoute.paramMap.subscribe(async params => {
            const idParam = params.get('id');
            this.id = idParam ? Number(idParam) : null;
            if (this.id !== null) {
                await this.loadAccountInfo();
            }
        });
    }

    public ngOnDestroy(): void {
        this._routeSub?.unsubscribe();
    }

    private async loadAccountInfo(): Promise<void> {
        try {
            this.isAccountLoading = true;
            this.accountInfo = await this._accountInfoService.getAccountInfo(this.id!);
            this.formattedAddress = this.buildFormattedAddress();    // ← set once after load
        } finally {
            this.isAccountLoading = false;
        }
    }

    private buildFormattedAddress(): string {
        if (!this.accountInfo) { return ''; }
        return [
            this.accountInfo.address,
            this.accountInfo.city,
            this.accountInfo.state?.name,
            this.accountInfo.zipCode
        ].filter(p => p).join(', ');
    }
}
```

**Rules:**
- **Never use `snapshot.paramMap.get()`** — if the ID changes in-place, the component won't reload.
- **Never call a method from a template for a derived value** — Angular calls it on every change-detection cycle. Compute once into a `public` field and bind to that field instead.
- `formattedAddress` (address display) follows this exact pattern across all professional detail pages (FOG inspector, CSI inspector, Backflow tester).

### Table Column — Nested Navigation Property
When a column needs to display a value from a navigation property (e.g., `waterSupplier.name`), use dot-notation in `field` directly. Do **not** create an `<ng-template>` and `@ViewChild` for this.

```typescript
// ✓ Correct — use dot-notation field
{
    field: 'waterSupplier.name',
    caption: 'Water Supplier',
    type: ColumnType.text
}

// ✗ Wrong — unnecessary cellTemplate for a simple display
@ViewChild('supplierNameCell', { static: true })
private supplierNameCellTemplate!: TemplateRef<CellTemplateData<ProfessionalWaterSupplier>>;

{
    field: 'waterSupplierId',
    caption: 'Water Supplier',
    cellTemplate: this.supplierNameCellTemplate,
    type: ColumnType.text
}
```

Only use `cellTemplate` / `cellComponent` when the cell needs custom rendering beyond displaying a field value (e.g., checkbox, currency, edit button).

### View Models over Template Function Calls (mandatory for tables/lists)
**Never call a component method from a template to produce a display value** — Angular re-invokes it on every change-detection cycle, and in a `vp-table`/`*ngFor` that is once **per row per cycle**. Instead, build a **view model** once, right after the data loads, with every displayed value pre-computed into a plain field, and bind the field.

```typescript
// ✗ Wrong — method called per row, every CD cycle; also forces display-only cellTemplates
interface Row extends BackflowTest {}
buildDescription(t: BackflowTest): string { return `${t.manufacturer} ${t.model} ${t.size} - ${t.deviceType}`; }
// column: { field:'', type: ColumnType.other, cellTemplate: this.descriptionCell }  // <ng-template>{{ buildDescription(row) }}</ng-template>

// ✓ Correct — view model with pre-computed fields, plain field-bound columns (no display cellTemplate)
interface ReplacementCandidateVm { id?: number; testDate?: string; serialNumber?: string; deviceDescription: string; propertyAddress: string; }
this.candidates = results.map(t => ({ id: t.id, testDate: t.testDate, serialNumber: t.serialNumber,
    deviceDescription: this.buildDeviceDescription(t), propertyAddress: this.buildPropertyAddress(t) }));  // built once
// columns bind fields directly:
{ field: 'testDate', caption: 'Test Date', type: ColumnType.date }
{ field: 'deviceDescription', caption: 'Device Description', type: ColumnType.text }
```
A view model **eliminates display-only `cellTemplate`s** (`ColumnType.date`/`ColumnType.text` render the field). Keep a `cellTemplate` only for genuinely interactive/custom cells (a Select/toggle button, checkbox, currency). Applies everywhere, not just detail pages — see also the "Professional Details Page Pattern" rule about computing derived values into fields.

### Confirmation Dialogs — use ModalHelperService (never hand-roll a modal)
For confirmation/message dialogs use the shared `ModalHelperService` from `@envirotrax/common-ui`, so look-and-feel stays centralized. **Do NOT hand-roll a Bootstrap modal** (`*ngIf="showModal"` + `class="modal d-block"` + `rgba(0,0,0,0.5)` backdrop + custom footer buttons) for confirmations.

```typescript
constructor(private readonly _modalHelper: ModalHelperService) {}

// confirm(): OK/Cancel; .result() emits ONLY when confirmed
this._modalHelper.confirm({
    title: 'Confirm Out of Service Request',
    messages: ['This assembly will be marked out of service because it was removed.', `Reason for removal: ${reason}`]
}).result().subscribe(() => this.completeSubmission());
```
- `confirm({ title?, type?, messages: string[] })` → OK/Cancel confirmation; `messages` is the body (one string per line).
- `showMessage({...})` → informational OK-only dialog.
- `showDeleteConfirmation()` / `showReactivateConfirmation()` → preset confirmations.
- `show<TConfig, TResult>(Component, config)` → open a custom component as a modal (for edit forms / rich content).
Build the confirmation summary as a `string[]` in TypeScript; don't recreate structured modal HTML.

### Standalone vp-input for Behavioral Flags (not filter fields)
When a control drives component behavior rather than sending a value to the backend filter (e.g. `latestOnly`, `inspectionHistory`), use a standalone `vp-input` outside `vp-filter-panel` with `[ngModelOptions]="{standalone: true}"`. Do NOT use `vp-filter-panel-field` for this — that would send the value as a `QueryProperty` to the backend filter.

```html
<!-- ✓ Correct — standalone, drives component property -->
<label class="form-label">Inspection history:</label>
<vp-input type="select"
          [options]="inspectionHistoryOptions"
          [ngModel]="latestOnly ? 'true' : 'false'"
          (ngModelChange)="onLatestOnlyChange($event)"
          [ngModelOptions]="{standalone: true}" />

<!-- ✗ Wrong — sends inspectionHistory as a QueryProperty filter to backend -->
<vp-filter-panel-field fieldName="inspectionHistory"
                       label="Inspection history"
                       type="select"
                       [options]="inspectionHistoryOptions"
                       [form]="searchForm" />
```

The component property and handler:
```typescript
public latestOnly: boolean = true;

public onLatestOnlyChange(value: string): void {
    this.latestOnly = value === 'true';
}
```

The service then passes `latestOnly` as a URL query param (not in the filter body):
```typescript
public getAll(pageInfo: PageInfo, query: Query, latestOnly: boolean): Promise<PagedData<MyModel>> {
    const url = this._urlResolver.resolveUrl('/api/professionals/my/endpoint');
    let params = this._queryHelper.buildQuery(pageInfo, query);
    params = params.append('latestOnly', String(latestOnly));
    return lastValueFrom(this._http.get<PagedData<MyModel>>(url, { params }));
}
```

### Date Range Pattern for Search Forms
Show both `inspectionDate` and `createdTime` (submission date) as **always-visible** daterange fields. Do NOT use a "Date to search" type-switcher dropdown with a single conditional date field — that is a V1 pattern deprecated in V2.

```html
<!-- ✓ Correct V2 pattern — both always visible -->
<div class="mb-3">
    <vp-filter-panel-field fieldName="inspectionDate"
                           label="Inspection date"
                           type="daterange"
                           [form]="searchForm" />
</div>
<div class="mb-3">
    <vp-filter-panel-field fieldName="createdTime"
                           label="Submission date"
                           type="daterange"
                           [form]="searchForm" />
</div>

<!-- ✗ Wrong V1 pattern — switcher + conditional visibility -->
<vp-input type="select" [(ngModel)]="inspectionDateType" ... />
<div [hidden]="inspectionDateType !== 'inspectionDate'">
    <vp-filter-panel-field fieldName="inspectionDate" type="daterange" ... />
</div>
```

### Angular Service Pattern
```typescript
@Injectable({ providedIn: 'root' })
export class MyService {
    constructor(
        private readonly _http: HttpClient,
        private readonly _urlResolver: UrlResolverService,
        private readonly _queryHelper: QueryHelperService
    ) {}

    public getAll(pageInfo: PageInfo, query: Query): Promise<PagedData<MyModel>> {
        const url = this._urlResolver.resolveUrl('/api/my-endpoint');
        return lastValueFrom(this._http.get<PagedData<MyModel>>(url, {
            params: this._queryHelper.buildQuery(pageInfo, query)
        }));
    }
}
```

### Module Registration
```typescript
// settings.module.ts
@NgModule({
    declarations: [
        GeneralSettingsComponent,
        CsiSettingsComponent,
        NewFeatureComponent  // ← add here
    ],
    imports: [CommonModule, FormsModule, RouterModule, SharedComponentsModule, SettingsRoutingModule]
})
export class SettingsModule {}
```

### Professional Routes
Professional routes require `RoleDefinitions.Professional` (base role). Use `ROLE_DEFINITIONS.PROFESSIONAL` — do NOT list all sub-roles individually on child routes. The parent `professionals` route in `app-routing-module.ts` already guards with the base role.

```typescript
// professional-routing.module.ts
{
    path: 'sites',
    title: 'Property Records',
    component: SiteListComponent,
    canActivate: [RoleGuard],
    data: {
        roles: [ROLE_DEFINITIONS.PROFESSIONAL]
    }
}
```

## Checklist for New Settings Feature

- [ ] Component files in `admin/settings/{feature-name}/`
- [ ] Declared in `settings.module.ts`
- [ ] Route added to `settings-routing.module.ts`
- [ ] Home tile added to `home.component.ts` with correct `routerLink`
- [ ] Nav tabs at top of HTML with color fix in component styles
- [ ] Form uses `NgForm` + `if (form.valid)` guard
- [ ] Loading spinner wraps content
- [ ] Toast notifications on save success and failure
- [ ] `vp-input` used instead of raw `<select>`/`<input>`
- [ ] Enum options use `{ id, text }` format
- [ ] TypeScript enum file created in shared models
- [ ] Model interface updated to use enum types

### TypeScript Model Conventions
- Model interface properties for enum fields must use the TypeScript enum type, not `number`:
```typescript
import { BackflowTestResult, BackflowReasonForTest } from './backflow-test-enums';
export interface BackflowTest {
    testResult?: BackflowTestResult;      // ✓ typed enum
    reasonForTest?: BackflowReasonForTest; // ✓ typed enum
    // NOT: testResult?: number            // ✗ raw number
}
```
- FK references in model interfaces must use the existing TypeScript model type — **never inline object shapes**:
```typescript
import { Professional } from '../professionals/professional';
import { ProfessionalUser } from '../professionals/professional-user';
import { Site } from '../sites/site';
import { State } from '../lookup/state';
import { WaterSupplierUser } from '../users/water-supplier-user';

export interface BackflowTest {
    site?: Site | null;                   // ✓ typed model
    professional?: Professional | null;   // ✓ typed model
    bpat?: ProfessionalUser | null;       // ✓ typed model
    propertyState?: State | null;         // ✓ typed model
    approvedBy?: WaterSupplierUser | null; // ✓ typed model
    // NOT: propertyState?: { id?: number; name?: string; code?: string } | null;  // ✗ inline shape
}
```
- Free-text fields from `nvarchar(max)` columns (`comments`, `rejectedReason`) use `string?`

### PermissionGuard for Water Supplier Routes
Water supplier routes use `PermissionGuard` (not `RoleGuard`):
```typescript
{
    path: 'tests',
    component: BackflowTestListComponent,
    canActivate: [PermissionGuard],
    data: {
        permissions: [{ type: PermissionType.BackflowTests, action: PermissionAction.CanView }]
    }
}
```

## Checklist for New Water Supplier Search Feature

- [ ] Component in `{domain}/{feature-name}/` (e.g., `backflow/tests/`)
- [ ] Declared in the domain module (e.g., `backflow.module.ts`)
- [ ] Route added to domain routing module with `canActivate: [PermissionGuard]` and `PermissionType`
- [ ] Both nav links in `app.ts` updated (water supplier menu ~line 207 AND professional menu ~line 542)
- [ ] Search form wrapped in `<form #searchForm="ngForm" (ngSubmit)="search(searchForm)">`
- [ ] `<vp-filter-panel (filterChange)="onFilterChange($event)">` wraps all filter sections
- [ ] All filter fields use `<vp-filter-panel-field>` — NOT raw `<vp-input>`
- [ ] Select option IDs are strings (`"0"`, `"true"`, `""`) — NOT numbers or booleans
- [ ] Status dropdowns use `borderColor` attribute with appropriate color convention
- [ ] Multiple `vp-section` blocks wrapped in `<div class="mt-3">` for spacing
- [ ] Side-by-side sections use `<div class="row mt-3">` + `col-md-6`
- [ ] `onFilterChange` sets `this.table.query.filter = queryProperties`
- [ ] Results hidden with `[hidden]="showResults"`, shown with `*ngIf="showResults"`
- [ ] "Search Again" button sets `showResults = false`
- [ ] `dp-loading-spinner` placed after the form div
- [ ] TypeScript enum file in `shared/models/{domain}/`
- [ ] Model interface uses enum types (not `number`) for enum fields
- [ ] Model interface uses existing TypeScript model types (not inline shapes) for FK references — `State | null`, `Professional | null`, `WaterSupplierUser | null`, etc.
- [ ] No create/edit buttons (read-only search view)
- [ ] Component does NOT define a `selector` (routed components don't need one)

## Checklist for New Professional Search Feature

- [ ] Component in `professionals/{feature-name}/`
- [ ] Declared in `professional.module.ts`
- [ ] Route added to `professional-routing.module.ts` with `roles: [ROLE_DEFINITIONS.PROFESSIONAL]`
- [ ] Nav link in `app.ts` points to `['professionals/{feature-name}']` (NOT `['{feature-name}']`)
- [ ] Form uses `<form #searchForm="ngForm" (ngSubmit)="search()">` (no form argument to `search()`)
- [ ] Search form wrapped in `<vp-filter-panel (filterChange)="onFilterChange($event)">`
- [ ] All filter fields use `<vp-filter-panel-field>` — NOT raw `<vp-input>`
- [ ] Behavioral flags (e.g. `latestOnly`) use standalone `vp-input` with `[ngModelOptions]="{standalone: true}"` — NOT `vp-filter-panel-field`
- [ ] Select option IDs are strings — use existing enums (`PropertyType.Residential.toString()`) — NOT hardcoded `"0"` / `"1"`
- [ ] String-valued fields (e.g. interceptor type) use string enums from `shared/enums/` — NOT string literals
- [ ] Water supplier scope dropdown uses `ProfessionalSupplierService.getAllMy(false, false, true)` with the correct feature flag, then maps to options in the component
- [ ] Both date fields (`inspectionDate` + `createdTime`) are always-visible — do NOT use a date type switcher
- [ ] `onFilterChange` sets `this.table.query.filter = queryProperties` (simple assignment, no transform unless mapping lte/gt values)
- [ ] Results shown with `*ngIf="showResults && !table.isLoading"`, search form hidden with `[hidden]="showResults"`
- [ ] Results section header shows item count: `header="... - {{table.items?.pageInfo?.totalItems || 0}} Results"`
- [ ] "No results" alert shown when `searchAttempted && !table.items?.data?.length`
- [ ] `searchAgain()` method resets `showResults = false` and `searchAttempted = false`
- [ ] `dp-loading-spinner` placed after the form div
- [ ] No `borderColor` on inspection result dropdowns (V1 had none)
- [ ] No create/edit buttons in professional read-only views
- [ ] Verify field list against `fog_inspectors/inspection_search.aspx` (professional view), NOT `water_suppliers/fog_inspection_search.aspx` (water supplier view — different fields)
