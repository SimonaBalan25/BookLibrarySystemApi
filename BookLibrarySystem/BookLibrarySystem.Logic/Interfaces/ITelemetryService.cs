

using Microsoft.ApplicationInsights.DataContracts;

namespace BookLibrarySystem.Logic.Interfaces
{
    public interface ITelemetryService
    {
        void TrackEvent(string eventName);

        void TrackEvent(string eventName, SeverityLevel level, IDictionary<string,string> properties);
    }
}
