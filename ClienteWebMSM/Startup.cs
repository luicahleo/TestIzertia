using System.Net;

namespace ClienteWebMSM;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Configuración del proxy
        //var proxySettings = Configuration.GetSection("SMSWS").Get<ProxySettings>();
        //if (proxySettings.Enabled)
        //{
        //    services.AddHttpClient("HttpClientWithProxy")
        //        .ConfigurePrimaryHttpMessageHandler(() =>
        //    {
        //        return new HttpClientHandler
        //        {
        //            Proxy = new WebProxy(proxySettings.Url, proxySettings.Port)
        //            {
        //                Credentials = new NetworkCredential(proxySettings.Username, proxySettings.Password)
        //            },
        //            UseProxy = true
        //        };
        //    });
        //}
        //else
        //{
        //    services.AddHttpClient("HttpClientWithProxy");
        //}


        services.Configure<ConfigureProxy>(config => Configuration.GetSection("Proxy").Bind(config));

        //Add http client factory
        services.AddHttpClient("HttpClientWithProxy")
           .ConfigurePrimaryHttpMessageHandler((serviceProvider) =>
           {
               var configurationOption = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<ConfigureProxy>>();
               var configuration = configurationOption.Value;
               var proxyAddress = configuration.ProxyUrl;
               var proxyUser = configuration.Username;
               var proxyPass = configuration.Password;
               var useProxy = false;

               var httpClientHandler = new HttpClientHandler();

               if (useProxy)
               {
                   httpClientHandler.Proxy = new WebProxy(proxyAddress)
                   {
                       Credentials = new NetworkCredential(proxyUser, proxyPass)
                   };
               }

               return httpClientHandler;
           });

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
    }
}
