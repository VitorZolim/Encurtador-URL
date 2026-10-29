using EncurtadorUrl.Context;
using EncurtadorUrl.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorLocal", policy =>
    {
        policy.WithOrigins("https://localhost:7298")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixo", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5; // Permite 5 requisição
        limiterOptions.Window = TimeSpan.FromSeconds(10); // A cada 10 segundos
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 2; // Se passar de 5, deixa 2 na fila de espera
    });

    // Devolve Status(429) quando o limite atingido
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Conectando Banco de Dados
string sqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite(sqlConnection));

builder.Services.AddScoped<IUrlShortService, UrlShortService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Ativar CORS e Rate Limiting
app.UseCors("BlazorLocal");
app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();
