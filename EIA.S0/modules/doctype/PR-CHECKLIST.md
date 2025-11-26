# DocType Module PR Checklist

Before merging a PR that touches the **DocType** module, ensure:

## API & Contracts

- [ ] `docs/api/governance/doctype.yaml` is updated if the API surface changed.
- [ ] Swagger/OpenAPI validation passes (e.g., `swagger-cli validate docs/api/governance/doctype.yaml`).
- [ ] DocType DTOs in `src/EIA.S0.Contracts/Dtos/DocTypeDtos.cs` are aligned with the OpenAPI spec.

## Persistence & Migrations

- [ ] A migration for `doc_type` table exists under `migrations/` and matches the domain model (includes `allowed_phases jsonb`).
- [ ] `DocTypeEntityTypeConfiguration` matches the schema (column names, types, indexes).
- [ ] `DocTypeRepository` changes are covered by unit/integration tests verifying `allowed_phases` mapping.

## Application Logic & Validation

- [ ] `DocTypeService` enforces:
  - [ ] `allowedPhases` is non-empty.
  - [ ] Every `allowedPhases` entry exists as a `PhaseDefinition.phaseCode`.
  - [ ] `defaultPhase` is contained in `allowedPhases`.
- [ ] Delete semantics (physical delete) are still correct and documented.
- [ ] Any new validation rules are covered by unit tests under `tests/EIA.S0.Application.Tests/DocTypes`.

## Events & Cache

- [ ] `governance.doctype.changed.v1` is published on create/update/delete with correct envelope and payload.
- [ ] `governance.cache.refresh.v1` is published after DocType changes with `reason = "DocTypeChanged"`.
- [ ] `DocTypeCache` is updated on create/update and cleared on delete.
- [ ] Event and cache behavior are covered by tests (e.g., event publish and cache contents asserted).

## Sync Integration

- [ ] `SyncService.GetAllDocTypesAsync` remains correct after changes (DocTypes are included in full-sync).
- [ ] Any sync-related changes are reflected in docs if behavior changes.

## Web API & E2E Tests

- [ ] `DocTypeController` CRUD endpoints remain consistent with contracts and behavior.
- [ ] Integration/E2E tests under `tests/EIA.S0.WebApi.Tests/DocTypes` pass (create → get → update → get → delete → get(404) if implemented).
- [ ] Error responses for validation errors are correctly mapped (HTTP 400, appropriate error messages/codes).

## Documentation

- [ ] `modules/doctype/README.md` is updated to reflect new rules, endpoints, or behavior.
- [ ] `modules/doctype/contract-tests.md` remains accurate (event samples, API validation commands).
- [ ] References to source documents (SRS, REST API, MQ spec, etc.) remain valid.

## Operational & Security

- [ ] Kafka and DB configurations are not hard-coded; sample values in `appsettings.Development.json` are appropriate for local/dev only.
- [ ] No secrets (passwords, keys) are committed in new configuration files.
- [ ] Logging and observability for DocType changes are adequate (no sensitive data logged).

## CI

- [ ] `dotnet test` passes for all affected projects.
- [ ] Any new or modified tests are deterministic and suitable for CI environments.