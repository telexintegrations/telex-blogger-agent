using BloggerAgent.Domain.Models;

namespace BloggerAgent.Domain.Commons
{
    public class CollectionType
    {
        public const string User = "Users";
        public const string Convesation = "Conversations";
        public const string Company = "Organizations";
        public const string Message = "Messages";
        public const string Blog = "Blogs";
        public const string ApiKey = "ApiKeys";

       public static string ResolveTagName<T>()
       {
           var type = typeof(T);

           return type == typeof(Company) ? CollectionType.Company :
                  type == typeof(Conversation) ? CollectionType.Convesation :
                  type == typeof(Blog) ? CollectionType.Blog :
                  type == typeof(Message) ? CollectionType.Message :
                  type == typeof(User) ? CollectionType.User :
                  type == typeof(ApiKey) ? CollectionType.ApiKey :
                  type.Name; // Fallback: use raw type name
       }
        
    }
}
