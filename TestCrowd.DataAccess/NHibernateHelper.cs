using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using TestCrowd.DataAccess.Model;

namespace TestCrowd.DataAccess
{
    public class NHibernateHelper
    {
        private static ISessionFactory _factory = null;

        public static ISession Session
        {
            get
            {
                if (_factory == null)
                {
                    _factory = Fluently.Configure().Database(MsSqlConfiguration.MsSql2012
                        .ConnectionString("Server=tcp:testcrowddb.database.windows.net,1433;Initial Catalog=TestCrowd;Persist Security Info=False;User ID=TestCrowdAdmin;Password=**Fox*96**;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
                        //.ConnectionString(x => x.FromConnectionStringWithKey("defaultConnection")))
                        .Mappings(x => x.FluentMappings.AddFromAssemblyOf<ApplicationUser>())
                        .BuildSessionFactory();             
                }

                return _factory.OpenSession();
            }
        }
    }
}
