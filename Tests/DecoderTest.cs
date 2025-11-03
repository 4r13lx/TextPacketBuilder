using System.Text;
using InfraStructure.Parsers;

namespace Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Decode_ShouldReturnValidPacket()
    {
        // Arrange: paquete con ID=1, payload="Hi"
        byte[] payload = Encoding.UTF8.GetBytes("Hi");
        byte checksum = (byte)(payload.Sum(b => b) % 256);

        var bytes = new List<byte>();
        bytes.AddRange(BitConverter.GetBytes(1)); // ID
        bytes.Add((byte)payload.Length);
        bytes.Add(checksum);
        bytes.AddRange(payload);

        // Act
        var result = PacketDecoder.Decode(bytes.ToArray()).ToList();

        // Assert
        Assert.That(result[0].Id, Is.EqualTo(1));
        Assert.That(result[0].Text, Is.EqualTo("Hi"));
    }

}