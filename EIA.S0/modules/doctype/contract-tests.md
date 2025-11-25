# DocType Module Contract Tests

This document describes how to verify DocType contracts for downstream consumers.

## Event Contract

Sample `governance.doctype.changed.v1` envelope:

- `docs/events/doctype_changed.json`

Validate that:

- `eventType` = `governance.doctype.changed.v1`
- `payload.docTypeId` is UUID
- `payload.docTypeCode` matches created/updated DocType code
- `payload.operationType` ∈ {`CREATED`, `UPDATED`, `DELETED`, `SYNC_FULL`}

## API Contract

Validate the DocType OpenAPI fragment:

```bash
swagger-cli validate docs/api/governance/doctype.yaml
```

## Test Commands

To run unit and integration tests once implemented:

```bash
dotnet test
```

Further environment-specific commands (e.g., Docker/Kafka setup) will be documented
as the messaging and cache integration is completed.
