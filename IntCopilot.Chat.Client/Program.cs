using GenerativeAI;
using GenerativeAI.Core;
using GenerativeAI.Types;
using IntCopilot.Chat.Client;
using IntCopilot.Chat.Client.Data;
using IntCopilot.DataAccess.Postgres.Configuration;
using IntCopilot.DataAccess.Postgres.DataAccess;
using IntCopilot.Shell.Gemini;
using IntCopilot.Shell.Gemini.Interfaces;
using IntSchool.Sharp.Core.Extensions;
using IntSchool.Sharp.Core.LifeCycle;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spectre.Console;

namespace IntCopilot.Chat.Client;

class Program
{
    /*static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        var userSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
        Api.Instance.XToken = userSettings.IntSchoolToken;
        
        Api.Instance.OnRemoteError += ((o,a) =>
        {
            Console.WriteLine(a.XToken);
        });
        Api.Instance.OnContentMappingError += ((o, a) =>
        {
            Console.WriteLine(a.Json);
        });
        
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger<PostgresStudentRepository> repoLogger = loggerFactory.CreateLogger<PostgresStudentRepository>();

        var dbSettings = new PostgresDbSettings
        {
            Host = userSettings.DBHost,
            Port = userSettings.DBPort,
            Username = userSettings.DBUser,
            Password = userSettings.DBPassword, 
            Database = userSettings.DBName
        };
        var currentSchoolYearId = await Api.Instance.GetCurrentSchoolYearAsync();
        var model = new GenerativeModel(userSettings.GeminiToken, userSettings.GeminiModel,systemInstruction:userSettings.ModelSystemInstruction+$"\n\nAdditional Information: Current School Year Id is:{currentSchoolYearId.SuccessResult.SchoolYearId}, Current Time is {DateTime.Now.ToUnixTimestampMilliseconds()}");
        model.FunctionCallingBehaviour = new FunctionCallingBehaviour
        {
            AutoCallFunction = true, // Gemini will suggest the function call but not execute it automatically.
            AutoReplyFunction = true,  // Gemini will automatically generate a response after the function call.
            AutoHandleBadFunctionCalls = false // Gemini will not attempt to handle errors from incorrect calls.
        };
        
        try
        {
            Console.WriteLine("Initializing repository via async factory...");
            await using var studentRepo = await PostgresStudentRepository.CreateAsync(dbSettings, repoLogger);
            Console.WriteLine("Repository initialized successfully.");
            var geminiShell = new IntCopilotGeminiShell(studentRepo);
            var googleTool = geminiShell.AsGoogleFunctionTool(); // Generated if GoogleFunctionTool = true, Use `new GenericFunctionTool(service.AsTools(),service.AsCalls())` if GoogleFunction = false `(default)`
            model.AddFunctionTool(googleTool);
            ChatSession session = model.StartChat();
            string userMessage = "Terry He妈妈电话是多少";
            var request = new GenerateContentRequest();
            request.AddText(userMessage);

            await foreach (var response in session.StreamContentAsync(request)) 
            {
                Console.Write(response.Text());
            }
        }
        catch (Exception ex)
        {
            
        }
    }*/

    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // --- 1. 配置服务 (ConfigureServices) ---

        // 绑定 appsettings.json 到强类型 AppSettings 类
        builder.Services.AddOptions<AppSettings>()
            .Bind(builder.Configuration.GetSection("AppSettings"))
            .ValidateDataAnnotations() // 启用数据注解验证
            .ValidateOnStart();       // 在启动时验证

