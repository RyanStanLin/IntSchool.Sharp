using IntCopilot.DataAccess.Postgres.Configuration;
using IntCopilot.Sniffer.StudentId.DependencyInjection;
using IntCopilot.Sniffer.StudentId.Worker;

namespace IntCopilot.Sniffer.StudentId.Client;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                // 1. 获取配置对象
                // hostContext.Configuration 已经自动加载了 appsettings.json
                var snifferConfigSection = hostContext.Configuration.GetSection("Sniffer");
                var postgresDbConfigSection = hostContext.Configuration.GetSection("Postgres");

                // 2. 调用你的DI扩展方法
                // 这个方法会完成所有服务的注册
                services.AddStudentSniffer(options =>
                {
                    // 3. 将配置节绑定到 options 对象上
                    snifferConfigSection.Bind(options);
                });
                
                services.Configure<PostgresDbSettings>(postgresDbConfigSection);

                // 4. (可选) 如果你想额外添加事件日志服务，可以在这里添加
                services.AddHostedService<SnifferEventLogger>();
                services.AddHostedService<ResultPersistenceService>();

                // 注意：你不再需要 services.AddHostedService<Worker>() 这一行了
                // 因为 AddStudentSniffer 内部已经注册了 StudentIdSnifferWorker
            });
}