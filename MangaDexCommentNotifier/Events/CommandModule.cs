using DisCatSharp.CommandsNext;
using DisCatSharp.CommandsNext.Attributes;
using DisCatSharp.Entities;
using MangaDexCommentNotifier.Services.Abstract;

namespace MangaDexCommentNotifier.Events
{
    internal class CommandModule : BaseCommandModule
    {
        public IMangaDexForumService ForumService { get; set; }

        /// <summary>
        /// Adds a new Mangadex Forum URI to track.
        /// </summary>
        /// <param name="ctx">Passed by <see cref="DisCatSharp"/></param>
        /// <param name="rawUri">Passed by <see cref="DisCatSharp"/>. Contains the raw message the user send.</param>
        [Command("add")]
        public async Task AddCommand(CommandContext ctx, string rawUri)
        {
            try
            {
                // Checks if the URI is valid and originates from forums.mangadex.org
                if (Uri.TryCreate(rawUri, UriKind.Absolute, out Uri? result) && result != null && result.Host == "forums.mangadex.org")
                {
                    int count = await ForumService.GetForumCommentCountAsync(result);
                    if (Program.AddUri(result, count))
                    {
                        await ctx.Message.RespondAsync(BuildMessageEmbed($"Alright! Now tracking that URI, which currently has {count} comment(s)."));
                    }
                    else
                    {
                        await ctx.Message.RespondAsync(BuildMessageEmbed($"I was already tracking this URI."));
                    }
                }
                else
                {
                    await ctx.Message.RespondAsync(BuildMessageEmbed("The URI was not in a correct format, can you check your URI again?"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Got Message: {ctx.Message.Content}");
                Console.WriteLine($"Which resulting in: ${ex.Message}");

                await ctx.Message.RespondAsync("Something went wrong");
            }
        }

        /// <summary>
        /// Removes a MangaDex Forum URI that's currently being tracked
        /// </summary>
        /// <param name="ctx">Passed by <see cref="DisCatSharp"/></param>
        /// <param name="rawUri">Passed by <see cref="DisCatSharp"/>. Contains the raw message the user send.</param>
        [Command("remove")]
        public async Task RemoveCommand(CommandContext ctx, string rawUri)
        {
            try
            {
                if (Uri.TryCreate(rawUri, UriKind.Absolute, out Uri? result) && result != null && result.Host == "forums.mangadex.org")
                {
                    if (Program.RemoveUri(result))
                    {
                        await ctx.Message.RespondAsync(BuildMessageEmbed($"Removed {rawUri}!"));
                    }
                    else
                    {
                        await ctx.Message.RespondAsync(BuildMessageEmbed("I wasn't even tracking this URI."));
                    }
                }
                else
                {
                    await ctx.Message.RespondAsync(BuildMessageEmbed("The URI was not in a correct format, can you check your URI again?"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Got Message: {ctx.Message.Content}");
                Console.WriteLine($"Which resulting in: ${ex.Message}");

                await ctx.Message.RespondAsync("Something went wrong");
            }
        }

        /// <summary>
        /// Registers the channel the message is send from as the channel to send
        /// comment updates.
        /// </summary>
        /// <param name="ctx">Passed by <see cref="DisCatSharp"/></param>
        [Command("here")]
        public async Task RegisterCommand(CommandContext ctx)
        {
            Program.SetChannel(ctx.Message.ChannelId);
            await ctx.Message.RespondAsync(BuildMessageEmbed("Alright! I'll notify all comments in this channel now."));
        }

        /// <summary>
        /// Forcibly scans the comments, will not work if a scan is already ongoing.
        /// </summary>
        /// <param name="ctx">Passed by <see cref="DisCatSharp"/></param>
        [Command("refresh")]
        public async Task RefreshCommand(CommandContext ctx)
        {
            await ctx.Message.RespondAsync(BuildMessageEmbed("Alright! Scanning all comment sections now!"));
            await Program.ScanForumsAsync(ctx.Client, ForumService);
        }

        private static DiscordEmbed BuildMessageEmbed(string title)
        {
            return new DiscordEmbedBuilder()
                .WithTitle(title)
                .WithColor(DiscordColor.Azure)
                .Build();
        }
    }
}
