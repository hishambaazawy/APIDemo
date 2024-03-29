using ApiData.Entities;
using APIDemo.Services;
using Microsoft.EntityFrameworkCore;
using ApiData.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Serilog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;
using APIDemo.Controllers;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = Environments.Development });
//Staging Environment 
//var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = Environments.Staging });
//Staging Environment 
//var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = Environments.Production });

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .MinimumLevel.Verbose());
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddApiVersioning(
  options =>
  {
      options.AssumeDefaultVersionWhenUnspecified = true;
      options.DefaultApiVersion = new ApiVersion(1, 0);
      options.ReportApiVersions = true;
  }
);
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(jwtSettings);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddAuthorization(options =>{options.AddPolicy("RequireAdminOrUser", policy => policy.RequireRole("Admin", "User"));});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<LogsService>();
builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddDbContext<SmartChargingContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
     b => b.MigrationsAssembly("APIDemo")));
builder.Services.AddScoped<MyHelper>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("V1", new OpenApiInfo() { Title = "API V1", Version = "V1.0" });
    c.SwaggerDoc("V2", new OpenApiInfo() { Title = "API V2", Version = "V2.0" });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.CustomSchemaIds(x => x.FullName);
    // Configure Swagger to use JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT token into this field (To Get Token Check Account/Login)",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
       {
         new OpenApiSecurityScheme
         {
           Reference = new OpenApiReference
           {
             Type = ReferenceType.SecurityScheme,
             Id = "Bearer"
           },
           Scheme = "oauth2",
           Name = "Bearer",
           In = ParameterLocation.Header,
         },
            new List<string>()
       }
    });
});
var app = builder.Build();
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint($"/swagger/V1/swagger.json", "V1.0");
        options.SwaggerEndpoint($"/swagger/V2/swagger.json", "V2.0");
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    }
    );
}
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.MapControllers();
//auto migration and db update
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<SmartChargingContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}
app.Run();
