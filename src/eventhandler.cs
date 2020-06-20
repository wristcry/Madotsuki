using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using madotsuki.config;

namespace madotsuki {
    public class eventhandler {
        private readonly DiscordSocketClient _discord;
        private readonly List<ulong> banned = new List<ulong>();

        public eventhandler(IServiceProvider services) {
            _discord = services.GetRequiredService<DiscordSocketClient>();
        }

        public async Task init() {
            _discord.JoinedGuild += guild_joined;
            _discord.LeftGuild += guild_left;

            _discord.UserJoined += user_joined;
            _discord.UserLeft += user_left;
            _discord.UserBanned += user_banned;
            _discord.UserUnbanned += user_unbanned;
            _discord.GuildMemberUpdated += user_updated;
            //_discord.MessageDeleted += message_deleted;
            //_discord.MessageUpdated += message_updated;
        }

        private async Task guild_joined(SocketGuild server) {
            data.add_server(server.Name, server.Id);
            await Task.Delay(5);
        }

        private async Task guild_left(SocketGuild server) {
            data.remove_server(server.Name);
            await Task.Delay(5);
        }

        private async Task user_joined(SocketGuildUser user) {
            if (user.Id == _discord.CurrentUser.Id)
                return;

            var Author = new EmbedAuthorBuilder()
                    .WithName("User Joined")
                    .WithIconUrl(user.GetAvatarUrl());

            var eb = new EmbedBuilder()
                .WithColor(new Color(255, 25, 25))
                .WithDescription("Username: " + user.Username + "\n" +
                "ID: " + user.Id.ToString())
                .WithAuthor(Author);

            if (data.contains_logchannel(user.Guild.Id)) await user.Guild.GetTextChannel(data.get_logchannel(user.Guild.Id)).SendMessageAsync("", false, eb.Build());
        }

        private async Task user_left(SocketGuildUser user) {
            if (user.Id == _discord.CurrentUser.Id) return;
            if (banned.Contains(user.Id)) return;

            var Author = new EmbedAuthorBuilder()
                    .WithName("User Left")
                    .WithIconUrl(user.GetAvatarUrl());

            var eb = new EmbedBuilder()
                .WithColor(new Color(255, 25, 25))
                .WithDescription("Username: " + user.Username + "\n" +
                "ID: " + user.Id.ToString() + "\n")
                .WithAuthor(Author);

            if (data.contains_logchannel(user.Guild.Id)) await user.Guild.GetTextChannel(data.get_logchannel(user.Guild.Id)).SendMessageAsync("", false, eb.Build());
        }

        private async Task user_banned(SocketUser user, SocketGuild server) {
            if (user.Id == _discord.CurrentUser.Id) return;

            banned.Add(user.Id);

            var Author = new EmbedAuthorBuilder()
                    .WithName("User Banned")
                    .WithIconUrl(user.GetAvatarUrl());

            var eb = new EmbedBuilder()
                .WithColor(new Color(255, 25, 25))
                .WithDescription("Username: " + user.Username + "\n" +
                "ID: " + user.Id.ToString() + "\n")
                .WithAuthor(Author);

            if (data.contains_logchannel(server.Id)) await server.GetTextChannel(data.get_logchannel(server.Id)).SendMessageAsync("", false, eb.Build());

            await Task.Delay(1000);
            banned.Remove(user.Id);
        }

        private async Task user_unbanned(SocketUser user, SocketGuild server) {
            if (user.Id == _discord.CurrentUser.Id)
                return;

            var Author = new EmbedAuthorBuilder()
                    .WithName("User Unbanned")
                    .WithIconUrl(user.GetAvatarUrl());

            var eb = new EmbedBuilder()
                .WithColor(new Color(255, 25, 25))
                .WithDescription("Username: " + user.Username + "\n" +
                "ID: " + user.Id.ToString() + "\n")
                .WithAuthor(Author);

            if (data.contains_logchannel(server.Id)) await server.GetTextChannel(data.get_logchannel(server.Id)).SendMessageAsync("", false, eb.Build());
        }

        private async Task user_updated(SocketGuildUser ouser, SocketGuildUser nuser) {
            if (ouser.Id == _discord.CurrentUser.Id || nuser.Id == _discord.CurrentUser.Id) return;
            if (ouser.Activity != nuser.Activity) return;
            if (ouser.Status != nuser.Status) return;

            var Author = new EmbedAuthorBuilder()
                    .WithName("User Updated")
                    .WithIconUrl(ouser.GetAvatarUrl());

            string desc = "Username: " + ouser.Username + "\n" +
                "ID: " + ouser.Id + "\n\n";
            if (ouser.Nickname != nuser.Nickname) {
                if (ouser.Nickname == null) desc += "Nickname: " + ouser.Username + " -> " + nuser.Nickname;
                else if (nuser.Nickname == null) desc += "Nickname: " + ouser.Nickname + " -> " + nuser.Username;
                else desc += "Nickname: " + ouser.Nickname + " -> " + nuser.Nickname;
            }
            /*else if (ouser.Roles != nuser.Roles) {
                string[] ouser_roles;
                string[] nuser_roles;
                if (ouser.Roles.Count > 1) {
                    ouser_roles = new string[ouser.Roles.Count + 1];
                    ouser_roles[0] = "everyone";
                    for (int i = 1; i < ouser.Roles.Count; i++)
                        ouser_roles[i] = ouser.Roles.ToArray()[i].Name;
                }
                else {
                    ouser_roles = new string[1];
                    ouser_roles[0] = "everyone";
                }
                if (nuser.Roles.Count > 1) {
                    nuser_roles = new string[nuser.Roles.Count + 1];
                    nuser_roles[0] = "everyone";
                    for (int i = 1; i < nuser.Roles.Count; i++)
                        nuser_roles[i] = nuser.Roles.ToArray()[i].Name;
                }
                else {
                    nuser_roles = new string[1];
                    nuser_roles[0] = "everyone";
                }
                desc += "Roles: " + (ouser_roles.Length > 1 ? string.Join(',', ouser_roles) : ouser_roles[0]) + " -> " + (nuser_roles.Length > 1 ? string.Join(',', nuser_roles) : nuser_roles[0]);
                //desc = desc[desc.Length] == ',' ? desc.Remove(desc.Length - 1, 1) : desc;
                ouser_roles = null;
                nuser_roles = null;
            }*/
            else if (ouser.AvatarId != nuser.AvatarId) desc += "Avatar ID: " + ouser.AvatarId + " -> " + nuser.AvatarId;
            else return;

            var eb = new EmbedBuilder()
                .WithColor(new Color(255, 25, 25))
                .WithDescription(desc)
                .WithAuthor(Author);

            if (data.contains_logchannel(ouser.Guild.Id)) await ouser.Guild.GetTextChannel(data.get_logchannel(ouser.Guild.Id)).SendMessageAsync("", false, eb.Build());
        }
    }
}
