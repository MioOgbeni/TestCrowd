using System;
using FluentNHibernate.Mapping;
using TestCrowd.DataAccess.Model;

namespace TestCrowd.DataAccess.Mappings
{
    public class CompanyMap :SubclassMap<Company>
    {
        public CompanyMap()
        {
            DiscriminatorValue("company");

            Map(x => x.CompanyName).Nullable().CustomSqlType("nvarchar(50)");

            HasMany(x => x.Reviews);
            Map(x => x.Rating).Nullable().CustomSqlType("TINYINT");

            HasOne(x => x.Address).PropertyRef(nameof(Address.User)).Cascade.All();

            Map(x => x.Credits).Nullable().CustomSqlType("INT");
        }
    }
}