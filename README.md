## Install Umbraco 
1. Install umbraco 
```
 PM > Install-Package UmbracoCms
 PM > Install-Package Umbraco.ModelsBuilder.Api
 PM > Install-Package Lecoati.LeBlender
```

2. Create your local mdf under App_Data.
3. Run the site

During installation select custom, and Custom Connection String, paste the following:
```
Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|UmbracoBootstrap.mdf;Integrated Security=True
```

2. Umbraco Backoffice Login
Username:   admin@example.com
Password    AdminAdmin

## Getting started

* Rename Project files
* Rename Project database, if not starting from scratch
* Change machineKey in web.config
* Change localhost port 
* Enable or disable SSL support
