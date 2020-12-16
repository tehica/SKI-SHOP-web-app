using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string DisplayName { get; set; }

        [Required]
        [EmailAddress]

        public string Email { get; set; }

        [Required]
        /*
            It expects atleast 1 small-case letter, 1 Capital letter, 1 digit, 1 special character and
            the length should be between 6-10 characters. The sequence of the characters is not important. 
            This expression follows the above 4 norms specified by microsoft for a strong password.
         */
        [RegularExpression("(?=^.{6,10}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s).*$",
                           ErrorMessage = "Password must have 1 uppercase, 1 lowercase, 1 number and 1 non alphanumeric and at least 6 characters")]
        public string Password { get; set; }
    }
}
