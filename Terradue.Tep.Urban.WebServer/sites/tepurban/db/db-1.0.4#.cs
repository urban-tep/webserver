USE $MAIN$;

/*****************************************************************************/

-- update table config ... \
INSERT INTO config (name, id_section, pos, internal, type, caption, hint, value, optional) VALUES ('gisat_statusUrl', NULL, NULL, '0', 'string', 'GISAT statut url to which the identifier is added', 'Enter the GISAT statut url to which the identifier is added', 'http://urban-tep.gisat.cz/tool/integration/status?id=', '1');
INSERT INTO config (name, id_section, pos, internal, type, caption, hint, value, optional) VALUES ('gisat_processUrl', NULL, NULL, '0', 'string', 'GISAT process url', 'Enter the GISAT statut url', 'http://urban-tep.gisat.cz/tool/integration/process', '1');
-- RESULT
