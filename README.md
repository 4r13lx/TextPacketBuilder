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

## Running with Docker

Build the image (there is a `TextPacketBuilder/Dockerfile` in the repo):

```bash
docker build -t textpacketbuilder:latest -f TextPacketBuilder/Dockerfile .
```

Run the container and set configuration via environment variables:

```bash
docker run --rm -e PacketSource__Type=http -e PacketSource__BaseUrl="http://host.docker.internal:5000/packets" textpacketbuilder:latest
```

If you prefer `mock` source:

```bash
docker run --rm -e PacketSource__Type=mock textpacketbuilder:latest
```

## Running with docker-compose

An example `docker-compose` service entry:

```yaml
services:
	textpacketbuilder:
		build:
			context: .
			dockerfile: TextPacketBuilder/Dockerfile
		environment:
			- PacketSource__Type=http
			- PacketSource__BaseUrl=http://packets:5000/packets
		depends_on:
			- packets

	packets:
		image: some/fake-packet-server:latest
		ports:
			- "5000:5000"
```

## Example GitHub Actions snippet

Below is a minimal job example that builds and runs the console app and sets `PacketSource` via environment variables. You can include this in `.github/workflows/` or adapt your existing workflow.

```yaml
name: dotnet-example

on: [push]

jobs:
	build-run:
		runs-on: ubuntu-latest
		steps:
			- uses: actions/checkout@v4
			- name: Setup .NET
				uses: actions/setup-dotnet@v4
				with:
					dotnet-version: '8.0.x'
			- name: Build
				run: dotnet build TextPacketBuilder/TextPacketBuilder.sln --configuration Release --no-restore
			- name: Run (mock source)
				run: dotnet run --project TextPacketBuilder/TextPacketBuilder.csproj --configuration Release
				env:
					PacketSource__Type: mock

```
