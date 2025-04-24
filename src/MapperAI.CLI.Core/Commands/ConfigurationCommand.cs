using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using MapperAI.CLI.Core.Services;
using MapperAI.Core.Clients.Models;
using MapperAI.Core.Enums;

namespace MapperAI.CLI.Core.Commands;

[Command("configuration", Description = "LLM Api Configuration.")]

public class ConfigurationCommand : ICommand
{
    [CommandOption("model_type", 't', Description = "Model type.", IsRequired = true)]
    public required int ModelType { get; set; }

    [CommandOption("model_name", 'n', Description = "Model name.", IsRequired = true)]
    public required string ModelName { get; set; }

    [CommandOption("key", 'k', Description = "Api key.", IsRequired = false)]
    public string? ApiKey { get; set; }

    [CommandOption("key_enviroment", 'e', Description = "Key is by environment.", IsRequired = false)]

    public bool IsKeyByEnvironment { get; set; } = false;

    
    public ValueTask ExecuteAsync(IConsole console)
    {
        ModelType modelType = GetModelType(ModelType);
        ClientConfiguration clientConfiguration = new()
        {
            Model = ModelName,
            Type = modelType,
            ApiKey = ApiKey
        };

        ClientConfigurationService configurationService = new ClientConfigurationService();
        configurationService.SaveClientConfigurationFile(IsKeyByEnvironment, clientConfiguration);
        return default;
    }

    private ModelType GetModelType(int modelTypeValue)
    {
        if (Enum.IsDefined(typeof(ModelType), modelTypeValue))
        {
            return (ModelType)modelTypeValue;
        }

        throw new ArgumentException("Model type is invalid!");
    }

}