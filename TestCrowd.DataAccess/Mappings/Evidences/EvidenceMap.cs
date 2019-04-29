using FluentNHibernate.Mapping;

namespace TestCrowd.DataAccess.Mappings.Evidences
{
    public class EvidenceMap : ClassMap<Model.Evidences.Evidence>
    {
        public EvidenceMap()
        {
            Table("Evidence");
            Id(x => x.Id).GeneratedBy.GuidNative().Default("NEWID()");

            Map(x => x.Name).Not.Nullable().CustomSqlType("varchar(100)");
            Map(x => x.RealName).Not.Nullable().CustomSqlType("varchar(100)");
            Map(x => x.Extension).Not.Nullable().CustomSqlType("varchar(100)");
            Map(x => x.Attached).Not.Nullable().CustomSqlType("DATETIME");
        }
    }
}