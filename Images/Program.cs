using Images.Collections;
using Images.Collections.Impl;
using Images.Data;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;
using Images.Services;
using Images.Services.Impl;
using Images.Configuration;
using Amazon.S3;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", builder => builder.WithOrigins("http://localhost:4200", "https://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

builder.Services.AddScoped<IImageCollection, ImageCollection>();
builder.Services.Configure<CloudflareR2Options>(
builder.Configuration.GetSection(CloudflareR2Options.SectionName));
builder.Services.AddSingleton<IAmazonS3>(services =>
    CloudflareR2ClientFactory.Create(services.GetRequiredService<IOptions<CloudflareR2Options>>().Value));
builder.Services.AddSingleton<IImageStorageService, R2StorageService>();
builder.Services.AddHostedService<ImageQueuePublisher>();
builder.Services.AddScoped<IImageManager, ImageManager>();
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
else
{
    app.UseExceptionHandler();
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
