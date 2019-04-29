using FluentNHibernate.Mapping;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Mappings.Tests
{
    public class TestGroupsMap :ClassMap<TestGroups>
    {
        public TestGroupsMap()
        {
            Table("TestGroups");
            Id(x => x.Id).GeneratedBy.GuidNative().Default("NEWID()");

            References(x => x.TestGroup).Not.Nullable();
            References(x => x.Tester).Not.Nullable();

            Map(x => x.Status).Not.Nullable().CustomType<GroupStatus>();

            Map(x => x.Takened).Not.Nullable().CustomSqlType("DATETIME");
            Map(x => x.Finished).Nullable().CustomSqlType("DATETIME");
        }
    }
}