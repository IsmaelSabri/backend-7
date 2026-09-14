using Email.Data;
using Email.Models;
using Email.Service;
using Email.Service.impl;
using Email.Services;
using Email.Services.impl;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));
builder.Services.AddDbContext<EmailDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("EmailDbConnection")));
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<IContactUsService, ContactUsService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapStaticAssets();
app.UseCors(options => options
    .WithOrigins(["http://localhost:3000", "http://localhost:8080", "http://localhost:4200", "https://localhost:4200",
        "https://host.docker.internal:4040", "https://localhost:4040"])
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());
app.Run();
