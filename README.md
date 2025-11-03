# TextPacketBuilder

![CI](https://github.com/4r13lx/TextPacketBuilder/actions/workflows/dotnet.yml/badge.svg)

TextPacketBuilder is a small .NET console application and library for decoding text packets.

## Configuration and packet source

The project resolves `IPacketSource` via dependency injection. Configuration is read from `appsettings.json` and environment variables. Use the `PacketSource` section to select the implementation and base URL.

Example `appsettings.json` (already included in the project):

```json
{
	"PacketSource": {
		"Type": "http",
		"BaseUrl": "http://localhost:5000/packets"
	}
}
```

Environment variables override JSON. To set using environment variables in bash, use the double-underscore convention for nested keys:

```bash
export PacketSource__Type=mock
export PacketSource__BaseUrl="http://localhost:5000/packets"
```

The console app registers an `HttpClient` typed client for `InfraStructure.Sources.HttpPacketSource` using `AddHttpClient<T>` and a Polly retry policy. If you choose `Type: "mock"`, the mock source will be used instead.

Note: The old static `PacketSourceFactory` was removed — IPacketSource selection is done via DI now.
