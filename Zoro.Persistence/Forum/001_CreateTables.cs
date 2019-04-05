using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Core.Logging;

namespace Zoro.Persistence.Forum
{
    [Migration("0.0.1", 1, ForumConstants.ProductName)]
    public class CreateTables : MigrationBase
    {
        private readonly UmbracoDatabase _database = ApplicationContext.Current.DatabaseContext.Database;
        private readonly DatabaseSchemaHelper _schemaHelper;

        public CreateTables(ISqlSyntaxProvider sqlSyntax, ILogger logger)
            : base(sqlSyntax, logger)
        {
            _schemaHelper = new DatabaseSchemaHelper(_database, logger, sqlSyntax);
        }

        public override void Up()
        {
            //_schemaHelper.CreateTable<ForumThread>();
            //_schemaHelper.CreateTable<ForumComment>();
        }

        public override void Down()
        {
            //_schemaHelper.CreateTable<ForumComment>();
            //_schemaHelper.CreateTable<ForumThread>();
        }
    }
}