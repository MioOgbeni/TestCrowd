using FluentNHibernate.Mapping;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Mappings.Tests
{
    public class TestGroupMap :ClassMap<TestGroup>
    {
        public TestGroupMap()
        {
            Table("TestGroup");
            Id(x => x.Id).GeneratedBy.GuidNative().Default("NEWID()");

            Map(x => x.Name).Not.Nullable().CustomSqlType("varchar(100)");
            Map(x => x.SkillDificulty).Not.Nullable().CustomSqlType("TINYINT");
            Map(x => x.TimeDificulty).Not.Nullable().CustomSqlType("TINYINT");

            HasMany(x => x.TestCases);

            Map(x => x.RewardMultiplier).Not.Nullable().CustomSqlType("DECIMAL").Precision(3).Scale(2);

            Map(x => x.Created).Not.Nullable().CustomSqlType("DATETIME");
            Map(x => x.AvailableTo).Not.Nullable().CustomSqlType("DATETIME");

            References(x => x.Creator).Column("CreatorId").Not.Nullable();

            Map(x => x.Rating).Not.Nullable().CustomSqlType("INT");
        }
    }
}