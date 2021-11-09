using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using AlphabetUpdate.Common.Helpers;
using AlphabetUpdate.Common.Services;
using AlphabetUpdateServer.Core;
using AlphabetUpdateServer.Models;
using AlphabetUpdateServer.Services;
using Newtonsoft.Json.Linq;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;

namespace AlphabetUpdateServerInstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            //args = "--appSettings=false --secureStorage=true --debug=true".Split(' ');

            var parser = new Parser(settings =>
            {
                settings.CaseSensitive = false;
            });

            parser.ParseArguments<CommandLineOptions>(args)
                .WithParsed(o => 
                {
                    var p = new Program(o);
                    p.Start().GetAwaiter().GetResult();
                });
        }

        public Program(CommandLineOptions o)
        {
            options = o;
        }

        CommandLineOptions options;

        public async Task Start()
        {
            Console.WriteLine(" [AlphabetUpdateServerInstaller] ");

            if (!(options.NewAppSettings ?? false) && !(options.NewSecureStorage ?? false))
            {
                Console.WriteLine("Nothing to do!");
                return;
            }

            var app = await readAppSettings() ?? new AppSettings();

            if (options.NewAppSettings ?? false)
                newAppSettings(app);

            if (options.NewSecureStorage ?? false)
            {
                ServerPassword serverPassword;
                app.UseSecureAesStorage = options.UseSecureAesStorage ?? false;
                if (app.UseSecureAesStorage)
                {
                    var (sp, ssAesKey, ssAesIV) = await newSecureAesStorage();
                    app.SecureStorageKey = ssAesKey;
                    app.SecureStorageIV = ssAesIV;
                    
                    Console.WriteLine($"ssAesKey : {app.SecureStorageKey}");
                    Console.WriteLine($"ssAesIv : {app.SecureStorageIV}");

                    serverPassword = sp;
                }
                else
                {
                    Console.WriteLine("Generating SecureKeys...");
                    var (secureKeys, rawPassword) = generateSecureKeys();
                    
                    app.SecureStorageKey = "";
                    app.SecureStorageIV = "";
                    app.SecureStorage = secureKeys;

                    serverPassword = new ServerPassword
                    {
                        AesIv = secureKeys.AesIV,
                        AesKey = secureKeys.AesKey,
                        RawPassword = rawPassword
                    };
                }
                
                Console.WriteLine("Generated server password : ");
                Console.WriteLine("==========");
                Console.WriteLine(JsonSerializer.Serialize(serverPassword));
                Console.WriteLine("==========");
                
                if (options.Debug)
                {
                    Console.WriteLine("Generated server password: ");
                    await testServerPassword(serverPassword);
                }
            }

            Console.WriteLine("Writing app settings...");
            await writeAppSettings(app);

            Console.WriteLine();
            Console.WriteLine("Done. ");
        }

        private void newAppSettings(AppSettings app)
        {
            Console.WriteLine("\n===== [UpdateFile] =====");
            var updateFile = new UpdateFileOptions()
            {
                Name = query("Name", app.UpdateFile?.Name),
                BaseUrl = query("BaseUrl", app.UpdateFile?.BaseUrl),
                InputDir = query("InputDir", app.UpdateFile?.InputDir),
                OutputDir = query("OutputDir", app.UpdateFile?.OutputDir),
                LauncherInfoPath = query("LauncherInfoPath", app.UpdateFile?.LauncherInfoPath),
                FilesCachePath = query("FilesCachePath", app.UpdateFile?.FilesCachePath)
            };

            Console.WriteLine("\n===== [Auth] =====");
            var auth = new AuthOptions()
            {
                Issuer = query("Issuer", updateFile.Name),
                ExpiresM = int.Parse(query("ExpiresM", "30"))
            };

            app.UpdateFile = updateFile;
            app.Auth = auth;

            Console.WriteLine();
            Console.WriteLine("New app settings generated");
        }

        private (SecureKeys, string) generateSecureKeys()
        {
            // password
            var salt = Guid.NewGuid().ToString().Replace("-", "");
            var password = Guid.NewGuid().ToString().Replace("-", "");

            var saltedPassword = password + salt;
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(saltedPassword);

            // password KEY/IV
            using var passwordAes = Aes.Create();
            passwordAes.Mode = CipherMode.CBC;
            passwordAes.Padding = PaddingMode.PKCS7;
            var passwordAesKey = Convert.ToBase64String(passwordAes.Key);
            var passwordAesIV = Convert.ToBase64String(passwordAes.IV);
            
            // JWT secret key
            using var rng = new RNGCryptoServiceProvider();
            var jwtSecretKeyBytes = new byte[32];
            rng.GetBytes(jwtSecretKeyBytes);
            var jwtSecretKey = Convert.ToBase64String(jwtSecretKeyBytes);
            
            var secureKeys = new SecureKeys
            {
                AesIV = passwordAesIV,
                AesKey = passwordAesKey,
                HashedPassword = hashedPassword,
                Salt = salt,
                SecretKey = jwtSecretKey
            };

            return (secureKeys, password);
        }
        
        private async Task<(ServerPassword, string, string)> newSecureAesStorage()
        {
            Console.WriteLine("Generating Keys...");
            
            // SecureStorage
            var (secureKeys, rawPassword) = generateSecureKeys();
            
            // secure storage KEY/IV
            using var ssAes = Aes.Create();
            ssAes.Mode = CipherMode.CBC;
            var ssAesKey = Convert.ToBase64String(ssAes.Key);
            var ssAesIV = Convert.ToBase64String(ssAes.IV);
            ssAes.Padding = PaddingMode.PKCS7;

            Console.WriteLine("Writing SecureStorage...");

            var ss = SecureAesStorage.FromAes(ssAes);
            await ss.Save(secureKeys);

            var serverPassword = new ServerPassword()
            {
                RawPassword = rawPassword,
                AesKey = secureKeys.AesKey,
                AesIv = secureKeys.AesIV
            };
            //await test(Convert.ToBase64String(ssAes.Key), Convert.ToBase64String(ssAes.IV));
            return (serverPassword, ssAesKey, ssAesIV);
        }

        private async Task<AppSettings> readAppSettings()
        {
            if (!File.Exists(options.AppSettingsPath))
                return null;

            return await Util.ReadJson<AppSettings>(options.AppSettingsPath);
        }

        private async Task writeAppSettings(AppSettings app)
        {
            if (!File.Exists(options.AppSettingsPath))
            {
                Console.WriteLine("no app settings file");
                return;
            }

            var content = await File.ReadAllTextAsync(options.AppSettingsPath);
            var jobj = JObject.Parse(content);

            jobj["SecureStorageKey"] = app.SecureStorageKey;
            jobj["SecureStorageIV"] = app.SecureStorageIV;
            jobj["Auth"] = JObject.FromObject(app.Auth);
            jobj["UpdateFile"] = JObject.FromObject(app.UpdateFile);
            jobj["SecureStorage"] = JObject.FromObject(app.SecureStorage);

            await File.WriteAllTextAsync(options.AppSettingsPath, jobj.ToString());
        }

        static async Task testServerPassword(ServerPassword serverPassword)
        {
            var aes = new AesObjectService(serverPassword.AesKey, serverPassword.AesIv);
            var ms = new MemoryStream();
            await aes.AesEncrypt(new LoginModel
            {
                Name = "devTest",
                Host = "127.0.0.1",
                Password = serverPassword.RawPassword
            }, ms);
            ms.Position = 0;
            var res = await aes.AesDecrypt<LoginModel>(ms);
            Console.WriteLine(JsonSerializer.Serialize(res));
            await ms.DisposeAsync();
        }
        
        static async Task test(string key, string iv)
        {
            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Key = Convert.FromBase64String(key);
            aes.IV = Convert.FromBase64String(iv);
            aes.Padding = PaddingMode.PKCS7;

            var ss = SecureAesStorage.FromAes(aes);
            var obj = await ss.Load();

            Console.WriteLine(JsonSerializer.Serialize(obj));
        }

        private string query(string name, string defaultValue)
        {
            Console.Write($"{name}? ");
            if (!string.IsNullOrEmpty(defaultValue))
                Console.Write($"({defaultValue})");

            Console.Write("\n> ");

            var v = Console.ReadLine();
            if (string.IsNullOrEmpty(v))
                return defaultValue;
            else
                return v;

        }
    }
}
