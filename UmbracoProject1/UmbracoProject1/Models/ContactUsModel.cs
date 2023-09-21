using System.ComponentModel.DataAnnotations;

namespace UmbracoProject1.Models
{
     
    public class ContactUsModel
    {
        public ContactUsModel() { }

        [Required(ErrorMessage = "Please enter your name.")]
        [MaxLength(100)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter the subject.")]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "Please enter your message.")]
        public string? Message { get; set; }

        [Required]
        public string GoogleCaptchaToken { get; set; }
    }
}
