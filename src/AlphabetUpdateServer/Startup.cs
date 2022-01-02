using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AlphabetUpdateServer.BasicAuth;
using AlphabetUpdateServer.Jwt;
using AlphabetUpdateServer.Models;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
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

            var authSchemeConfig = configuration.GetValue<string>("Auth:Scheme").ToLower();
            
            string authenticationScheme;
            if (authSchemeConfig == "jwt")
                authenticationScheme = JwtBearerDefaults.AuthenticationScheme;
            else
                authenticationScheme = BasicAuthenticationScheme.AuthenticationScheme;

            services.AddAuthentication(authenticationScheme)
                .AddJwtBearer(options =>
                {
                    var secretKey = Convert.FromBase64String(configuration["SecureStorage:SecretKey"]);
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["Auth:Issuer"],
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                        RoleClaimType = "r"
                    };
                    options.Events = new JwtBearerEvents()
                    {
                        OnTokenValidated = JwtTokenValidator.ValidateAudience
                    };
                })
                .AddBasicAuth(null);

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IClientAddressResolver, RemoteClientAddressResolver>();
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

            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        Result = false,
                        Message = exceptionHandler.Error
                    });
                });
            });

            var ss = app.ApplicationServices.GetService<ISecureStorage>();
            ss?.Load().GetAwaiter().GetResult();
            
            //app.UseHttpsRedirection();
            //var forwardedOptions = new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //};
            //forwardedOptions.KnownProxies.Clear();
            //forwardedOptions.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("172.16.0.0"), 12));
            //app.UseForwardedHeaders(forwardedOptions);

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
