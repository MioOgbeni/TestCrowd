using FluentNHibernate.Mapping;
using TestCrowd.DataAccess.Model;

namespace TestCrowd.DataAccess.Mappings
{
    public class CountryMap:ClassMap<Country>
    {
        public CountryMap()
        {
            Table("Country");
            Id(x => x.Id).GeneratedBy.GuidNative().Default("NEWID()");

            Map(x => x.IdentificationCode).Not.Nullable().CustomSqlType("varchar(5)");

            Map(x => x.Name).Not.Nullable().CustomSqlType("varchar(100)");
        }
    }
}