using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TestCrowd.DataAccess.Interface.Tests;

namespace TestCrowd.DataAccess.Model.Tests
{
    public class TestGroup:ITestGroup
    {
        public virtual Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public virtual string Name { get; set; }

        public virtual int SkillDificulty { get; set; }
        public virtual int TimeDificulty { get; set; }

        [Required(ErrorMessage = "At least one Test case is required")]
        public virtual IList<TestCase> TestCases { get; set; }


        [Required(ErrorMessage = "Reward multiplier is required")]
        [Range(0, 100, ErrorMessage = "Please enter valid number")]
        public virtual decimal RewardMultiplier { get; set; }

        public virtual Company Creator { get; set; }
        
        public virtual DateTime Created { get; set; }

        [Required(ErrorMessage = "Available to is required")]
        [DataType(DataType.DateTime)]
        public virtual DateTime AvailableTo { get; set; }

        public virtual int Rating { get; set; }

        public virtual int CountRating()
        {
            if (TestCases.Count == 0)
            {
                return 0;
            }
            else
            {
                int localRating = 0;

                foreach (TestCase testCase in TestCases)
                {
                    localRating = (int) (localRating + (testCase.Reviews.Count == 0 ? 0 : testCase.Reviews.Average(x => x.Rating)));
                }

                return localRating;
            }
        }

        public virtual int CountSkillDificulty()
        {
            if (TestCases.Count == 0)
            {
                return 0;
            }
            else
            {
                int count = 0;
                decimal sum = 0;

                foreach (TestCase testCase in TestCases)
                {
                    count++;
                    sum = sum + testCase.SkillDificulty;
                }

                return (int) Math.Ceiling(sum/count);
            }
        }

        public virtual int CountTimeDificulty()
        {
            if (TestCases.Count == 0)
            {
                return 0;
            }
            else
            {
                int count = 0;
                decimal sum = 0;

                foreach (TestCase testCase in TestCases)
                {
                    count++;
                    sum = sum + testCase.TimeDificulty;
                }

                return (int)Math.Ceiling(sum/count);
            }
        }
    }
}