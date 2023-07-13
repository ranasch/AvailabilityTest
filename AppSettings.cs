namespace AvailabilityTest;
using System.Collections.Generic;

public class AppSettings
{
    public string AppInsightsInstrumentationKey { get; set; }

    public List<string> EndpointsToTest { get; set; }

    public int timeoutInMs { get; set; }
}
