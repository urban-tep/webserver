USE $MAIN$;

-- Add config...\
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('sso-eosso-secret', 'string', 'Eosso secret password', 'Eosso secret password', 'TBD', '0');
UPDATE config SET value='https://www.terradue.com/t2api/eosso/user' WHERE name='t2portal-usr-endpoint';
-- RESULT