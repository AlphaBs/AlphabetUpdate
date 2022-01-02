using System;
using Microsoft.AspNetCore.Authentication;

namespace AlphabetUpdateServer.BasicAuth
{
    public static class BasicAuthenticationScheme
    {
        public static readonly string AuthenticationScheme = "BasicAuth";

        public static AuthenticationBuilder AddBasicAuth(
            this AuthenticationBuilder builder, 
            Action<BasicAuthenticationSchemeOptions>? configureOptions)
        {
            
            return builder.AddScheme<BasicAuthenticationSchemeOptions, BasicAuthenticationHandler>(
                AuthenticationScheme, configureOptions);
        }
    }
    
    public class BasicAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public string Realm { get; set; } = "";
    }
}