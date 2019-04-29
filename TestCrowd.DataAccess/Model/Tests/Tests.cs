using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TestCrowd.DataAccess.Interface;
using TestCrowd.DataAccess.Interface.Tests;
using TestCrowd.DataAccess.Model.Evidences;

namespace TestCrowd.DataAccess.Model.Tests
{
    public enum TestsStatus { Takened, Finished, Rejected, Reviewed, Null }

    public class Tests :ITests
    {
        public virtual Guid Id { get; set; }

        public virtual TestCase Test {get; set; }
        public virtual Tester Tester { get; set; }
        public virtual IList<Evidence> Evidences { get; set; }
        public virtual TestsStatus Status { get; set; }

        [Required(ErrorMessage = "Test status is required")]
        public virtual TestStatus TestStatus { get; set; }
        public virtual DateTime Takened { get; set; }
        public virtual DateTime? Finished { get; set; }
        public virtual DateTime? Rejected { get; set; }
    }
}