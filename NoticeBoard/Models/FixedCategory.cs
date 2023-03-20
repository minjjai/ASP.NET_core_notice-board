using NoticeBoard.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace noticeboard.models
{
    public class FixedCategory
    {
        [Key]  
        public int Id { get; set; }
        public string Categories { get; set; }
    }
}
