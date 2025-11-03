using Domain.Entities;
using Domain.Interfaces;

namespace InfraStructure.Sources;

public class MockPacketSource : IPacketSource
{
    private List<TextPacket> _packets = new()
    {
        new() { Sequence = 1, Word = "El" },
        new() { Sequence = 2, Word = "sol" },
        new() { Sequence = 3, Word = "brillaba" },
        new() { Sequence = 4, Word = "en" },
        new() { Sequence = 5, Word = "el" },
        new() { Sequence = 6, Word = "horizonte." }
    };

    public Task<IEnumerable<TextPacket>> GetTextPacketsAsync() =>
        Task.FromResult<IEnumerable<TextPacket>>(_packets);

    public Task RestartAsync()
    {
        _packets = new()
        {
            new() { Sequence = 1, Word = "Era" },
            new() { Sequence = 2, Word = "una" },
            new() { Sequence = 3, Word = "tarde" },
            new() { Sequence = 4, Word = "de" },
            new() { Sequence = 5, Word = "otoño." }
        };
        return Task.CompletedTask;
    }
}
