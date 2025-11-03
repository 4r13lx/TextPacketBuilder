using System;
using Domain.Entities;

namespace Domain.Services;

using Domain.Interfaces;

public class PacketValidator : IPacketValidator
{
    public bool Validate(IEnumerable<TextPacket> packets)
    {
        var seq = packets.Select(p => p.Sequence).ToList();
        return seq.Distinct().Count() == seq.Count &&
               seq.OrderBy(n => n).SequenceEqual(Enumerable.Range(1, seq.Count));
    }
}