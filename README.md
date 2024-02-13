# APIDemo
Default Environments config IS Development 
swagger docs enabled only for Development os Staging  

this demo is connencted to database on the cloud (sqlserver),
to  connect localy user the following steps 
1- create   local database on your computer (sqlserver )
2- change connection string in the appsettings file .
3 -rebuild 
4 - run 
-------------------------------------------
default login 
{
  "email": "hisham.baazawy@gmail.com",
  "password": "wYX%0<|HK09"
}
------------------------------------------
use AccountController/Login to get token  then used on the authorization 
all action Require User Role or admin 
