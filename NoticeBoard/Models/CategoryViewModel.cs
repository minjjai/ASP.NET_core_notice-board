﻿using Microsoft.AspNetCore.Mvc.Rendering;
using noticeboard.models;
using NoticeBoard.Models;
using System.Collections.Generic;


namespace NoticeBoard.Models;

public class CategoryViewModel
{
    public int? PostId { get; set; }
    public string Nickname { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int Category { get; set; }

    public List<FixedCategory> FixedCategories { get; set; }
    public IEnumerable<SelectListItem> Categories { get; set; }
}
