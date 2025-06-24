using GenerativeAI;
using GenerativeAI.Core;
using GenerativeAI.Types;
using IntCopilot.Chat.Client.Data;
using IntCopilot.DataAccess.Postgres.Configuration;
using IntCopilot.DataAccess.Postgres.DataAccess;
using IntCopilot.Shell.Gemini;
using IntCopilot.Shell.Gemini.Interfaces;
using IntSchool.Sharp.Core.LifeCycle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IntCopilot.Chat.Client;

class Program
{
    static async Task Main(string[] args)
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
        
        var model = new GenerativeModel(userSettings.GeminiToken, userSettings.GeminiModel,systemInstruction:userSettings.ModelSystemInstruction);
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
            string userMessage = "jerry最近请假都是啥原因";
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
    }
}