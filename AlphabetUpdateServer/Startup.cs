using System;
using AlphabetUpdateServer.Models;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace AlphabetUpdateServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AlphabetUpdateServer", Version = "v1" });
            });

            var updateFileOptions = Configuration.GetSection(UpdateFileOptions.UpdateFile);
            services.Configure<UpdateFileOptions>(updateFileOptions);

            var authOptions = Configuration.GetSection(AuthOptions.Auth);
            services.Configure<AuthOptions>(authOptions);
            
            if (Configuration.GetValue<bool>("UseSecureAesStorage"))
                services.AddSingleton<ISecureStorage, SecureAesStorage>();
            else
                services.AddSingleton<ISecureStorage, SecureConfigStorage>();

            var secretKey = Convert.FromBase64String(Configuration["SecureStorage:SecretKey"]);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = Configuration["Auth:Issuer"],
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
                    };
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("manager",
                    policy => policy.Requirements.Add(new JwtRequirement("manager")));
            });
            services.AddSingleton<IAuthorizationHandler, JwtAuthorizationHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                logger.LogInformation("Development mode");

                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AlphabetUpdateServer v1"));

                app.UseStaticFiles(new StaticFileOptions
                {
                    ServeUnknownFileTypes = true
                });
                app.UseDirectoryBrowser();
            }

            var ss = app.ApplicationServices.GetService<ISecureStorage>();
            if (ss != null)
                ss.Load().GetAwaiter().GetResult();
            
            app.UseHttpsRedirection();

            //app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
            //                       ForwardedHeaders.XForwardedProto
            //});

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
