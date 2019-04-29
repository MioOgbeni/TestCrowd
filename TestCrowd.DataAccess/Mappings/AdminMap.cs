using System;
using FluentNHibernate.Mapping;
using TestCrowd.DataAccess.Model;

namespace TestCrowd.DataAccess.Mappings
{
    public class AdminMap : SubclassMap<Admin>
    {
        public AdminMap()
        {
            DiscriminatorValue("admin");

            Map(x => x.FirstName).Nullable().CustomSqlType("nvarchar(50)");
            Map(x => x.LastName).Nullable().CustomSqlType("nvarchar(50)");
        }
    }
}