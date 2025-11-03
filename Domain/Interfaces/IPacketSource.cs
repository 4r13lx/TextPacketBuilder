using Domain.Entities;

namespace Domain.Interfaces;

public interface IPacketSource
{
    Task<IEnumerable<TextPacket>> GetTextPacketsAsync();
    Task RestartAsync();
}
