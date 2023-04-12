using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace NoticeBoard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateWebHostBuilder(args).Build().Run();
                //run호출하는 메인과 호스트 설정을 시작하는 코드 분리 EFcore 사용시 필요
                //앱을 실행하지 않고 호스트 구성을 하기위해 디자인 타임에 호출가능
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args) //CreateDefaultBuilder를 호출하여 호스트 설정 시작
            .UseStartup<Startup>(); 
        //호스트에서 사용할 서비스와 Request Pipeline 등을 지정하기 위해 프레임워크에서 Startup 클래스를 참조할 어셈블리를 결정
    }
}