using Microsoft.AspNetCore.Builder;
using NotesAppMiniApi.Models;
using NotesAppMiniApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

var users = new List<User>();
var notes = new List<Note>();

app.MapUserEndpoints(users);
app.MapNoteEndpoints(users, notes);

app.Run();