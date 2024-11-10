using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace kawtn.IO
{
    public static class ApplicationInventory
    {
        static readonly bool IsWindows;
        static readonly bool IsMacOS;
        static readonly bool IsLinux;

        public static readonly Inventory Temporary;

        static ApplicationInventory()
        {
            IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            IsMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            string tempPath = Path.Join(Path.GetTempPath(), $"kawtn.IO-{Path.GetRandomFileName()}");
            Temporary = new(tempPath);
            Temporary.Create();
        }

        public static string GetDirectory(params string[] path)
        {
            string joinpath = Path.Join(path);

            if (IsWindows)
                return Path.GetFullPath(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), joinpath));

            if (IsMacOS)
                return Path.GetFullPath(Path.Join("Library", "Application Support", joinpath));

            return Path.GetFullPath(joinpath);
        }

        public static string GetDirectoryByOS(string windows, string macos, string linux)
        {
            if (IsWindows) return GetDirectory(windows);
            if (IsMacOS) return GetDirectory(macos);
            if (IsLinux) return GetDirectory(linux);

            throw new PlatformNotSupportedException();
        }
    }
}
