using FluentNHibernate.Mapping;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Mappings.Tests
{
    public class TestCaseMap : ClassMap<TestCase>
    {
        public TestCaseMap()
        {
            Table("TestCase");
            Id(x => x.Id).GeneratedBy.GuidNative().Default("NEWID()");

            References(x => x.Category).Column("CategoryId");
            References(x => x.SoftwareType).Column("SoftwareTypeId");

            Map(x => x.Name).Not.Nullable().CustomSqlType("varchar(100)");
            Map(x => x.SkillDificulty).Not.Nullable().CustomSqlType("TINYINT");
            Map(x => x.TimeDificulty).Not.Nullable().CustomSqlType("TINYINT");

            Map(x => x.Description).Nullable().CustomSqlType("varchar(MAX)");

            Map(x => x.Reward).Not.Nullable().CustomSqlType("INT");

            Map(x => x.Created).Not.Nullable().CustomSqlType("DATETIME");
            Map(x => x.AvailableTo).Not.Nullable().CustomSqlType("DATETIME");

            References(x => x.Creator).Column("CreatorId").Not.Nullable();

            Map(x => x.Rating).Not.Nullable().CustomSqlType("TINYINT");
            HasMany(x => x.Reviews);
            HasMany(x => x.Evidences);

            Map(x => x.IsInGroup).CustomSqlType("BIT").Default("0");
        }
    }
}