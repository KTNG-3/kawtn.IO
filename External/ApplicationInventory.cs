using System;
using System.IO;
using System.Runtime.InteropServices;

namespace kawtn.IO.External
{
    public static class ApplicationInventory
    {
        static readonly bool IsLinux;
        static readonly bool IsWindows;
        static readonly bool IsMacOS;

        public static readonly Inventory Temporary;

        static ApplicationInventory()
        {
            IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            IsMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

            string tempLocation = Location.Join(Path.GetTempPath(), $"kawtn.IO-{Path.GetRandomFileName()}");
            Temporary = new Inventory(tempLocation);
        }

        static Inventory GetLinux(string location)
        {
            return new Inventory(location);
        }

        static Inventory GetWindows(string location)
        {
            string invLocation = Location.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), location);

            return new Inventory(invLocation);
        }

        static Inventory GetMacOS(string location)
        {
            string invLocation = Location.Join("Library", "Application Support", location);

            return new Inventory(invLocation);
        }

        public static Inventory? Get(string location)
        {
            if (IsLinux)
                return ApplicationInventory.GetLinux(location);

            if (IsWindows)
                return ApplicationInventory.GetWindows(location);

            if (IsMacOS)
                return ApplicationInventory.GetMacOS(location);

            return null;
        }

        public static Inventory? GetByOS(string linux, string windows, string macos)
        {
            if (IsLinux)
                return ApplicationInventory.GetLinux(linux);

            if (IsWindows)
                return ApplicationInventory.GetWindows(windows);

            if (IsMacOS)
                return ApplicationInventory.GetMacOS(macos);

            return null;
        }
    }
}
