# PhaseDefinition Module - Implementation Plan (Phase 1.3)

This document tracks the detailed implementation steps for the PhaseDefinition module.

## Overall Goals

- Implement full CRUD for PhaseDefinition.
- Enforce business rules (phaseCode uniqueness, allowedTransitions validity, order constraints).
- Integrate with Kafka via IMessageQueue (governance.phase.changed.v1 + governance.cache.refresh.v1).
- Implement PhaseCache and integrate with SyncService.
- Provide REST API, migrations, tests, and documentation.

## Implementation Steps (Mapped to Files / Commands)

1. **API Spec**
   - Create `docs/api/governance/phase.yaml` describing:
     - `GET /api/governance/phase?page&size`
     - `GET /api/governance/phase/{id}`
     - `POST /api/governance/phase`
     - `PUT /api/governance/phase/{id}`
     - `DELETE /api/governance/phase/{id}`
   - Purpose: Define public REST contract for PhaseDefinition.
   - Expected Output: Valid OpenAPI fragment (`swagger-cli validate` passes).

2. **DB Migration SQL**
   - Create `migrations/0001_create_phase_definition_table.sql` (exact file name may be adjusted later).
   - Schema for table `phase_definition`:
     - `id uuid primary key`
     - `phase_code varchar(64) unique not null`
     - `display_name varchar(128) not null`
     - `"order" int not null`
     - `allowed_transitions jsonb not null`
     - `properties jsonb null`
     - `created_at timestamp not null`
     - `updated_at timestamp not null`
     - Index on `(order)`.
   - Purpose: Persist PhaseDefinition in DB aligned with domain model.
   - Expected Output: SQL file that can be applied to PostgreSQL.

3. **Domain Entity - PhaseDefinition**
   - Create `src/EIA.S0.Domain/Governance/Entities/PhaseDefinition.cs`.
   - Fields:
     - `Id` (from AggregateRoot)
     - `PhaseCode`, `DisplayName`, `Order`, `AllowedTransitionPhaseCodes` (List<string>), `PropertiesJson` (string? or JSON-typed), `CreatedAt`, `UpdatedAt`.
   - Methods:
     - Constructor with full arguments.
     - `UpdateBasicInfo(...)`.
   - Purpose: Introduce domain model for PhaseDefinition.
   - Expected Output: Compilable entity with unit tests planned.

4. **EF Core EntityConfiguration for PhaseDefinition**
   - Create `src/EIA.S0.Infrastructure/EntityFrameworkCore/EntityConfigurations/PhaseDefinitionEntityTypeConfiguration.cs`.
   - Map:
     - Table: `phase_definition`
     - `AllowedTransitionPhaseCodes` → `allowed_transitions jsonb`
     - `PropertiesJson` → `properties jsonb`
     - `Order` → `"order"` int
     - Unique index on `phase_code`.
   - Purpose: Map domain entity to DB schema.
   - Expected Output: EF configuration class applied by `EiaS0dbContext`.

5. **Repository - PhaseRepository**
   - Create `src/EIA.S0.Infrastructure/Governance/Repositories/PhaseRepository.cs`.
   - Implement `IRepository<PhaseDefinition>` using pattern similar to `DocTypeRepository`.
   - Additional method:
     - `Task<PhaseDefinition?> GetByCodeAsync(string phaseCode)`.
   - Purpose: Provide persistence access for PhaseDefinition.
   - Expected Output: Working repository with basic CRUD and query by code.

6. **Application Cache - PhaseCache**
   - Create `src/EIA.S0.Application/Governance/Cache/PhaseCache.cs`.
   - Data structures:
     - `ConcurrentDictionary<string, PhaseDefinition>` by `id`.
     - `ConcurrentDictionary<string, PhaseDefinition>` by `phaseCode`.
   - Methods: `GetById`, `GetByCode`, `Set`, `Remove`, `Clear`.
   - Purpose: In-memory read cache for PhaseDefinition.
   - Expected Output: Cache usable by services and Sync.

