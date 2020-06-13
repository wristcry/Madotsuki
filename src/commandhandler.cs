﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
//using Microsoft.Extensions.Configuration;

namespace madotsuki {
    public class commandhandler {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;

        public commandhandler(IServiceProvider services) {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            _commands.CommandExecuted += command_executed;
            _discord.MessageReceived += message_received;
        }

        public async Task init() {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task message_received(SocketMessage rawMessage) {
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;
            if (!message.HasStringPrefix(config.prefix, ref argPos)) return;

            var context = new SocketCommandContext(_discord, message);
            if (context.Guild != null)
                debug.log(context.User.Username + "[id=" + context.User.Id + ", guild_id=" + context.Guild.Id + ", guild_name=" + context.Guild.Name + "]" + ": " + context.Message.Content);
            else
                debug.log(context.User.Username + "[id=" + context.User.Id + ", pm message" + "]" + ": " + context.Message.Content);
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        public async Task command_executed(Optional<CommandInfo> command, ICommandContext context, IResult result) {
            if (!command.IsSpecified)
                return;

            if (result.IsSuccess)
                return;

            // return if exception was called by owner assertation
            if (result.ToString().Contains("AssertationFailedException"))
                return;

            if (result.ToString().Contains("BadArgCount")) {
                await context.Channel.SendMessageAsync("Invalid command arguments.");
                return;
            }

            await context.Channel.SendMessageAsync("error: " + result);
        }
    }
}