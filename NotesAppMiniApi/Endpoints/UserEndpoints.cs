using NotesAppMiniApi.Models;

namespace NotesAppMiniApi.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app, List<User> users)
    {
        app.MapPost("/user", (string login) =>
        {
            users.Add(new User
            {
                Id = users.Count + 1,
                Login = login
            });

            return Results.NoContent();
        });

        app.MapGet("/user", () => users);
    }
}
