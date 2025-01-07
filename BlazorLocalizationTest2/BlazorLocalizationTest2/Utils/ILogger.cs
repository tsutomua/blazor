using System;
using System.Collections.Generic;

namespace Utils
{
    public interface ILogger
    {
        void TrackEvent(string message, string correlationId, Dictionary<string, string> properties = null, Dictionary<string, double> metrics = null);
        void TrackException(Exception ex, string message, string correlationId, Dictionary<string, string> properties = null);
    }
}