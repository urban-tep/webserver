
-- VERSION 1.2.1

USE $MAIN$;

-- Initializing tepurban data model ... \ 

-- Adding extended entity types for dataseries... \
CALL add_type($ID$, 'Terradue.TepUrban.Controller.DataSeries, Terradue.TepUrban.WebServer', 'Terradue.Portal.Series, Terradue.Portal', 'tepurban data series', 'tepurban data series', NULL);
-- RESULT

/*****************************************************************************/
-- Adding entity base type for data packages ... \
CALL add_type($ID$, 'Terradue.TepUrban.Controller.DataPackage, Terradue.TepUrban.WebServer', NULL, 'Data Package', 'Data Packages', 'data/package');
SET @type_id = (SELECT LAST_INSERT_ID());
-- RESULT

-- Adding privileges for data packages ... \
SET @priv_pos = (SELECT MAX(pos) FROM priv);
INSERT INTO priv (id_type, operation, pos, name) VALUES
    (@type_id, 'v', @priv_pos + 1, 'DataPackage: view'),
    (@type_id, 'c', @priv_pos + 2, 'DataPackage: create'),
    (@type_id, 'm', @priv_pos + 3, 'DataPackage: change'),
    (@type_id, 'M', @priv_pos + 4, 'DataPackage: control'),
    (@type_id, 'd', @priv_pos + 5, 'DataPackage: delete');
-- RESULT

/*****************************************************************************/

UPDATE type SET custom_class = 'Terradue.TepUrban.Controller.UserTep, Terradue.TepUrban.WebServer' WHERE class = 'Terradue.Portal.User, Terradue.Portal';

/*****************************************************************************/

-- Add row for users
INSERT IGNORE INTO usrcert (id_usr) SELECT id from usr;

ALTER TABLE usrcert
ADD COLUMN `status` TINYINT(3) NULL DEFAULT 1;

-- RESULT

/*****************************************************************************/

-- Changing REST URL keywords for WPS-related classes ... \
UPDATE type SET keyword='cr/wps' WHERE class='Terradue.Portal.WpsProvider, Terradue.Portal';
UPDATE type SET keyword='service/wps' WHERE class='Terradue.Portal.WpsProcessOffering, Terradue.Portal';
-- Update types ... \
UPDATE type SET keyword='twitter' WHERE class='Terradue.News.TwitterNews, Terradue.News';
UPDATE type SET keyword='tumblr' WHERE class='Terradue.News.TumblrNews, Terradue.News';
-- RESULT

/*****************************************************************************/

