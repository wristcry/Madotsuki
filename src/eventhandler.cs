using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            data.server_add(server.Name.ToString(), server.Id.ToString());
            await Task.Delay(5);
        }

        private async Task guild_left(SocketGuild server) {
            data.server_remove(server.Id.ToString());
            await Task.Delay(5);
        }

        private async Task user_joined(SocketGuildUser user) {
            if (user.Id == _discord.CurrentUser.Id)
                return;

            ulong channelid = data.logchannel_get(user.Guild.Id.ToString());

            var Author = new EmbedAuthorBuilder()
                    .WithName("User Joined")
                    .WithIconUrl(user.GetAvatarUrl());

            var eb = new EmbedBuilder()
                .WithColor(new Color(255, 25, 25))
                .WithDescription("Username: " + user.Username + "\n" +
                "ID: " + user.Id.ToString())
                .WithAuthor(Author);

            if (channelid != 0)
                await user.Guild.GetTextChannel(channelid).SendMessageAsync("", false, eb.Build());
        }

        private async Task user_left(SocketGuildUser user) {
            if (user.Id == _discord.CurrentUser.Id) return;
            if (banned.Contains(user.Id)) return;
            
            ulong channelid = data.logchannel_get(user.Guild.Id.ToString());

            var Author = new EmbedAuthorBuilder()
                    .WithName("User Left")
                    .WithIconUrl(user.GetAvatarUrl());

            var eb = new EmbedBuilder()
                .WithColor(new Color(255, 25, 25))
                .WithDescription("Username: " + user.Username + "\n" +
                "ID: " + user.Id.ToString() + "\n")
                .WithAuthor(Author);

            if (channelid != 0)
                await user.Guild.GetTextChannel(channelid).SendMessageAsync("", false, eb.Build());
        }

        private async Task user_banned(SocketUser user, SocketGuild server) {
            if (user.Id == _discord.CurrentUser.Id) return;

            banned.Add(user.Id);

            ulong channelid = data.logchannel_get(server.Id.ToString());

            var Author = new EmbedAuthorBuilder()
                    .WithName("User Banned")
                    .WithIconUrl(user.GetAvatarUrl());

            var eb = new EmbedBuilder()
                .WithColor(new Color(255, 25, 25))
                .WithDescription("Username: " + user.Username + "\n" +
                "ID: " + user.Id.ToString() + "\n")
                .WithAuthor(Author);

            if (channelid != 0)
                await server.GetTextChannel(channelid).SendMessageAsync("", false, eb.Build());

            await Task.Delay(1000);
            banned.Remove(user.Id);
        }

        private async Task user_unbanned(SocketUser user, SocketGuild server) {
            if (user.Id == _discord.CurrentUser.Id)
                return;

            ulong channelid = data.logchannel_get(server.Id.ToString());

            var Author = new EmbedAuthorBuilder()
                    .WithName("User Unbanned")
                    .WithIconUrl(user.GetAvatarUrl());

            var eb = new EmbedBuilder()
                .WithColor(new Color(255, 25, 25))
                .WithDescription("Username: " + user.Username + "\n" +
                "ID: " + user.Id.ToString() + "\n")
                .WithAuthor(Author);

            if (channelid != 0)
                await server.GetTextChannel(channelid).SendMessageAsync("", false, eb.Build());
        }

        private async Task user_updated(SocketGuildUser ouser, SocketGuildUser nuser) {
            if (ouser.Id == _discord.CurrentUser.Id || nuser.Id == _discord.CurrentUser.Id) return;
            if (ouser.Activity != nuser.Activity) return;
            if (ouser.Status != nuser.Status) return;

            ulong channelid = data.logchannel_get(ouser.Guild.Id.ToString());

            var Author = new EmbedAuthorBuilder()
                    .WithName("User Updated")
                    .WithIconUrl(ouser.GetAvatarUrl());

            string desc = "Username: " + ouser.Username + "\n" +
                "ID: " + ouser.Id + "\n\n";
            if (ouser.Nickname != nuser.Nickname) desc += "Nickname: " + ouser.Nickname + " -> " + nuser.Nickname;
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

            if (channelid != 0)
                await ouser.Guild.GetTextChannel(channelid).SendMessageAsync("", false, eb.Build());
        }
    }
}
