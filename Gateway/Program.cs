
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = long.MaxValue);
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
options.ForwardedHeaders =
    ForwardedHeaders.XForwardedFor |
    ForwardedHeaders.XForwardedProto;
});
var app = builder.Build();
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
app.MapReverseProxy();
app.UseHttpsRedirection();
app.UseCors(options => options
                .WithOrigins(["http://localhost:3000", "http://localhost:8080", "http://localhost:4200", "https://localhost:4200",
                "https://host.docker.internal:4001","https://host.docker.internal:4000","https://localhost:4001","https://localhost:4000"])// React, Vue, Angular
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                );
app.Run();
