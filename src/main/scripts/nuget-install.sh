rm -rf packages/*
nuget restore
mkdir -p Terradue.Tep.Urban.WebServer/core
mkdir -p Terradue.Tep.Urban.WebServer/modules
mkdir -p Terradue.Tep.Urban.WebServer/services
cp -pr packages/**/content/core/** Terradue.Tep.Urban.WebServer/core
cp -pr packages/**/content/modules/** Terradue.Tep.Urban.WebServer/modules
