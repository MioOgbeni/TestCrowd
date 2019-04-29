using System;
using TestCrowd.DataAccess.Interface.Evidences;

namespace TestCrowd.DataAccess.Model.Evidences
{
    public class Evidence :IEvidence
    {
        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }
        public virtual string RealName { get; set; }
        public virtual string Extension { get; set; }
        public virtual DateTime Attached { get; set; }
    }
}