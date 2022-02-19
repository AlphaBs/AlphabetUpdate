using AlphabetUpdate.Client.Patch.Core.Services;
using System.IO;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client.Patch.Services
{
    public class FileExtensionEnabler : IFileEnabler
    {
        private string getDisabledFilePath(string path)
        {
            return path + "_b";
        }

        public Task<bool> CanDisable(string path)
        {
            var result = File.Exists(path);
            return Task.FromResult(result);
        }

        public Task<bool> CanEnable(string path)
        {
            var result = File.Exists(getDisabledFilePath(path));
            return Task.FromResult(result);
        }

        // does not check validation
        public Task EnableFile(string path)
        {
            var disablePath = getDisabledFilePath(path);

            if (File.Exists(path))
                File.Delete(path);
            File.Move(disablePath, path);

            return Task.CompletedTask;
        }

        // does not check validation
        public Task DisableFile(string path)
        {
            var disablePath = getDisabledFilePath(path);

            if (File.Exists(disablePath))
                File.Delete(disablePath);
            File.Move(path, disablePath);

            return Task.CompletedTask;
        }
    }
}
