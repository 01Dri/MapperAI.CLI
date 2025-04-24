using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using MapperAI.Core.Clients;
using MapperAI.Core.Clients.Models;
using MapperAI.Core.Enums;
using MapperAI.Core.Mappers;
using MapperAI.Core.Mappers.Models;
using MapperAI.Core.Serializers;
using MapperAI.Services;

namespace MapperAI.Commands;

[Command("convert", Description = "Converts a file from one language to another.")]
public class ConvertCommand : ICommand
{
    [CommandOption("input", 'i', Description = "Path of the input file.", IsRequired = true)]
    public string InputPath { get; set; }

    [CommandOption("file", 'f', Description = "Filename", IsRequired = false)]
    public string? Filename { get; set; }

    [CommandOption("language", 'l', Description = "Output language (e.g., cs).", IsRequired = true)]
    public string Language { get; set; }
    
    [CommandOption("ouput", 'o', Description = "Output folder", IsRequired = true)]
    public string OutputFolder { get; set; }

    
    public async ValueTask ExecuteAsync(IConsole console)
    {
        ClientConfigurationService configurationService = new();
        ClientConfiguration? clientConfiguration = configurationService.GetClientConfiguration();
        if (clientConfiguration == null)
            throw new ApplicationException(
                "If is your first time using MapperAI CLI, you need to configure with configuration command");
        var serializer = new MapperSerializer();
        var mapper = new FileMapper(new ClientFactoryAI(serializer), serializer);
        var configuration = new FileMapperConfiguration(InputPath, OutputFolder)
        {
            FileName = Filename,
            Extension = Language
        };
        await mapper.MapAsync(configuration, clientConfiguration);
    }
}