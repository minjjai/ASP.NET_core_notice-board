using Microsoft.AspNetCore.Mvc.Rendering;
using NoticeBoard.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NoticeBoard.Models;

public class CategoryViewModel
{
    public int? FileId { get; set; }
    public int? PostId { get; set; }
    [StringLength(10, MinimumLength = 1)]
    public string Nickname { get; set; }
    [StringLength(30, MinimumLength = 3)]
    public string Title { get; set; }
    [StringLength(1000, MinimumLength = 10)]
    public string Content { get; set; }
    public string Category { get; set; }
    //public List<AttachFile>? AttachFiles { get; set; }
    //public IEnumerable<SelectListItem> FileNames { get; set; }
    public List<FixedCategory> FixedCategories { get; set; }
    public IEnumerable<SelectListItem> Categories { get; set; }
}

