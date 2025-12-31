using NotesAppMiniApi.Models;

namespace NotesAppMiniApi.Extensions;

//Насколько вообще адекватно выглядит этот метод расширения, учитывая, что он добавляет методы для листов, заполненных вполне конкретными объектами из моего приложения?
//Может стоит создать какие-нибудь NotesHandler, UsersHandler и распределить функционал этих методов между ними?
public static class ListExtensions
{
    public static bool TryFindUserNote(this List<Note> notes, User user, int noteId, out Note note)
    {
        note = notes.FirstOrDefault(n => n.Id == noteId && n.UserId == user.Id);

        if (note == null)
            return false;

        return true;
    }

    public static bool TryFindUser(this List<User> users, string login, out User user)
    {
        user = users.FirstOrDefault(u => u.Login == login);

        if (user == null)
            return false;

        return true;
    }
}
