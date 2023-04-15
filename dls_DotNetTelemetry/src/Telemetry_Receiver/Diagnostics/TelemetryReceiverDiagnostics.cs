using Telemetry_Receiver.Options;
using Microsoft.Extensions.Options;
using OpenTelemetry.Context.Propagation;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Telemetry_Receiver.Diagnostics
{
    public class TelemetryReceiverDiagnostics
    {
        public readonly ActivitySource ActivitySource = new ActivitySource(nameof(TelemetryReceiver));
        public readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

        private readonly TelemetryReceiverLogging _logs;
        private readonly IOptionsMonitor<TelemetryReceiverOptions> _options;
        private Meter _receiverMeter = default!;

        private Counter<long> _httpEventProcessingExceptions = default!;
        private Counter<long> _httpEventProcessingCount = default!;
        private Histogram<long> _httpEventProcessingTime = default!;

        private KeyValuePair<string, object?>[] _defaultTags = default!;

        public TelemetryReceiverDiagnostics(TelemetryReceiverLogging logs, IHostEnvironment hostEnvironment, IOptionsMonitor<TelemetryReceiverOptions> options)
        {
            _logs = logs ?? throw new ArgumentNullException(nameof(logs));
            _ = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _options = options ?? throw new ArgumentNullException(nameof(options));

            _receiverMeter = new Meter(nameof(TelemetryReceiver));

            InitializeCounters(hostEnvironment.EnvironmentName);
        }

        private void InitializeCounters(string environmentName)
        {
            _receiverMeter = new Meter(nameof(TelemetryReceiver));

            _httpEventProcessingCount = _receiverMeter.CreateCounter<long>(
                name: TelemetryReceiverConstants.HTTP_EVENT_PROCESSING_COUNT_METRIC_NAME,
                description: TelemetryReceiverConstants.HTTP_EVENT_PROCESSING_COUNT_METRIC_DESCRIPTION);

            _httpEventProcessingExceptions = _receiverMeter.CreateCounter<long>(
                name: TelemetryReceiverConstants.HTTP_EVENT_PROCESSING_EXCEPTIONS_METRIC_NAME,
                description: TelemetryReceiverConstants.HTTP_EVENT_PROCESSING_EXCEPTIONS_METRIC_DESCRIPTION);

            _httpEventProcessingTime = _receiverMeter.CreateHistogram<long>(
              name: TelemetryReceiverConstants.HTTP_EVENT_PROCESSING_TIME_METRIC_NAME,
              description: TelemetryReceiverConstants.HTTP_EVENT_PROCESSING_TIME_METRIC_DESCRIPTION);

            _defaultTags = new KeyValuePair<string, object?>[]
            {
                 new KeyValuePair<string, object?>(TelemetryReceiverConstants.COUNTER_TAG_ENVIRONMENT_NAME, environmentName),
                 new KeyValuePair<string, object?>(TelemetryReceiverConstants.COUNTER_TAG_MACHINE_NAME, Environment.MachineName)
            };
        }

        public void StartingReceiver()
        {
            _logs.StartingReceiver();
        }

        public void StartedReceiver()
        {
            _logs.StartedReceiver();
        }

        public void StoppedReceiver()
        {
            _logs.StoppedReceiver();
        }

        public void EventProcessingFailed(Exception error)
        {
            _httpEventProcessingExceptions.Add(1, _defaultTags);

            _logs.HttpEventProcessingFailed(error);
        }

        public void EventReceived()
        {
            _httpEventProcessingCount.Add(1, _defaultTags);

            _logs.HttpEventReceived();
        }

        public void EventProcessed(long processingTime)
        {
            _httpEventProcessingTime.Record(processingTime, _defaultTags.ToArray());

            _logs.HttpEventProcessed();
        }
    }
}
