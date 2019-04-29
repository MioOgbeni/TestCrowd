using System;
using Microsoft.AspNet.Identity;
using TestCrowd.DataAccess.Interface;

namespace TestCrowd.DataAccess.Model
{
    public class ApplicationRole : IApplicationRole, IRole<Guid>
    {
        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string RoleName { get; set; }
    }
}
