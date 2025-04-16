using System.Text;
using System.Globalization;
using BookShop.ADMIN.ServicesAdmin.AdminServices;
using BookShop.ADMIN.ServicesAdmin.ReviewServices;
using BookShop.ADMIN.ServicesAdmin.WarehouseServices;
using BookShop.Auth.DataAuth.Validators;
using BookShop.Auth.JWT;
using BookShop.Auth.ServicesAuth.Classes;
using BookShop.Auth.ServicesAuth.Interfaces;
using BookShop.Auth.SharedAuth;
using BookShop.Data.Contexts;
using BookShop.Mappings;
using BookShop.Services.Implementations;
using BookShop.Services.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Локализация
var culture = builder.Configuration.GetValue<string>("Culture") ?? "en-US";
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(culture);
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(culture);

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// DB Context
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрируем глобальный Exception Middleware (если он определён)
builder.Services.AddTransient<GlobalExceptionMiddleware>();

// Controllers: добавляем настройки JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

// Регистрация FluentValidation — обновлено для версии 12.x:
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();

// Конфигурация JWT — используем секцию "JWT"
var jwtSettings = builder.Configuration.GetSection("JWT").Get<JwtOptions>();
if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.SecretKey))
{
    throw new InvalidOperationException("JWT configuration is missing or SecretKey is not set.");
}
var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

// CORS – замените на конкретное значение в продакшене при необходимости
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Регистрация сервисов
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IOrderService, OrderService>();
// builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
// builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAccountService,AccountService>();
builder.Services.AddScoped<IAuthService, AuthService>();



builder.Services.AddLogging(options =>
{
    options.AddConsole(); // Логирование в консоль
    options.AddDebug();   // Логирование в отладку
});


// // Конфигурация EmailSettings (проверьте, что в appsettings.json корректно прописан раздел "EmailSettings")
// builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// JWT авторизация
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookShop API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the token in the format: Bearer {your_token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme 
            {
                Reference = new OpenApiReference 
                { 
                    Type = ReferenceType.SecurityScheme, 
                    Id = "Bearer" 
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Подключение глобального Exception Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Swagger UI только для Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookShop API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
