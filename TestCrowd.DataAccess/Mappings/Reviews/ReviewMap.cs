using FluentNHibernate.Mapping;
using TestCrowd.DataAccess.Model.Reviews;

namespace TestCrowd.DataAccess.Mappings.Reviews
{
    public class ReviewMap :ClassMap<Review>
    {
        public ReviewMap()
        {
            Table("Review");
            Id(x => x.Id).GeneratedBy.GuidNative().Default("NEWID()");

            References(x => x.Creator).Column("CreatorId").Not.Nullable();
            Map(x => x.Content).Not.Nullable().CustomSqlType("varchar(MAX)");
            Map(x => x.Rating).Not.Nullable().CustomSqlType("TINYINT");
            Map(x => x.Created).Not.Nullable().CustomSqlType("DATETIME");
        }
    }
}