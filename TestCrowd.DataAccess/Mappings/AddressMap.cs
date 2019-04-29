using FluentNHibernate.Mapping;
using TestCrowd.DataAccess.Model;

namespace TestCrowd.DataAccess.Mappings
{
    public class AddressMap:ClassMap<Address>
    {
        public AddressMap()
        {
            Table("Address");
            Id(x => x.Id).GeneratedBy.Guid();

            References(x => x.User).Column("UserId").Unique();

            Map(x => x.Street).Nullable().CustomSqlType("varchar(MAX)");
            Map(x => x.City).Nullable().CustomSqlType("varchar(MAX)");
            Map(x => x.PostCode).Nullable().CustomSqlType("varchar(MAX)");

            References(x => x.Country).Nullable();
        }
    }
}