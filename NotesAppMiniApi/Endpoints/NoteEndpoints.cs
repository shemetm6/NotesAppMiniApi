using NotesAppMiniApi.Extensions;
using NotesAppMiniApi.Models;

namespace NotesAppMiniApi.Endpoints;

public static class NoteEndpoints
{
    public static void MapNoteEndpoints(this WebApplication app, List<User> users, List<Note> notes)
    {
        app.MapPost("/note", (string login, 
            string title, 
            string description, 
            bool? completionStatus,
            ExecutionPriority? executionPriority) =>
        {
            if (users.TryFindUser(login, out User user) == false)
                return Results.NotFound();

            notes.Add(new Note()
            {
                Id = notes.Count + 1, //Может продублироваться id при создании новой заметки, после удаления старых. Но мне кажется, что я и так перемудрил. Подправлю, если понадобится
                Title = title,
                Description = description,
                CreationDate = DateTime.UtcNow,
                CompletionStatus = completionStatus ?? false,
                UserId = user.Id,
                ExecutionPriority = executionPriority ?? ExecutionPriority.Low
            });

            return Results.NoContent(); //вот здесь изначально был Results.Ok(notes), по аналогии с консольным приложением. Но есть ощущение, что возвращать конкретные данные должен только GET-запрос. Это так?
        });

        app.MapGet("/note", (string login,
            bool? completionStatus,
            ExecutionPriority? executionPriority,
            NotesSort? notesSort) =>
        {
            if (users.TryFindUser(login, out User user) == false)
                return Results.NotFound();

            var query = notes.Where(n => n.UserId == user.Id);

            if (completionStatus != null)
                query = query.Where(n => n.CompletionStatus == completionStatus);

            if (executionPriority != null)
                query = query.Where(n => n.ExecutionPriority == executionPriority);

            if (notesSort == NotesSort.CreationDate) //сортировка выглядит как говно т.к. не понял как отобразить в интерфейсе названия элементов енума.
                query = query.OrderBy(n => n.CreationDate);

            if (notesSort == NotesSort.ExecutionPriority)
                query = query.OrderBy(n => n.ExecutionPriority);

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

            if (note.CompletionStatus == true)
                return Results.StatusCode(403); //я не понял как выдать ошибку доступа, кроме как указать вручную. А есть ощущение, что указывать код ошибки вручную не лучшая практика.

            note.Title = title ?? note.Title;
            note.Description = description ?? note.Description;
            note.CompletionStatus = completionStatus ?? note.CompletionStatus;
            note.ExecutionPriority = executionPriority ?? note.ExecutionPriority; //Было бы круто в интерфейсе сваггера отобразить элементы enum в виде названий, а не числовых значений констант. Но я не понял как

            return Results.NoContent();
        });

        app.MapDelete("/note", (string login, int noteId) =>
        {
            if (users.TryFindUser(login, out User user) == false)
                return Results.NotFound();

            if (notes.TryFindUserNote(user, noteId, out Note note) == false)
                return Results.NotFound();

            notes.Remove(note);

            return Results.NoContent();
        });
    }
}
