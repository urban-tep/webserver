USE $MAIN$;

/*****************************************************************************/

-- update table type ... \
update type set class='Terradue.Tep.Controller.DataSeries, Terradue.Tep' WHERE class='Terradue.TepUrban.Controller.DataSeries, Terradue.TepUrban.WebServer';
update type set class='Terradue.Tep.Controller.DataPackage, Terradue.Tep' WHERE class='Terradue.TepUrban.Controller.DataPackage, Terradue.TepUrban.WebServer';
update type set custom_class='Terradue.Tep.Controller.UserTep, Terradue.Tep' WHERE custom_class='Terradue.TepUrban.Controller.UserTep, Terradue.TepUrban.WebServer';
-- RESULT
