using OpenAI.Extensions;
using HackathonAI.Application;
using HackathonAI.Infrastructure;
using HackathonAI.Presentation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// register OpenAI service
var apiKey = builder.Configuration.GetSection("OpenAI:ApiKey").Value!;
builder.Services.AddOpenAIService(s => s.ApiKey = apiKey);



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddApplication()
    .AddIntrastructure()
    .AddPresentation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
