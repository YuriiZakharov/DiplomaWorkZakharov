namespace DiplomaWork_Admin.Models
{
    public class Request
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
        public int PlaceId { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsDeclined { get; set; }
        public string DeclineText { get; set; }
    }
}
