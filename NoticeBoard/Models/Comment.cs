﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoticeBoard.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        //public string? Nickname { get; set; }

        [StringLength(100, MinimumLength = 2)]
        public string? Content { get; set; }
        public DateTime LastUpdated { get; set; }
        public int? PostId { get; set; }
        public Post Post { get; set; }
    }
}
