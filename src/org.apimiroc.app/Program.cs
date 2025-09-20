using org.apimiroc.core.config;

var builder = WebApplication.CreateBuilder(args);

// Configuración básica de servicios
builder.Services.AddControllers();
builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
JwtConfig.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Middleware
app.UseGlobalMiddlewares();

// Seeder
await DbSeederConfig.SeedAsync(app);

// Swagger UI
app.UseSwaggerDocumentation();

// Migraciones de D
DatabaseConfig.MigrateDatabase(app.Services);

app.MapControllers();
app.Run();
