# DocType Module

## Overview

The **DocType** module manages global document type definitions for S0 Governance BC, including:

- CRUD operations for DocTypes:
  - `code`, `name`, `description`
  - `allowedPhases` (phase codes)
  - `defaultPhase`
  - `categoryId`
  - `aiDraftPromptTemplateId`
  - `metadata`, `customFields`
- Validation of relationships to **PhaseDefinition**:
  - `allowedPhases` must reference existing `PhaseDefinition.phaseCode` values.
  - `defaultPhase` must be contained in `allowedPhases`.
- Event publication:
  - `governance.doctype.changed.v1` on create/update/delete.
  - `governance.cache.refresh.v1` after changes (internal cache invalidation).
- Local in-memory cache:
  - `DocTypeCache` keyed by `id` and `code`.
- Participation in full-sync:
  - `SyncService.GetAllDocTypesAsync` returns all DocType entries for sync broadcasts.

Authoritative reference documents:

- **S0-领域模型类图与数据库表结构**
- **S0 - 需求规格说明书 (SRS)**
- **S0 - REST API**
- **S0 - Governance BC 消息队列与事件驱动架构规范 (Kafka 版)**
- **消息队列组件技术文档 (VZhen.MessageQueue)**
- **Master Architecture Specification v1.0 (Phase 1.1)**
- **ASP.NET Core DDD 开发规范**
- Local bundle path:  
  `file:///mnt/data/需求、设计、技术文档（7个，以---开始...---结束分割）.txt`

## REST API

Base path: `/api/governance/doctype`

Endpoints (as per `docs/api/governance/doctype.yaml`):

- `GET /api/governance/doctype?page=1&size=50`
- `GET /api/governance/doctype/{id}`
- `POST /api/governance/doctype`
- `PUT /api/governance/doctype/{id}`
- `DELETE /api/governance/doctype/{id}`

### Paging

- `page` (default: 1, min: 1)
- `size` (default: 50, min: 1, max: 200)

The controller currently implements simple in-memory paging over repository results ordered by `code`.

## Business Rules

1. **DocType code**
   - Must be non-empty.
   - Duplicate codes are not allowed (future enhancement will attach explicit error code 40004).

2. **allowedPhases**
   - Must not be empty.
   - All items must reference existing `PhaseDefinition.phaseCode` entries.
   - Service validates this via `IRepository<PhaseDefinition>`; if a code does not exist, a `DomainException` is thrown and mapped to HTTP 400.

3. **defaultPhase**
   - Must be included in `allowedPhases`.
   - Violations result in a `DomainException`.

4. **Delete semantics**
   - `DELETE /api/governance/doctype/{id}` performs **physical delete** of DocType records.
   - After delete, a `governance.doctype.changed.v1` event with `operationType = "DELETED"` is published.
   - This behavior follows user decision B=NO (no forced soft-delete). If downstream requires soft-delete later, behavior will be documented and flagged.

5. **Timestamps**
   - All `created_at` / `updated_at` timestamps are stored as UTC in the database (handled by EF Core value converters).

6. **AI Prompt Template linkage**
   - `aiDraftPromptTemplateId` is reserved for linking to PromptTemplate.
   - Full referential integrity check will be implemented when the PromptTemplate module is available.

## Events

(内容同前略)

## Cache

(内容同前略)

## Sync

(内容同前略)

## Running Tests

From repository root:
