namespace AFRI_AusCare.Models
{
    public class Album
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public List<Gallery>? Galleries { get; set; }
        public Event? Event { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAlbum { get; set; }
        public AlbumType AlbumType { get; set; }
    }
}
