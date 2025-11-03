using System;

namespace Domain.Entities;

public class TextPacket
{
    public int Sequence { get; set; } // numero de orden
    public string Word { get; set; } = ""; // palabra del texto
    public string Checksum { get; set; } = ""; // validar integridad
}
