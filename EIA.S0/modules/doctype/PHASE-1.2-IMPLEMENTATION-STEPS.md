# Phase 1.2 – DocType Module Implementation Steps

This file lists all implementation steps required to bring the DocType module to full completion per **Coding Task Document – Phase 1.2**.

Each step corresponds to a concrete code file or command and will be applied sequentially.

---

## Step 1 – Add DocType SQL migration

**Goal:** Ensure a concrete SQL migration for `doc_type` table exists under `migrations/` and matches schema in S0 docs (including `allowed_phases jsonb`).

**Actions:**

- Create `migrations/0001_create_doc_type_table.sql` with:
  - `id uuid primary key`
  - `code varchar(64) unique not null`
  - `name varchar(128) not null`
  - `description text null`
  - `allowed_phases jsonb not null`
  - `default_phase varchar(64) not null`
  - `category_id uuid null`
  - `ai_draft_prompt_template_id uuid null`
  - `metadata jsonb null`
  - `custom_fields jsonb null`
  - `created_at timestamp not null`
  - `updated_at timestamp not null`
  - indexes on `(code)`, `(category_id)`, `(default_phase)`.

**Expected Output:** SQL file ready to apply to PostgreSQL.

---

## Step 2 – Extend DocTypeService with PhaseDefinition existence validation

**Goal:** Implement validation rule that every code in `allowedPhases` exists in `PhaseDefinition` table.

**Actions:**

- Update `DocTypeService` to:
  - Inject `IRepository<PhaseDefinition>` via constructors (core + events partial).
  - Implement `EnsureAllAllowedPhasesExistAsync` to:
    - For each distinct phase code in `allowedPhases`, query `PhaseDefinition` (AnyAsync or repository query).
    - Throw `DomainException("allowedPhases 中的阶段编码[...]不存在.")` or similar on missing codes.

**Expected Output:** Updated `DocTypeService` (both partials) with fully implemented allowedPhases validation.

---

## Step 3 – Add unit tests for allowedPhases existence validation

**Goal:** Verify new validation behavior in `DocTypeService`.

**Actions:**

- Update/extend `tests/EIA.S0.Application.Tests/DocTypes/DocTypeServiceTests.cs`:
  - Positive test: existing PhaseDefinitions for all allowedPhases → CreateDocTypeAsync succeeds.
  - Negative test: at least one allowedPhases code missing → CreateDocTypeAsync throws `DomainException`.
  - Mirror tests for Update if needed.

**Expected Output:** Tests covering allowedPhases existence rule, all passing.

---

## Step 4 – Define DocType DTO contracts

**Goal:** Introduce DTOs for DocType requests/responses that match `docs/api/governance/doctype.yaml`.

**Actions:**

- Create `src/EIA.S0.Contracts/Dtos/DocTypeDtos.cs` with:
  - `DocTypeDto`
  - `CreateDocTypeRequest`, `CreateDocTypeResponse`
  - `UpdateDocTypeRequest`, `UpdateDocTypeResponse`
  - `DeleteDocTypeResponse`
- Align fields with OpenAPI spec:
  - `code`, `name`, `description`, `allowedPhases`, `defaultPhase`, `categoryId`, `aiDraftPromptTemplateId`, `metadata`, `customFields`.

**Expected Output:** DTOs ready for controller use and Swagger generation.

---

## Step 5 – Implement DocTypeController CRUD endpoints

**Goal:** Provide full API surface for DocType as per spec.

**Actions:**

- Update `src/EIA.S0.WebApi/Controllers/Governance/DocTypeController.cs` so it:
  - Injects `DocTypeService`.
  - Implements:
    - `GET /api/governance/doctype` with `page`, `size`.
    - `GET /api/governance/doctype/{id}` returning 404 if not found.
    - `POST /api/governance/doctype` returning `201 Created` with `CreateDocTypeResponse`.
    - `PUT /api/governance/doctype/{id}` returning `UpdateDocTypeResponse`.
    - `DELETE /api/governance/doctype/{id}` returning `DeleteDocTypeResponse`.
  - Maps between domain `DocType` and `DocTypeDto`.
  - Uses JSON-serialized `metadata/customFields` properties.

**Expected Output:** Fully functional DocTypeController aligned with `doctype.yaml`.

---

## Step 6 – Add DocTypeService list method

**Goal:** Support list endpoint with basic paging.

**Actions:**

- Extend `DocTypeService` with:
  - `Task<IReadOnlyCollection<DocType>> ListDocTypesAsync(int page, int size, IQuerySpecification<DocType> spec)`
    - Use repository `GetListAsync` with an “all” spec, then apply `OrderBy(Code)` and `Skip/Take`.
- Use this method from `DocTypeController.GetListAsync`.

**Expected Output:** Service listing method and working paging semantics.

---

## Step 7 – Add DocType repository/jsonb mapping integration test

**Goal:** Validate `allowed_phases` JSONB mapping end-to-end.

**Actions:**

