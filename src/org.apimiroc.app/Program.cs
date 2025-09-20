using org.apimiroc.core.config;

var builder = WebApplication.CreateBuilder(args);

// Configuración básica de servicios
builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// Middleware global
app.UseGlobalMiddlewares();

// Swagger
app.UseSwaggerDocumentation();

// BD
DatabaseConfig.MigrateDatabase(app.Services);

app.MapControllers();
app.Run();
