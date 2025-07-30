using BloggerAgent.Domain.Models;

namespace BloggerAgent.Domain.Commons.constants
{
    public class CollectionType
    {
        public const string User = "Users";
        public const string Convesation = "Conversations";
        public const string Company = "Organizations";
        public const string Message = "Messages";
        public const string Blog = "Blogs";
        public const string ApiKey = "ApiKeys";
        public const string BlogTask = "BlogTasks";

       public static string ResolveTagName<T>()
       {
           var type = typeof(T);

           return type == typeof(Company) ? Company :
                  type == typeof(Conversation) ? Convesation :
                  type == typeof(Models.Blog) ? Blog :
                  type == typeof(Message) ? Message :
                  type == typeof(User) ? User :
                  type == typeof(ApiKey) ? ApiKey :
                  type.Name; // Fallback: use raw type name
       }
        
    }
}
