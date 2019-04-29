using FluentNHibernate.Mapping;
using TestCrowd.DataAccess.Model.Disputes;

namespace TestCrowd.DataAccess.Mappings.Disputes
{
    public class DisputeMessageMap :ClassMap<DisputeMessage>
    {
        public DisputeMessageMap()
        {
            Table("DisputeMessage");
            Id(x => x.Id).GeneratedBy.GuidNative().Default("NEWID()");

            References(x => x.User).Column("UserId").Not.Nullable();

            Map(x => x.Message).Not.Nullable().CustomSqlType("varchar(300)");
            Map(x => x.Created).Not.Nullable().CustomSqlType("DATETIME");
        }
    }
}