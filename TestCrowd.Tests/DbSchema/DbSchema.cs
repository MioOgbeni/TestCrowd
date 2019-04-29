using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Tool.hbm2ddl;
using TestCrowd.DataAccess.Model;

namespace TestCrowd.Tests.DbSchema
{
    [TestClass]
    public class DbSchema
    {
        [TestMethod]
        public void GenerateSchema()
        {
            var configuration = Fluently.Configure().Database(MsSqlConfiguration.MsSql2012
                .ConnectionString("Server=tcp:testcrowddb.database.windows.net,1433;Initial Catalog=TestCrowd;Persist Security Info=False;User ID=TestCrowdAdmin;Password=**Fox*96**;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
                //.ConnectionString(x => x.FromConnectionStringWithKey("defaultConnection")))
                .Mappings(x => x.FluentMappings.AddFromAssemblyOf<ApplicationUser>())
                .BuildConfiguration();

            new SchemaExport(configuration).Execute(false, true, false);
        }

        [TestMethod]
        public void UpdateSchema()
        {
            var configuration = Fluently.Configure().Database(MsSqlConfiguration.MsSql2012
                    .ConnectionString("Server=tcp:testcrowddb.database.windows.net,1433;Initial Catalog=TestCrowd;Persist Security Info=False;User ID=TestCrowdAdmin;Password=**Fox*96**;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
                    //.ConnectionString(x => x.FromConnectionStringWithKey("defaultConnection")))
                .Mappings(x => x.FluentMappings.AddFromAssemblyOf<ApplicationUser>())
                .BuildConfiguration();

            new SchemaUpdate(configuration).Execute(false, true);
        }
    }
}