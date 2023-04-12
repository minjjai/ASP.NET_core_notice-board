using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection; //어떤 타입의 객체가 필요할 때 Framework이 알아서 해당 타입의 객체를 생성해서 제공해 주는 Framework 서비스
using Microsoft.Extensions.Logging;
using NoticeBoard.Core.Interfaces;
using NoticeBoard.Models;
using NoticeBoard.Infrastructure;
using Azure.Core;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace NoticeBoard
{
    //Startup - Request Pipeline
    public class Startup
    {
        //웹 프로그램에서 어떤 Framework Service를 사용할 지를 지정하거나 개발자의 Custom Type에 대한 Dependancy Injection을 정의하는 일을 함
        public void ConfigureServices(IServiceCollection services)
        {
            //IocConfig.Configure(services);

            services.AddDbContext<IAppDbContext, AppDbContext>(options => options.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=NoticeBoardContext;User ID=sa;Password=123qwe!@#QWE;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;"));
            services.AddMvc();
            services.AddScoped<INoticeBoardRepository, EFNoticeboardRepository>();
        }

        //HTTP Request가 들어오면 ASP.NET이 어떻게 반응해야 하는지를 지정하는 것
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //}
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
