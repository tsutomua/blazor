using System;
using System.Collections.Generic;

namespace Utils
{
    public class TestLogger : ILogger
    {
        public void TrackEvent(string message, string correlationId, Dictionary<string, string> properties = null, Dictionary<string, double> metrics = null)
        {
            // No op.
        }

        public void TrackException(Exception ex, string message, string correlationId, Dictionary<string, string> properties = null)
        {
            // No op.
        }
    }
}
