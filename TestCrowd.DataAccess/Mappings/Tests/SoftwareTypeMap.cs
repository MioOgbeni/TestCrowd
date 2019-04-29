using FluentNHibernate.Mapping;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Mappings.Tests
{
    public class SoftwareTypeMap :ClassMap<SoftwareType>
    {
        public SoftwareTypeMap()
        {
            Table("SoftwareType");
            Id(x => x.Id).GeneratedBy.GuidNative().Default("NEWID()");

            Map(x => x.Name).Not.Nullable().CustomSqlType("varchar(100)");
            Map(x => x.Description).Nullable().CustomSqlType("varchar(MAX)");
            Map(x => x.Valid).Not.Nullable().CustomSqlType("BIT");

            Map(x => x.Created).Not.Nullable().CustomSqlType("DATETIME");
            Map(x => x.LastChange).Not.Nullable().CustomSqlType("DATETIME");

            References(x => x.ChangedBy).Column("ChangedById").Not.Nullable();
        }
    }
}