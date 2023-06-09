﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace NoticeBoard.Models;

public class PostCategoryViewModel
{
    public List<Post>? Posts { get; set; } //게시글 목록
    public SelectList? Categories { get; set; } //카테고리 목록
    public string? PostCategory { get; set; } //선택한 카테고리가 담김
    public string? SearchString { get; set; } //검색인풋에 입력한 테스트가 담김
    public string? SortOrder { get; set; }
}