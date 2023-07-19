using System;
using System.Net.NetworkInformation;
using System.Text;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AvailabilityTest
{
    public class AvailabilityTests
    {
        private readonly AppSettings _appSettings;
        public AvailabilityTests(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        [FunctionName("TestAvailability")]
        public void Run([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var appInsights = new TelemetryClient(
                new Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration
                {
                    ConnectionString = $"InstrumentationKey={_appSettings.AppInsightsInstrumentationKey}"
                });

            Ping pingSender = new Ping();

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            // Wait 10 seconds for a reply.
            int timeout = _appSettings.timeoutInMs;

            // Set options for transmission:
            // The data can go through 64 gateways or routers
            // before it is destroyed, and the data packet
            // cannot be fragmented.
            PingOptions options = new PingOptions(64, true);

            foreach (var endpoint in _appSettings.EndpointsToTest)
            {
                // Send the request.
                PingReply reply = pingSender.Send(endpoint, timeout, buffer, options);

                if (reply.Status == IPStatus.Success)
                {
                    appInsights.TrackAvailability(reply.Address.ToString(), DateTimeOffset.Now, TimeSpan.FromSeconds(reply.RoundtripTime), "TestAvailabilityFct", true);
                    Console.WriteLine("Address: {0}", reply.Address.ToString());
                    Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                    Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
                    Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                    Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);
                }
                else
                {
                    appInsights.TrackAvailability(endpoint, DateTimeOffset.Now, TimeSpan.FromSeconds(0), "TestAvailabilityFct", false);
                    Console.WriteLine(reply.Status);
                }
            }

            
        }
    }
}
 