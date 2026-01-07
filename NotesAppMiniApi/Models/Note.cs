namespace NotesAppMiniApi.Models;

public class Note
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTime CreationDate { get; set; }
    public bool CompletionStatus { get; set; }
    public required int UserId { get; set; }
    public ExecutionPriority ExecutionPriority { get; set; }
}
