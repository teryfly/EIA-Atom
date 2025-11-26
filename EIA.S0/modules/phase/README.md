# PhaseDefinition Module

## Overview

The PhaseDefinition module manages global workflow phases, including:

- CRUD for phases (`phaseCode`, `displayName`, `order`, `allowedTransitions`, `properties`).
- Validation of `phaseCode` uniqueness and `allowedTransitions`.
- Event publishing to Kafka (`governance.phase.changed.v1` + `governance.cache.refresh.v1`).
- PhaseCache updates on write.
- Participation in full-sync (SyncService).

Authoritative references:

- S0-领域模型类图与数据库表结构
- S0 - 需求规格说明书 (SRS)
- S0 - REST API
- S0 - Governance BC 消息队列与事件驱动架构规范
- 消息队列组件技术文档 (VZhen.MessageQueue)
- Master Architecture Specification v1.0 (Phase 1.1)
- ASP.NET Core DDD 开发规范
- `/mnt/data/需求、设计、技术文档（7个，以---开始...---结束分割）.txt`

## REST API

Base path: `/api/governance/phase`

- `GET /api/governance/phase?page=1&size=50`
- `GET /api/governance/phase/{id}`
- `POST /api/governance/phase`
- `PUT /api/governance/phase/{id}`
- `DELETE /api/governance/phase/{id}`

OpenAPI fragment: `docs/api/governance/phase.yaml`.

## Business Rules

1. `phaseCode` must be globally unique.
2. `allowedTransitions` must contain only existing phase codes.
3. `order` is an integer ordering field; duplicates are currently allowed but consumers should not rely on that.
4. Delete behavior:
   - Physical delete is allowed by default.
   - Future integration with `system.disablePhaseDeletion` will block deletes when configured.
   - Before delete, service will check whether any DocType references the phase (DocType.AllowedPhaseCodes). Current implementation leaves this check as a future enhancement.
5. All timestamps are UTC.

## Events

### governance.phase.changed.v1

- Envelope: see `docs/events/phase_changed.json`.
- Payload fields:
  - `phaseId` (UUID string)
  - `phaseCode`
  - `displayName`
  - `isActive` (bool, currently always `true`)
  - `operationType` ∈ {`CREATED`, `UPDATED`, `DELETED`, `SYNC_FULL`}

### governance.cache.refresh.v1

- Internal cache refresh trigger.
- Payload: `{ "reason": "PhaseChanged", "originInstance": "<machine-name>" }`.

## Cache

- Local in-memory cache (`PhaseCache`) indexed by `id` and `phaseCode`.
- Updated immediately after create/update/delete.
- Cross-instance invalidation via `governance.cache.refresh.v1`.

## Sync

- `SyncService.GetAllPhasesAsync` returns all PhaseDefinition entries, so full-sync flows can broadcast current phase state.

## Running Tests

From repository root:
