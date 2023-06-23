using DisCatSharp;
using DisCatSharp.CommandsNext;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using MangaDexCommentNotifier.Events;
using MangaDexCommentNotifier.Models;
using MangaDexCommentNotifier.Services;
using MangaDexCommentNotifier.Services.Abstract;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace MangaDexCommentNotifier
{
    internal static class Program
    {
        private static Settings m_Settings;

        private static bool m_IsScanning = false;

        static void Main(string[] args)
        {
            try
            {
                if (File.Exists("settings.json"))
                {
                    string settingsString = File.ReadAllText("settings.json");
                    Settings? settings = JsonSerializer.Deserialize<Settings>(settingsString);

                    m_Settings = settings != null ? settings : new Settings();
                }
                else
                {
                    m_Settings = new Settings();
                    WriteSettings();
                }

                Console.WriteLine($"Loaded settings, monitoring comments on {m_Settings.ForumUris.Count} forums.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Caught an error while getting settings. Exiting...");

                return;
            }

            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            ServiceProvider provider = new ServiceCollection()
                .AddSingleton<IMangaDexForumService>((_) => new MangaDexForumService())
                .BuildServiceProvider();

            if (string.IsNullOrEmpty(m_Settings.DiscordBotToken))
            {
                Console.WriteLine("A settings.json file is found, but a Discord Token has not been given.");
                return;
            }

            DiscordClient discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = m_Settings.DiscordBotToken,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContent
            });

            CommandsNextExtension commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new List<string>() { "[" },
                ServiceProvider = provider
            });

            commands.RegisterCommands<CommandModule>();

            await discord.ConnectAsync(new DiscordActivity("MangaDex comments!", ActivityType.Watching));

            // Initial scan
            await ScanForumsAsync(discord, new MangaDexForumService());

            var timer = new PeriodicTimer(TimeSpan.FromMinutes(m_Settings.ScanTimeoutInMinutes));

            // Every tick a new scan is initialized
            while (await timer.WaitForNextTickAsync())
            {
                await ScanForumsAsync(discord, new MangaDexForumService());
            }

        }

        /// <summary>
        /// Scans the MangaDex Forums that are present in the <see cref="m_Settings"/>.
        /// </summary>
        /// <param name="client">The <see cref="DiscordClient"/> to send messages from</param>
        /// <param name="service">The <see cref="IMangaDexForumService"/> to use to scan the MangaDex Forums</param>
        public static async Task ScanForumsAsync(DiscordClient client, IMangaDexForumService service)
        {
            if (!m_IsScanning)
            {
                Console.WriteLine($"Scanning {m_Settings.ForumUris.Count} forums");

                m_IsScanning = true;

                for (int i = 0; i < m_Settings.ForumUris.Count; i++)
                {
                    try
                    {
                        int count = await service.GetForumCommentCountAsync(m_Settings.ForumUris[i].Uri);

                        if (count != m_Settings.ForumUris[i].CommentAmount)
                        {
                            string title = await service.GetForumTitle(m_Settings.ForumUris[i].Uri);
                            int amount = count - m_Settings.ForumUris[i].CommentAmount;

                            DiscordEmbed embed = new DiscordEmbedBuilder()
                                .WithTitle(title)
                                .WithUrl(m_Settings.ForumUris[i].Uri)
                                .WithDescription($"Has {(amount == 1 ? "a " : " ")}{amount} new comment{(amount == 1 ? "" : "s")}!")
                                .WithColor(DiscordColor.Azure)
                                .Build();

                            await client.SendMessageAsync(await client.GetChannelAsync(m_Settings.ChannelId), embed);

                            m_Settings.ForumUris[i] = new ForumEntry(m_Settings.ForumUris[i].Uri, count);
                        }

                        // Timer inbetween requests as to not overload MangaDex's Forums
                        Thread.Sleep(TimeSpan.FromSeconds(m_Settings.InbetweenTimeoutInSeconds));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Something went wrong while checking {m_Settings.ForumUris[i].Uri}");
                        Console.WriteLine(ex.Message);
                    }
                }

                // Update values
                WriteSettings();

                m_IsScanning = false;
            }
        }

        /// <summary>
        /// Adds a new MangaDex Forum URI to the <see cref="m_Settings"/>.
        /// </summary>
        /// <param name="uri">The MangaDex Forum URI to start tracking</param>
        /// <param name="count">The original amount of comments that were already
        /// present on the MangaDex Forum URI</param>
        /// <returns><see langword="true"/> if the forum has succesfully been added.
        /// <see langword="false"/> if the forum was already being tracked</returns>
        public static bool AddUri(Uri uri, int count)
        {
            if (m_Settings.ForumUris.FindIndex(x => x.Uri == uri) != -1)
            {
                return false;
            }

            m_Settings.ForumUris.Add(new ForumEntry(uri, count));

            WriteSettings();

            return true;
        }

        /// <summary>
        /// Removes a tracked new MangaDex Forum URI from the <see cref="m_Settings"/>.
        /// </summary>
        /// <param name="uri">The MangaDex Forum URI to remove</param>
        /// <returns><see langword="true"/> if the forum has succesfully been removed.
        /// <see langword="false"/> if the forum was not yet being tracked</returns>
        public static bool RemoveUri(Uri uri)
        {
            int index = m_Settings.ForumUris.FindIndex(x => x.Uri == uri);

            if (index != -1)
            {
                m_Settings.ForumUris.RemoveAt(index);

                WriteSettings();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the Discord channel ID in the <see cref="m_Settings"/>.
        /// </summary>
        /// <param name="channel"></param>
        public static void SetChannel(ulong channel)
        {
            m_Settings.ChannelId = channel;
            WriteSettings();
        }

        /// <summary>
        /// Writes the <see cref="m_Settings"/> to disk.
        /// </summary>
        private static void WriteSettings()
        {
            string settingsString = JsonSerializer.Serialize(m_Settings);
            File.WriteAllText("settings.json", settingsString);
        }
    }
}