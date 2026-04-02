using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Mini_ETRM.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

#region Controllers
builder.Services.AddControllers();
#endregion

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

#region Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region Services
//builder.Services.AddScoped<Mini-ETRM.Application.Services.UserService>();
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.MapGet("/", () => Results.Redirect("/scalar/v1"))
       .ExcludeFromDescription();
}

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();