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

        [LoggerMessage(EventId = 100, Level = LogLevel.Debug, Message = "Query {queryName} requested from queries file.")]
        public partial void RequestedQuery(string queryName);

        [LoggerMessage(EventId = 101, Level = LogLevel.Error, Message = "Queries file path has not been in appsettings file.")]
        public partial void QueriesFileNameNotSet();

        [LoggerMessage(EventId = 102, Level = LogLevel.Error, Message = "Queries file not found in path {queriesFile}")]
        public partial void QueriesFileNotFound(string queriesFile);

        [LoggerMessage(EventId = 103, Level = LogLevel.Error, Message = "Could not find query {queryName} in queries file.")]
        public partial void QueryNotFound(string queryName);

        [LoggerMessage(EventId = 104, Level = LogLevel.Debug, Message = "Query {queryName} successfully found in queries file.")]
        public partial void QueryFound(string queryName);

        [LoggerMessage(EventId = 105, Level = LogLevel.Error, Message = "There are more than one query with name '{queryName}' on XML file '{queryXmlFile}'. Query name must be unique.")]
        public partial void QueryNotUnique(string queryName, string queryXmlFile);

        [LoggerMessage(EventId = 106, Level = LogLevel.Error, Message = "Can't find CDATA with query string for query name '{queryName}' on xml file '{queryXmlFile}'.")]
        public partial void CDataNotFoundForQuery(string queryName, string queryXmlFile);

        [LoggerMessage(EventId = 200, Level = LogLevel.Information, Message = "The receiver endpoints API is starting production.")]
        public partial void StartingReceiver();

        [LoggerMessage(EventId = 201, Level = LogLevel.Information, Message = "The receiver endpoints API is started.")]
        public partial void StartedReceiver();

        [LoggerMessage(EventId = 202, Level = LogLevel.Information, Message = "The receiver endpoints API is stopped.")]
        public partial void StoppedReceiver();

        [LoggerMessage(EventId = 300, Level = LogLevel.Error, Message = "Failed processing HTTP event.")]
        public partial void HttpEventProcessingFailed(Exception exception);

        [LoggerMessage(EventId = 301, Level = LogLevel.Error, Message = "Received HTTP event.")]
        public partial void HttpEventReceived();

        [LoggerMessage(EventId = 302, Level = LogLevel.Debug, Message = "Processed HTTP event successfully.")]
        public partial void HttpEventProcessed();

        [LoggerMessage(EventId = 401, Level = LogLevel.Error, Message = "Error getting weather forecasts from database.")]
        public partial void ErrorGettingWeatherForecast(Exception exception);
    }
}
