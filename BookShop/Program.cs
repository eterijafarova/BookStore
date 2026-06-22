// 3
using System.Globalization;
using System.Text;
using BookShop.ADMIN.ServicesAdmin.AdminServices;
using BookShop.ADMIN.ServicesAdmin.ReviewServices;
using BookShop.Auth.JWT;
using BookShop.Auth.ServicesAuth.Classes;
using BookShop.Auth.ServicesAuth.Interfaces;
using BookShop.BlobStorage;
using BookShop.Data.Contexts;
using BookShop.Helpers;
using BookShop.Hubs;
using BookShop.Services.Implementations;
using BookShop.Services.Interfaces;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using AutoMapper;
using BookShop.Services.Interfaces;
using BookShop.Services.Implementations;
var builder = WebApplication.CreateBuilder(args);


// CULTURE


var culture = builder.Configuration.GetValue<string>("Culture") ?? "en-US";
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(culture);
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(culture);


// DATABASE

builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));


// AUTOMAPPER

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


// CONTROLLERS

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });


// HTTP CONTEXT

builder.Services.AddHttpContextAccessor();


// SERVICES

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IAdressService, AdressService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IPromoCodeService, PromoCodeService>();

builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<CloudinaryService>();

builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMessageService, MessageService>();


// VALIDATION

builder.Services.AddFluentValidationAutoValidation();


// JWT

var jwtSettings = builder.Configuration
    .GetSection("JWT")
    .Get<JwtOptions>();

if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.SecretKey))
{
    throw new InvalidOperationException("JWT configuration is missing.");
}

var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(key),

                ClockSkew = TimeSpan.Zero
            };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken =
                    context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken)
                    && path.StartsWithSegments("/supportHub"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });


// SIGNALR

builder.Services.AddSignalR();

builder.Services.AddSingleton<IUserIdProvider,
    CustomUserIdProvider>();


// CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// AUTHORIZATION

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserPolicy",
        policy => policy.RequireRole("User"));

    options.AddPolicy("AdminPolicy",
        policy => policy.RequireRole("Admin"));
});


// SWAGGER

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "BookShop API",
            Version = "v1"
        });

    c.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Bearer {token}"
        });

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
        });
});


// URL

builder.WebHost.UseUrls("http://localhost:13779");


// BUILD

var app = builder.Build();


// MIDDLEWARE

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseStaticFiles();

app.UseCors("AllowFrontend");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

// CONTROLLERS

app.MapControllers();

// HUBS

app.MapHub<ChatHub>("/chatHub");
app.MapHub<OrderHub>("/orderHub");

// RUN

app.Run();