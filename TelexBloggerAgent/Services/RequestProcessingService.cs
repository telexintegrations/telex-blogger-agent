using TelexBloggerAgent.Dtos;
using TelexBloggerAgent.Enums;
using TelexBloggerAgent.Helpers;
using TelexBloggerAgent.IServices;

namespace TelexBloggerAgent.Services
{
    public class RequestProcessingService : IRequestProcessingService
    {
        private static readonly HashSet<string> BlogKeywords = new()
        { "create", "generate", "write", "blog", "article", "post", "compose" };

        private static readonly HashSet<string> TopicKeywords = new()
        { "suggest", "give", "need", "blog", "topic", "topics", "idea", "recommend" };

        //private readonly CompanyService _companyService;

        //public RequestProcessingService(CompanyService companyService)
        //{
        //    _companyService = companyService;
        //}

        private string GetSettingValue(List<Setting> settings, string key)
        {
            return settings.FirstOrDefault(s => s.Label == key)?.Default.ToString() ?? "";
        }

        public async Task<Request> ProcessUserInputAsync(GenerateBlogDto blogDto)
        {
            
            var userInput = blogDto.Message;

            // Step 1: Classify request (e.g., fetch blog, summarize, generate, etc.)
            var classification = ClassifyRequest(userInput);

            var prompt = userInput;
            var systemMessage = GenerateSystemMessage(classification, blogDto.Settings);

            // Step 2: Generate appropriate prompt based on classification
            if (classification == RequestType.BlogRequest)
            {
                prompt = GenerateBlogPrompt(userInput, blogDto.Settings);
            }
           
            // Step 4: Return structured response
            return new Request
            {
                SystemMessage = systemMessage,
                UserPrompt = prompt
            };
        }



        private string GenerateBlogPrompt(string userPrompt, List<Setting> settings)
        {
            // Retrieve settings dynamically
            string companyName = GetSettingValue(settings, "company_name");
            string companyOverview = GetSettingValue(settings, "company_overview");
            string companyWebsite = GetSettingValue(settings, "company_website");
            string tone = GetSettingValue(settings, "tone");
            string blogLength = GetSettingValue(settings, "blog_length");
            string format = GetSettingValue(settings, "format");

            // Base prompt structure
            string prompt = $"{userPrompt}.";

            // Adjust tone dynamically
            prompt += tone switch
            {
                "Professional" => " Use a formal and authoritative tone suitable for industry professionals.",
                "Casual" => " Use a conversational and friendly tone to keep the content engaging.",
                "Persuasive" => " Craft the content in a marketing-focused way, encouraging action and conversions.",
                "Informative" => " Keep the content objective, educational, and easy to understand.",
                _ => ""
            };

            // Incorporate company branding if provided
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                prompt += $" Align the content with the company, {companyName}";
            }

            if (!string.IsNullOrWhiteSpace(companyOverview))
            {
                prompt += $" Align the content with the company {companyOverview}.";
            }

            // Adjust content length
            prompt += blogLength switch
            {
                "short" => " Keep the article concise, around 400 words, focusing on key points.",
                "medium" => " Provide a balanced article, around 800 words, with in-depth insights.",
                "long" => " Create a comprehensive article, around 1000+ words, with detailed analysis.",
                _ => ""
            };

            // Format preference
            prompt += format switch
            {
                "Outline" => " Return the response as a structured outline with key points.",
                "Summary" => " Summarize the content concisely, highlighting key takeaways.",
                "Full Article" => " Structure the response with a title, introduction, body, and conclusion, but do not explicitly categorize them.",
                _ => ""
            };

            if (!string.IsNullOrWhiteSpace(companyWebsite))
            {
                prompt += $" Add a call to action in the conclusion of the article. Use the raw link to the company website: {companyWebsite}";
            }


            // Formatting preferences
            prompt += " Use ALL CAPS for section headers and important words, and use ✅ for bullet points.";
            prompt += " Return the content as plain text without markdown formatting.";

            return prompt;
        }

