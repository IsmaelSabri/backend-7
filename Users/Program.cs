using Users.Jwt;
using Users.Profiles;
using AutoMapper;
using Users.Services;
using Users.Services.impl;
using Users.Collections;
using Users.Collections.Impl;
using Users.Hubs;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Nest;
using Users.Models;
using Users.Services.Impl;
using Sieve.Models;
using Sieve.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
var settings = new ElasticsearchClientSettings(new Uri("https://host.docker.internal:9200"))
    .CertificateFingerprint("39:CE:1C:B8:BB:7C:63:32:75:82:07:FD:97:31:88:FE:2F:21:32:04:32:5B:2E:E4:58:0C:50:15:3A:EC:61:E0")
    .Authentication(new BasicAuthentication("elastic", "letmein"))
    .EnableDebugMode(cd =>
    {
        //var request = System.Text.Encoding.Default.GetString(cd.RequestBodyInBytes);
        Console.WriteLine(cd.DebugInformation);
    });
var client = new ElasticsearchClient(settings);
builder.Services.AddSingleton(client);
// this for the queryes
var connectionSettings = new ConnectionSettings(new Uri("https://host.docker.internal:9200"))
        .BasicAuthentication("elastic", "letmein")
        .CertificateFingerprint("39:CE:1C:B8:BB:7C:63:32:75:82:07:FD:97:31:88:FE:2F:21:32:04:32:5B:2E:E4:58:0C:50:15:3A:EC:61:E0");
var nestClient = new ElasticClient(connectionSettings);
builder.Services.AddSingleton(nestClient);
builder.Services.AddScoped<IElasticService<User>, ElasticService<User>>();
builder.Services.Configure<SieveOptions>(builder.Configuration.GetSection("Sieve"));
builder.Services.AddSingleton<SieveProcessor>();
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
                .WithOrigins(["http://localhost:3000", "http://localhost:8080", "http://localhost:4200", "https://localhost:4200"])// React, Vue, Angular
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
            );
app.Run();
