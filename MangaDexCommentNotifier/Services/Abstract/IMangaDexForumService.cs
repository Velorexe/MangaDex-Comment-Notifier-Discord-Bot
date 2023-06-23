namespace MangaDexCommentNotifier.Services.Abstract
{
    internal interface IMangaDexForumService
    {
        /// <summary>
        /// Returns the amount of comments on the passed MangaDex Forum URI.
        /// </summary>
        /// <param name="forumUri">The MangaDex Forum URI to scan the amount of comments of</param>
        Task<int> GetForumCommentCountAsync(Uri forumUri);

        /// <summary>
        /// Returns the title of the MangaDex Forum URI from the first comment in the comments
        /// (from MangaDex themselves).
        /// </summary>
        /// <param name="forumUri">The MangaDex Forum URI to get the title from</param>
        Task<string> GetForumTitle(Uri forumUri);
    }
}
