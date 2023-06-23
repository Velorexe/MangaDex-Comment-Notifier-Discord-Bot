namespace MangaDexCommentNotifier.Models
{
    [Serializable]
    internal class Settings
    {
        public List<ForumEntry> ForumUris { get; set; } = new List<ForumEntry>();

        public ulong ChannelId { get; set; } = 0;

        public byte ScanTimeoutInMinutes { get; set; } = 30;
        public byte InbetweenTimeoutInSeconds { get; set; } = 5;

        public string DiscordBotToken { get; set; } = string.Empty;
    }

    internal struct ForumEntry
    {
        public Uri Uri { get; set; }

        public int CommentAmount { get; set; }

        public ForumEntry(Uri uri)
        {
            this.Uri = uri;
            this.CommentAmount = 1;
        }

        public ForumEntry(Uri uri, int commentAmount)
        {
            this.Uri = uri;
            this.CommentAmount = commentAmount;
        }
    }
}
