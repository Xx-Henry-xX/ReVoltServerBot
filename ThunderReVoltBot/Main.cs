using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Discord.Net;

public class Program
{
    private DiscordSocketClient _client;

    public static Task Main(string[] args) => new Program().MainAsync();

    public async Task MainAsync()
    {
        _client = new DiscordSocketClient();

        _client.Log += Log;
        _client.Ready += Client_Ready;
        _client.SlashCommandExecuted += SlashCommandHandler;

        //안전상의 이유로 토큰 하드코딩은 비추
        var token = File.ReadAllText("token.txt");
        //var token = Environment.GetEnvironmentVariable("DiscordBotToken");

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(-1);
    }

    private static Task Log(LogMessage message)
    {
        switch (message.Severity)
        {
            case LogSeverity.Critical:
            case LogSeverity.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogSeverity.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogSeverity.Info:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogSeverity.Verbose:
            case LogSeverity.Debug:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
        }
        Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
        Console.ResetColor();
        
        return Task.CompletedTask;
    }

    public async Task Client_Ready()
    {
        ulong guildId = 932920780080173056; //팀 X_H_X 섭 id
        //ulong guildId = 394428307749339144; //리볼트 아시아 섭 id

        //특정 길드(디코섭) 한정 커맨드 생성
        var guild = _client.GetGuild(guildId);
        var guildCommand = new SlashCommandBuilder()
            .WithName("maplist")
            .WithDescription("Display a link to the list of map and their id (for use with /map)");

        //글로벌 커맨드 생성
        //var globalCommand = new SlashCommandBuilder()
        //    .WithName("first-global-command")
        //    .WithDescription("This is my first global slash command");

        try
        {
            // builder 1개당 Create 1번
            await guild.CreateApplicationCommandAsync(guildCommand.Build());

            // 글로벌의 경우 _client에 직접
            // await _client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
        }
        catch (HttpException exception)
        {
            await Log(new LogMessage(LogSeverity.Critical, "Client_Ready()", "Exception thrown while preparing commands", exception));
        }
    }

    private async Task SlashCommandHandler(SocketSlashCommand command)
    {
        switch (command.Data.Name)
        {
            case "maplist":
                await command.RespondAsync("https://docs.google.com/spreadsheets/d/1ihspscSFO9AOp2E8sD2ntKl7tlqSHd8NQfYOdf-s14g/edit#gid=610019541");
                break;
        }
    }
}
