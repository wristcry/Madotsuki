using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using Discord.Commands;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Data;

namespace madotsuki {
    public class commands : InteractiveBase<SocketCommandContext> {
        // cuz inline and #define sucks
        // am i right, microsoft?
        private static bool is_owner(SocketUser user) {
            return user.Id.ToString() == data.ownerid;
        }

        private static bool is_allowed(SocketUser user) {
            return data.is_allowed(user.Id);
        }

        private async Task ASSERTUSER(SocketUser user) {
            if (!is_owner(user) && !is_allowed(user)) {
                await ReplyAsync("This command can only be executed by my owner or any allowed user.");
                throw new OwnerAssertationFailedException();
            }
        }

        private async Task ASSERTOWNER(SocketUser user) {
            if (!is_owner(user)) {
                await ReplyAsync("This command can only be executed by my owner.");
                throw new OwnerAssertationFailedException();
            }
        }

        private static SocketGuildUser getuser(SocketGuild guild, string name) {
            SocketGuildUser result = null;
            for (int i = 0; i < guild.Users.Count; i++)
                if (guild.Users.ToArray()[i].Username.ToLower() == name.ToLower()) result = guild.Users.ToArray()[i];
            return result;
        }

        private static SocketGuildUser getuser(SocketGuild guild, ulong id) {
            SocketGuildUser result = null;
            for (int i = 0; i < guild.Users.Count; i++)
                if (guild.Users.ToArray()[i].Id == id) result = guild.Users.ToArray()[i];
            return result;
        }

        private static SocketRole getrole(SocketGuild guild, string name) {
            SocketRole result = null;
            for (int i = 0; i < guild.Roles.Count; i++)
                if (guild.Roles.ToArray()[i].Name == name) result = guild.Roles.ToArray()[i];
            return result;
        }

        static Dictionary<ulong, poll> polls = new Dictionary<ulong, poll>();

        [Command("ping", RunMode = RunMode.Async)]
        public async Task command_ping() {
            var watch = Stopwatch.StartNew();
            await ReplyAsync("Pong!");
            watch.Stop();
            var r = (int)watch.ElapsedMilliseconds;
            await ReplyAsync("delay: " + r.ToString() + "ms");
        }

        [Command("shutdown", RunMode = RunMode.Async)]
        public async Task command_shutdown() {
            await ASSERTOWNER(Context.User);
            await ReplyAsync("\uD83D\uDC4B See ya!");
            await Context.Client.LogoutAsync();
            Process.GetCurrentProcess().Kill();
        }

        [Command("restart", RunMode = RunMode.Async)]
        public async Task command_restart() {
            await ASSERTOWNER(Context.User);
            await ReplyAsync("\uD83D\uDC4C Restarting...");
            await Context.Client.LogoutAsync();
            ProcessStartInfo procsi;
            if (!utils.is_linux()) procsi = new ProcessStartInfo("Madotsuki.exe");
            else procsi = new ProcessStartInfo("Madotsuki");
            procsi.UseShellExecute = true;
            procsi.RedirectStandardOutput = false;
            Process proc = new Process();
            proc.StartInfo = procsi;
            proc.Start();
            Process.GetCurrentProcess().Kill();
        }

        [Command("sleep", RunMode = RunMode.Async)]
        public async Task command_sleep(int time) {
            await ASSERTOWNER(Context.User);
            await ReplyAsync("\uD83D\uDC4C Entering sleep mode...");
            await Context.Client.LogoutAsync();
            await Task.Delay(time);
            ProcessStartInfo procsi;
            if (!utils.is_linux()) procsi = new ProcessStartInfo("Madotsuki.exe");
            else procsi = new ProcessStartInfo("Madotsuki");
            procsi.UseShellExecute = true;
            procsi.RedirectStandardOutput = false;
            Process proc = new Process();
            proc.StartInfo = procsi;
            proc.Start();
            Process.GetCurrentProcess().Kill();
        }

