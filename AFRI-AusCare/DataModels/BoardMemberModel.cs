using System.ComponentModel.DataAnnotations;

namespace AFRI_AusCare.DataModels
{
    public class BoardMemberModel
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Role { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [Required]
        public IFormFile? ImageFile { get; set; }
        public bool IsDeleted { get; set; }
    }
}