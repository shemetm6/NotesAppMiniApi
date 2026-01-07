using Microsoft.AspNetCore.Builder;
using NotesAppMiniApi.Models;
using NotesAppMiniApi.Endpoints;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

var users = new List<User>();
var notes = new List<Note>();

app.MapUserEndpoints(users);
app.MapNoteEndpoints(users, notes);

app.Run();