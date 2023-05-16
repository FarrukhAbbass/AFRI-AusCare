namespace AFRI_AusCare.Models
{
    public class BoardMember
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Role { get; set; }
        public string? ImageUrl { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Twitter { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
