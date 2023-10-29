using infrastructure.DataModels;
using service.Services;

namespace api.GraphQL.Types;

[GraphQLName("Post")]
public class PostGql
{
    public int Id { get; set; }
    public int? AuthorId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    
    [GraphQLIgnore]
    public static PostGql FromModel(Post model)
    {
        return new PostGql()
        {
            Id = model.Id,
            AuthorId = model.AuthorId,
            Title = model.Title,
        };
    }
    
    public UserGql? GetAuthor([Service] UserService service)
    {
        return UserGql.FromModel(service.GetById((int)AuthorId));
    }

    public string GetContent([Service] PostService service)
    {
        return service.GetById(Id).Content;
    }
}