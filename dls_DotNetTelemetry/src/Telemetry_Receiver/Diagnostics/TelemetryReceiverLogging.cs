namespace Telemetry_Receiver.Diagnostics
{
    // This one is a partial class because the dotnet source generators creates the rest of the code for LoggerMessage
    public partial class TelemetryReceiverLogging
    {
        private readonly ILogger _logger;

        public TelemetryReceiverLogging(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(nameof(TelemetryReceiver));
        }

        [LoggerMessage(EventId = 100, Level = LogLevel.Information, Message = "The receiver endpoints API is starting production.")]
        public partial void StartingReceiver();

        [LoggerMessage(EventId = 101, Level = LogLevel.Information, Message = "The receiver endpoints API is started.")]
        public partial void StartedReceiver();

        [LoggerMessage(EventId = 102, Level = LogLevel.Information, Message = "The receiver endpoints API is stopped.")]
        public partial void StoppedReceiver();

        [LoggerMessage(EventId = 200, Level = LogLevel.Error, Message = "Failed processing HTTP event.")]
        public partial void HttpEventProcessingFailed(Exception exception);

        [LoggerMessage(EventId = 201, Level = LogLevel.Error, Message = "Received HTTP event.")]
        public partial void HttpEventReceived();

        [LoggerMessage(EventId = 202, Level = LogLevel.Debug, Message = "Processed HTTP event successfully.")]
        public partial void HttpEventProcessed();
    }
}
