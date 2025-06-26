using GenerativeAI;
using GenerativeAI.Types;
using IntCopilot.Chat.Client.Data;
using IntCopilot.DataAccess.Postgres.DataAccess;
using IntCopilot.Shell.Gemini;
using IntCopilot.Shell.Gemini.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace IntCopilot.Chat.Client;

/// <summary>
/// 主聊天应用程序，负责处理用户交互和AI对话循环。
/// </summary>
public class ChatApplication : IHostedService
{
    private readonly ILogger<ChatApplication> _logger;
    private readonly GenerativeModel _model;
    private readonly IIntSchoolFunctions _shell;

    public ChatApplication(
        ILogger<ChatApplication> logger,
        GenerativeModel model,
        IIntSchoolFunctions shell) 
    {
        _logger = logger;
        _model = model;
        _shell = shell;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("聊天应用启动...");

        // 将Shell功能注册为AI工具
        var googleTool = _shell.AsGoogleFunctionTool();
        _model.AddFunctionTool(googleTool);

        // 初始化聊天会话
        var session = _model.StartChat();

        // 欢迎信息
        AnsiConsole.Write(new FigletText("IntCopilot").Centered().Color(Color.Aqua));
        AnsiConsole.MarkupLine("[grey]输入 '[red]exit[/]' 或 '[red]quit[/]' 退出程序。[/]");
        AnsiConsole.WriteLine();

        try
        {
            await RunChatLoopAsync(session, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("用户取消，应用正在关闭。");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "应用发生致命错误。");
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
        }

        AnsiConsole.MarkupLine("[yellow]See you next time！[/]");
    }

    private async Task RunChatLoopAsync(ChatSession session, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var userMessage = AnsiConsole.Prompt(
                new TextPrompt<string>("[green]你:[/]")
                    .PromptStyle("green")
                    .AllowEmpty()
            );

            if (string.IsNullOrWhiteSpace(userMessage))
            {
                continue;
            }

            if (userMessage.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                userMessage.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            try
            {
                var request = new GenerateContentRequest();
                request.AddText(userMessage);

                AnsiConsole.Markup("[blue]IntCopilot:[/]");
                await AnsiConsole.Status()
                    .StartAsync("[yellow]IntCopilot Thinking...[/]", async ctx =>
                    {
                        await foreach (var response in session.StreamContentAsync(request, cancellationToken))
                        {
                            if (!string.IsNullOrEmpty(response.Text()))
                            {
                                AnsiConsole.Write(response.Text());
                            }
                        }
                    });

                AnsiConsole.WriteLine();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理消息时发生错误: {UserMessage}", userMessage);
                AnsiConsole.MarkupLine($"[red]抱歉，处理你的请求时发生错误: {ex.Message}[/]");
            }
            AnsiConsole.WriteLine();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("聊天应用正在停止...");
        return Task.CompletedTask;
    }
}