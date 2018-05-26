## Cloning project
1. On your machine, creata a new repository
`git init foo`
2. Then pull this repository into the new one
```
cd foo
git pull https://rasmuseeg@bitbucket.org/rasmuseeg/umbraco-bootstrap.git
``` 
3. Run Rename.ps1 using powershell
4. Type your new Solution name
5. Type your new Project root name
6. Open using visual studio
7. Re-install missing nuget packages
8. Run the site
9. Login to umbraco backoffice using
```
Username:   admin@example.com
Password:   AdminAdmin
```


## Starting from scratch 
1. Install the following umbraco packages
```
 PM > Install-Package UmbracoCms
 PM > Install-Package Umbraco.ModelsBuilder.Api
 PM > Install-Package Lecoati.LeBlender
```

2. Create your local mdf under App_Data.
3. Run the site
4. During installation select custom, and Custom Connection String, paste the following:
```
Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=UmbracoBootstrap;AttachDbFilename=|DataDirectory|UmbracoBootstrap.mdf;Integrated Security=True;
```

## TODO

* Rename Project files
* Rename Project database, if not starting from scratch
* Change machineKey in web.config
* Change localhost port 
* Enable or disable SSL support