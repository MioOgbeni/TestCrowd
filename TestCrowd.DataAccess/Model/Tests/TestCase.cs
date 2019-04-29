using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using NHibernate.Type;
using TestCrowd.DataAccess.Interface.Tests;
using TestCrowd.DataAccess.Model.Evidences;
using TestCrowd.DataAccess.Model.Reviews;

namespace TestCrowd.DataAccess.Model.Tests
{
    public class TestCase : ITestCase
    {
        public virtual Guid Id { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public virtual TestCategory Category { get; set; }

        [Required(ErrorMessage = "Software Type is required")]
        public virtual SoftwareType SoftwareType { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public virtual string Name { get; set; }

        [Required(ErrorMessage = "Skill Difficulty is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Select at least one star")]
        public virtual int SkillDificulty { get; set; }

        [Required(ErrorMessage = "Time Difficulty is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Select at least one star")]
        public virtual int TimeDificulty { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Description is required")]
        public virtual string Description { get; set; }

        [Required(ErrorMessage = "Reward is required")]
        public virtual int Reward { get; set; }

        public virtual IList<Evidence> Evidences { get; set; }

        public virtual Company Creator { get; set; }

        public virtual DateTime Created { get; set; }

        [Required(ErrorMessage = "Available to is required")]
        [DataType(DataType.DateTime)]
        public virtual DateTime AvailableTo { get; set; }

        public virtual IList<Review> Reviews { get; set; }
        public virtual int Rating { get; set; }

        public virtual bool IsInGroup { get; set; }

        public virtual void CountRating()
        {
            Rating =  (int) (Reviews.Count == 0 ? 0 : Reviews.Average(x => x.Rating));
        }
    }
}