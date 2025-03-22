using TelexBloggerAgent.Enums;

namespace TelexBloggerAgent.Helpers
{  

    public class RequestClassifier
    {
        private static readonly HashSet<string> BlogKeywords = new()
        { "create", "generate", "write", "blog", "article", "post", "compose" };

        private static readonly HashSet<string> TopicKeywords = new()
        { "suggest", "give", "need", "topic", "idea", "recommend" };

        public static RequestType ClassifyRequest(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return RequestType.Uncertain;

            // Normalize input: convert to lowercase and split into words
            var words = message.ToLower().Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

            int blogScore = words.Count(word => BlogKeywords.Contains(word) && words.Count() > 1);
            int topicScore = words.Count(word => TopicKeywords.Contains(word) && words.Count() > 1);

            if (blogScore > topicScore)
                return RequestType.BlogRequest;

            if (topicScore > blogScore)
                return RequestType.TopicRequest;


            return RequestType.Uncertain;
        }
    }

    // Example Usage
    public class RequestLogger
    {
        //private readonly DatabaseContext _dbContext;

        //public RequestLogger(DatabaseContext dbContext)
        //{
        //    _dbContext = dbContext;
        //}

        //public async Task LogRequest(string message, RequestType type, string channelId)
        //{
        //    var requestLog = new RequestLog
        //    {
        //        ChannelId = channelId,
        //        Message = message,
        //        RequestType = type.ToString(),
        //        Timestamp = DateTime.UtcNow
        //    };

        //    _dbContext.RequestLogs.Add(requestLog);
        //    await _dbContext.SaveChangesAsync();
        //}
    }

}
