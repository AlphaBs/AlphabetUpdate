﻿using CommandLine;

namespace AlphabetUpdateServerInstaller
{
    class CommandLineOptions
    {
        [Option('p', "appsettingsPath", 
            Required = false, 
            Default = "appsettings.json",
            HelpText = "Set the path of appsettings.json")]
        public string AppSettingsPath { get; set; }

        [Option('a', "appSettings", 
            Required = false, 
            Default = true,
            HelpText = "Create new app settings")]
        public bool? NewAppSettings { get; set; }

        [Option('s', "secureStorage", 
            Required = false, 
            Default = true,
            HelpText = "Create new keys and secure storage")]
        public bool? NewSecureStorage { get; set; }
        
        [Option("useSecureAesStorage",
            Required = false,
            Default = false)]
        public bool? UseSecureAesStorage { get; set; }

        [Option("debug",
            Required = false,
            Default = false)]
        public bool Debug { get; set; }
    }
}
