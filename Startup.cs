using Microsoft.Azure.Functions.Extensions.DependencyInjection;
[assembly: FunctionsStartup(typeof(AvailabilityTest.Startup))]

namespace AvailabilityTest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var configBuild = new ConfigurationBuilder()
           .SetBasePath(Environment.CurrentDirectory)
           .AddJsonFile("appsettings.json", true)
           .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
           .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
           .AddEnvironmentVariables();

        var config = configBuild.Build();

        var appSettings = config.GetSection("AppSettings").Get<AppSettings>();

        builder.Services.AddSingleton(appSettings);
    }
}
