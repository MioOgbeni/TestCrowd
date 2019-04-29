using FluentNHibernate.Mapping;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Mappings.Tests
{
    public class TestStatusMap :ClassMap<TestStatus>
    {
        public TestStatusMap()
        {
            Table("TestStatus");
            Id(x => x.Id).GeneratedBy.GuidNative().Default("NEWID()");
            Map(x => x.Status).CustomSqlType("varchar(40)").Not.Nullable();
        }
    }
}