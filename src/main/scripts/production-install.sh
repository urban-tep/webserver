#! /bin/sh

#configuration
portal_path="/usr/local/tepurban/webserver"
site="tepurban"
db="tepurban"
dump="/tmp/tepurban-db-dump.sql"
dbuser="root"
dbRootUsr="root"
dbRootPass="install"
dbUser="portal"
dbUserpass="pomho1l@T2"

mkdir -p $portal_path/services
mkdir -p $portal_path/modules

mkdir /var/www/.config
chown apache:apache /var/www/.config

mkdir $portal_path/sites/$site/logs
chown apache:apache $portal_path/sites/$site/logs

#link services
mkdir -p $portal_path/sites/$site/root/services
mkdir -p $portal_path/sites/$site/root/files
chown apache:apache $portal_path/sites/$site/root/files

#dynamic hostname
sed -i -e 's/${PORTALWEBSERVER}/'$HOSTNAME'/g' $portal_path/sites/$site/root/web.config
sed -i -e 's/${PORTALWEBSERVER}/'$HOSTNAME'/g' $portal_path/sites/$site/config/*
sed -i -e 's/${PORTALWEBSERVER}/'$HOSTNAME'/g' /etc/httpd/conf.d/*tepurban*

#dumped database copy
if [ -f $dump ] 
then 
    echo "Copy of the dumped database \`$db\`"
	mysql -u $dbRootUsr --password=$dbRootPass -e "drop database if exists \`$db\`"
	mysql -u $dbRootUsr --password=$dbRootPass -e "create database \`$db\`"
	mysql -u $dbRootUsr --password=$dbRootPass -e "GRANT SELECT, INSERT, UPDATE, DELETE on \`$db\`.* to '$dbUser'@'localhost' IDENTIFIED by '$dbUserpass'"
	mysql -u $dbRootUsr --password=$dbRootPass $db < $dump
else
	echo "No copy of the dumped database $db"
	mysql -u $dbRootUsr --password=$dbRootPass -e "GRANT SELECT, INSERT, UPDATE, DELETE on \`$db\`.* to '$dbUser'@'localhost' IDENTIFIED by '$dbUserpass'" 
fi

#mono and mysql
mono $portal_path/sites/$site/root/bin/Terradue.Portal.AdminTool.exe auto -r $portal_path/sites/$site/root -u $dbRootUsr -p $dbRootPass -S $db -H localhost

echo "[INFO ] TEP URBAN Shibboleth configuration is installed in /etc/shibboleth/shibboleth2-tepurban.xml"
echo "[INFO ] Copy it as /etc/shibboleth/shibboleth2.xml and restart shibd to enable TEP Urban SP"

chkconfig --add tepurban-agent

/etc/init.d/tepurban-agent start