        // 注册日志
        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddConsole();
        });
        
        // 注册 IntSchool API 单例服务
        builder.Services.AddSingleton(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<Program>>();
            var settings = sp.GetRequiredService<IOptions<AppSettings>>().Value;
            
            Api.Instance.XToken = settings.IntSchoolToken;
            
            Api.Instance.OnRemoteError += (_, a) => logger.LogError("IntSchool API 远程错误. Token: {Token}", a.XToken);
            Api.Instance.OnContentMappingError += (_, a) => logger.LogError("IntSchool API 内容映射错误. JSON: {Json}", a.Json);

            return Api.Instance;
        });

        // =========================================================================
        // == 修正点：使用接口注册异步服务 (IStudentRepository)
        // =========================================================================

        // 步骤 1: 注册一个返回 Task<IStudentRepository> 的异步工厂。
        // DI容器现在知道如何 "异步地创建" 一个 IStudentRepository。
        builder.Services.AddSingleton(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<PostgresStudentRepository>>();
            var settings = sp.GetRequiredService<IOptions<AppSettings>>().Value;
            var dbSettings = new PostgresDbSettings
            {
                Host = settings.DBHost,
                Port = settings.DBPort,
                Username = settings.DBUser,
                Password = settings.DBPassword,
                Database = settings.DBName
            };
            
            // 关键：方法返回 Task<IStudentRepository>，我们直接返回这个Task。
            return PostgresStudentRepository.CreateAsync(dbSettings, logger);
        });

        // 步骤 2: 注册解析后的 IStudentRepository 接口。
        // 任何需要注入 IStudentRepository 的服务，DI容器会先找到上面的Task，
        // 然后 await 它，最后将结果（一个IStudentRepository实例）注入。
        // 这将异步初始化对其他服务透明化。
        builder.Services.AddSingleton<IStudentRepository>(sp =>
        {
            var repoTask = sp.GetRequiredService<Task<IStudentRepository>>();
            return repoTask.GetAwaiter().GetResult(); // 在启动时同步等待是安全的
        });

        // 步骤 3: 注册 Gemini Shell。
        // DI 容器现在知道如何创建 IntCopilotGeminiShell，因为它知道如何提供其依赖项 IStudentRepository。
        // (这里假设 IntCopilotGeminiShell 的构造函数需要一个 IStudentRepository)
        builder.Services.AddSingleton<IIntSchoolFunctions, IntCopilotGeminiShell>();

        // =========================================================================

        // 注册核心的 Gemini AI 模型
        builder.Services.AddSingleton(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<GenerativeModel>>();
            var settings = sp.GetRequiredService<IOptions<AppSettings>>().Value;
            var api = sp.GetRequiredService<Api>();

            string schoolYearInfo = "Unknown";
            try 
            {
                var currentSchoolYearIdResult = api.GetCurrentSchoolYearAsync().GetAwaiter().GetResult();
                if (currentSchoolYearIdResult.IsSuccess)
                {
                    schoolYearInfo = currentSchoolYearIdResult.SuccessResult.SchoolYearId.ToString();
                }
                else
                {
                    logger.LogWarning("无法获取当前学年ID，将使用默认值。错误: {Error}", currentSchoolYearIdResult.ErrorResult.Message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "获取当前学年ID时发生异常。");
            }
            
            var systemInstruction = $"{settings.ModelSystemInstruction}\n\nAdditional Information: Current School Year Id is:{schoolYearInfo}, Current Time is {DateTime.Now.ToUnixTimestampMilliseconds()}";

            var model = new GenerativeModel(settings.GeminiToken, settings.GeminiModel, systemInstruction: systemInstruction)
            {
                FunctionCallingBehaviour = new FunctionCallingBehaviour
                {
                    AutoCallFunction = true,
                    AutoReplyFunction = true,
                    AutoHandleBadFunctionCalls = false
                }
            };
            return model;
        });

        // 注册我们的主应用逻辑为托管服务
        builder.Services.AddHostedService<ChatApplication>();

        // --- 2. 构建并运行 Host ---
        try
        {
            using var host = builder.Build();
            await host.RunAsync();
        }
        catch (OptionsValidationException ex)
        {
            AnsiConsole.MarkupLine("[bold red]错误: 配置文件 (appsettings.json) 无效。请检查以下配置项：[/]");
            foreach (var failure in ex.Failures)
            {
                AnsiConsole.MarkupLine($"[red]- {failure}[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine("[bold red]应用启动失败，发生致命错误。[/]");
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
        }
    }
}