SET @type_id = (SELECT id FROM type WHERE class = 'Terradue.Cloud.OneCloudProvider, Terradue.Cloud');
INSERT INTO cloudprov (id_type, caption, address, web_admin_url) VALUES (@type_id, 'Terradue ONE server', 'http://cloud.terradue.int:2633/RPC2', 'http://cloud.terradue.int:2633/RPC2');
INSERT INTO onecloudprov (id, admin_usr, admin_pwd) VALUES (@@IDENTITY, 'serveradmin', 'TO_BE_UPDATED');
SET @prov_id = (SELECT LAST_INSERT_ID());
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('One-default-provider', 'int', 'OpenNebula default provider', 'Enter the value of the identifier of the Opennebula default provider', @prov_id, '0');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('One-access', 'string', 'OpenNebula access url', 'Enter the value of the Opennebula access url', 'https://cloud-dev.terradue.int', '0');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('One-GEP-grpID', 'int', 'Id of GEP group on ONE controller', 'Enter the Id of GEP group on ONE controller', '141', '0');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('EmailCertificateResetSubject', 'string', 'Certificate Reset email body', 'Certificate Reset email body', '[$(SITENAME)] - Certificate removal', '0');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('EmailCertificateResetBody', 'string', 'Certificate Reset email subject', 'Certificate Reset email subject', 'Dear user,\n\nthis is an automatic email to inform you that your request for certificate removal has been processed. \n\nYou can proceed now with a new certificate request from your profile page on\n$(URL).\n\nWith our best regards,\nthe Operations Support team at Terradue', '0');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('EmailCertificateUploadSubject', 'string', 'Certificate Upload email subject', 'Certificate Upload email subject', '[$(SITENAME)] - Certificate upload', '0');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('EmailCertificateUploadBody', 'string', 'Certificate Upload email body', 'Certificate Upload email body', 'Dear Support,\n\nThis is an automatic email to inform you that user\n$(USERNAME)\nhas just uploaded his certificate on the geohazard platform.', '0');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('EmailCertificateRemovalSubject', 'string', 'Certificate Removal email subject', 'Certificate Removal email subject', '[$(SITENAME)] - Certificate removal for user $(USERNAME)', '0');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('EmailCertificateRemovalBody', 'string', 'Certificate Removal email body', 'Certificate Removal email body', 'Dear support,\n\nThis is an automatic email to inform you that user\n$(USERNAME)\n has just asked to remove his certificate on the geohazard platform.\nPlease proceed with the following actions:\n- remove his certificate from the Terradue\'s CA to allow him requesting a new one\n- reset the user status from $(URL)', '0');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('UrlCertificateReset', 'string', 'Certificate Reset url', 'Certificate Reset url', '$(BASEURL)/#!user/admin/$(USERNAME)', '0');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('UrlCertificate', 'string', 'Certificate url', 'Certificate url', '$(BASEURL)/#!settings/profile', '0');
INSERT INTO config (`name`, `internal`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('GpodWpsUser', '0', 'string', 'Gpod WPS Username', 'Enter the name of the Gpod wps user', 'TO_BE_UPDATED', '1');
INSERT INTO config (`name`, `internal`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('GpodWpsUserId', '0', 'int', 'Gpod WPS User Id', 'Enter the Id of the Gpod Wps user', 'TO_BE_UPDATED', '1');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('report-ignored-ids', 'string', 'Report ignored ids', 'Enter the Report ignored ids', '1', '1');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('IT4ISupportEmail', 'string', 'IT4I Support Email', 'IT4I Support Email', 'support.utep@it4i.cz', '1');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('appExternalPostUserLevel', 'string', 'Minimum user level to be able to create external app on POST', 'Minimum user level to be able to create external app on POST', '3', '1');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('catalog-admin-username', 'string', 'Catalog admin user name', 'Catalog admin user name', 'TO_BE_UPDATED', '1');
INSERT INTO config (`name`, `type`, `caption`, `hint`, `value`, `optional`) VALUES ('catalog-admin-apikey', 'string', 'Catalog admin api key', 'Catalog admin api key', 'TO_BE_UPDATED', '1');

UPDATE config SET value='urban-tep.eo.esa.int' WHERE name='Github-client-name';
UPDATE config SET value='TO_BE_UPDATED' WHERE name='Github-client-id';
UPDATE config SET value='TO_BE_UPDATED' WHERE name='Github-client-secret';
UPDATE config SET value='TO_BE_UPDATED' WHERE name='Tumblr-apiKey';
UPDATE config SET value='TO_BE_UPDATED' WHERE name='Twitter-consumerKey';
UPDATE config SET value='TO_BE_UPDATED' WHERE name='Twitter-consumerSecret';
UPDATE config SET value='TO_BE_UPDATED-iuj7ZgIqZwk2YpsrWC9fLnmnUH6CjA4f5M9i6hI' WHERE name='Twitter-token';
UPDATE config SET value='TO_BE_UPDATED' WHERE name='Twitter-tokenSecret';
UPDATE config SET value='https://ca.terradue.com/gpodcs/cgi/certreq.cgi' WHERE name='CertificateRequestUrl';
UPDATE config SET value='https://ca.terradue.com/gpodcs/cgi/certdown.cgi' WHERE name='CertificateDownloadUrl';
UPDATE config SET value='Terradue Support' WHERE name='MailSender';
UPDATE config SET value='support@terradue.com' WHERE name='MailSenderAddress';
UPDATE config SET value='relay.terradue.int' WHERE name='SmtpHostname';
UPDATE config SET value='Dear user,\n\nYour account $(USERNAME) has been created on $(SITEURL).\n\nWe must verify the authenticity of your email address.\n\nTo do so, please click on the following link:\n$(ACTIVATIONURL)\n\nWith our best regards,\nthe Operations Support team at Terradue' WHERE name='RegistrationMailBody';
UPDATE config SET value='[$(PORTAL)] - Registration' WHERE `name`='RegistrationMailSubject';
UPDATE config SET value='$(BASEURL)/#!settings/profile&token=$(TOKEN)' WHERE `name`='EmailConfirmationUrl';
UPDATE config SET value='Urban Tep' WHERE `name`='SiteName';


/*****************************************************************************/

UPDATE auth SET `activation_rule`='2' WHERE `identifier`='umsso';

/*****************************************************************************/

CREATE TABLE wpsjob (
    id int unsigned NOT NULL auto_increment,
    id_usr int unsigned NOT NULL COMMENT 'FK: User',
    identifier varchar(50) NOT NULL COMMENT 'Unique identifier',
    remote_identifier varchar(50) NULL DEFAULT NULL COMMENT 'Unique remote identifier',
    name varchar(100) NOT NULL COMMENT 'WPS Job name',
    wps varchar(100) NOT NULL COMMENT 'FK: WPS Service identifier',
    process varchar(100) NOT NULL COMMENT 'Process name',
    params varchar(1000) NOT NULL COMMENT 'Wps job parameters',
    status_url varchar(200) NOT NULL COMMENT 'Wps job status url',
    created_time datetime NOT NULL COMMENT 'Wps created date',
    CONSTRAINT pk_wpsjob PRIMARY KEY (id),
    CONSTRAINT fk_wpsjob_usr FOREIGN KEY (id_usr) REFERENCES usr(id) ON DELETE CASCADE,
    UNIQUE INDEX (identifier)
) Engine=InnoDB COMMENT 'Wps jobs';
CALL add_type($ID$, 'Terradue.TepUrban.Controller.WpsJob, Terradue.TepUrban.WebServer', NULL, 'Wps Job', 'Wps Job', 'job/wps');
SET @type_id = (SELECT LAST_INSERT_ID());
-- RESULT

-- Adding priv for Wps Job ... \
SET @priv_pos = (SELECT MAX(pos) FROM priv);
INSERT INTO priv (id_type, operation, pos, name, enable_log) VALUES
    (@type_id, 'v', @priv_pos + 1, 'WpsJob: view', 1),
    (@type_id, 'c', @priv_pos + 2, 'WpsJob: create', 1),
    (@type_id, 'm', @priv_pos + 3, 'WpsJob: change', 1),
    (@type_id, 'd', @priv_pos + 4, 'WpsJob: delete', 1),
    (@type_id, 'p', @priv_pos + 5, 'WpsJob: make public', 1);
-- RESULT

CREATE TABLE wpsjob_priv (
    id_wpsjob int unsigned NOT NULL COMMENT 'FK: wpsjob set',
    id_usr int unsigned COMMENT 'FK: User',
    id_grp int unsigned COMMENT 'FK: Group',
    CONSTRAINT fk_wpsjob_priv_wpsjob FOREIGN KEY (id_wpsjob) REFERENCES wpsjob(id) ON DELETE CASCADE,
    CONSTRAINT fk_wpsjob_priv_usr FOREIGN KEY (id_usr) REFERENCES usr(id) ON DELETE CASCADE,
    CONSTRAINT fk_wpsjob_priv_grp FOREIGN KEY (id_grp) REFERENCES grp(id) ON DELETE CASCADE
) Engine=InnoDB COMMENT 'User/group privileges on wpsjob';

/*****************************************************************************/

-- Update priv for User ... \
SET @type_id = (SELECT id FROM type WHERE class='Terradue.Portal.User, Terradue.Portal');
SET @priv_pos = (SELECT MAX(pos) FROM priv);
INSERT INTO priv (id_type, operation, pos, name, enable_log) VALUES
    (@type_id, 'l', @priv_pos + 1, 'User: Login', 1);
-- RESULT

/*****************************************************************************/

-- Add privilege scores
INSERT INTO priv_score (id_priv, score_usr, score_owner) SELECT id, 1, 1 FROM priv WHERE name = 'DataPackage: view';
INSERT INTO priv_score (id_priv, score_usr, score_owner) SELECT id, 0, 2 FROM priv WHERE name = 'DataPackage: create';
INSERT INTO priv_score (id_priv, score_usr, score_owner) SELECT id, 0, 1 FROM priv WHERE name = 'DataPackage: change';
INSERT INTO priv_score (id_priv, score_usr, score_owner) SELECT id, 0, 1 FROM priv WHERE name = 'DataPackage: delete';
INSERT INTO priv_score (id_priv, score_usr, score_owner) SELECT id, 0, 1 FROM priv WHERE name = 'DataPackage: make public';
INSERT INTO priv_score (id_priv, score_usr, score_owner) SELECT id, 1, 1 FROM priv WHERE name = 'WpsJob: view';
INSERT INTO priv_score (id_priv, score_usr, score_owner) SELECT id, 0, 2 FROM priv WHERE name = 'WpsJob: create';
INSERT INTO priv_score (id_priv, score_usr, score_owner) SELECT id, 0, 1 FROM priv WHERE name = 'WpsJob: change';
INSERT INTO priv_score (id_priv, score_usr, score_owner) SELECT id, 0, 1 FROM priv WHERE name = 'WpsJob: delete';
INSERT INTO priv_score (id_priv, score_usr, score_owner) SELECT id, 0, 1 FROM priv WHERE name = 'WpsJob: make public';
-- RESULT

-- Update feature size
ALTER TABLE feature
CHANGE COLUMN `title` `title` VARCHAR(50) NOT NULL ,
CHANGE COLUMN `description` `description` VARCHAR(400) NULL DEFAULT NULL ,
CHANGE COLUMN `image_url` `image_url` VARCHAR(2000) NULL DEFAULT NULL ,
CHANGE COLUMN `button_link` `button_link` VARCHAR(2000) NULL DEFAULT NULL ;
-- RESULT

-- Add domains...\
INSERT INTO domain (identifier, name, description, kind, icon_url, discuss) VALUES ('utep','UTEP consortium','The UTEP consortium community gather all parters users.',3,'https://urban-tep.eo.esa.int/styles/img/icons/logo_tep_urban.png', 'urban/co-utep');
INSERT INTO domain (identifier, name, description, kind, icon_url, discuss) VALUES ('worldbank','World Bank','World bank users.',4,'https://pbs.twimg.com/profile_images/657619924836327424/eS6-JUwh.png','urban/co-worldbank');
-- RESULT