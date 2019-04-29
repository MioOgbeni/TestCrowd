using FluentNHibernate.Mapping;
using TestCrowd.DataAccess.Model;

namespace TestCrowd.DataAccess.Mappings
{
    public class ApplicationRoleMap : ClassMap<ApplicationRole>
    {
        public ApplicationRoleMap()
        {
            Table("ApplicationRole");
            Id(x => x.Id).GeneratedBy.GuidNative().Default("NEWID()");
            Map(x => x.Name).CustomSqlType("varchar(20)").Not.Nullable();
            Map(x => x.RoleName).CustomSqlType("varchar(50)").Not.Nullable();
        }
    }
}