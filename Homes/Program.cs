using Homes.Collections;
using Homes.Collections.Impl;
using Homes.Data;
using Homes.Profiles;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IHomeCollection, HomeCollection>();
builder.Services.Configure<SieveOptions>(builder.Configuration.GetSection("Sieve"));
builder.Services.AddScoped<SieveProcessor>();
builder.Services.AddScoped<ISieveCustomFilterMethods, HomeCollection>();
//StripeConfiguration.ApiKey = builder.Configuration.GetSection("StripeSettings:PrivateKey").Get<string>();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new HomeProfile()));
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
                .WithOrigins([ "http://localhost:3000", "http://localhost:8080", "http://localhost:4200","https://localhost:3030",
                "https://localhost:4200", "https://localhost:4040", "https://host.docker.internal:4040" ])// React, Vue, Angular
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed(origin => true)
            );
app.Run();