        [Command("whoami", RunMode = RunMode.Async)]
        public async Task command_whoami() {
            string role;
            if (is_owner(Context.User))
                role = "Owner";
            else if (is_allowed(Context.User))
                role = "Allowed";
            else
                role = "None";
            string[] mg;
            if (Context.User.MutualGuilds.Count > 0) {
                mg = new string[Context.User.MutualGuilds.Count];
                for (int i = 0; i < Context.User.MutualGuilds.Count; i++)
                    mg[i] = Context.User.MutualGuilds.ToArray()[i].Name;
            } 
            else mg = new string[1] { "No" };
            var eb = new EmbedBuilder()
              .WithColor(new Color(255, 25, 25))
              .WithAuthor(new EmbedAuthorBuilder().WithName("Some information about you, " + Context.User.Username))
              .WithDescription("Username: " + Context.User.Username + "\n" +
              "ID: " + Context.User.Id + "\n" +
              "Are you bot?: " + Context.User.IsBot + "\n" +
              "Are you webhook user?: " + Context.User.IsWebhook + "\n" +
              "Bot role: " + role + "\n" +
              "Our mutual servers amount: " + Context.User.MutualGuilds.Count + "\n" +
              "Our mutual servers: " + string.Join(',', mg) + "\n" +
              "Avatar ID: " + Context.User.AvatarId + "\n" +
              "Avatar: [128](" + Context.User.GetAvatarUrl(ImageFormat.Auto, (ushort)128) + ") | " +
              "[256](" + Context.User.GetAvatarUrl(ImageFormat.Auto, (ushort)256) + ") | " +
              "[512](" + Context.User.GetAvatarUrl(ImageFormat.Auto, (ushort)512) + ") | " +
              "[1024](" + Context.User.GetAvatarUrl(ImageFormat.Auto, (ushort)1024) + ")");
            await ReplyAsync("", false, eb.Build());
        }

        [Command("leaveserver", RunMode = RunMode.Async)]
        public async Task command_leaveserver(ulong guildid) {
            await ASSERTOWNER(Context.User);

            try {
                await Context.User.SendMessageAsync("Leaving guild " + Context.Client.GetGuild(guildid).Name + "[" + Context.Client.GetGuild(guildid).Id.ToString() + "]...");
                await Context.Client.GetGuild(guildid).LeaveAsync();
            }
            catch (Exception ex) {
                await Context.User.SendMessageAsync("Unable to leave guild with id " + guildid.ToString() + "\n" + "```" + "\n" + "Exception: " + "\n \n" + ex.ToString() + "\n" + "```");
            }
        }

        [Command("debug", RunMode = RunMode.Async)]
        public async Task command_debug() {
            await ASSERTOWNER(Context.User);

            var watch = Stopwatch.StartNew();
            await ReplyAndDeleteAsync(@"Collecting info...");
            watch.Stop();
            var r = (int)watch.ElapsedMilliseconds;

            long size = 0;
            FileInfo[] fis = new DirectoryInfo("data").GetFiles();
            foreach (FileInfo fi in fis) {
                size += fi.Length;
            }
            DirectoryInfo[] dis = new DirectoryInfo("data").GetDirectories();
            foreach (DirectoryInfo di in dis) {
                FileInfo[] fiss = di.GetFiles();
                foreach (FileInfo fii in fiss) {
                    size += fii.Length;
                }
            }
            size = size / 1024;

            var eb = new EmbedBuilder()
              .WithColor(new Color(255, 25, 25))
              .WithAuthor(new EmbedAuthorBuilder().WithName("Hi, " + Context.User.Username))
              .WithDescription("Delay: " + r.ToString() + "ms" + "\n" +
              "Uptime: " + utils.get_uptime() + "\n" +
              "Data Size: " + size.ToString() + "kb" + "\n" +
              "Log files amount: " + Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "data/logs")).Length.ToString() + "\n" +
              "OS: " + utils.get_OS_name() + "\n" +
              "Memory to allocate: " + ((GC.GetTotalMemory(true) / 1024) / 1024).ToString() + "mb" + "\n" +
              "Memory used: " + ((Process.GetCurrentProcess().PrivateMemorySize64 / 1024) / 1024).ToString() + "mb (" + utils.count_percents((double)(Process.GetCurrentProcess().PrivateMemorySize64 / 1024), (double)utils.get_mem_amount()).ToString() + "%)" + "\n" +
              "Framework: " + typeof(Discord.TokenUtils).Assembly.FullName.Split(',')[0] + typeof(Discord.TokenUtils).Assembly.FullName.Split(',')[1].Replace("Version=", " v") + "\n" +
              "Servers: " + Context.Client.Guilds.Count.ToString() + "\n" +
              "Polls: " + polls.Count.ToString() + "\n" +
              "Current Server Name: " + Context.Guild.Name + "\n" +
              "Current Server ID: " + Context.Guild.Id + "\n" +
              "Current Text Channel Name: " + Context.Message.Channel.Name + "\n" +
              "Current Text Channel ID: " + Context.Message.Channel.Id.ToString() + "\n" +
              "Current Message ID: " + Context.Message.Id.ToString() + "\n");

