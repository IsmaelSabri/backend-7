using Users.Repositories;
using Users.Jwt;
using Users.Profiles;
using AutoMapper;
using Users.Services;
using Users.Services.impl;
using Users.Hubs;
using Users.Data;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<SieveOptions>(builder.Configuration.GetSection("Sieve"));
builder.Services.AddSingleton<SieveProcessor>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
var mapperConfig = new MapperConfiguration(m => m.AddProfile(new UserProfile()));
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", builder => builder.WithOrigins("http://localhost:4200", "https://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});
builder.Services.AddScoped<IUserCollection, UserCollection>();
builder.Services.AddScoped<JwtResource>();
builder.Services.AddSignalR();
var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");
builder.Services.AddDbContext<UserDb>(options =>
options.UseNpgsql(connectionString));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // show details
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();
app.MapHub<ChatHub>("/chat-hub");
app.MapControllers();
app.UseCors(options => options
                .WithOrigins(["http://localhost:3000", "http://localhost:8080", "http://localhost:4200", "https://localhost:4200"
                , "https://host.docker.internal:4040", "https://localhost:4040"])// React, Vue, Angular
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
            );
app.Run();
