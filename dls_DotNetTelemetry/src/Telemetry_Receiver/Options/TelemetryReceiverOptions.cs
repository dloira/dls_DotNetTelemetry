namespace Telemetry_Receiver.Options
{
    public class TelemetryReceiverOptions
    {
        public TelemetryReceiverDatabaseOptions Database { get; set; } = default!;
    }

    public class TelemetryReceiverDatabaseOptions
    {
        public string ConnectionString { get; set; } = default!;

        public string QueryXmlFilePath { get; set; } = default!;
    }
}
