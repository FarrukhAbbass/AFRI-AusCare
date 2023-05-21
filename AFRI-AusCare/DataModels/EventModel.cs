using System.ComponentModel.DataAnnotations;

namespace AFRI_AusCare.DataModels
{
    public class EventModel
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? Author { get; set; }
        public string? ImageUrl { get; set; }
        [Required]
        public IFormFile? ImageFile { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int AlbumId { get; set; }
        public AlbumModel? Album { get; set; }

    }
}
