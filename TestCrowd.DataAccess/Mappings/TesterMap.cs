using System;
using FluentNHibernate.Mapping;
using TestCrowd.DataAccess.Model;

namespace TestCrowd.DataAccess.Mappings
{
    public class TesterMap :SubclassMap<Tester>
    {
        public TesterMap()
        {
            DiscriminatorValue("tester");

            Map(x => x.FirstName).Nullable().CustomSqlType("nvarchar(50)");
            Map(x => x.LastName).Nullable().CustomSqlType("nvarchar(50)");

            HasOne(x => x.Address).PropertyRef(nameof(Address.User)).Cascade.All();

            Map(x => x.Credits).Nullable().CustomSqlType("INT");
        }
    }
}