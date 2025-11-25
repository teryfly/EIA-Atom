namespace EIA.S0.WebApi.Settings;

/// <summary>
/// WebApi 层用于绑定配置的 KafkaSettings.
/// </summary>
public class KafkaSettings
{
    /// <summary>
    /// 消费组 / UserID.
    /// </summary>
    public string UserID { get; set; } = string.Empty;

    /// <summary>
    /// Kafka bootstrap servers.
    /// </summary>
    public string BootstrapServers { get; set; } = string.Empty;
}