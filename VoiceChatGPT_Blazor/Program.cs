using OpenAI.Extensions;
using VoiceChatGPT_Blazor.Data;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// register OpenAI service
var apiKey = builder.Configuration.GetSection("OpenAI:ApiKey").Value!;
builder.Services.AddOpenAIService(s => s.ApiKey = apiKey);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<TranslateService>();
builder.Services.AddSingleton<WhisperService>();
builder.Services.AddSpeechRecognition();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
