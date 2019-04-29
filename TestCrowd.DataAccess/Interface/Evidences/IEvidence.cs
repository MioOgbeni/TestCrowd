using System;

namespace TestCrowd.DataAccess.Interface.Evidences
{
    public interface IEvidence:IEntity
    {
        string Name { get; set; }
        string RealName { get; set; }
        string Extension { get; set; }
        DateTime Attached { get; set; }
    }
}