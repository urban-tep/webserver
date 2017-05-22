USE $MAIN$;

-- update communities and roles ... \
INSERT INTO domain (identifier, name, description, kind, icon_url, discuss) VALUES ('utep','UTEP consortium','The UTEP consortium community gather all parters users.',3,'https://urban-tep.eo.esa.int/styles/img/icons/logo_tep_urban.png', 'urban/co-utep');
INSERT INTO domain (identifier, name, description, kind, icon_url, discuss) VALUES ('worldbank','World Bank','World bank users.',4,'https://pbs.twimg.com/profile_images/657619924836327424/eS6-JUwh.png','urban/co-worldbank');

SET @role_id = (SELECT id FROM role WHERE identifier='starter');
DELETE FROM rolegrant WHERE id_role=@role_id;
SET @role_id = (SELECT id FROM role WHERE identifier='explorer');
DELETE FROM rolegrant WHERE id_role=@role_id;
-- RESULT

-- update services ...\
SET @domain_id = (SELECT id FROM domain WHERE identifier='utep');
UPDATE service SET id_domain=@domain_id, tags='subsetting' WHERE identifier IN ('4201e8b5-3ee8-4ae3-a896-6c84e433c434','e4c46987-5d8f-4218-9356-9cf9a34e9e27','496eaeba-b773-4090-89f7-0f79ce2f154c','fd20bc19-1ce5-446a-bfd5-8cef54350f9f');
-- RESULT
