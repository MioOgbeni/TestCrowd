using System;
using System.Data;
using FluentNHibernate.Mapping;
using TestCrowd.DataAccess.Model;

namespace TestCrowd.DataAccess.Mappings
{
    public class ApplicationUserMap : ClassMap<ApplicationUser>
    {
        public ApplicationUserMap()
        {
            Table("ApplicatoinUser");
            Id(x => x.Id).GeneratedBy.GuidNative().Default("NEWID()");
            Map(x => x.UserName).Not.Nullable().CustomSqlType("varchar(50)");
            Map(x => x.Email).Not.Nullable().CustomSqlType("nvarchar(255)");
            Map(x => x.PasswordHash).Not.Nullable().CustomSqlType("varchar(MAX)");
            References(x => x.Role).Column("RoleId").Not.Nullable();

            DiscriminateSubClassesOnColumn("RoleDiscriminant");
        }
    }
}