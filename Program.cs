using Microsoft.AspNetCore.OpenApi;
using Swashbuckle.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// Add middleware to the request pipeline.

if(!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/home/error");
} else {
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpLogging();

app.Use(async (context, next) =>
{
    var startTime = DateTime.UtcNow;
    Console.WriteLine($"the request started at {startTime}");
    await next();
    var endTime = DateTime.UtcNow;
    Console.WriteLine($"the request ended at {endTime}");
    var duration = endTime - startTime;
    Console.WriteLine($"the request took {duration.TotalMilliseconds} ms");
});

app.Use(async (context,next) =>
{
    Console.WriteLine($"the request path is {context.Request.Path}");
    await next();
    Console.WriteLine($"the response code is {context.Response.StatusCode}");
});

// Map endpoints to the request pipeline.

app.MapGet("/", () => "Hello World!");

app.MapGet("/throw", () => {throw new Exception("Test exception");});

app.MapGet("/home/error", () => "An error occurred.");


app.Run();