using Microsoft.AspNetCore.Mvc.Rendering;

namespace NoticeBoard.Models
{
    public class PostsViewModel
    {
        public List<Post> Posts { get; set; }
        public List<FixedCategory> FixedCategories { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public string? PostCategory { get; set; } //선택한 카테고리가 담김
        public string? SearchString { get; set; } //검색인풋에 입력한 테스트가 담김
        public string? SortOrder { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
