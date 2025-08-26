using Invoicing.Api.Middleware;
using Invoicing.Api.Responses;
using Invoicing.Application;
using Invoicing.Infrastructure;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug(); // appears in Rider/VS debug

builder.Services.AddControllers();

// Replace ASP.NET's default "400 with ModelState dump" with the ApiResponse envelope.
// This runs when model binding or DataAnnotations fail before the controller executes.
builder.Services.Configure<ApiBehaviorOptions>(opt =>
{
    opt.InvalidModelStateResponseFactory = ctx =>
    {
        var errs = ctx.ModelState
            .Where(kv => kv.Value?.Errors.Count > 0)
            .SelectMany(kv => kv.Value!.Errors.Select(e =>
                new ApiError("validation", e.ErrorMessage, kv.Key)))
            .ToList();

        var body = ApiResponse<object>.Fail(errs, ctx.HttpContext.TraceIdentifier);
        return new BadRequestObjectResult(body);
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Global exception guard
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();

public partial class Program { }
