USE $MAIN$;

-- Update action...\
UPDATE action SET enabled='0' WHERE `identifier`='RefreshWpsjobStatus';
UPDATE action SET enabled='0' WHERE `identifier`='RefreshWpsjobResultNb';
-- RESULT