using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NoticeBoard.Core.Interfaces;
using NoticeBoard.Models;
using NoticeBoard.Infrastructure;


namespace NoticeBoard
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<AppDbContext>( //실제 디비에 연결할땐 주석처리하고 컨텍스트에 디비연결해주면되나
            //    optionsBuilder => optionsBuilder.UseInMemoryDatabase("InMemoryDb"));
            services.AddMvc().SetCompatibilityVersion((CompatibilityVersion.Version_2_2));
            services.AddScoped<INoticeBoardRepository, EFNoticeboardRepository>();
        }

        public void Configure(IApplicationBuilder app,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                var repository = app.ApplicationServices.GetService<INoticeBoardRepository>();
                //InitiallizeDatabaseAsync(repository).Wait();
            }

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }

        //public static async Task InitiallizeDatabaseAsync(INoticeBoardRepository repo)
        //{
        //    var postList = await repo.ListAsync();
        //    if (!postList.Any())
        //    {
        //        await repo.AddAsync(GetTestPost());
        //    }
        //}

        //public static Post GetTestPost()
        //{
        //    var post = new Post()
        //    {
        //        PostId = 1,
        //        Title = "Dummy Post 1",
        //        Content = "This is a dummy post.",
        //        LastUpdated = DateTime.Now,
        //        Views = 0,
        //        Category = "1",
        //        Nickname = "1"
        //    };
        //    return post;
        //}
    }
}
