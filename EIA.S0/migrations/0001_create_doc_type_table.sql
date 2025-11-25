CREATE TABLE IF NOT EXISTS doc_type (
    id                  UUID            PRIMARY KEY,
    code                VARCHAR(64)     NOT NULL UNIQUE,
    name                VARCHAR(128)    NOT NULL,
    description         TEXT            NULL,
    allowed_phases      JSONB           NOT NULL,
    default_phase       VARCHAR(64)     NOT NULL,
    category_id         UUID            NULL,
    ai_draft_prompt_template_id UUID    NULL,
    metadata            JSONB           NULL,
    custom_fields       JSONB           NULL,
    created_at          TIMESTAMP       NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC'),
    updated_at          TIMESTAMP       NOT NULL DEFAULT (NOW() AT TIME ZONE 'UTC')
);

CREATE INDEX IF NOT EXISTS idx_doctype_code ON doc_type(code);
CREATE INDEX IF NOT EXISTS idx_doctype_category ON doc_type(category_id);
CREATE INDEX IF NOT EXISTS idx_doctype_defaultphase ON doc_type(default_phase);