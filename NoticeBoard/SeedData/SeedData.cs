using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using noticeboard.models;
using NoticeBoard.Core.Model;
using System;
using System.Linq;

namespace NoticeBoard.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new NoticeBoardContext(
            serviceProvider.GetRequiredService<
                DbContextOptions<NoticeBoardContext>>()))
        {
            // Look for any FixedCategories.
            if (context.FixedCategories.Any())
            {
                return;   // DB has been seeded
            }
            context.FixedCategories.AddRange(
                new FixedCategory
                {
                    Categories = "OOTD",
                },
                new FixedCategory
                {
                    Categories = "Chat",
                },
                new FixedCategory
                {
                    Categories = "DevelopmentStory",
                },
                new FixedCategory
                {
                    Categories = "Stock",
                },
                new FixedCategory
                {
                    Categories = "Anime",
                }
            );
            context.SaveChanges();
        }
    }
}