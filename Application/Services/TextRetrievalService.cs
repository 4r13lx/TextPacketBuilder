using Domain.Entities;
using Domain.Interfaces;
using Domain.Services;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class TextRetrievalService
{
    private readonly IPacketSource _packetSource;
    private readonly Domain.Interfaces.IPacketValidator _validator;
    private readonly TextAssembler _assembler;
    private readonly ILogger<TextRetrievalService> _logger;

    public TextRetrievalService(
        IPacketSource packetSource,
    Domain.Interfaces.IPacketValidator validator,
        TextAssembler assembler,
        ILogger<TextRetrievalService> logger)
    {
        _packetSource = packetSource;
        _validator = validator;
        _assembler = assembler;
        _logger = logger;
    }

    public async Task<string> GetFullTextAsync(bool restart = true)
    {
        if (restart)
        {
            _logger.LogInformation("Reiniciando fuente de paquetes...");
            await _packetSource.RestartAsync();
        }

        _logger.LogInformation("Obteniendo paquetes...");
        var packets = await _packetSource.GetTextPacketsAsync();

        if (!_validator.Validate(packets))
        {
            _logger.LogError("Paquetes inválidos o corruptos");
            throw new InvalidOperationException("Secuencia de paquetes no válida");
        }

        var result = _assembler.Assemble(packets);
        _logger.LogInformation("Texto reconstruido correctamente");
        return result;
    }
}
