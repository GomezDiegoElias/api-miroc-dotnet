using FluentValidation;
using Newtonsoft.Json.Serialization;
using org.apimiroc.app.Validations;
using org.apimiroc.core.config;

var builder = WebApplication.CreateBuilder(args);

// Configuración básica de servicios
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
        options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

builder.Services.AddCorsPolicy(builder.Configuration);

// Registrar FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<UserUpdateValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<UserCreateValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<ClientValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<ProviderValidation>();

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
JwtConfig.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Middleware
app.UseGlobalMiddlewares();

// Seeder
//await DbSeederConfig.SeedAsync(app);

// Swagger UI
app.UseSwaggerDocumentation();

// Migraciones de D
DatabaseConfig.MigrateDatabase(app.Services);

app.MapGet("/", () =>
{

    var currentTime = DateTime.Now.ToString("HH:mm:ss");

    return Results.Ok(new
    {
        message = "¡API Funcionando!",
        time = currentTime,
        status = true
    });

}).RequireRateLimiting("fijo");

app.MapControllers().RequireRateLimiting("fijo");
app.Run();
