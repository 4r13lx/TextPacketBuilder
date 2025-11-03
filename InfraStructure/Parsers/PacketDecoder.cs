using System.Text;

namespace Infrastructure.Parsers
{
    public class Packet
    {
        public int Id { get; set; }
        public byte[] Payload { get; set; } = [];
        public byte Checksum { get; set; }

        public string Text => Encoding.UTF8.GetString(Payload);
    }

    public static class PacketDecoder
    {
        public static IEnumerable<Packet> Decode(byte[] block)
        {
            var packets = new List<Packet>();
            int offset = 0;

            while (offset < block.Length)
            {
                if (offset + 6 > block.Length)
                    throw new InvalidDataException("Bloque incompleto para leer cabecera del paquete.");

                // ID del paquete (4 bytes)
                int packetId = BitConverter.ToInt32(block, offset);
                offset += 4;

                // Longitud del payload (1 byte)
                byte payloadLength = block[offset];
                offset += 1;

                // Checksum (1 byte)
                byte checksum = block[offset];
                offset += 1;

                // Payload (N bytes)
                if (offset + payloadLength > block.Length)
                    throw new InvalidDataException("Bloque incompleto para leer payload.");

                byte[] payload = new byte[payloadLength];
                Array.Copy(block, offset, payload, 0, payloadLength);
                offset += payloadLength;

                // Validar checksum
                byte computedChecksum = (byte)(payload.Sum(b => b) % 256);
                if (computedChecksum != checksum)
                    throw new InvalidDataException($"Checksum inválido en paquete {packetId}");

                packets.Add(new Packet
                {
                    Id = packetId,
                    Payload = payload,
                    Checksum = checksum
                });
            }

            return packets;
        }
    }
}