7. **Application Events DTO - PhaseChangedEvent**
   - Create `src/EIA.S0.Application/Governance/Events/PhaseChangedEvent.cs`.
   - Define:
     - `PhaseChangedEventPayload` with `PhaseId`, `PhaseCode`, `DisplayName`, `IsActive` (bool, default true), `OperationType`.
     - Reuse existing `GovernanceEventEnvelope<T>` (already in DocType events file) or make it generic enough (if needed, move to separate reusable file).
   - Purpose: Strongly typed event contract within application layer.
   - Expected Output: Event payload types for publishing.

8. **Application Service - PhaseService (core logic)**
   - Create `src/EIA.S0.Application/Governance/Phases/PhaseService.cs` (partial if required).
   - Dependencies:
     - `IRepository<PhaseDefinition>`
     - `IRepository<DocType>` (for delete reference check)
     - `IUnitOfWork`
     - `TimeProvider`
   - Methods:
     - `Task<PhaseDefinition> CreateAsync(...)`
     - `Task<PhaseDefinition> UpdateAsync(string id, ...)`
     - `Task<PhaseDefinition?> GetAsync(string id)`
     - `Task<IReadOnlyCollection<PhaseDefinition>> GetListAsync(int page, int size)`
     - `Task<bool> DeleteAsync(string id, bool force = false, CancellationToken cancellationToken = default)`
   - Business rules enforced:
     - `phaseCode` uniqueness (check via repo; throw DomainException with message for 40004).
     - `order` uniqueness or document if duplicates are allowed.
   - Purpose: Encapsulate core use cases and validations.
   - Expected Output: Service without event/cache integration (handled in separate partial).

9. **Application Service - PhaseService.Events & Cache**
   - Create `src/EIA.S0.Application/Governance/Phases/PhaseService.Events.cs`.
   - Additional dependencies (via constructor):
     - `PhaseCache`
     - `Func<string, object, CancellationToken, Task>` event publish delegate (same pattern as DocTypeService).
   - Responsibilities:
     - After create/update/delete:
       - Update `PhaseCache` (Set/Remove).
       - Publish `governance.phase.changed.v1` with envelope & payload.
       - Publish `governance.cache.refresh.v1` with reason `PhaseChanged`.
   - Purpose: Integrate service with messaging and cache while separating concerns.
   - Expected Output: Fully integrated PhaseService with side effects.

10. **SystemParameter Integration for Delete Rules**
    - Extend `PhaseService` to depend on a lightweight abstraction for SystemParameter (e.g., an interface or a simple service stub for now).
    - Implement behavioral check for `system.disablePhaseDeletion`:
      - If true, reject delete with DomainException.
    - Implement DocType reference check:
      - If any existing DocType has `AllowedPhaseCodes` containing `phaseCode`, reject delete unless `force` (behavior documented in README).
    - Purpose: Respect global settings and referential integrity.
    - Expected Output: Delete behavior wired to parameters and DocType references.

11. **SyncService Enhancement to Include Phases**
    - Update `src/EIA.S0.Application/Governance/Sync/SyncService.cs`:
      - Inject `IRepository<PhaseDefinition>`.
      - Add method `Task<IReadOnlyCollection<PhaseDefinition>> GetAllPhasesAsync(IQuerySpecification<PhaseDefinition> spec)`.
    - Purpose: Make phases available for full-sync.
    - Expected Output: SyncService now capable of returning DocTypes + Phases.

12. **Web DI - Infrastructure Wiring for Phase Module**
    - Update `src/EIA.S0.WebApi/Extensions/InfrastructureExtensions.cs`:
      - Register `IRepository<PhaseDefinition>, PhaseRepository`.
      - Ensure `PhaseCache` is registered (singleton).
      - Configure `PhaseService` using the same pattern as `DocTypeService` for injecting `EventPublisher`-based delegate.
    - Purpose: Wire up services and repositories for runtime use.
    - Expected Output: DI container aware of Phase module components.

