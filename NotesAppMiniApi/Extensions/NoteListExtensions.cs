using NotesAppMiniApi.Models;

namespace NotesAppMiniApi.Extensions;

public static class NoteListExtensions
{
    public static bool TryFindUserNote(this List<Note> notes, User user, int noteId, out Note note)
    {
        note = notes.FirstOrDefault(n => n.Id == noteId && n.UserId == user.Id);

        return note != null;
    }
}
