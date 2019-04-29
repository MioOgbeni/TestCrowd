using System;
using System.ComponentModel.DataAnnotations;
using TestCrowd.DataAccess.Interface.Tests;

namespace TestCrowd.DataAccess.Model.Tests
{
    public class SoftwareType:ISoftwareType
    {
        public virtual Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(20, ErrorMessage = "{0} can be only {1} characters long.", MinimumLength = 0)]
        public virtual string Name { get; set; }

        [StringLength(80, ErrorMessage = "{0} can be only {1} characters long.", MinimumLength = 0)]
        public virtual string Description { get; set; }

        public virtual bool Valid { get; set; }

        public virtual DateTime Created { get; set; }
        public virtual DateTime LastChange { get; set; }
        public virtual Admin ChangedBy { get; set; }
    }
}