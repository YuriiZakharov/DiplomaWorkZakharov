﻿namespace DiplomaWork.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string CreatedDate { get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }

    }
}
