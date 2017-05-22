USE $MAIN$;

-- update config ... \
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('IT4ISupportEmail', 'string', 'IT4I Support Email', 'IT4I Support Email', 'support.utep@it4i.cz', '1');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('appExternalPostUserLevel', 'string', 'Minimum user level to be able to create external app on POST', 'Minimum user level to be able to create external app on POST', '3', '1');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('catalog-admin-username', 'string', 'Catalog admin user name', 'Catalog admin user name', 'TO_BE_UPDATED', '1');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('catalog-admin-apikey', 'string', 'Catalog admin api key', 'Catalog admin api key', 'TO_BE_UPDATED', '1');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('puma-apps-index', 'string', 'PUMA default apps index', 'PUMA default apps index', 'urban-puma', '1');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('puma-apps-identifier', 'string', 'PUMA default apps identifier', 'PUMA default apps identifier', 'urban-puma', '1');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('puma-apps-title', 'string', 'PUMA default apps title', 'PUMA default apps title', 'Data Analytics Toolbox', '1');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('puma-apps-description', 'string', 'PUMA default apps description', 'PUMA default apps description', 'Data Analytics Toolbox thematic application', '1');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('puma-apps-icon', 'string', 'PUMA default apps icon', 'PUMA default apps icon', 'https://store.terradue.com/urban-apps/images/puma.png', '1');
-- RESULT
