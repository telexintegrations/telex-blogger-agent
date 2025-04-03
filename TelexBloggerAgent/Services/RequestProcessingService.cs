using TelexBloggerAgent.Dtos;
using TelexBloggerAgent.Enums;
using TelexBloggerAgent.Helpers;
using TelexBloggerAgent.IServices;

namespace TelexBloggerAgent.Services
{
    public class RequestProcessingService : IRequestProcessingService
    {
        private static readonly HashSet<string> BlogKeywords = new()
        { "create", "generate", "write", "blog", "article", "post", "compose", "piece", "draft" };

        private static readonly HashSet<string> TopicKeywords = new()
        { "suggest", "give", "need", "blog", "topic", "topics", "idea", "recommend", "blog ideas", "article ideas", "content ideas" };

        private static readonly HashSet<string> RefineKeywords = new()
        { "edit", "refine", "improve", "enhance", "polish", "revise", "rewrite", "update", "restructure", "modify", "optimize", "adjust", "tweak", "fix", "correct", "proofread", "streamline", "better", "make better", "more concise", "shorten", "expand" };


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
            else if (classification == RequestType.RefinementRequest) 
            {
                prompt = GenerateRefinementPrompt(userInput, blogDto.Settings);
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
            string systemMessage = "Your name is Mike. You are a blogging AI assistant.";

            // Retrieve settings dynamically
            string companyName = GetSettingValue(settings, "company_name");
            string companyOverview = GetSettingValue(settings, "company_overview");
            string tone = GetSettingValue(settings, "tone");

            // Base system message with company details
            systemMessage += $" You are assisting {companyName}. {companyOverview}.";


            switch (requestType)
            {
                case RequestType.BlogRequest:
                    systemMessage += " You generate well-structured, engaging, and informative blog posts." +
                                     " Your responses should be professional and insightful." +
                                     " Ensure the content is SEO-friendly and follows a " + tone + " tone.";
                    break;

                case RequestType.RefinementRequest:
                    systemMessage += " You help improve existing blog posts by making them more engaging, polished, and optimized." +
                                     " You focus on clarity, conciseness, and structure while preserving the original tone." +
                                     " Correct any grammatical errors, enhance readability, and apply SEO best practices." +
                                     " Use a " + tone + " tone as per the company's branding.";
                    break;

                case RequestType.TopicRequest:
                    systemMessage += " You provide creative and trending blog topic ideas." +
                                     " Offer unique angles that will attract readers and generate engagement." +
                                     " If the user specifies a niche, tailor suggestions accordingly." +
                                      " Ensure the topics align with " + companyName + "'s industry and audience preferences.";
                    break;

                default:
                    systemMessage += " If the request is unclear, ask the user for clarification before proceeding.";
                    break;
            }

            // Formatting preferences
            systemMessage += " Use ALL CAPS for important words, and use ✅ for bullet points." +
                             " Return responses WITHOUT markdown formatting.";

            return systemMessage;
        }

        public static RequestType ClassifyRequest(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return RequestType.Uncertain;
            }

            // Normalize input: convert to lowercase and split into words
            var words = message.ToLower().Split(new[] { ' ', '.', ',', '!', '?', '-', ';', ':' }, StringSplitOptions.RemoveEmptyEntries);

            int blogScore = words.Count(word => BlogKeywords.Contains(word));
            int topicScore = words.Count(word => TopicKeywords.Contains(word));
            int refineScore = words.Count(word => RefineKeywords.Contains(word));

            // Determine classification based on highest score
            if (refineScore > blogScore && refineScore > topicScore)
            {
                return RequestType.RefinementRequest;
            }
            else if (blogScore >= topicScore && blogScore > 1)
            {
                return RequestType.BlogRequest;
            }
            else if (topicScore > blogScore)
            {
                return RequestType.TopicRequest;
            }

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