            await ReplyAsync("", false, eb.Build());
        }

        [Command("logs", RunMode = RunMode.Async)]
        public async Task command_logs() {
            await ASSERTOWNER(Context.User);
            string[] f = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "data/logs"));
            if (f.Length > 0) {
                string s = "```\n";
                for (int i = 0; i < f.Length; i++) {
                    string[] spl = f[i].Split(@"\".ToCharArray()[0]);
                    s += spl[spl.Length - 1] + "\n";
                    spl = null;
                }

                s += "```";
                await ReplyAsync(s);
                s = null;
            }
            else await ReplyAsync("Log files is not exist.");
            f = null;
        }

        [Command("getlog", RunMode = RunMode.Async)]
        public async Task command_getlog(string date) {
            await ASSERTOWNER(Context.User);
            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "data/logs/" + date + ".log"))) {
                await Context.User.SendFileAsync(Path.Combine(Directory.GetCurrentDirectory(), "data/logs/" + date + ".log"));
                await ReplyAsync(MentionUtils.MentionUser(Context.User.Id) + ", log file was sent to you in PM message.");
            }
            else await ReplyAsync("The log file for this date is missing.");
        }

        [Command("guid", RunMode = RunMode.Async)]
        public async Task command_guid(int amount, bool hyphens) {
            if (amount <= 20) {
                List<string> guids = new List<string>();
                for (int i = 0; i < amount; i++)
                    if (hyphens) guids.Add(Guid.NewGuid().ToString());
                    else guids.Add(Guid.NewGuid().ToString().Replace("-", ""));
                await ReplyAsync("```\n" + string.Join("\n", guids.ToArray()) + "\n```");
                guids = null;
            }
            else await ReplyAsync("Sorry, but you can't generate more than 20 GUIDs.");
        }

        [Command("guid", RunMode = RunMode.Async)]
        public async Task command_guid(int amount) {
            if (amount <= 20) {
                List<string> guids = new List<string>();
                for (int i = 0; i < amount; i++)
                    guids.Add(Guid.NewGuid().ToString());
                await ReplyAsync("```\n" + string.Join("\n", guids.ToArray()) + "\n```");
                guids = null;
            }
            else await ReplyAsync("Sorry, but you can't generate more than 20 GUIDs.");
        }

        [Command("guid", RunMode = RunMode.Async)]
        public async Task command_guid() {
            await ReplyAsync("```\n" + Guid.NewGuid().ToString() + "\n```");
        }

        [Command("coinflip", RunMode = RunMode.Async)]
        public async Task command_coinflip() {
            int s = random.rand_s(100);
            if (s >= 49)
                await ReplyAsync("Heads" + ", " + s);
            else
                await ReplyAsync("Tails" + ", " + s);
        }

        [Command("roll", RunMode = RunMode.Async)]
        public async Task command_roll() {
            await ReplyAsync(random.rand_s(100).ToString());
        }

        [Command("roll", RunMode = RunMode.Async)]
        public async Task command_roll(int max) {
            await ReplyAsync(random.rand_s(max).ToString());
        }

        [Command("roll", RunMode = RunMode.Async)]
        public async Task command_roll(int min, int max) {
            await ReplyAsync(random.rand_s(min, max).ToString());
        }

        [Command("invite", RunMode = RunMode.Async)]
        public async Task command_invite() {
            await Context.User.SendMessageAsync("https://discordapp.com/oauth2/authorize?client_id=706716469253242902&scope=bot&permissions=8");
            await ReplyAsync(MentionUtils.MentionUser(Context.User.Id) + ", invite link has been sent to you");
        }

        [Command("setlogchannel", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.ViewAuditLog)]
        public async Task command_setlogchannel() {
            var channel = Context.Guild.GetTextChannel((ulong)data.get_logchannel(Context.Guild.Id));
            if (data.contains_logchannel(Context.Guild.Id)) {
                if (Context.Channel.Id == channel.Id) {
                    await ReplyAsync("Channel " + Context.Channel.Name + " is already selected for logging.");
                    return;
                }
                else {
                    await ReplyAsync("This server already have channel that selected for logging: " + channel.Name);
                    return;
                }
            }
            if (data.add_logchannel(Context.Guild.Id, Context.Channel.Id)) await ReplyAsync("Channel " + Context.Channel.Name + " has been successfully selected for logging.");
            channel = null;
        }

        [Command("unsetlogchannel", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.ViewAuditLog)]
        public async Task command_unsetlogchannel() {
            if (!data.contains_logchannel(Context.Guild.Id)) {
                await ReplyAsync("This server doesn't have any channels selected for logging.");
                return;
            }
            var channel = Context.Guild.GetTextChannel(data.get_logchannel(Context.Guild.Id));
            if (channel.Id != Context.Channel.Id) {
                await ReplyAsync("Can't deselect current channel, because another channel is selected for logging: " + channel.Name);
                return;
            }
            if (data.remove_logchannel(Context.Guild.Id)) await ReplyAsync("Channel " + Context.Channel.Name + " is no longer selected for logging.");
            channel = null;
        }

        [Command("islogchannel", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.ViewAuditLog)]
        public async Task command_islogchannel() {
            if (!data.contains_logchannel(Context.Guild.Id)) {
                await ReplyAsync("This server doesn't have any channels selected for logging.");
                return;
            }
            var channel = Context.Guild.GetTextChannel(data.get_logchannel(Context.Guild.Id));
            if (Context.Channel.Id == data.get_logchannel(Context.Guild.Id)) await ReplyAsync("Channel " + Context.Channel.Name + " is selected for logging.");
            else await ReplyAsync("Channel " + Context.Channel.Name + " is not selected for logging. Channel that selected for logging: " + channel.Name);
            channel = null;
        }

        [Command("deletemessages", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task command_deletemessages() {
            var messages = await Context.Channel.GetMessagesAsync(2).FlattenAsync();
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
            await ReplyAndDeleteAsync("Last message in channel " + Context.Channel.Name + " has been deleted");
        }

        [Command("deletemessages", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task command_deletemessages(int amount) {
            if (amount > 50) {
                await ReplyAsync("Sorry, but you can't delete more than 50 messages.");
                return;
            }
            var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
            await ReplyAndDeleteAsync("Last " + amount.ToString() + " messages in channel " + Context.Channel.Name + " has been deleted");
        }

        [Command("mute", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task command_mute(ulong id) {
            var u = Context.Guild.GetUser(id); //getuser(Context.Guild, user);
            if (Context.Guild.GetUser(id) == null) {
                await ReplyAsync("User not found on this server.");
                return;
            }
            var r = getrole(Context.Guild, "Muted");
            if (r != null) {
                if (Context.Guild.GetUser(id).Roles.Contains(r)) {
                    await ReplyAsync("This user is already muted.");
                    return;
                }

                await Context.Guild.GetUser(id).AddRoleAsync(r);
                await ReplyAsync("User " + MentionUtils.MentionUser(Context.Guild.GetUser(id).Id) + " is muted now.");
            }
            else {
                GuildPermissions perms = new GuildPermissions(/*createInstantInvite*/true, /*kickMembers*/false, /*banMembers*/false, /*administrator*/false, 
                    /*manageChannels*/false, /*manageGuild*/false, /*addReactions*/true, /*viewAuditLog*/false, 
                    /*viewChannel*/false, /*sendMessages*/false, /*sendTTSMessages*/false, /*manageMessages*/false, 
                    /*embedLinks*/false, /*attachFiles*/false, /*readMessageHistory*/true, /*mentionEveryone*/false, 
                    /*useExternalEmojis*/false, /*connect*/true, /*speak*/false, /*muteMembers*/false, 
                    /*deafenMembers*/false, /*moveMembers*/false, /*useVoiceActivation*/false, /*prioritySpeaker*/false, 
                    /*stream*/false, /*changeNickname*/true, /*manageNicknames*/false, /*manageRoles*/false, 
                    /*manageWebhooks*/false, /*manageEmojis*/false);
                await Context.Guild.CreateRoleAsync("Muted", perms, null, false, null);
                r = getrole(Context.Guild, "Muted");
                await Context.Guild.GetUser(id).AddRoleAsync(r);
                await ReplyAsync("User " + MentionUtils.MentionUser(Context.Guild.GetUser(id).Id) + " is muted now.");
            }
        }

        [Command("unmute", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task command_unmute(string user) {
            var u = getuser(Context.Guild, user);
            if (u == null) {
                await ReplyAsync("User not found on this server.");
                return;
            }
            var r = getrole(Context.Guild, "Muted");
            if (r != null) {
                if (!u.Roles.Contains(r)) {
                    await ReplyAsync("This user is not muted.");
                    return;
                }

                await u.RemoveRoleAsync(r);
                await ReplyAsync("User " + MentionUtils.MentionUser(u.Id) + " is no more muted.");
            }
            else await ReplyAsync("This user is not muted.");
        }

        [Command("math", RunMode = RunMode.Async)]
        public async Task command_math(string expression) {
            try {
                await ReplyAsync(Convert.ToDouble(new DataTable().Compute(expression, null)).ToString());
            }
            catch {
                await ReplyAsync("Incorrect math expression.");
            }
        }

        [Command("poll", RunMode = RunMode.Async)]
        public async Task command_poll(string time, params string[] args) {
            try {
                if (polls.ContainsKey(Context.Guild.Id)) {
                    await ReplyAsync("This server is already have active poll right now.");
                    return;
                }

                poll p = new poll();
                p.channelid = Context.Channel.Id;
                p.start_time = DateTimeOffset.Now.ToUnixTimeSeconds();
                p.estimated_time = Convert.ToInt64(time);
                string[] a = string.Join(' ', args).Split('|');
                if (a.Length != 2) await ReplyAsync("Incorrect poll format.");
                p.name = a[0];
                List<poll_option> po = new List<poll_option>();
                string[] a2 = a[1].Split(',');
                for (int i = 0; i < a.Length; i++)
                    po.Add(new poll_option(a2[i], 0));
                p.options = po.ToArray();
                p.voters = new List<ulong>();
                polls.Add(Context.Guild.Id, p);
                a = null;
                a2 = null;
                po = null;

                string opts = string.Empty;
                foreach (var opt in p.options)
                    opts += "- " + opt.name + "\n";
                var eb_started = new EmbedBuilder()
                  .WithColor(new Color(255, 25, 25))
                  .WithAuthor(new EmbedAuthorBuilder().WithName(Context.User.Username + " has started the vote!\n\n" +
                  p.name))
                  .WithDescription("Vote will end in: " + p.estimated_time.ToString() + " seconds" + "\n" +
                  "Options: \n" +
                  opts + "\n" +
                  "Use !vote <option> for voting");
                opts = null;
                await ReplyAsync("", false, eb_started.Build());

                while (DateTimeOffset.Now.ToUnixTimeSeconds() < (polls[Context.Guild.Id].start_time + polls[Context.Guild.Id].estimated_time))
                    await Task.Delay(1000);

                polls.Remove(Context.Guild.Id);
                string results = string.Empty;
                foreach (var opt in p.options)
                    results += opt.name + " - " + utils.count_percents((double)opt.voted, (double)p.voters.Count).ToString() + "%\n";
                var eb_ended = new EmbedBuilder()
                  .WithColor(new Color(255, 25, 25))
                  .WithAuthor(new EmbedAuthorBuilder().WithName("Poll has been ended!"))
                  .WithDescription("Results: " + "\n" +
                  results);
                results = null;
                await ReplyAsync("", false, eb_ended.Build());
            }
            catch {
                await ReplyAsync("Incorrect poll format.");
            }
        }

        [Command("vote", RunMode = RunMode.Async)]
        public async Task command_vote(string option) {
            try {
                if (!polls.ContainsKey(Context.Guild.Id)) {
                    await ReplyAsync("This server doesn't have any active poll right now.");
                    return;
                }

                bool b = false;
                for (int i = 0; i < polls[Context.Guild.Id].options.Length; i++)
                    if (polls[Context.Guild.Id].options[i].name.ToLower() == option.ToLower()) b = true;
                if (!b) {
                    await ReplyAsync("Current poll doesn't have this option.");
                    return;
                }

                if (polls[Context.Guild.Id].voters.Contains(Context.User.Id)) {
                    await ReplyAsync(MentionUtils.MentionUser(Context.User.Id) + ", you already voted in this poll.");
                    return;
                }

                for (int i = 0; i < polls[Context.Guild.Id].options.Length; i++) {
                    if (polls[Context.Guild.Id].options[i].name.ToLower() == option.ToLower()) {
                        polls[Context.Guild.Id].options[i].voted += 1;
                        polls[Context.Guild.Id].voters.Add(Context.User.Id);
                    }
                }
            }
            catch {
                await ReplyAsync("Incorrect poll format.");
            }
        }
    }
}
