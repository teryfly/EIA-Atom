-- Seed initial PhaseDefinition data for integration / DB tests

INSERT INTO phase_definition (
    id,
    phase_code,
    display_name,
    "order",
    allowed_transitions,
    properties,
    created_at,
    updated_at
) VALUES
    (
        '11111111-1111-1111-1111-111111111111',
        'DRAFT',
        'Draft',
        1,
        '["REVIEW"]'::jsonb,
        '{"level":"draft"}'::jsonb,
        NOW(),
        NOW()
    ),
    (
        '22222222-2222-2222-2222-222222222222',
        'REVIEW',
        'Review',
        2,
        '[]'::jsonb,
        '{"level":"review"}'::jsonb,
        NOW(),
        NOW()
    )
ON CONFLICT (phase_code) DO NOTHING;