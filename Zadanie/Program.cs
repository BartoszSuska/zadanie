using Microsoft.EntityFrameworkCore;
using System;
using Zadanie.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Zadanie.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//konfiguracja JWT
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    var key = Encoding.UTF8.GetBytes(
        builder.Configuration["Jwt:Key"]);

    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

//konfiguracja CORS, pozwalaj¹ca na komunikacjê z frontendem Vue.js dzia³aj¹cym na localhost:5173
builder.Services.AddCors(options => {
    options.AddPolicy("AllowVue",
        policy => {
            policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


//konfiguracja Entity Framework Core z SQL Server, u¿ywaj¹c connection string z appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

//automatyczne tworzenie bazy danych i dodawanie domyœlnego u¿ytkownika admina, jeœli tabela Users jest pusta
using (var scope = app.Services.CreateScope()) {
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

    if(!context.Users.Any()) {
        var adminUser = new User {
            Email = "admin@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123")
        };
        context.Users.Add(adminUser);
        context.SaveChanges();
    }
}

app.UseCors("AllowVue");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//dodanie middleware do obs³ugi autoryzacji, który bêdzie sprawdza³ token JWT w nag³ówkach ¿¹dañ i pozwala³ na dostêp do chronionych endpointów tylko dla zalogowanych u¿ytkowników
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
