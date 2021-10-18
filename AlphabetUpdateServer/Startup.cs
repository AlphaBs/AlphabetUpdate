using System;
using System.IO;
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
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace AlphabetUpdateServer
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            this.configuration = configuration; 
            this.environment = env;
        }

        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;

        private string resolvePath(string path)
        {
            var fullPath = path.Replace("[root]", environment.WebRootPath);
            fullPath = Path.GetFullPath(fullPath);

            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);

            return fullPath;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AlphabetUpdateServer", Version = "v1" });
            });

            var updateFileConf = configuration.GetSection(UpdateFileOptions.UpdateFile);
            var updateFileOptions = new UpdateFileOptions();
            updateFileConf.Bind(updateFileOptions);

            updateFileOptions.BaseUrl.Trim('/');
            updateFileOptions.InputDir = resolvePath(updateFileOptions.InputDir);
            updateFileOptions.OutputDir = resolvePath(updateFileOptions.OutputDir);
            
            services.Configure<AuthOptions>(configuration.GetSection(AuthOptions.Auth));
            services.Configure<UpdateFileOptions>(configuration.GetSection(UpdateFileOptions.UpdateFile));
            services.PostConfigure<UpdateFileOptions>(options =>
            {
                options.InputDir = resolvePath(options.InputDir);
                options.OutputDir = resolvePath(options.OutputDir);
                options.BaseUrl = options.BaseUrl.Trim('/');
            });
            
            if (configuration.GetValue<bool>("UseSecureAesStorage"))
                services.AddSingleton<ISecureStorage, SecureAesStorage>();
            else
                services.AddSingleton<ISecureStorage, SecureConfigStorage>();

            var secretKey = Convert.FromBase64String(configuration["SecureStorage:SecretKey"]);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["Auth:Issuer"],
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
        public void Configure(IApplicationBuilder app, ILogger<Startup> logger)
        {
            if (environment.IsDevelopment())
            {
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
            
            logger.LogInformation("WebRootPath: {WebRootPath}", environment.WebRootPath);
        }
    }
}
