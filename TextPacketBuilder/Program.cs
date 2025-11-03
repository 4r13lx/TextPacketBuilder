using Application.Factories;
using Application.Services;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

var services = new ServiceCollection();

// Logging + dependencias
services.AddLogging(cfg => cfg.AddConsole());
services.AddSingleton<PacketValidator>();
services.AddSingleton<TextAssembler>();

// Fuente de datos configurable
bool debugMode = false;
var packetSource = PacketSourceFactory.Create(debugMode ? "mock" : "http");
services.AddSingleton(packetSource);

// Caso de uso principal
services.AddTransient<TextRetrievalService>();

var provider = services.BuildServiceProvider();

var service = provider.GetRequiredService<TextRetrievalService>();

try
{
    var text = await service.GetFullTextAsync(restart: !debugMode);
    Console.WriteLine("\n📘 Texto reconstruido:");
    Console.WriteLine(text);
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
}
