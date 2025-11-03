using Domain.Entities;

namespace Domain.Interfaces;

public interface IPacketValidator
{
    bool Validate(IEnumerable<TextPacket> packets);
}
