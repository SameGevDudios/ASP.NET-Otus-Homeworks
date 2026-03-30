using Microsoft.AspNetCore.HttpOverrides;

namespace CorsSetup
{
    public class Startup
    {
        public readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var allowedOrigins = Configuration.GetSection("FrontendSettings:AllowedOrigins").Get<string[]>();

            if (allowedOrigins is null || allowedOrigins.Count() == 0)
            {
                throw new Exception("No origins configured. Make sure you set tp appsettings.json");
            }

            services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", builder =>
                {
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownIPNetworks.Clear();
                options.KnownProxies.Clear();
            });

            services.AddControllers();
            services.AddOpenApiDocument(settings =>
            {
                settings.DocumentName = "v1";
                settings.Title = "Forecast API";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseOpenApi();
            app.UseSwaggerUi(x =>
            {
                x.DocExpansion = "list";
            });

            app.UseRouting();

            app.UseCors("FrontendPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
