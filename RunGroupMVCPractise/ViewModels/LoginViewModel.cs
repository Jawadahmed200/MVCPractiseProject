using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RunGroupMVCPractise.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name="Email Address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
