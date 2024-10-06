// using Microsoft.Extensions.Options;
// using MongoDB.Driver;
// using BackendServices;
// using BackendServices.Configurations;
// using BackendServices.Services;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.IdentityModel.Tokens;
// using System.Text;
// using Microsoft.OpenApi.Models;
//
//
// var builder = WebApplication.CreateBuilder(args);
//
// // Add services to the container.
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
// builder.Services.AddControllers(); // This is required for controller mapping
//
// // MongoDB Configuration
// builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));
// builder.Services.AddSingleton<IMongoClient>(s =>
// {
//     var mongoSettings = s.GetRequiredService<IOptions<MongoDBSettings>>().Value;
//     return new MongoClient(mongoSettings.ConnectionString);
// });
// builder.Services.AddScoped(s =>
// {
//     var mongoSettings = s.GetRequiredService<IOptions<MongoDBSettings>>().Value;
//     var client = s.GetRequiredService<IMongoClient>();
//     return client.GetDatabase(mongoSettings.DatabaseName);
// });
//
// // Register IProductRepository and ProductService
// builder.Services.AddScoped<IProductRepository, ProductRepository>(); // Register the repository
// builder.Services.AddScoped<ProductService>(); // Register ProductService
// builder.Services.AddScoped<IRankingRepository, RankingRepository>();
// builder.Services.AddScoped<RankingService>();
//
// // Configure Authentication
// var key = builder.Configuration["Jwt:Key"];
// if (string.IsNullOrEmpty(key))
// {
//     throw new ArgumentNullException("JWT Key is missing in appsettings.json");
// }
//
// builder.Services.AddAuthentication(options =>
//     {
//         options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//         options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//     })
//     .AddJwtBearer(options =>
//     {
//         options.Events = new JwtBearerEvents
//         {
//             OnAuthenticationFailed = context =>
//             {
//                 Console.WriteLine("Authentication failed: " + context.Exception.Message);
//                 return Task.CompletedTask;
//             },
//             OnTokenValidated = context =>
//             {
//                 Console.WriteLine("Token validated.");
//                 return Task.CompletedTask;
//             },
//             OnMessageReceived = context =>
//             {
//                 Console.WriteLine("Message received.");
//                 return Task.CompletedTask;
//             }
//         };
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = false,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = builder.Configuration["Jwt:Issuer"],
//             //ValidAudience = builder.Configuration["Jwt:Audience"],
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//         };
//     });
//
// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrator"));
//     options.AddPolicy("Vendor", policy => policy.RequireRole("Vendor"));
//     options.AddPolicy("CSR", policy => policy.RequireRole("CSR"));
// });
//
// // builder.Services.AddSwaggerGen(c =>
// // {
// //     c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
// //
// //     // Add JWT token support in Swagger
// //     c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
// //     {
// //         In = ParameterLocation.Header,
// //         Description = "Please enter a valid token with 'Bearer' prefix (e.g. 'Bearer {token}')",
// //         Name = "Authorization",
// //         Type = SecuritySchemeType.ApiKey,
// //         Scheme = "Bearer"
// //     });
// //
// //     c.AddSecurityRequirement(new OpenApiSecurityRequirement
// //     {
// //         {
// //             new OpenApiSecurityScheme
// //             {
// //                 Reference = new OpenApiReference
// //                 {
// //                     Type = ReferenceType.SecurityScheme,
// //                     Id = "Bearer"
// //                 }
// //             },
// //             Array.Empty<string>()
// //         }
// //     });
// // });
//
//
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
//
//     // Add JWT token support in Swagger
//     c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//     {
//         In = ParameterLocation.Header,
//         Description = "Please enter 'Bearer' followed by your JWT token",
//         Name = "Authorization",
//         Type = SecuritySchemeType.ApiKey,
//         Scheme = "Bearer",
//         BearerFormat = "JWT" // Optional, clarifies the type of token
//     });
//
//     c.AddSecurityRequirement(new OpenApiSecurityRequirement
//     {
//         {
//             new OpenApiSecurityScheme
//             {
//                 Reference = new OpenApiReference
//                 {
//                     Type = ReferenceType.SecurityScheme,
//                     Id = "Bearer"
//                 },
//                 Scheme = "oauth2",
//                 Name = "Bearer",
//                 In = ParameterLocation.Header,
//             },
//             new List<string>()
//         }
//     });
// });
//
//
//
// var app = builder.Build();
//
// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
//
// app.UseHttpsRedirection();
// app.UseAuthentication();
// app.UseAuthorization();
//
// // Map controllers
// app.MapControllers(); // This will map the controllers for Swagger
//
// app.Run();
//


