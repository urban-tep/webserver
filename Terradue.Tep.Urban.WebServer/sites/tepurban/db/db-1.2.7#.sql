USE $MAIN$;

-- Update config...\
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('calvalusApi-baseUrl', 'string', 'calvalusApi-baseUrl', 'calvalusApi-baseUrl', 'http://www.brockmann-consult.de/calrestdev/api', '1');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('calvalusApi-username', 'string', 'calvalusApi-username', 'calvalusApi-username', '', '1');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('calvalusApi-password', 'string', 'calvalusApi-password', 'calvalusApi-password', '', '1');
-- RESULT


