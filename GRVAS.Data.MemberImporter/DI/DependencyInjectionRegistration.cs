using Autofac;

namespace GRVAS.Training.CeuEmailCreator.DI;

internal static class DependencyInjectionRegistration
{
    private const string DC_CONNECTION_STRING = "server=192.168.1.170;user=root;database=grvas_data;port=3306;password=Swimmerboy97";
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        //place holder

    }

    public static void ConfigureContainer(this ContainerBuilder builder, IConfiguration configuration)
    {
        // Hangfire
        //builder.RegisterType<HangfireJobServerService>().As<IHostedService>().SingleInstance();
        builder.RegisterType<RecurringJobManager>().As<IRecurringJobManager>().SingleInstance();

        //Google Sheets
        builder.RegisterType<CredentialProvider>().As<ICredentialProvider>().SingleInstance();
        builder.RegisterType<DataImporter>().As<IDataImporter>().SingleInstance();

        //Processor
        builder.RegisterType<MemberProcessor>().As<IHostedService>().SingleInstance();

        //Database
        builder.RegisterType<DataWriter>().As<IDataWriter>().SingleInstance()
            .WithParameter("connectionString", DC_CONNECTION_STRING);
        builder.RegisterType<TableManager>().As<ITableManager>().SingleInstance()
            .WithParameter("connectionString", DC_CONNECTION_STRING);

        //pi@Talos1
    }


}
