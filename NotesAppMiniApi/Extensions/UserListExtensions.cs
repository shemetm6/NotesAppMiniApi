using NotesAppMiniApi.Models;

namespace NotesAppMiniApi.Extensions;

public static class UserListExtensions
{
    public static bool TryFindUser(this List<User> users, string login, out User user)
    {
        user = users.FirstOrDefault(u => u.Login == login);

        return user != null;
    }
}
