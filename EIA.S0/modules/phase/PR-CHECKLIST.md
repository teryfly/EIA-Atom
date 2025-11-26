# Phase Module PR Checklist

Before merging a PR touching the PhaseDefinition module, ensure:

- [ ] `docs/api/governance/phase.yaml` updated (if API changed) and `swagger-cli validate` passes.
- [ ] `migrations/*_create_phase_definition_table.sql` present and aligned with domain model.
- [ ] `PhaseDefinition` entity and EF mapping updated consistently.
- [ ] `PhaseRepository` changes tested (unit/integration where applicable).
- [ ] `PhaseService` business rules covered by unit tests:
  - [ ] phaseCode uniqueness
  - [ ] allowedTransitions validation
  - [ ] delete behavior and referential checks (when implemented)
- [ ] Event publishing (`governance.phase.changed.v1`, `governance.cache.refresh.v1`) verified via tests.
- [ ] `PhaseCache` behavior verified (updates on create/update/delete).
- [ ] `SyncService` includes PhaseDefinition in full-sync.
- [ ] Web API (`PhaseController`) integration tests pass (create → get → update → get → delete → get(404)).
- [ ] `docs/events/phase_changed.json` kept in sync with payload.
- [ ] `modules/phase/README.md` updated to reflect any behavioral or rule changes.
- [ ] `dotnet test` passes for all affected test projects.