using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using TestCrowd.DataAccess.Interface;

namespace TestCrowd.DataAccess.Model
{
    public class ApplicationUser : IApplicationUser, IUser<Guid>
    {
        public virtual Guid Id { get; set; }

        [Required(ErrorMessage = "Account Name is required")]
        public virtual string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Email does not have a valid format!")]
        public virtual string Email { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression("^(?=.*[a-zA-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{6,100}$", ErrorMessage = "Password must contain at least one special character and number!")]
        [DataType(DataType.Password)]
        public virtual string PasswordHash { get; set; }

        [Compare(nameof(PasswordHash), ErrorMessage = "Passwords must be same!")]
        [Required(ErrorMessage = "Password is required!")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression("^(?=.*[a-zA-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{6,100}$", ErrorMessage = "Password must contain at least one special character and number!")]
        [DataType(DataType.Password)]
        public virtual string PasswordConfirmationHash { get; set; }

        public virtual ApplicationRole Role { get; set; }
    }
}
