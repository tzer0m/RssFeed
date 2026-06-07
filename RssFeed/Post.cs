namespace RssFeed
{
    /// <summary>
    /// Post object
    /// </summary>
    public class Post
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Summary
        /// </summary>
        public string? Summary { get; set; }

        /// <summary>
        /// Content
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// Published at time
        /// </summary>
        public DateTime PublishedAt { get; set; }
    }
}