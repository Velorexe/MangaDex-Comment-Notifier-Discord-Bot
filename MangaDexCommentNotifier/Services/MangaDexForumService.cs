using HtmlAgilityPack;
using MangaDexCommentNotifier.Services.Abstract;
using System.Web;

namespace MangaDexCommentNotifier.Services
{
    internal class MangaDexForumService : IMangaDexForumService
    {
        /// <summary>
        /// The webscraper from <see cref="HtmlAgilityPack"/>
        /// </summary>
        private HtmlWeb _web;

        public MangaDexForumService()
        {
            _web = new HtmlWeb();
        }

        public async Task<int> GetForumCommentCountAsync(Uri forumUri)
        {
            HtmlDocument doc = await _web.LoadFromWebAsync(forumUri.ToString());
            HtmlNodeCollection commentNodes = doc.DocumentNode.SelectNodes("//article[@id]");

            return commentNodes.Count;
        }

        public async Task<string> GetForumTitle(Uri forumUri)
        {
            HtmlDocument doc = await _web.LoadFromWebAsync(forumUri.ToString());
            HtmlNode titleNode = doc.DocumentNode.SelectSingleNode("//h1[@class=\"p-title-value\"]");

            return HttpUtility.HtmlDecode(titleNode.InnerText);
        }
    }
}
