using System.Buffers;
using System.Text;
using System.Text.Json;
using Json.Schema;
using MQTTnet;

namespace tounsiYoyo.Services;

public sealed class MqttTimeService : IAsyncDisposable
{
    private readonly string _brokerHost;
    private readonly int _brokerPort;
    private readonly string _topic;
    private readonly string? _username;
    private readonly string? _password;
    private readonly JsonSchema _schema;
    private IMqttClient? _client;

    public event Action<DateTimeOffset>? TimeReceived;
    public event Action<string>? StatusChanged;

    public bool IsConnected => _client?.IsConnected == true;

    public MqttTimeService(
        string brokerHost,
        int brokerPort,
        string topic,
        string? username = null,
        string? password = null,
        string? schemaPath = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(brokerHost);
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);
        ArgumentOutOfRangeException.ThrowIfLessThan(brokerPort, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(brokerPort, 65535);

        _brokerHost = brokerHost.Trim();
        _brokerPort = brokerPort;
        _topic = topic.Trim();
        _username = username;
        _password = password;

        string resolvedSchemaPath = schemaPath ?? Path.Combine(AppContext.BaseDirectory, "time.schema.json");
        _schema = JsonSchema.FromText(File.ReadAllText(resolvedSchemaPath));
    }

    public async Task ConnectAsync()
    {
        if (IsConnected)
        {
            return;
        }

        IMqttClient client = new MqttClientFactory().CreateMqttClient();
        client.ApplicationMessageReceivedAsync += HandleMqttMessageAsync;
        client.DisconnectedAsync += eventArgs =>
        {
            if (ReferenceEquals(_client, client))
            {
                _client = null;
                client.Dispose();
                StatusChanged?.Invoke("Connexion MQTT interrompue.");
            }

            return Task.CompletedTask;
        };
        _client = client;

        try
        {
            MqttClientOptionsBuilder optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer(_brokerHost, _brokerPort);
            if (!string.IsNullOrWhiteSpace(_username))
            {
                optionsBuilder.WithCredentials(_username, _password);
            }

            await client.ConnectAsync(optionsBuilder.Build());
            await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(_topic).Build());
            StatusChanged?.Invoke($"Connecté et abonné à « {_topic} ».");
        }
        catch
        {
            _client = null;
            client.Dispose();
            throw;
        }
    }

    public async Task DisconnectAsync()
    {
        IMqttClient? client = _client;
        _client = null;
        if (client is null)
        {
            return;
        }

        try
        {
            if (client.IsConnected)
            {
                await client.DisconnectAsync();
            }
        }
        finally
        {
            client.Dispose();
        }
    }

    private Task HandleMqttMessageAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
    {
        string payload = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload.ToArray());

        try
        {
            using JsonDocument document = JsonDocument.Parse(payload);
            JsonElement json = document.RootElement;
            if (!_schema.Evaluate(json).IsValid)
            {
                StatusChanged?.Invoke("Message invalide : le message ne respecte pas time.schema.json.");
                return Task.CompletedTask;
            }

            long unixTimestamp = json.GetProperty("payload").GetProperty("timestamp").GetInt64();
            DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp);
            TimeReceived?.Invoke(dateTime);
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException or FormatException or ArgumentOutOfRangeException)
        {
            StatusChanged?.Invoke($"Message horaire invalide : {ex.Message}");
        }

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
    }
}
