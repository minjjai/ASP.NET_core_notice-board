using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoticeBoard.Models;

public class FixedCategory
{
    [Key]  
    public int Id { get; set; }
    public string Categories { get; set; }
}
