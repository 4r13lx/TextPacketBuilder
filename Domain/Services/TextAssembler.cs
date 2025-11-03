using System;
using Domain.Entities;

namespace Domain.Services;

public class TextAssembler
{
    public string Assemble(IEnumerable<TextPacket> textPackets)
    {
        return string.Join(" ", textPackets
            .OrderBy(x=>x.Sequence)
            .Select(x => x.Word));
    }
}
