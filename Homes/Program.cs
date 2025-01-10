using AutoMapper;
using Homes.Collections;
using Homes.Data;
using Homes.Dto;
using Homes.Infrastructure;
using Homes.Models;
using Homes.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Sieve.Models;
using Sieve.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IHomeCollection, HomeCollection>();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.Configure<SieveOptions>(builder.Configuration.GetSection("Sieve"));
var mapperConfig = new MapperConfiguration(m => m.AddProfile(new HomeProfile()));
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddSingleton<SieveProcessor>();
builder.Services.AddMvc();
builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = long.MaxValue);
var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");
builder.Services.AddDbContext<HouseDb>(options =>
options.UseNpgsql(connectionString));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseRouting(); //
app.MapControllers();
app.UseCors(options => options
                .WithOrigins([ "http://localhost:3000", "http://localhost:8080", "http://localhost:4200",
                "https://localhost:4200", "https://localhost:4040", "https://host.docker.internal:4040" ])// React, Vue, Angular
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed(origin => true)
            );
app.Run();
