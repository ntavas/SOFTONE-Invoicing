using Invoicing.Api.Middleware;
using Invoicing.Api.Responses;
using Invoicing.Application;
using Invoicing.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Invoicing API", Version = "v1" });

    // Dev-only: pass company id via header so you can test easily in Swagger
    c.AddSecurityDefinition("DemoCompany", new OpenApiSecurityScheme
    {
        Description = "Dev-only header. Enter a company id (e.g., 1).",
        Name = "X-Demo-CompanyId",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    // Bearer (for later)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // Make the DemoCompany header required globally (for now).
    // Remove this when real auth middleware is in place, or switch to per-action requirements.
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "DemoCompany" }
            },
            Array.Empty<string>()
        }
    });

    // Do NOT add Bearer as global yet, or every endpoint will demand a token before we’ve implemented auth.
});

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
