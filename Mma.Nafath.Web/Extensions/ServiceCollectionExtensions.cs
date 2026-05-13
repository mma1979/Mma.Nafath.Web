using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mma.Nafath.Web.Configuration;
using Mma.Nafath.Web.Services;
using System.Security.Cryptography.X509Certificates;

namespace Mma.Nafath.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNafathWebAuthentication(this IServiceCollection services, Action<NafathWebOptions> configureOptions)
        {
            services.Configure(configureOptions);

            services.AddHttpClient<INafathWebService, NafathWebService>((serviceProvider, client) =>
            {
                // Basic configuration can be done here if needed, 
                // but NafathWebService constructor also handles BaseUrl.
            })
            .ConfigurePrimaryHttpMessageHandler(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<NafathWebOptions>>().Value;
                var handler = new HttpClientHandler();

                if (options.UseCertificate && !string.IsNullOrEmpty(options.CertificatePath))
                {
                    try
                    {
                        // Use X509CertificateLoader for .NET 9+ or X509Certificate2 constructor
                        // We'll use X509Certificate2 for broader compatibility if possible, 
                        // but the project uses net10.0, so X509CertificateLoader is fine.
                        
#if NET9_0_OR_GREATER
                        var certificate = X509CertificateLoader.LoadPkcs12FromFile(
                            options.CertificatePath, 
                            options.CertificatePassword, 
                            X509KeyStorageFlags.MachineKeySet);
#else
                        var certificate = new X509Certificate2(
                            options.CertificatePath, 
                            options.CertificatePassword, 
                            X509KeyStorageFlags.MachineKeySet);
#endif
                            
                        handler.ClientCertificates.Add(certificate);
                        handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    }
                    catch (Exception)
                    {
                        // Log or handle certificate loading error
                        // Since we are in DI registration, we might want to let it fail or log a warning.
                    }
                }

                return handler;
            });

            return services;
        }
    }
}
