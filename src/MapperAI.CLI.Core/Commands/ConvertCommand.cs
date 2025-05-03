using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using MapperAI.CLI.Core.Services;
using MapperAI.Core.Clients;
using MapperAI.Core.Clients.Models;
using MapperAI.Core.Mappers;
using MapperAI.Core.Mappers.Models;
using MapperAI.Core.Serializers;

namespace MapperAI.CLI.Core.Commands;

[Command("convert", Description = "Converts a file from one language to another.")]
public class ConvertCommand : ICommand
{
    [CommandOption("input", 'i', Description = "Path of the input file.", IsRequired = true)]
    public required string InputPath { get; set; }

    [CommandOption("file", 'f', Description = "Filename", IsRequired = false)]
    public string? Filename { get; set; }

    [CommandOption("language", 'l', Description = "Output language (e.g., cs).", IsRequired = true)]
    public required string Language { get; set; }
    
    [CommandOption("ouput", 'o', Description = "Output folder", IsRequired = true)]
    public required string OutputFolder { get; set; }

    
    public async ValueTask ExecuteAsync(IConsole console)
    {
        ClientConfigurationService configurationService = new();
        MapperClientConfiguration? clientConfiguration = configurationService.GetClientConfiguration();
        if (clientConfiguration == null)
            throw new ApplicationException("If is your first time using MapperAI CLI, you need to configure with configuration command");
        var serializer = new MapperSerializer();
        var mapper = new FileMapper(new MapperClientFactory(serializer), serializer, clientConfiguration);
        var configuration = new FileMapperConfiguration(InputPath, OutputFolder)
        {
            FileName = Filename,
            Extension = Language
        };
        await mapper.MapAsync(configuration);
    }
} 