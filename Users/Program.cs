using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Users.Repositories;
using Users.Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Users.Profiles;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Users.Services;
using Users.Services.impl;
using Users.Collections;
using Users.Collections.Impl;
using Users.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPasswordHasher,PasswordHasher>();
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
builder.Services.AddScoped<IChatCollection, ChatCollection>();
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
app.MapHub<ChatHub>("/hubs/chat");
app.MapControllers();
app.UseCors(options => options
                .WithOrigins(new[] { "http://localhost:3000", "http://localhost:8080", "http://localhost:4200" })// React, Vue, Angular
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
            );
app.Run();
