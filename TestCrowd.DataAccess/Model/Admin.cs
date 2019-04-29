using System.ComponentModel.DataAnnotations;
using TestCrowd.DataAccess.Interface;

namespace TestCrowd.DataAccess.Model
{
    public class Admin : ApplicationUser, IAdmin
    {
        [Required(ErrorMessage = "First Name is required")]
        public virtual string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public virtual string LastName { get; set; }
    }
}