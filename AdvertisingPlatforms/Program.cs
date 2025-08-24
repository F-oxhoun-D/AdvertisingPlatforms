using AdvertisingPlatforms;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x => x.SwaggerDoc("v1", new OpenApiInfo { Title = "Advertising platform app", Version = "v1" }));

var app = builder.Build();

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

app.MapGet("/", async (HttpContext context) =>
{
    var html = await File.ReadAllTextAsync("Pages/Index.html");
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(html);
}).WithName("index");

app.MapGet("/upload", async (HttpContext context) =>
{
    var html = await File.ReadAllTextAsync("Pages/UploadFile.html");
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(html);
}).WithName("upload");

app.MapPost("/upload", FileHandler.Upload);

app.MapGet("/location", async (HttpContext context) =>
{
    var html = await File.ReadAllTextAsync("Pages/Platforms.html");
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(html);
});

app.MapGet("/platforms", async (HttpContext context) =>
{
    var location = context.Request.Query["location"].ToString();
    var platforms = PlatformHandler.FindPlatformsForLocation(location);

    var html = await File.ReadAllTextAsync("Pages/Platforms.html");
    var platformListHtml = string.Join("<br/>", platforms.Select(p => p));

    html = html.Replace("{{platforms}}", platformListHtml);

    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(html);
});

Initialization.Init();

app.Run();
