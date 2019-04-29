using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TestCrowd.DataAccess.Interface.Reviews;

namespace TestCrowd.DataAccess.Model.Reviews
{
    public class Review :IReview
    {
        public virtual Guid Id { get; set; }
        public virtual ApplicationUser Creator { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Description is required")]
        public virtual string Content { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Select at least one star")]
        public virtual int Rating { get; set; }

        public virtual DateTime Created { get; set; }
    }
}