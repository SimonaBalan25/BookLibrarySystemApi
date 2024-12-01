
using BookLibrarySystem.Logic.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace BookLibrarySystem.Logic.Services
{
    public class TelemetryService : ITelemetryService
    {
        private readonly TelemetryClient _telemetryClient;

        public TelemetryService(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public void TrackEvent(string eventName)
        {
            _telemetryClient.TrackTrace(eventName);
        }

        public void TrackEvent(string eventName, SeverityLevel level, IDictionary<string, string> properties)
        {
            _telemetryClient.TrackTrace(eventName, level, properties);
        }
    }
}
