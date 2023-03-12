using Amazon.Lambda.Core;
using GRVAS.Admin.MemberImporter.Processor;
using GRVAS.Data.MemberImporter.Sheets;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace GRVAS.Admin.MemberImporter;

public class Function
{
    /// <summary>
    /// Default constructor that Lambda will invoke.
    /// </summary>
    public bool FunctionHandler()
    {
        try
        {
            LambdaLogger.Log("Member importer function Triggered");

            // Start up container
            var serviceProvider = ConfigureServices();

            var memberProcessor = serviceProvider.GetService<IMemberProcessor>();

            memberProcessor?.ProcessAsync();

            LambdaLogger.Log("Function Finished");
            return true;
        }
        catch (Exception e)
        {
            LambdaLogger.Log($"Function finished with exception: [{e}]");
            return false;
        }
    }

    private static IServiceProvider ConfigureServices()
    {
        try
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.dev.json", optional: false)
               .Build();

            // Environment Variables
            var environment = (Environment.GetEnvironmentVariable("ENV") ?? "dev").ToLower();

            var builder = new ContainerBuilder();
            var services = new ServiceCollection();

            // Services
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                builder.AddLambdaLogger();
            });

            //var senderEmail = Environment.GetEnvironmentVariable(SENDER_EMAIL);
            //var destinationEmail = Environment.GetEnvironmentVariable(DESTINATION_EMAIL);

            builder.RegisterType<MemberProcessor>().As<IMemberProcessor>().SingleInstance();
            builder.RegisterType<DataImporter>().As<IDataImporter>().SingleInstance();
            builder.RegisterType<CredentialProvider>().As<ICredentialProvider>().SingleInstance();

            //Build 
            builder.Populate(services);
            var container = builder.Build();
            var serviceProvider = new AutofacServiceProvider(container);
            return serviceProvider;
        }
        catch (Exception e)
        {
            LambdaLogger.Log($"An error occured while configuring services for the lambda. Ex: {e}");

            return null;
        }
    }

}