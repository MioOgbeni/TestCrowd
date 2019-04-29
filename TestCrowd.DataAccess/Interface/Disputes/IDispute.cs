using System;
using System.Collections.Generic;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Model.Disputes;
using TestCrowd.DataAccess.Model.Evidences;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Interface.Disputes
{
    public interface IDispute:IEntity
    {
        Company Company { get; set; }
        Tester Tester { get; set; }
        Admin Resolver { get; set; }

        Status Status { get; set; }

        TestCase TestCase { get; set; }
        IList<DisputeMessage> MessagesHistory { get; set; }

        IList<Evidence> Evidences { get; set; }

        DateTime Created { get; set; }
        DateTime LastUpdate { get; set; }
        DateTime Resolved { get; set; }
        DateTime AutoResolveDate { get; set; }
    }
}