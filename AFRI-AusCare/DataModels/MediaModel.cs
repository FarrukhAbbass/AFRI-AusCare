using System.ComponentModel.DataAnnotations;

namespace AFRI_AusCare.DataModels
{
    public class MediaModel
    {
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        [Required]
        public string? MediaURL { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

        [Required]
        public IFormFile? ImageFile { get; set; }
    }
}