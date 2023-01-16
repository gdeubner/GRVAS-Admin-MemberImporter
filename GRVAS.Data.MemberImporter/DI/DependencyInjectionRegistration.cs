using Autofac;
using GRVAS.Data.MemberImporter.Job;
using GRVAS.Data.MemberImporter.Sheets;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GRVAS.Training.CeuEmailCreator.DI;

internal static class DependencyInjectionRegistration
{
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

        //Database
        builder.RegisterType<DataImporter>().As<IDataImporter>().SingleInstance();

        //Processor
        builder.RegisterType<MemberProcessor>().As<IHostedService>().SingleInstance();
    }


}
