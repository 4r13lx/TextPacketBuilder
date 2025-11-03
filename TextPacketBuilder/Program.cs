using Application.Services;
using Domain.Services;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

// Logging + dependencias
services.AddLogging(cfg => cfg.AddConsole());
services.AddSingleton<PacketValidator>();
services.AddSingleton<TextAssembler>();

// Configure HttpClient for HttpPacketSource and register implementations
// Use environment variable PACKET_SOURCE_TYPE to switch between "mock" and "http"
var packetSourceType = Environment.GetEnvironmentVariable("PACKET_SOURCE_TYPE") ?? "http";

services.AddHttpClient<InfraStructure.Sources.HttpPacketSource>(client =>
{
    client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("PACKET_SOURCE_BASEURL") ?? "http://test.com:8080");
});

// Register IPacketSource depending on configuration
if (packetSourceType.Equals("mock", StringComparison.OrdinalIgnoreCase))
{
    services.AddSingleton<Domain.Interfaces.IPacketSource, InfraStructure.Sources.MockPacketSource>();
}
else
{
    services.AddTransient<Domain.Interfaces.IPacketSource, InfraStructure.Sources.HttpPacketSource>();
}

// Caso de uso principal
services.AddTransient<TextRetrievalService>();

var provider = services.BuildServiceProvider();

var service = provider.GetRequiredService<TextRetrievalService>();

try
{
    var text = await service.GetFullTextAsync(restart: !packetSourceType.Equals("mock", StringComparison.OrdinalIgnoreCase));
    Console.WriteLine("\n📘 Texto reconstruido:");
    Console.WriteLine(text);
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
}
