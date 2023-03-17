using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoticeBoard.Models
{
    public class Post
    {
        public int PostId { get; set; }

        [StringLength(10, MinimumLength = 1)]
        public string? Nickname { get; set; }

        [StringLength(30, MinimumLength = 3)]
        public string Title { get; set; }
        //public DateTime ReleaseDate { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime LastUpdated { get; set; }

        [StringLength(60, MinimumLength = 10)]
        public string Content { get; set; }
        public string Category { get; set; }
        public List<Comment>? Comments { get; set; }
        //Array를 쓰면 성능은 좋아지지만 LINQ말고 순환문을 써야함
    }
}
