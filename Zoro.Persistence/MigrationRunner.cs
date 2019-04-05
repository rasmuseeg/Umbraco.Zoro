using Semver;
using System;
using System.Linq;
using System.Reflection;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;

namespace Zoro.Persistence
{
    public class MigrationsConfig : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            HandleMigrations(applicationContext);
        }

        private static void HandleMigrations(ApplicationContext context)
        {
            try
            {
                const string productName = GlobalConfig.ApplicationName;
                var currentVersion = new SemVersion(0, 0, 0);

                // get all migrations already executed
                var migrations = context.Services.MigrationEntryService.GetAll(productName);

                // get the latest migration for "UDF" executed
                var latestMigration = migrations.OrderByDescending(x => x.Version).FirstOrDefault();

                if (latestMigration != null)
                    currentVersion = latestMigration.Version;

                var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
                var targetVersion = new SemVersion(assemblyVersion);
                if (targetVersion == currentVersion)
                    return;

                var migrationsRunner = new MigrationRunner(
                  context.Services.MigrationEntryService,
                  context.ProfilingLogger.Logger,
                  currentVersion,
                  targetVersion,
                  productName);

                migrationsRunner.Execute(context.DatabaseContext.Database);
            }
            catch (Exception e)
            {
                LogHelper.Error<MigrationsConfig>("Error running " + GlobalConfig.ApplicationName + " migration", e);
            }
        }
    }
}
