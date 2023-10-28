using infrastructure.DataModels;

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
            Content = model.Content
        };
    }
}