// using System.Text;
// using BackendServices;
// using BackendServices.Configurations;
// using BackendServices.Helpers;
// using BackendServices.Models;
// using BackendServices.Services;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.IdentityModel.Tokens;
//
// var builder = WebApplication.CreateBuilder(args);
//
// // Add MongoDB settings
// builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));
// builder.Services.AddSingleton<UserService>();
// builder.Services.AddSingleton<VendorService>();
// builder.Services.AddSingleton<ProductService>();  // Register ProductService
// builder.Services.AddScoped<IUserRepository, UserRepository>();
// builder.Services.AddScoped<IVendorRepository, VendorRepository>();
// builder.Services.AddScoped<IProductRepository, ProductRepository>();
// // builder.Services.AddSingleton<OrderService>();
// // builder.Services.AddSingleton<NotificationService>();
//
// // Add JWT settings
// builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
// builder.Services.AddSingleton<JwtHelper>();
//
// // Add JWT authentication
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
//             ValidAudience = builder.Configuration["JwtSettings:Audience"],
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
//         };
//     });
// // Add services to the container.
//
// builder.Services.AddControllers();
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
//
// var app = builder.Build();
//
// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
//
// app.UseHttpsRedirection();
//
// app.UseAuthorization();
//
// app.MapControllers();
//
// app.Run();


// using System.Text;
// using BackendServices;
// using BackendServices.Configurations;
// using BackendServices.Helpers;
// using BackendServices.Models;
// using BackendServices.Services;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.IdentityModel.Tokens;
//
// var builder = WebApplication.CreateBuilder(args);
//
// // Add MongoDB settings
// builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));
// builder.Services.AddScoped<UserService>();  // Change to Scoped
// builder.Services.AddScoped<VendorService>();  // Change to Scoped
// builder.Services.AddScoped<ProductService>();  // Change to Scoped
//
// // Register repositories as scoped services
// builder.Services.AddScoped<IUserRepository, UserRepository>();
// builder.Services.AddScoped<IVendorRepository, VendorRepository>();
// builder.Services.AddScoped<IProductRepository, ProductRepository>();
//
// // Add JWT settings
// builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
// builder.Services.AddSingleton<JwtHelper>();  // JwtHelper can remain as singleton
//
// // Add JWT authentication
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
//             ValidAudience = builder.Configuration["JwtSettings:Audience"],
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
//         };
//     });
//
// // Add services to the container
// builder.Services.AddControllers();
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
//
// var app = builder.Build();
//
// // Configure the HTTP request pipeline
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
//
// app.UseHttpsRedirection();
// app.UseAuthentication();  // Ensure Authentication is used
// app.UseAuthorization();
// app.MapControllers();
//
// app.Run();


using System.Text;
using BackendServices;
using BackendServices.Configurations;
using BackendServices.Helpers;
using BackendServices.Models;
using BackendServices.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;  // Ensure this is included for MongoDBSettings
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;  // Ensure this is included for MongoDB

var builder = WebApplication.CreateBuilder(args);

// Add MongoDB settings and register MongoDB client and database
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoClient(settings.ConnectionString);  // MongoClient setup
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);  // MongoDatabase setup
});

// Register services as scoped
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<VendorService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<EmailService>(); // Registering EmailService


// Register repositories as scoped services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>(); 
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

// Add JWT settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton<JwtHelper>();  // JwtHelper remains as a singleton

// Add JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
        };
    });

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();  // Ensure Authentication is used
app.UseAuthorization();
app.MapControllers();

app.Run();
