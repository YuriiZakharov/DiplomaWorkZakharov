using System.Collections.Generic;

namespace DiplomaWork_WebSite.Models
{
    public class ChatViewModel
    {
        public int ChatId { get; set; }
        public string UserImageBase64 { get; set; }
        public string UserName { get; set; }
        public string UserLastName { get; set; }
        public IEnumerable<Place> UserPlaces { get; set; }
    }
}