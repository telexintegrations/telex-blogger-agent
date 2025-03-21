using TelexBloggerAgent.Dtos;
using TelexBloggerAgent.Enums;
using TelexBloggerAgent.IServices;

namespace TelexBloggerAgent.Services
{
    public class RequestProcessingService 
    {
        private static readonly HashSet<string> BlogKeywords = new()
        { "create", "generate", "write", "blog", "article", "post", "compose" };

        private static readonly HashSet<string> TopicKeywords = new()
        { "suggest", "give", "need", "topic", "idea", "recommend" };


        private readonly IBlogAgentService _aiService;

        public RequestProcessingService(IBlogAgentService aiService)
        {
            _aiService = aiService;
        }

        public async Task<string> ProcessUserInputAsync(string userInput)
        {
            // Step 1: Classify request (e.g., fetch blog, summarize, generate, etc.)
            var classification = ClassifyRequest(userInput).ToString();

            // Step 2: Generate appropriate prompt based on classification
            var prompt = GeneratePrompt(userInput, classification);

            // Step 3: Send to AI and retrieve response
            var aiResponse = await _aiService.GenerateResponse(prompt);

            // Step 4: Return structured response
            return aiResponse;
            
        }

        private string GetSettingValue(List<Setting> settings, string key)
        {
            return settings.FirstOrDefault(s => s.Label == key)?.Default.ToString() ?? "";
        }

        private string GeneratePrompt(string userInput, string classification)
        {
            return classification switch
            {
                "FetchBlogDraft" => $"Retrieve the latest blog draft related to: {userInput}",
                "TopicRequest" => $"Summarize the following: {userInput}",
                "BlogRequest" => $"Generate a blog post on: {userInput}",
                _ => userInput // Default case
            };
        }

        private string FormatBlogPrompt(string userPrompt, List<Setting> settings)
        {
            // Retrieve settings dynamically
            string companyName = GetSettingValue(settings, "company_name");
            string companyOverview = GetSettingValue(settings, "company_overview");
            string companyWebsite = GetSettingValue(settings, "company_website");
            string tone = GetSettingValue(settings, "tone");
            string blogLength = GetSettingValue(settings, "blog_length");
            string format = GetSettingValue(settings, "format");

            // Base prompt structure
            string prompt = $"{userPrompt}. Your name name is Mike. You are a blog expert";

            // Adjust tone dynamically
            prompt += tone switch
            {
                "Professional" => "Use a formal and authoritative tone suitable for industry professionals.",
                "Casual" => " Use a conversational and friendly tone to keep the content engaging.",
                "Persuasive" => " Craft the content in a marketing-focused way, encouraging action and conversions.",
                "Informative" => " Keep the content objective, educational, and easy to understand.",
                _ => ""
            };

            // Incorporate company branding if provided
            if (!string.IsNullOrWhiteSpace(companyName) && !string.IsNullOrWhiteSpace(companyOverview))
            {
                prompt += $" Align the content with the company, {companyName}; {companyOverview}.";
            }

            // Adjust content length
            prompt += blogLength switch
            {
                "short" => " Keep the article concise, around 300 words, focusing on key points.",
                "medium" => " Provide a balanced article, around 600 words, with in-depth insights.",
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
            string systemMessage = "Your name is Mike. You are an AI writing assistant designed for blogging and content generation.";

            string companyName = GetSettingValue(settings, "company_name");
            string companyOverview = GetSettingValue(settings, "company_overview");
            string tone = GetSettingValue(settings, "tone");

            if (!string.IsNullOrWhiteSpace(companyName) && !string.IsNullOrWhiteSpace(companyOverview))
            {
                systemMessage += $" The content should align with {companyName}'s brand: {companyOverview}.";
            }

            systemMessage += tone switch
            {
                "Professional" => " Maintain a professional and authoritative tone.",
                "Casual" => " Use a conversational and friendly tone.",
                "Persuasive" => " Craft persuasive, marketing-focused content.",
                "Informative" => " Keep the content clear, educational, and factual.",
                _ => ""
            };

            return systemMessage;
        }

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
    }
}
