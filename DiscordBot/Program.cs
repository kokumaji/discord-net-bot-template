using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DiscordBot
{
    public class Config
    {
        public string Key { get; set; }
        public string Prefix { get; set; }

    }

    class Program
    {

        private static Config _config;

        static void Main(string[] args)
        {
            _config = LoadConfig();
            new Program().RunBotAsync().GetAwaiter().GetResult();
        }

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            _client.Ready += Client_Ready;
            _client.Log += _client_Log;

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, _config.Key);

            await _client.StartAsync();

            

            await Task.Delay(-1);

        }

        private static Config LoadConfig()
        {
            string _filePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            var json = File.ReadAllText(_filePath + "/config.json");
            var config = JsonConvert.DeserializeObject<Config>(json);

            return config;

        }

        private Task Client_Ready()
        {
            return _client.SetGameAsync(String.Format("prefix '{0}'", _config.Prefix), null, ActivityType.Listening);
        }

        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);

            if (message.Author.IsBot) return;

            int argPos = 0;

            if(message.HasStringPrefix(_config.Prefix, ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
            }
        }
    }
}
