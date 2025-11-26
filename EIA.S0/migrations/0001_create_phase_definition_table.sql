-- PhaseDefinition table creation script
-- Aligns with S0-领域模型类图与数据库表结构

CREATE TABLE IF NOT EXISTS phase_definition
(
    id                  uuid PRIMARY KEY,
    phase_code          varchar(64)  NOT NULL,
    display_name        varchar(128) NOT NULL,
    "order"             integer      NOT NULL,
    allowed_transitions jsonb        NOT NULL,
    properties          jsonb        NULL,
    created_at          timestamp    NOT NULL,
    updated_at          timestamp    NOT NULL
);

-- Ensure phase_code uniqueness
CREATE UNIQUE INDEX IF NOT EXISTS ux_phase_definition_phase_code
    ON phase_definition (phase_code);

-- Optional index on order for ordering queries
CREATE INDEX IF NOT EXISTS idx_phase_definition_order
    ON phase_definition ("order");