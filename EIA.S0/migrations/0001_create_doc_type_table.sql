-- Migration: create doc_type table for DocType aggregate
-- This SQL is aligned with "S0-领域模型类图与数据库表结构" and Phase 1.2 Coding Task.

CREATE TABLE IF NOT EXISTS doc_type (
    id                          uuid            PRIMARY KEY,
    code                        varchar(64)     NOT NULL UNIQUE,
    name                        varchar(128)    NOT NULL,
    description                 text            NULL,
    allowed_phases              jsonb           NOT NULL,
    default_phase               varchar(64)     NOT NULL,
    category_id                 uuid            NULL,
    ai_draft_prompt_template_id uuid            NULL,
    metadata                    jsonb           NULL,
    custom_fields               jsonb           NULL,
    created_at                  timestamp       NOT NULL,
    updated_at                  timestamp       NOT NULL
);

-- Indexes to support lookups by code, category, and default phase
CREATE INDEX IF NOT EXISTS idx_doctype_code
    ON doc_type (code);

CREATE INDEX IF NOT EXISTS idx_doctype_category
    ON doc_type (category_id);

CREATE INDEX IF NOT EXISTS idx_doctype_defaultphase
    ON doc_type (default_phase);