using Microsoft.Build.Framework;

namespace AFRI_AusCare.DataModels
{
    public class AlbumModel
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        public List<GalleryModel> Galleries { get; set; }
        public EventModel EventModel { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAlbum { get; set; }
    }
}
