using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using App.Configurations;
using Microsoft.Extensions.Hosting; // to access type hostoption; BackgroundServiceExceptionBehavior  
using BackendStoreManager;
using FileHandler;
using PartDataManager;
namespace BackendServices {

    public static class Services{

        public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder, SystemConfigurations configs ){
            //not to stop due to failure of background services
            builder.Services.Configure<HostOptions>(
                hostOptions=>
                {
                    hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
                }
            );
            builder.Services.AddCors(options =>
                {
                    options.AddDefaultPolicy(
                        policy =>
                        {
                            policy.AllowAnyOrigin();
                            policy.AllowAnyHeader();
                        });
                });
            builder.Services.AddSingleton<runTimeConfiguration>();
            builder.Services.AddSingleton<authKeeper>();
            builder.Services.AddScoped<TokenManager>();
            builder.Services.AddTransient<IFileHandler,ResultHandler>(sp=>new ResultHandler(configs.resultFile));
            builder.Services.AddSingleton<IpartDataHandler,PartDataHandler>(sp=>new PartDataHandler(configs.qdasConfig));
            return builder;
        }
    }
}
