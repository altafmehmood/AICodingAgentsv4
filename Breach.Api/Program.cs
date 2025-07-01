using Breach.Api.Middleware;
using Breach.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

// Configure options
builder.Services.Configure<HaveIBeenPwnedOptions>(builder.Configuration.GetSection("HaveIBeenPwnedOptions"));
builder.Services.Configure<ClaudeOptions>(builder.Configuration.GetSection("ClaudeOptions"));

// Register services
builder.Services.AddScoped<IHaveIBeenPwnedService, HaveIBeenPwnedService>();
builder.Services.AddScoped<IClaudeService, ClaudeService>();
builder.Services.AddScoped<IPdfService, PdfService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
