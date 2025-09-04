namespace BloggerAgent.Domain.IRepositories
{
    public interface IEntity
    {
        string Id { get; set; }
        string Tag { get; set; }    
    }
}