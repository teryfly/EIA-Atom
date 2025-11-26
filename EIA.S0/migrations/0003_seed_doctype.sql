-- Seed initial DocType data for integration / DB tests
-- Depends on DRAFT and REVIEW phases seeded in 0002_seed_phase_definition.sql

INSERT INTO doc_type (
    id,
    code,
    name,
    description,
    allowed_phases,
    default_phase,
    category_id,
    ai_draft_prompt_template_id,
    metadata,
    custom_fields,
    created_at,
    updated_at
) VALUES
    (
        '33333333-3333-3333-3333-333333333333',
        'DOC_INTEGRATION',
        'Integration Test DocType',
        'Seeded DocType for integration tests',
        '["DRAFT","REVIEW"]'::jsonb,
        'DRAFT',
        NULL,
        NULL,
        '{"origin":"seed","module":"doctype"}'::jsonb,
        '{"sampleField":123}'::jsonb,
        NOW(),
        NOW()
    )
ON CONFLICT (code) DO NOTHING;