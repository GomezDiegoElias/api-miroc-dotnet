using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using org.apimiroc.app.Filters;
using org.apimiroc.app.Validations;
using org.apimiroc.core.config;
using Serilog;
using Serilog.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Configuracion de versionado de api
// Configuracion minima para soportar versionado por query string, header y media type
builder.Services.AddApiVersioning(opt =>
{
    opt.AssumeDefaultVersionWhenUnspecified = true; // Si no se especifica, usa la version por defecto
    opt.ReportApiVersions = true; // Reporta las versiones soportadas en las respuestas
    opt.DefaultApiVersion = new ApiVersion(1, 0); // Version por defecto 1.0
    opt.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"), // Lee version desde query string
        new HeaderApiVersionReader("X-API-Version"),     // Lee version desde header
        new MediaTypeApiVersionReader("ver")        // Lee version desde media type
    );
}).AddMvc()
.AddApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV"; // Formato de grupo de version
    setup.SubstituteApiVersionInUrl = true; // Sustituye version en URL
});

// Configuración de Serilog
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()       // Permite agregar propiedades de contexto
        .Enrich.WithMachineName()      // Nombre de la máquina/servidor
        .Enrich.WithThreadId()         // ID del hilo
        .Enrich.WithExceptionDetails() // Excepciones enriquecidas
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {SourceContext} ({MachineName}/{ThreadId}) {Message:lj}{NewLine}{Exception}"
        )
        .WriteTo.File(
            path: "../logs/log-.txt",             // Carpeta fuera del proyecto
            rollingInterval: RollingInterval.Day, // Archivo nuevo cada día
            retainedFileCountLimit: 7,            // Guarda solo 7 días
            fileSizeLimitBytes: 10_000_000,       // 10 MB por archivo
            rollOnFileSizeLimit: true,            // Crea nuevo si supera tamaño
            shared: false,                        // Una sola instancia escribe
            buffered: true                         // Escritura con buffer en memoria
        );
});


// Configuración básica de servicios
// Agrega controladores con filtro de validación personalizado y configuración de JSON
builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>())
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
        options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// Deshabilita el filtro automático de validación de modelos para usar el filtro personalizado
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddCorsPolicy(builder.Configuration);

// Registrar FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<UserUpdateValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<UserCreateValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<ClientValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<ProviderValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<ConstructionValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<ConceptValidation>();
builder.Services.AddValidatorsFromAssemblyContaining<MovementValidation>();

// Validacion con relaciones de ID de la version 2
builder.Services.AddValidatorsFromAssemblyContaining<ConstructionValidationV2>();
builder.Services.AddValidatorsFromAssemblyContaining<MovementValidationV2>();

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
JwtConfig.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Middleware
app.UseGlobalMiddlewares();

// Swagger UI
app.UseSwaggerDocumentation();

// Migraciones de D
DatabaseConfig.MigrateDatabase(app.Services);

// Seeder
//await DbSeederConfig.SeedAsync(app);

app.MapGet("/", (ILogger<Program> logger) =>
{

    logger.LogInformation("API llamada correctamente al endpoint raíz '/'");

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

public partial class Program { }