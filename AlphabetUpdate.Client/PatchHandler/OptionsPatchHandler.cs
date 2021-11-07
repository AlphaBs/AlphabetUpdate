using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Downloader;
using CmlLib.Utils;
using log4net;

namespace AlphabetUpdate.Client.PatchHandler
{
    public class OptionsPatchHandler : IPatchHandler
    {
        private static readonly ILog log = LogManager.GetLogger(nameof(OptionsPatchHandler));
        
        public OptionsPatchHandler()
        {
            
        }

        public event DownloadFileChangedHandler? FileChanged;
        public event ProgressChangedEventHandler? ProgressChanged;
        public async Task Patch(PatchContext context)
        {
            
        }
        
        private void SetOptionFiles(MinecraftPath path, Dictionary<string, string> tagFileName)
        {
            if (tagFileName.Count > 0)
            {
                string res;
                if (tagFileName.TryGetValue("res_", out res))
                {
                    SetResourcePack(path, res);
                }

                string shader;
                if (tagFileName.TryGetValue("shader_", out shader))
                {
                    shader = Path.GetFileName(shader);
                    log.Info("optionsshaders.txt, " + shader);
                    var shaderPath = Path.Combine(path.BasePath, "optionsshaders.txt");
                    var shaderoptions = GetShaderOptions(shaderPath);

                    shaderoptions["shaderPack"] = shader;

                    SaveShaderOptions(shaderPath, shaderoptions);
                }
            }
        }

        private void SetResourcePack(MinecraftPath path, string res)
        {
            res = Path.GetFileName(res);
            log.Info("options.txt " + res);

            var optionPath = Path.Combine(path.BasePath, "options.txt");
            var gameoptions = GetOptions(optionPath);
            gameoptions?.SetRawValue("resourcePacks", $"[\"{res}\"]");
            gameoptions?.Save();
        }

        private GameOptionsFile GetOptions(string optionPath)
        {
            GameOptionsFile gameoptions;

            if (File.Exists(optionPath))
                gameoptions = GameOptionsFile.ReadFile(optionPath);
            else
                gameoptions = null;
            return gameoptions;
        }

        private Dictionary<string, string> GetShaderOptions(string optionPath)
        {
            Dictionary<string, string> options = new Dictionary<string, string>();
            if (File.Exists(optionPath))
            {
                using (var sr = new StreamReader(File.OpenRead(optionPath)))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(line))
                            continue;

                        var spl = line.Split('=');

                        var key = spl[0];
                        string value = null;

                        if (spl.Length > 1)
                        {
                            var values = new string[spl.Length - 1];

                            Array.Copy(spl, 1, values, 0, spl.Length - 1);
                            value = string.Join("=", values);
                        }

                        options[key] = value;
                    }
                }
            }

            return options;
        }

        private void SaveShaderOptions(string optionPath, Dictionary<string, string> options)
        {
            try
            {
                using (var sw = new StreamWriter(File.OpenWrite(optionPath)))
                {
                    foreach (KeyValuePair<string, string> item in options)
                    {
                        string line = item.Key;
                        if (item.Value != null)
                            line += "=" + item.Value;

                        sw.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Info("SaveShaderOptions Exception");
                log.Info(ex);
            }
        }
    }
}