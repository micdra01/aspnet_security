using infrastructure.DataModels;
using service.Services;

namespace api.GraphQL.Types;

[GraphQLName("User")]
public class UserGql
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? AvatarUrl { get; set; }
    public required bool IsAdmin { get; set; }
    
    [GraphQLIgnore]
    public static UserGql FromModel(User model)
    {
        return new UserGql()
        {
            Id = model.Id,
            FullName = model.FullName,
            Email = model.Email,
            AvatarUrl = model.AvatarUrl,
            IsAdmin = model.IsAdmin,
        };
    }
    
    public IEnumerable<PostGql> GetPosts([Service] PostService service)
    {
        return service.GetByAuthor(this.Id).Select(PostGql.FromModel);
    }
}