        private string GenerateSystemMessage(RequestType requestType, List<Setting> settings)
        {
            string systemMessage = "Your name is Mike. You are a blogger agent designed to help companies and its users with blogging and content generation." +
                "If the user asks for you to generate or write or blog post, ensure the response is a well-structured, engaging, and informative article." +
                "The responses should align with company's brand." +
                "If the user asks for topics or ideas, ensure to include trending keywords too." +
                "Only Introduce yourself when getting acquainted with the user for the first time" +
                "Use ALL CAPS for important words, and use ✅ for bullet points." +
                "Return response without markdown formatting";          

            return systemMessage;
        }

        public static RequestType ClassifyRequest(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return RequestType.Uncertain;

            // Normalize input: convert to lowercase and split into words
            var words = message.ToLower().Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

            int blogScore = words.Count(word => BlogKeywords.Contains(word));
            int topicScore = words.Count(word => TopicKeywords.Contains(word));

            if (blogScore > topicScore && blogScore > 2)
                return RequestType.BlogRequest;

            return RequestType.Uncertain;
        }

        public string GetBlogIntervalOption(GenerateBlogDto blogDto)
        {
            // Retrieve settings dynamically
            string blogInterval = GetSettingValue(blogDto.Settings, "blog_generation_interval");

            return blogInterval;
        }


        public async Task<Request> ProcessRefinementRequestAsync(RefineBlogDto blogDto)
        {
            var userInput = blogDto.Message;

            // Step 1: Generate refinement prompt
            var prompt = GenerateRefinementPrompt(userInput, blogDto.Settings);
            var systemMessage = GenerateSystemMessage(RequestType.RefinementRequest, blogDto.Settings);

            // Step 2: Return structured response
            return new Request
            {
                SystemMessage = systemMessage,
                UserPrompt = prompt
            };
        }

        private string GenerateRefinementPrompt(string userPrompt, List<Setting> settings)
        {
            // Retrieve settings dynamically
            string companyName = GetSettingValue(settings, "company_name");
            string companyOverview = GetSettingValue(settings, "company_overview");
            string companyWebsite = GetSettingValue(settings, "company_website");
            string tone = GetSettingValue(settings, "tone");
            string blogLength = GetSettingValue(settings, "blog_length");
            string format = GetSettingValue(settings, "format");

            // Base prompt structure
            string prompt = $"Refine the following content based on the user's request: {userPrompt}. " +
                           "Make the following changes: {userPrompt}. " +
                           "Return the refined content as plain text without markdown formatting.";

            // Adjust tone dynamically
            prompt += tone switch
            {
                "Professional" => " Use a formal and authoritative tone suitable for industry professionals.",
                "Casual" => " Use a conversational and friendly tone to keep the content engaging.",
                "Persuasive" => " Craft the content in a marketing-focused way, encouraging action and conversions.",
                "Informative" => " Keep the content objective, educational, and easy to understand.",
                _ => ""
            };

            // Incorporate company branding if provided
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                prompt += $" Align the content with the company, {companyName}";
            }

            if (!string.IsNullOrWhiteSpace(companyOverview))
            {
                prompt += $" Align the content with the company {companyOverview}.";
            }

            // Adjust content length
            prompt += blogLength switch
            {
                "short" => " Keep the article concise, around 400 words, focusing on key points.",
                "medium" => " Provide a balanced article, around 800 words, with in-depth insights.",
                "long" => " Create a comprehensive article, around 1000+ words, with detailed analysis.",
                _ => ""
            };

            // Format preference
            prompt += format switch
            {
                "Outline" => " Return the response as a structured outline with key points.",
                "Summary" => " Summarize the content concisely, highlighting key takeaways.",
                "Full Article" => " Structure the response with a title, introduction, body, and conclusion, but do not explicitly categorize them.",
                _ => ""
            };

            if (!string.IsNullOrWhiteSpace(companyWebsite))
            {
                prompt += $" Add a call to action in the conclusion of the article. Use the raw link to the company website: {companyWebsite}";
            }

            // Formatting preferences
            prompt += " Use ALL CAPS for section headers and important words, and use ✅ for bullet points.";
            prompt += " Return the content as plain text without markdown formatting.";

            return prompt;
        }
    }
}

