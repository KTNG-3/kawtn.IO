using System;
using System.IO;
using System.Runtime.InteropServices;

namespace kawtn.IO.External
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

            string tempPath = Location.Join(Path.GetTempPath(), $"kawtn.IO-{Path.GetRandomFileName()}");
            Temporary = new Inventory(tempPath);
        }

        public static Inventory Get(params string[] path)
        {
            string joinPath = Location.Join(path);

            if (IsWindows)
                joinPath = Location.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), joinPath);

            if (IsMacOS)
                joinPath = Location.Join("Library", "Application Support", joinPath);

            return new Inventory(joinPath);
        }

        public static Inventory GetByOS(string windows, string macos, string linux)
        {
            if (IsWindows) return Get(windows);
            if (IsMacOS) return Get(macos);
            if (IsLinux) return Get(linux);

            throw new PlatformNotSupportedException();
        }
    }
}
