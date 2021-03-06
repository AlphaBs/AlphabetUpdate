using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AlphabetUpdateHub.Models;
using AlphabetUpdateHub.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AlphabetUpdateHub
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration conf)
        {
            environment = env;
            configuration = conf;
        }

        private readonly IWebHostEnvironment environment;
        private readonly IConfiguration configuration;
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            services.Configure<DatabaseSettings>(
                configuration.GetSection(nameof(DatabaseSettings)));
            var authOptionsConf = configuration.GetSection(AuthOptions.AuthOptionName);
            var authOptions = new AuthOptions();
            authOptionsConf.Bind(authOptions);
            services.Configure<AuthOptions>(authOptionsConf);

            services.AddSingleton<UpdateServerMetadataService>();

            var secretKey = Convert.FromBase64String(authOptions.SecretKey);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = authOptions.Issuer,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
                    };
                });
            services.AddAuthorization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILogger<Startup> logger)
        {
            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();
            var forwardedOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };
            forwardedOptions.KnownProxies.Clear();
            forwardedOptions.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("172.16.0.0"), 12));
            app.UseForwardedHeaders(forwardedOptions);

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            //logger.LogInformation("WebRootPath: {WebRootPath}", environment.WebRootPath);
        }
    }
}