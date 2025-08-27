CREATE EXTENSION IF NOT EXISTS pgcrypto;
       
INSERT INTO company (name, api_token_hash, is_active)
VALUES
    ('Acme Corp',                digest('token_company_001','sha256'), true),
    ('Globex Ltd',               digest('token_company_002','sha256'), true),
    ('Initech',                  digest('token_company_003','sha256'), true),
    ('Umbrella PLC',             digest('token_company_004','sha256'), true),
    ('Stark Industries',         digest('token_company_005','sha256'), true),
    ('Wayne Enterprises',        digest('token_company_006','sha256'), true),
    ('Wonka Factory',            digest('token_company_007','sha256'), true),
    ('Tyrell Corp',              digest('token_company_008','sha256'), true),
    ('Cyberdyne Systems',        digest('token_company_009','sha256'), true),
    ('Hooli',                    digest('token_company_010','sha256'), true),
    ('Aperture Science',         digest('token_company_011','sha256'), true),
    ('Black Mesa',               digest('token_company_012','sha256'), true),
    ('Vandelay Industries',      digest('token_company_013','sha256'), true),
    ('Pied Piper',               digest('token_company_014','sha256'), true),
    ('Soylent Co',               digest('token_company_015','sha256'), true),
    ('Oscorp',                   digest('token_company_016','sha256'), true),
    ('Monsters Inc',             digest('token_company_017','sha256'), true),
    ('Paper Street Soap Co',     digest('token_company_018','sha256'), true),
    ('Massive Dynamic',          digest('token_company_019','sha256'), true),
    ('Sterling Cooper',          digest('token_company_020','sha256'), true)
    ON CONFLICT (name) DO NOTHING;
