using Core.Collections;
using Core.Collections.impl;
using Core.Jwt;
using Core.Profiles;
using Core.Services;
using Core.Services.impl;
using Core.Hubs;
using Core.Data;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using Core.Configuration;
using Core.Collections.Impl;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient("Overpass", client =>
{
    client.BaseAddress = new Uri("https://overpass-api.de/");
    client.Timeout = TimeSpan.FromSeconds(30);

    client.DefaultRequestHeaders.UserAgent.ParseAdd(
        "FuturCasa/1.0"
    );

    client.DefaultRequestHeaders.Accept.ParseAdd(
        "application/json"
    );
});
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
});
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new UserProfile()));
builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new HomeProfile()));
builder.Services.AddHttpClient<IImageService, ImageService>();
builder.Services.AddHttpClient<IEmailService, EmailService>(client => client.BaseAddress = new Uri("http://localhost:3030"));
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", builder => builder.WithOrigins("http://localhost:4200", "https://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});
var StripeSetting = builder.Configuration.GetSection("StripeSettings");
builder.Services.Configure<StripeOptions>(options =>
{
    options.PublishableKey = StripeSetting.GetSection("STRIPE_PUBLISHABLE_KEY").Value!;
    options.SecretKey = StripeSetting.GetSection("STRIPE_SECRET_KEY").Value!;
    options.WebhookSecret = StripeSetting.GetSection("STRIPE_WEBHOOK_SECRET").Value!;
});
builder.Services.AddScoped<IUserCollection, UserCollection>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IResetTokenService, ResetTokenService>();
builder.Services.AddScoped<IHomeCollection, HomeCollection>();
builder.Services.AddScoped<IChats, ChatsCollection>();
builder.Services.AddScoped<IExtraContent, ExtraContentCollection>();
builder.Services.AddScoped<IExtraCollection, ExtraCollection>();
builder.Services.AddScoped<ITransaccionCollection, TransaccionCollection>();
builder.Services.AddScoped<ILineaTransaccionCollection, LineaTransaccionCollection>();
builder.Services.AddScoped<IWebHookCollection, WebHookCollection>();
builder.Services.AddScoped<IPlanCollection, PlanCollection>();
builder.Services.AddScoped<IPlanSubscriptionCollection, PlanSubscriptionCollection>();
builder.Services.AddScoped<IOrganizationCollection, OrganizationCollection>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.Configure<SieveOptions>(builder.Configuration.GetSection("Sieve"));
builder.Services.AddScoped<ISieveCustomFilterMethods, CustomFiltersCollection>();
builder.Services.AddScoped<SieveProcessor>();
builder.Services.AddScoped<JwtResource>();
builder.Services.AddSignalR();
var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");
builder.Services.AddDbContext<CoreDb>(options =>
options.UseNpgsql(connectionString));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseForwardedHeaders();
app.UseForwardedHeaders();
app.Use(async (context, next) =>
{
    var remoteIp = context.Connection.RemoteIpAddress?.ToString();

    if (!string.IsNullOrWhiteSpace(remoteIp))
    {
        context.Request.Headers["X-Forwarded-For"] = remoteIp;
        context.Request.Headers["X-Real-IP"] = remoteIp;
    }

    await next();
});
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
