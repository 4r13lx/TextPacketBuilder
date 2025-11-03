using System.Net.Http.Json;
using Domain.Entities;
using Domain.Interfaces;
using InfraStructure.Parsers;

namespace InfraStructure.Sources;

public class HttpPacketSource : IPacketSource
{
    private readonly HttpClient _httpClient;

    public HttpPacketSource(string baseUrl)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public async Task<IEnumerable<TextPacket>> GetTextPacketsAsync()
    {
        var response = await _httpClient.GetAsync("/challenge/get-next-packet");
        response.EnsureSuccessStatusCode();

        byte[] blockData = await response.Content.ReadAsByteArrayAsync();

        var decodedPackets = PacketDecoder.Decode(blockData);

        return decodedPackets.Select(p => new TextPacket
        {
            Sequence = p.Id,
            Word = p.Text
        });

        //return await response.Content.ReadFromJsonAsync<IEnumerable<TextPacket>>() 
        //       ?? Enumerable.Empty<TextPacket>();
    }

    public async Task RestartAsync()
    {
        var response = await _httpClient.PostAsync("/challenge/restart?userId=546", null);
        //response.EnsureSuccessStatusCode();
    }
}

