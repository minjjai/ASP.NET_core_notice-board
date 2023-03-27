using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace NoticeBoard.Models
{
    public class AttachFile
    {
        [Key]
       public int FileId { get; set; }
       public string FileName { get; set; }
       public string FilePath { get; set; }
       public byte[] FileData { get; set; }
       public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
