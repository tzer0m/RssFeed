using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Xml.Linq;

namespace RssFeed.Controllers
{
    /// <summary>
    /// Feed controller
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="configuration">Application configuration</param>
    [ApiController]
    [Route("")]
    public class FeedController(AppDbContext context, IConfiguration configuration) : ControllerBase
    {
        /// <summary>
        /// Database context
        /// </summary>
        private readonly AppDbContext Context = context;

        /// <summary>
        /// Configuration
        /// </summary>
        private readonly IConfiguration Configuration = configuration;

        /// <summary>
        /// Get RSS feed
        /// </summary>
        /// <param name="count">Number of items to return (max 100)</param>
        /// <returns>Posts as an RSS feed</returns>
        [HttpGet("")]
        public IActionResult GetFeed(int count = 20)
        {
            count = Math.Min(count, 100);
            List<Post> items = [.. Context.RSS.OrderByDescending(p => p.PublishedAt).Take(count)];

            string baseUrl = Configuration["BaseUrl"] ?? "http://localhost:5239";

            XDocument feed = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("rss",
                    new XAttribute("version", "2.0"),
                    new XAttribute(XNamespace.Xmlns + "content", "http://purl.org/rss/1.0/modules/content/"),
                    new XElement("channel",
                        new XElement("title", "My Feed"),
                        new XElement("link", $"{baseUrl}"),
                        new XElement("description", "Latest posts"),
                        items.Select(p =>
                            new XElement("item",
                                new XElement("title", p.Title),
                                new XElement("description", p.Summary),
                                new XElement(XName.Get("encoded", "http://purl.org/rss/1.0/modules/content/"), p.Content),
                                new XElement("pubDate", p.PublishedAt.ToString("R")),
                                new XElement("guid", new XAttribute("isPermaLink", "false"), p.Id.ToString()
                                )
                            )
                        )
                    )
                )
            );

            return Content(feed.ToString(), "application/rss+xml", Encoding.UTF8);
        }

        /// <summary>
        /// Create a new post
        /// </summary>
        /// <param name="post">The post to create</param>
        /// <returns>200 if posted, 401 if not authorized</returns>
        [HttpPost("post")]
        public async Task<IActionResult> CreatePost([FromBody] Post post)
        {
            string? apiKey = Request.Headers["X-API-Key"].FirstOrDefault();
            if (apiKey != Configuration["ApiKey"])
                return Unauthorized();

            post.PublishedAt = DateTime.UtcNow;
            Context.RSS.Add(post);
            await Context.SaveChangesAsync();
            return Ok(post);
        }
    }
}