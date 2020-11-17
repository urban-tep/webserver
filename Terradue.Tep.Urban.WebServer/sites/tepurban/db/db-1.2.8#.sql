USE $MAIN$;

-- Update usr_auth...\
UPDATE usr_auth SET username = (SELECT email FROM usr WHERE id = usr_auth.id_usr) WHERE id_auth=(SELECT id FROM auth WHERE identifier='umsso');
-- RESULT



