using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Discord.Addons.Interactive;
using System.Diagnostics;
using System.Threading;

namespace madotsuki {
    class Program {
        public const int MAX_THREADS = 3;
        
        public static DiscordSocketClient _client;
        public static thread[] _threads = new thread[MAX_THREADS];

        static void Main(string[] args) {
            Thread.Sleep(1500);
            _threads[0] = new thread("Main Thread", 0, Thread.CurrentThread);

            if (!utils.is_supported()) {
                debug.log("Sorry, but your OS is not supported by Madotsuki.");
                Console.ReadKey();
                Process.GetCurrentProcess().Kill();
            }

            utils.init();
            data.init();
            debug.init();
            
            for (int i = 0; i < _threads.Length; i++)
                if (_threads[i].id != 0) _threads[i].t.Start();
            
            new Program().maintask().GetAwaiter().GetResult();
            
            Console.ReadKey();
        }

        public async Task maintask() {
            using (var services = init_services()) {
                _client = services.GetRequiredService<DiscordSocketClient>();

                _client.Log += log;
                services.GetRequiredService<CommandService>().Log += log;

                try {
                    await _client.LoginAsync(TokenType.Bot, data.token);
                    await _client.StartAsync();
                }
                catch {
                    debug.log("Invalid token. Please, make sure that you inserted valid token into the configuration file." + "\n \n" + "This window will automatically close in 15 seconds.");
                    for (int i = 0; i < _threads.Length; i++)
                        if (_threads[i].id != 0) _threads[i].t.Abort();
                    await Task.Delay(15000);
                    Process.GetCurrentProcess().Kill();
                }

                await services.GetRequiredService<commandhandler>().init();
                await services.GetRequiredService<eventhandler>().init();

                await _client.SetStatusAsync(UserStatus.DoNotDisturb);
                await _client.SetGameAsync(data.prefix + "help");

                await Task.Delay(-1);
            }
        }

        private ServiceProvider init_services() {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<commandhandler>()
                .AddSingleton<HttpClient>()
                .AddSingleton<InteractiveService>()
                .AddSingleton<eventhandler>()
                .BuildServiceProvider();
        }

        private Task log(LogMessage log) {
            // skip log message if exception was called by owner assertation
            if (log.ToString().Contains("AssertationFailedException"))
                return Task.CompletedTask;
            debug.log("[Discord.NET] " + log.ToString());
            return Task.CompletedTask;
        }

        private Task ready() {
            debug.log($"{_client.CurrentUser} is connected!");
            return Task.CompletedTask;
        }
    }
}
