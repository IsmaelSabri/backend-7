using Amazon.S3;
using ImageWorker;
using Images.Configuration;
using Images.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<CloudflareR2Options>(builder.Configuration.GetSection(CloudflareR2Options.SectionName));
builder.Services.AddSingleton<IAmazonS3>(services =>
    CloudflareR2ClientFactory.Create(services.GetRequiredService<IOptions<CloudflareR2Options>>().Value));
builder.Services.AddDbContext<ImageDb>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection")));
builder.Services.AddSingleton<ImageProcessor>();
builder.Services.AddHostedService<ImageConsumer>();
await builder.Build().RunAsync();
