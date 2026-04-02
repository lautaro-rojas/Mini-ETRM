using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Mini_ETRM.Infrastructure.Data;
using Mini_ETRM.Application.Commands;
using Mini_ETRM.Domain.Interfaces;
using Mini_ETRM.Infrastructure.Repositories;
using Mini_ETRM.Infrastructure.Services;
using Mini_ETRM.Infrastructure.Cahing;

var builder = WebApplication.CreateBuilder(args);

#region OpenAPI + Scalar
// dotnet add package Microsoft.OpenApi
// dotnet add package Scalar.AspNetCore
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        var bearerScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            In = ParameterLocation.Header,
            BearerFormat = "JWT"
        };

        document.Components ??= new OpenApiComponents();
        document.AddComponent("Bearer", bearerScheme);

        return Task.CompletedTask;
    });

    options.AddOperationTransformer((operation, context, cancellationToken) =>
    {
        // Get the attributes you put on the endpoint (e.g., [Authorize], [AllowAnonymous])
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;
        
        var hasAuthorize = metadata.OfType<Microsoft.AspNetCore.Authorization.IAuthorizeData>().Any();
        var hasAllowAnonymous = metadata.OfType<Microsoft.AspNetCore.Authorization.IAllowAnonymous>().Any();

        // If the endpoint has [Authorize] and does NOT have [AllowAnonymous]...
        if (hasAuthorize && !hasAllowAnonymous)
        {
            // Add the security requirement (the lock icon)
            var securityRequirement = new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
            };

            operation.Security ??= new List<OpenApiSecurityRequirement>();
            operation.Security.Add(securityRequirement);
        }

        return Task.CompletedTask;
    });
});
#endregion

// 1. Configurar Base de Datos (SQL Server)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Registrar MediatR (Capa de Aplicación)
// Le indicamos a MediatR que escanee el assembly donde está ExecuteTradeCommandHandler
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssemblyContaining<ExecuteTradeCommandHandler>());

// 3. Registrar Servicios de Dominio / Infraestructura
// ITradeRepository es Scoped porque DbContext por defecto es Scoped (una instancia por request HTTP)
builder.Services.AddScoped<ITradeRepository, TradeRepository>();

// IMarketDataCache DEBE ser Singleton. Todos los requests HTTP y el BackgroundService 
// deben compartir exactamente la misma instancia de memoria.
builder.Services.AddSingleton<IMarketDataCache, MarketDataCache>();

// 4. Registrar el Simulador de Mercado (Background Service)
builder.Services.AddHostedService<MarketDataSimulatorService>();

builder.Services.AddControllers();

var app = builder.Build();

// Crear la base de datos automáticamente al arrancar (Solo para la PoC)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.MapGet("/", () => Results.Redirect("/scalar/v1"))
       .ExcludeFromDescription();
}

// Configure the HTTP request pipeline.
app.UseAuthorization();
app.UseRouting();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();