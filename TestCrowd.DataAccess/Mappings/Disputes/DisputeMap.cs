using FluentNHibernate.Mapping;
using TestCrowd.DataAccess.Model.Disputes;

namespace TestCrowd.DataAccess.Mappings.Disputes
{
    public class DisputeMap :ClassMap<Dispute>
    {
        public DisputeMap()
        {
            Table("Dispute");
            Id(x => x.Id).GeneratedBy.GuidNative().Default("NEWID()");

            References(x => x.Company).Column("CompanyId").Not.Nullable();
            References(x => x.Tester).Column("TesterId").Not.Nullable();
            References(x => x.Resolver).Column("ResolverId").Not.Nullable();
            References(x => x.TestCase).Column("TestCaseId").Not.Nullable();

            Map(x => x.Status).Not.Nullable().CustomType<Status>();

            HasMany(x => x.MessagesHistory);

            HasMany(x => x.Evidences);

            Map(x => x.Created).Not.Nullable().CustomSqlType("DATETIME");
            Map(x => x.LastUpdate).Not.Nullable().CustomSqlType("DATETIME");
            Map(x => x.Resolved).Nullable().CustomSqlType("DATETIME");
            Map(x => x.AutoResolveDate).Not.Nullable().CustomSqlType("DATETIME");
        }       
    }
}