using RabbitMQ.Client;

namespace Images.Services;

public static class ImageQueue
{
    public const string Name = "image-processing";

    public static ConnectionFactory CreateFactory(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"] ?? "localhost",
            VirtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/",
            AutomaticRecoveryEnabled = false
        };
        var configuredPort = configuration["RabbitMQ:Port"];
        if (configuredPort != null)
        {
            if (!int.TryParse(configuredPort, out var port) || port is < 1 or > 65535)
                throw new InvalidOperationException("RabbitMQ:Port no es valido.");
            factory.Port = port;
        }
        var username = configuration["RabbitMQ:Username"];
        var password = configuration["RabbitMQ:Password"];
        if (username != null || password != null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(username);
            ArgumentException.ThrowIfNullOrWhiteSpace(password);
            factory.UserName = username;
            factory.Password = password;
        }
        return factory;
    }
}
