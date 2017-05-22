USE $MAIN$;

-- alter tables ... \
ALTER TABLE wpsprovider CHANGE COLUMN `url` `url` VARCHAR(200) NULL DEFAULT NULL COMMENT 'Base WPS access point' ;
-- RESULT
