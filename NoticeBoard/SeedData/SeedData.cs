//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using NoticeBoard.Data;
//using System;
//using System.Linq;

//namespace NoticeBoard.Models;

//public static class SeedData
//{
//    public static void Initialize(IServiceProvider serviceProvider)
//    {
//        using (var context = new NoticeBoardContext(
//            serviceProvider.GetRequiredService<
//                DbContextOptions<NoticeBoardContext>>()))
//        {
//            // Look for any posts.
//            if (context.Posts.Any())
//            {
//                return;   // DB has been seeded
//            }
//            context.Posts.AddRange(
//                new Post
//                {
//                    Title = "안녕하세요",
//                    //CreatedUtc = ,
//                    Content = "테스트",
//                },
//                new Post
//                {
//                    Title = "테스트2",
//                    //CreatedUtc = ,
//                    Content = "ㅌㅅㅌ",
//                },
//                new Post
//                {
//                    Title = "ㅌㅅㅌㅌㅅㅌ",
//                    //CreatedUtc = ,
//                    Content = "ㅌㅅㅌ",
//                },
//                new Post
//                {
//                    Title = "ㅌㅅㅌ",
//                    //CreatedUtc = ,
//                    Content = "ㅌㅅㅌ",
//                }
//            );
//            context.SaveChanges();
//        }
//    }
//}