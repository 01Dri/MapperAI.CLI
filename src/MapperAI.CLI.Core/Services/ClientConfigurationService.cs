using System.Runtime.InteropServices;
using System.Text.Json;
using MapperAI.Core.Clients.Models;
using MapperAI.Core.Enums;

namespace MapperAI.Services;

public class ClientConfigurationService
{

    private readonly ClientConfiguration _clientConfiguration;

    private readonly string _windowsPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MapperAI");
    
    private readonly string _linuxPath = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME") ??
                                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config");

    public ClientConfigurationService()
    {
    }

    public ClientConfigurationService(ClientConfiguration clientConfiguration)
    {
        _clientConfiguration = clientConfiguration;
    }

    public ClientConfiguration? GetClientConfiguration()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            string fileJson = File.ReadAllText(Path.Combine(_windowsPath, "config.json"));
            ClientConfigurationJson? jsonConfig = JsonSerializer.Deserialize<ClientConfigurationJson>(fileJson);
            if (jsonConfig == null)
                throw new ApplicationException(
                    "If is your first time using MapperAI CLI, you need to configure with configuration command");
            return ToResult(jsonConfig);
        }

        return ToResult(JsonSerializer.Deserialize<ClientConfigurationJson>(File.ReadAllText(_linuxPath)));
    }


    public void SaveClientConfigurationFile(bool isKeyByEnvironment)
    {
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Directory.CreateDirectory(_windowsPath);
            CreateJsonFile(_clientConfiguration, _windowsPath, isKeyByEnvironment);
            return;
        }
        
        CreateJsonFile(_clientConfiguration, _linuxPath, isKeyByEnvironment);

    }
    
    private void CreateJsonFile(ClientConfiguration clientConfiguration, string path, bool isKeyByEnvironment)
    {
        string clientConfigurationJson = JsonSerializer.Serialize(new ClientConfigurationJson(clientConfiguration.Model, clientConfiguration.ApiKey, clientConfiguration.Type, isKeyByEnvironment));
        string jsonPath = Path.Combine(path, "config.json"); 
        File.WriteAllText(jsonPath, clientConfigurationJson);
    }

    private ClientConfiguration ToResult(ClientConfigurationJson? json)
    {
        return new ClientConfiguration()
        {
            ApiKey = json.IsKeyByEnvironment
                ? Environment.GetEnvironmentVariable(json.ApiKey ?? "")
                : json.ApiKey,
            Model = json.Model,
            Type = json.Type
        };
    }
}

class ClientConfigurationJson
{
    public string Model { get; set; }

    public string? ApiKey { get; set; }

    public ModelType Type { get; set; }
    
    public bool IsKeyByEnvironment { get; set; }

    public ClientConfigurationJson(string model, string? apiKey, ModelType type, bool isKeyByEnvironment)
    {
        Model = model;
        ApiKey = apiKey;
        Type = type;
        IsKeyByEnvironment = isKeyByEnvironment;
    }
}