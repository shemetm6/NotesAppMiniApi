using Microsoft.AspNetCore.Builder;
using NotesAppMiniApi.Models;
using NotesAppMiniApi.Extensions;
using NotesAppMiniApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var users = new List<User>();
var notes = new List<Note>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapUserEndpoints(users);
app.MapNoteEndpoints(users, notes);

app.Run();

