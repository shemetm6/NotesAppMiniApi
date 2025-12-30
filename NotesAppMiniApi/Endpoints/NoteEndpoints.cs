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
                //Может продублироваться id при создании новой заметки. 
                //Но мне кажется, что я и так перемудрил. Подправлю при необходимости
                Id = notes.Count + 1, 
                Title = title,
                Description = description,
                CreationDate = DateTime.UtcNow,
                CompletionStatus = completionStatus ?? false,
                UserId = user.Id,
                ExecutionPriority = executionPriority ?? ExecutionPriority.Low
            });

            //Вот здесь изначально был Results.Ok(notes), чтобы сразу отображать список для удобства. По аналогии с консольным приложением.
            //Но есть ощущение, что возвращать конкретные данные должен только GET-запрос. Это так?
            return Results.NoContent(); 
        });

        //Жоска хочу отобразить названия констант из енума NotesSort, но не понимаю как. Из-за этого сортировка в интерфейсе сваггера выглядит супер неинтуитивно.
        //Но возможно сама идея с enum была не очень, я хз
        //Также по SRP тут тоже какой-то пиздец, обработчик ищет юзера, фильтрует, сортирует. Но по условию задачи вроде можно было делать в рамках одного запроса
        //И как я понял, на одном route должен быть один get-запрос. Или нет?
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

            //Я не понял как выдать ошибку доступа, кроме как указать вручную.
            //А есть ощущение, что указывать код ошибки вручную не лучшая практика. Или нормально?
            if (note.CompletionStatus == true)
                return Results.StatusCode(403);  

            note.Title = title ?? note.Title;
            note.Description = description ?? note.Description;
            note.CompletionStatus = completionStatus ?? note.CompletionStatus;

            //Опять же хочется в интерфейсе отобразить названия элементов енума, чтобы не тыкать на загадочные цифры.
            //Но я не понял как
            note.ExecutionPriority = executionPriority ?? note.ExecutionPriority; 

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