- Update `tests/EIA.S0.Infrastructure.Tests/DocTypes/DocTypeRepositoryTests.cs` to:
  - Spin up an in-memory or Npgsql-backed `EiaS0dbContext` (in-memory is acceptable for mapping assertions; JSONB type can be simulated).
  - Use `DocTypeRepository` to add a DocType with `AllowedPhaseCodes = ["P1","P2"]`.
  - Save changes and re-query; assert list contents and ordering of `AllowedPhaseCodes`.

**Expected Output:** Non-placeholder test confirming allowed_phases mapping.

---

## Step 8 – Implement DocType event publish tests via mocked EventPublisher/IMessageQueue

**Goal:** Ensure `governance.doctype.changed.v1` is published with correct payload.

**Actions:**

- Add/extend tests under `tests/EIA.S0.Application.Tests/DocTypes/DocTypeServiceTests.cs`:
  - Use a mock delegate or fake `EventPublisher`/`IMessageQueue` to capture topic and envelope.
  - For create/update/delete, assert:
    - Topic = `governance.doctype.changed.v1`.
    - Envelope payload contains `DocTypeId`, `DocTypeCode`, correct `OperationType`.

**Expected Output:** Tests confirming event publication semantics.

---

## Step 9 – Implement DocType cache refresh tests

**Goal:** Verify `DocTypeCache` update and cache refresh event.

**Actions:**

- Extend tests to:
  - After create/update, inspect injected `DocTypeCache` to ensure `GetById` and `GetByCode` return the new entity.
  - Confirm that a `governance.cache.refresh.v1` envelope with `reason = "DocTypeChanged"` is published.

**Expected Output:** Tests asserting cache behavior and cache refresh publishing.

---

## Step 10 – Implement DocType API end-to-end tests

**Goal:** Validate full HTTP pipeline for DocType.

**Actions:**

- Create `tests/EIA.S0.WebApi.Tests/DocTypes/DocTypeControllerTests.cs`:
  - Use `CustomApplicationFactory`.
  - Scenario:
    1. `POST /api/governance/doctype` with a DocType payload including `allowedPhases`.
    2. Check `201 Created` and parse `id`.
    3. `GET /api/governance/doctype/{id}` and assert fields, including `allowedPhases` round-trip.

**Expected Output:** Passing E2E test confirming end-to-end behavior.

---

## Step 11 – Implement error response model and mapping

**Goal:** Provide standardized error codes and HTTP mapping.

**Actions:**

- Create or extend a DTO (e.g., `ErrorResponseDto`) under Contracts.
- Extend `ExceptionHandlerMiddleware` to:
  - Recognize `DomainException` subtypes / message patterns or custom exception type containing error code.
  - Return body with `{ code, message, details }`.
- Update DocTypeService and/or controller to throw structured exceptions with codes for:
  - Duplicate code (`40004 DUPLICATE_CODE`).
  - Phase not found (`40002 PHASE_NOT_FOUND`), etc.

**Expected Output:** Consistent error response model and tests verifying mapping.

---

## Step 12 – Add DocType-specific logging

**Goal:** Enhance observability for DocType operations.

**Actions:**

- Inject `ILogger<DocTypeService>` into `DocTypeService` and log at key points:
  - Start/end of create/update/delete.
  - Validation failures.
  - Event publish failures (if surfaced).
- Optionally add logs in `DocTypeController` for HTTP-level actions.

**Expected Output:** DocType-specific structured logs without breaking tests.

---

## Step 13 – Implement PromptTemplate referential integrity stub (optional for now)

**Goal:** Prepare ground for future PromptTemplate integration.

**Actions:**

- Add an interface, e.g., `IPromptTemplateReadService` or repository once PromptTemplate module exists.
- Implement a simple stub or TODO-based check for `aiDraftPromptTemplateId`, clearly documented.
- Do not break compilation; tests can be added later when PromptTemplate is available.

**Expected Output:** Clear extension point for prompt template checks, documented in README.

---

## Step 14 – Enhance modules/doctype/README.md

**Goal:** Provide a complete module README as per Coding Task.

**Actions:**

- Update `modules/doctype/README.md` to include:
  - Module responsibilities and overview.
  - Detailed API endpoints and behavior.
  - Business rules:
    - allowedPhases refer to existing phases.
    - defaultPhase in allowedPhases.
    - Delete semantics (physical delete).
    - Events published.
  - Test commands (`dotnet test`) and how to run integration tests.
  - Reference to doc bundle path: `file:///mnt/data/需求、设计、技术文档（7个，以---开始...---结束分割）.txt`.

**Expected Output:** Comprehensive README for DocType consumers and maintainers.

---

## Step 15 – Add DocType PR checklist

**Goal:** Ensure consistent review for DocType-related changes.

**Actions:**

- Create `modules/doctype/PR-CHECKLIST.md` similar to Phase module checklist, including:
  - Migration present and aligned.
  - DocTypeService tests updated.
  - Event contracts verified.
  - Cache behavior.
  - API tests passing.
  - README updated.

**Expected Output:** PR checklist file for DocType module.
