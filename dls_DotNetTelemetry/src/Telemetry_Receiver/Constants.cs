namespace Telemetry_Receiver
{
    public static class TelemetryReceiverConstants
    {
        public const string COUNTER_TAG_ENVIRONMENT_NAME = "Environment";
        public const string COUNTER_TAG_MACHINE_NAME = "Rol";
        public const string COUNTER_TAG_PROCESSOR_NAME = "Processor";

        public const string HTTP_EVENT_PROCESSING_TIME_METRIC_NAME = "dls_dotnet_telemetry_receiver_http_event_processing_time";
        public const string HTTP_EVENT_PROCESSING_TIME_METRIC_DESCRIPTION = "The elapsed time to process an http event on milliseconds.";
        public const string HTTP_EVENT_PROCESSING_COUNT_METRIC_NAME = "dls_dotnet_telemetry_receiver_http_event_processed_count";
        public const string HTTP_EVENT_PROCESSING_COUNT_METRIC_DESCRIPTION = "The number of http event count processed.";
        public const string HTTP_EVENT_PROCESSING_EXCEPTIONS_METRIC_NAME = "dls_dotnet_telemetry_receiver_http_event_processing_exceptions";
        public const string HTTP_EVENT_PROCESSING_EXCEPTIONS_METRIC_DESCRIPTION = "The number of exceptions processing http events.";

        public const string SECRET_CHARACTERS = "*****";
    }
}
