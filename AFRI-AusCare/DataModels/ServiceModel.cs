using System.ComponentModel.DataAnnotations;

namespace AFRI_AusCare.DataModels
{
    public class ServiceModel
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }



        [Required]
        public IFormFile? ImageFile { get; set; }
    }
}
