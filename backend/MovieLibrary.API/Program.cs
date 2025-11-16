using Microsoft.EntityFrameworkCore;
using MovieLibrary.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=movies.db"));
builder.Services.AddCors(opt => opt.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:5173", "http://localhost:3000")
     .AllowAnyMethod()
     .AllowAnyHeader()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.MapControllers();
app.Run();