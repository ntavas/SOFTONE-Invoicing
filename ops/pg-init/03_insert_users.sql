INSERT INTO users (company_id, display_name, email)
SELECT c.company_id, 'Acme User', 'acme@example.com'
FROM company c
WHERE c.name = 'Acme Corp'
  AND NOT EXISTS (SELECT 1 FROM users u WHERE u.email = 'acme@example.com');

INSERT INTO users (company_id, display_name, email)
SELECT c.company_id, 'Globex User', 'globex@example.com'
FROM company c
WHERE c.name = 'Globex Ltd'
  AND NOT EXISTS (SELECT 1 FROM users u WHERE u.email = 'globex@example.com');

INSERT INTO users (company_id, display_name, email)
SELECT c.company_id, 'Initech User', 'initech@example.com'
FROM company c
WHERE c.name = 'Initech'
  AND NOT EXISTS (SELECT 1 FROM users u WHERE u.email = 'initech@example.com');

INSERT INTO users (company_id, display_name, email)
SELECT c.company_id, 'Umbrella User', 'umbrella@example.com'
FROM company c
WHERE c.name = 'Umbrella PLC'
  AND NOT EXISTS (SELECT 1 FROM users u WHERE u.email = 'umbrella@example.com');

INSERT INTO users (company_id, display_name, email)
SELECT c.company_id, 'Stark User', 'stark@example.com'
FROM company c
WHERE c.name = 'Stark Industries'
  AND NOT EXISTS (SELECT 1 FROM users u WHERE u.email = 'stark@example.com');

INSERT INTO users (company_id, display_name, email)
SELECT c.company_id, 'Wayne User', 'wayne@example.com'
FROM company c
WHERE c.name = 'Wayne Enterprises'
  AND NOT EXISTS (SELECT 1 FROM users u WHERE u.email = 'wayne@example.com');

INSERT INTO users (company_id, display_name, email)
SELECT c.company_id, 'Wonka User', 'wonka@example.com'
FROM company c
WHERE c.name = 'Wonka Factory'
  AND NOT EXISTS (SELECT 1 FROM users u WHERE u.email = 'wonka@example.com');

INSERT INTO users (company_id, display_name, email)
SELECT c.company_id, 'Tyrell User', 'tyrell@example.com'
FROM company c
WHERE c.name = 'Tyrell Corp'
  AND NOT EXISTS (SELECT 1 FROM users u WHERE u.email = 'tyrell@example.com');

INSERT INTO users (company_id, display_name, email)
SELECT c.company_id, 'Cyberdyne User', 'cyberdyne@example.com'
FROM company c
WHERE c.name = 'Cyberdyne Systems'
  AND NOT EXISTS (SELECT 1 FROM users u WHERE u.email = 'cyberdyne@example.com');

INSERT INTO users (company_id, display_name, email)
SELECT c.company_id, 'Hooli User', 'hooli@example.com'
FROM company c
WHERE c.name = 'Hooli'
  AND NOT EXISTS (SELECT 1 FROM users u WHERE u.email = 'hooli@example.com');

INSERT INTO users (company_id, display_name, email)
SELECT c.company_id, 'Aperture User', 'aperture@example.com'
FROM company c
WHERE c.name = 'Aperture Science'
  AND NOT EXISTS (SELECT 1 FROM users u WHERE u.email = 'aperture@example.com');

INSERT INTO users (company_id, display_name, email)
SELECT c.company_id, 'Black Mesa User', 'blackmesa@example.com'
FROM company c
WHERE c.name = 'Black Mesa'
  AND NOT EXISTS (SELECT 1 FROM users u WHERE u.email = 'blackmesa@example.com');

INSERT INTO users (company_id, display_name, email)
SELECT c.company_id, 'Vandelay User', 'vandelay@example.com'
FROM company c
WHERE c.name = 'Vandelay Industries'
  AND NOT EXISTS (SELECT 1 FROM users u WHERE u.email = 'vandelay@example.com');

INSERT INTO users (company_id, display_name, email)
SELECT c.company_id, 'Pied Piper User', 'piedpiper@example.com'
FROM company c
WHERE c.name = 'Pied Piper'
  AND NOT EXISTS (SELECT 1 FROM users u WHERE u.email = 'piedpiper@example.com');

INSERT INTO users (company_id, display_name, email)
SELECT c.company_id, 'Soylent User', 'soylent@example.com'
FROM company c
WHERE c.name = 'Soylent Co'
  AND NOT EXISTS (SELECT 1 FROM users u WHERE u.email = 'soylent@example.com');

INSERT INTO users (company_id, display_name, email)
SELECT c.company_id, 'Oscorp User', 'oscorp@example.com'
FROM company c
WHERE c.name = 'Oscorp'
  AND NOT EXISTS (SELECT 1 FROM users u WHERE u.email = 'oscorp@example.com');

INSERT INTO users (company_id, display_name, email)
SELECT c.company_id, 'Monsters Inc User', 'monstersinc@example.com'
FROM company c
WHERE c.name = 'Monsters Inc'
  AND NOT EXISTS (SELECT 1 FROM users u WHERE u.email = 'monstersinc@example.com');

INSERT INTO users (company_id, display_name, email)
SELECT c.company_id, 'Paper Street User', 'paperstreet@example.com'
FROM company c
WHERE c.name = 'Paper Street Soap Co'
  AND NOT EXISTS (SELECT 1 FROM users u WHERE u.email = 'paperstreet@example.com');

INSERT INTO users (company_id, display_name, email)
SELECT c.company_id, 'Massive Dynamic User', 'massivedynamic@example.com'
FROM company c
WHERE c.name = 'Massive Dynamic'
  AND NOT EXISTS (SELECT 1 FROM users u WHERE u.email = 'massivedynamic@example.com');

INSERT INTO users (company_id, display_name, email)
SELECT c.company_id, 'Sterling Cooper User', 'sterlingcooper@example.com'
FROM company c
WHERE c.name = 'Sterling Cooper'
  AND NOT EXISTS (SELECT 1 FROM users u WHERE u.email = 'sterlingcooper@example.com');