13. **Contracts - Phase DTOs**
    - Create DTOs in `src/EIA.S0.Contracts/Dtos`:
      - `PhaseDto` (response).
      - `CreatePhaseRequest`, `CreatePhaseResponse`.
      - `UpdatePhaseRequest`, `UpdatePhaseResponse`.
      - `DeletePhaseResponse`.
    - Purpose: Stable API contracts for Phase REST endpoints.
    - Expected Output: DTO classes used by Web API controllers.

14. **Web API - PhaseController**
    - Create `src/EIA.S0.WebApi/Controllers/Governance/PhaseController.cs`.
    - Endpoints:
      - `GET /api/governance/phase?page&size` → list (PhaseDto[]).
      - `GET /api/governance/phase/{id}` → single PhaseDto or 404.
      - `POST /api/governance/phase` → CreatePhaseResponse (201).
      - `PUT /api/governance/phase/{id}` → UpdatePhaseResponse.
      - `DELETE /api/governance/phase/{id}` → DeletePhaseResponse or conflict/forbidden based on rules.
    - Map DomainException to 400 using existing middleware; for duplicate code map to `40004` error message text (documented).
    - Purpose: Expose PhaseDefinition operations via HTTP.
    - Expected Output: Fully functional controller mapped to PhaseService and DTOs.

15. **Docs & Events Samples**
    - Create `docs/events/phase_changed.json`:
      - Example envelope for `governance.phase.changed.v1`.
    - Create `modules/phase/README.md`:
      - Describe business rules, API usage, delete behavior, allowedTransitions rules, sync behavior.
      - Include how to run tests and how to verify events.
    - Purpose: Provide consumer-facing documentation and contracts.
    - Expected Output: Markdown + JSON doc files.

16. **Unit Tests - Domain & Repository**
    - New tests under `tests/EIA.S0.Domain.Tests/Phases/PhaseDefinitionEntityTests.cs`:
      - Verify construction, AllowedTransitionPhaseCodes behavior, JSON-serialization of selected fields.
    - New tests under `tests/EIA.S0.Infrastructure.Tests/Phases/PhaseRepositoryTests.cs`:
      - Using in-memory EF Core to ensure `allowed_transitions` round-trips.
    - Purpose: Validate domain and persistence mapping.
    - Expected Output: Passing unit tests for entity & repository.

17. **Unit Tests - PhaseService Business Rules**
    - New tests under `tests/EIA.S0.Application.Tests/Phases/PhaseServiceTests.cs`:
      - Create with unique phaseCode (success).
      - Create with duplicate phaseCode (throws DomainException for duplicate).
      - Invalid allowedTransitions referencing non-existing phases (exception).
      - Order behavior (e.g., duplicates allowed or rejected based on chosen rule).
      - Delete rules with system parameter & DocType reference mocks.
    - Purpose: Cover main business rules at service level.
    - Expected Output: Green tests ensuring validation logic.

18. **Integration Tests - API E2E**
    - New tests under `tests/EIA.S0.WebApi.Tests/Phases/PhaseControllerTests.cs`:
      - Use `CustomApplicationFactory`.
      - Scenario: create → get → update → get → delete → get (404).
      - Use in-memory DB (UnitTest environment) and mock IMessageQueue to avoid Kafka.
    - Purpose: Verify full HTTP pipeline for PhaseDefinition.
    - Expected Output: Stable end-to-end tests.

19. **Integration Tests - Event Publishing & Cache**
    - New tests under appropriate project (`tests/EIA.S0.Application.Tests` or `tests/EIA.S0.WebApi.Tests`) to verify:
      - `PhaseService` calls event publish delegate with envelope for `governance.phase.changed.v1`.
      - `PhaseCache` updated after create/update/delete.
      - `governance.cache.refresh.v1` published.
    - Purpose: Ensure messaging and caching behavior meets spec.
    - Expected Output: Tests confirming calls and cache states.

20. **PR Checklist Document**
    - Create `modules/phase/PR-CHECKLIST.md`:
      - Items: migrations updated, unit tests, integration tests, docs, events, API spec updated, SystemParameter rules covered.
    - Purpose: Operational checklist for reviews.
    - Expected Output: Simple Markdown checklist used during PR.
