using FluentNHibernate.Mapping;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Mappings.Tests
{
    public class TestsMap :ClassMap<Model.Tests.Tests>
    {
        public TestsMap()
        {
            Table("Tests");
            Id(x => x.Id).GeneratedBy.GuidNative().Default("NEWID()");

            References(x => x.Test).Not.Nullable();
            References(x => x.Tester).Not.Nullable();
            HasMany(x => x.Evidences);

            Map(x => x.Status).Not.Nullable().CustomType<TestsStatus>();
            References(x => x.TestStatus).Nullable();
            Map(x => x.Takened).Not.Nullable().CustomSqlType("DATETIME");
            Map(x => x.Finished).Nullable().CustomSqlType("DATETIME");
            Map(x => x.Rejected).Nullable().CustomSqlType("DATETIME");
        }
    }
}