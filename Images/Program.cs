using Images.Collections;
using Images.Collections.Impl;
using Images.Data;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;
using Images.Profiles;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new ImageProfile()));
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", builder => builder.WithOrigins("http://localhost:4200", "https://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

builder.Services.AddScoped<IImageCollection, ImageCollection>();
builder.Services.AddScoped<ISieveCustomFilterMethods, ImageCollection>();
builder.Services.AddScoped<SieveProcessor>();
var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");
builder.Services.AddDbContext<ImageDb>(options =>
options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // show details
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();
app.MapControllers();
app.UseCors(options => options
                .WithOrigins(["http://localhost:3000", "http://localhost:8080", "http://localhost:4200", "https://localhost:4200"
                , "https://host.docker.internal:4040", "https://localhost:4040"])// React, Vue, Angular
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
            );
app.Run();