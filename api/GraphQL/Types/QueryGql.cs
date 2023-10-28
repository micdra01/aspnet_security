﻿using service;
using service.Services;

namespace api.GraphQL.Types;

[GraphQLName("Query")]
public class QueryGql
{
    public UserGql? GetMe(
        [GlobalState(GlobalStateKeys.Session)] SessionData? session,
        [Service] UserService service
    )
    {
        if (session == null) return null;
        var model = service.GetById(session.UserId);
        if (model == null) return null;
        return UserGql.FromModel(model);
    }
    
    public IEnumerable<PostGql> GetPosts([Service] PostService service)
    {
        return service.GetAll().Select(PostGql.FromModel);
    }
    
    public PostGql? GetPost([Service] PostService service, int id)
    {
        var model = service.GetById(id);
        if (model == null) return null;
        return PostGql.FromModel(model);
    }
}