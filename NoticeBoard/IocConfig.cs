using Microsoft.EntityFrameworkCore;
using NoticeBoard.Core.Interfaces;
using NoticeBoard.Infrastructure;

namespace NoticeBoard
{
    public class IocConfig
    {
        public static IServiceCollection Configure(IServiceCollection services)
        {
            services.AddDbContext< IAppDbContext, AppDbContext > (options => options.UseInMemoryDatabase("TestDb"));
            services.AddScoped<INoticeBoardRepository, EFNoticeboardRepository>();
            services.AddMvc();
            return services;
        }
    }
}
