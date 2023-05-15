using System.ComponentModel.DataAnnotations;

namespace AFRI_AusCare.DataModels
{
    public class ContactFormViewModel
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Subject { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 10)]
        public string Message { get; set; }
    }
}
