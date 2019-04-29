using System;
using TestCrowd.DataAccess.Interface.Disputes;

namespace TestCrowd.DataAccess.Model.Disputes
{
    public class DisputeMessage:IDisputeMessage
    {
        public virtual Guid Id { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual DateTime Created { get; set; }
        public virtual string Message { get; set; }
    }
}