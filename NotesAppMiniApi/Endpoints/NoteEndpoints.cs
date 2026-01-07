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
            if (!users.TryFindUser(login, out User user))
                return Results.NotFound();

            var note = new Note()
            {
                Id = notes.Count + 1,
                Title = title,
                Description = description,
                CreationDate = DateTime.UtcNow,
                CompletionStatus = completionStatus ?? false,
                UserId = user.Id,
                ExecutionPriority = executionPriority ?? ExecutionPriority.Low
            };

            notes.Add(note);

            return Results.Created("/note", note.Id); 
        });

        app.MapGet("/note", (string login,
            bool? completionStatus,
            ExecutionPriority? executionPriority,
            NotesSort? notesSort) =>
        {
            if (!users.TryFindUser(login, out User user))
                return Results.NotFound();

            var query = notes.Where(n => n.UserId == user.Id);

            if (completionStatus != null)
                query = query.Where(n => n.CompletionStatus == completionStatus);

            if (executionPriority != null)
                query = query.Where(n => n.ExecutionPriority == executionPriority);

            switch (notesSort)
            {
                case NotesSort.CreationDate:
                    query = query.OrderBy(n => n.CreationDate);
                    break;
                case NotesSort.ExecutionPriority:
                    query = query.OrderBy(n => n.ExecutionPriority);
                    break;
                default:
                    break;
            }

            var filteredNotes = query.ToList();

            return Results.Ok(filteredNotes);
        });

        app.MapPut("/note", (string login,
            int noteId,
            string? title,
            string? description,
            bool? completionStatus,
            ExecutionPriority? executionPriority) =>
        {
            if (!users.TryFindUser(login, out User user))
                return Results.NotFound();

            if (!notes.TryFindUserNote(user, noteId, out Note note))
                return Results.NotFound();

            if (note.CompletionStatus)
                return Results.BadRequest();  

            note.Title = title ?? note.Title;
            note.Description = description ?? note.Description;
            note.CompletionStatus = completionStatus ?? note.CompletionStatus;

            note.ExecutionPriority = executionPriority ?? note.ExecutionPriority; 

            return Results.NoContent();
        });

        app.MapDelete("/note", (string login, int noteId) =>
        {
            if (!users.TryFindUser(login, out User user))
                return Results.NotFound();

            if (!notes.TryFindUserNote(user, noteId, out Note note))
                return Results.NotFound();

            notes.Remove(note);

            return Results.NoContent();
        });
    }
}
