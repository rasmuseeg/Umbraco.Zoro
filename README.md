## Install Umbraco 
1. Install umbraco 
```
 PM > Install-Package UmbracoCms
```

2. Create your local mdf under App_Data.
3. Run the site

During installation select custom, and Custom Connection String, paste the following:
```
Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|nola_umbraco.mdf;Integrated Security=True
```

