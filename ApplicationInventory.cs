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
        }

        public static Inventory GetInventory(params string[] path)
        {
            string joinPath = Path.Join(path);

            if (IsWindows)
                joinPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), joinPath);

            if (IsMacOS)
                joinPath = Path.Join("Library", "Application Support", joinPath);

            return new(joinPath);
        }

        public static Inventory GetInventoryByOS(string windows, string macos, string linux)
        {
            if (IsWindows) return GetInventory(windows);
            if (IsMacOS) return GetInventory(macos);
            if (IsLinux) return GetInventory(linux);

            throw new PlatformNotSupportedException();
        }
    }
}
