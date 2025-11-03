using Domain.Interfaces;
using InfraStructure.Sources;

namespace Application.Factories;

public static class PacketSourceFactory
{
    public static IPacketSource Create(string type)
        => type.ToLower() switch
        {
            "http" => new HttpPacketSource("http://test.com:8080"),
            "mock" => new MockPacketSource(),
            _ => throw new NotSupportedException($"Fuente '{type}' no soportada")
        };
}
