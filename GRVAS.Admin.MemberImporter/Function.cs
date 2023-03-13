using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using GRVAS.Admin.MemberImporter.Dynamo;
using GRVAS.Admin.MemberImporter.Processor;
using GRVAS.Data.MemberImporter.Sheets;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace GRVAS.Admin.MemberImporter;

public class Function
{
    private const string TABLE_NAME = "GrvasMembers";

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

            var result = memberProcessor.ProcessAsync().Result;

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

            builder.RegisterType<AmazonDynamoDBClient>().As<IAmazonDynamoDB>().SingleInstance();

            //var senderEmail = Environment.GetEnvironmentVariable(SENDER_EMAIL);
            //var destinationEmail = Environment.GetEnvironmentVariable(DESTINATION_EMAIL);
            
            //Processor
            builder.RegisterType<MemberProcessor>().As<IMemberProcessor>().SingleInstance();
            
            //Google sheeets
            builder.RegisterType<DataImporter>().As<IDataImporter>().SingleInstance();
            builder.RegisterType<CredentialProvider>().As<ICredentialProvider>().SingleInstance();

            //Dynamo
            builder.RegisterType<TableCreator>().As<ITableCreator>().SingleInstance()
                .WithParameter("tableName", TABLE_NAME);
            builder.RegisterType<TableDeleter>().As<ITableDeleter>().SingleInstance()
                .WithParameter("tableName", TABLE_NAME);
            builder.RegisterType<MemberInserter>().As<IMemberInserter>().SingleInstance()
                .WithParameter("tableName", TABLE_NAME);


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