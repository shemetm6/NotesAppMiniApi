using NotesAppMiniApi.Extensions;
using NotesAppMiniApi.Models;

namespace NotesAppMiniApi.Endpoints;

public static class NoteEndpoints
{
    public static void MapNoteEndpoints(this WebApplication app, List<User> users, List<Note> notes)
    {
        app.MapPost("/note", (string login, string title, string description) =>
        {
            if (users.TryFindUser(login, out User user) == false)
                return Results.NotFound();

            notes.Add(new Note()
            {
                Id = notes.Count + 1, //может продублироваться id при создании новой заметки, но мне кажется, что я и так перемудрил 
                Title = title,
                Description = description,
                CreationDate = DateTime.UtcNow,
                CompletionStatus = false,
                UserId = user.Id,
                ExecutionPriority = ExecutionPriority.Low
            });

            return Results.NoContent(); //вот здесь у меня изначально был Results.Ok(notes), по аналогии с консольным приложением, но есть ощущение, что возвращать конкретные данные должен только GET-запрос. Это так?
        });

        app.MapGet("/note", (string login, bool? completionStatus, ExecutionPriority? executionPriority) =>
        {
            if (users.TryFindUser(login, out User user) == false)
                return Results.NotFound();

            var query = notes.Where(n => n.UserId == user.Id);

            if (completionStatus != null)
                query = query.Where(n => n.CompletionStatus == completionStatus);

            if (executionPriority != null)
                query = query.Where(n => n.ExecutionPriority == executionPriority);

            var filteredNotes = query.ToList();

            if (filteredNotes.Count == 0)
                return Results.NotFound();

            return Results.Ok(filteredNotes);
        });

        app.MapPut("/note", (string login,
            int noteId,
            string? title,
            string? description,
            bool? completionStatus,
            ExecutionPriority? executionPriority) =>
        {
            if (users.TryFindUser(login, out User user) == false)
                return Results.NotFound();

            if (notes.TryFindUserNote(user, noteId, out Note note) == false)
                return Results.NotFound();

            note.Title = title ?? note.Title;
            note.Description = description ?? note.Description;
            note.CompletionStatus = completionStatus ?? note.CompletionStatus;
            note.ExecutionPriority = executionPriority ?? note.ExecutionPriority; //Было бы круто в интерфейсе сваггера отобразить элементы enum в виде названий, а не числовых значений констант. Но я не понял как

            var userNotes = notes.Where(n => n.UserId == user.Id).ToList();

            return Results.NoContent();
        });

        app.MapDelete("/note", (string login, int noteId) =>
        {
            if (users.TryFindUser(login, out User user) == false)
                return Results.NotFound();

            if (notes.TryFindUserNote(user, noteId, out Note note) == false)
                return Results.NotFound();

            notes.Remove(note);

            var userNotes = notes.Where(n => n.UserId == user.Id).ToList();

            return Results.NoContent();
        });
    }
}
