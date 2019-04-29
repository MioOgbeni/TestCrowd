using System;
using System.Collections.Generic;
using TestCrowd.DataAccess.Interface.Disputes;
using TestCrowd.DataAccess.Model.Evidences;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Model.Disputes
{
    public enum Status { Pending, InProgress, Approved, Rejected }

    public class Dispute : IDispute
    {
        public virtual Guid Id { get; set; }

        public virtual Company Company { get; set; }
        public virtual Tester Tester { get; set; }
        public virtual Admin Resolver { get; set; }

        public virtual Status Status { get; set; }

        public virtual TestCase TestCase { get; set; }
        public virtual IList<DisputeMessage> MessagesHistory { get; set; }

        public virtual IList<Evidence> Evidences { get; set; }

        public virtual DateTime Created { get; set; }
        public virtual DateTime LastUpdate { get; set; }
        public virtual DateTime Resolved { get; set; }
        public virtual DateTime AutoResolveDate { get; set; }
    }
}