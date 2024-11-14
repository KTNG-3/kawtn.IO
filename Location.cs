﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kawtn.IO
{
    // Path
    public class Location
    {
        public string Data { get; private set; }

        public Location(params string[] str)
        {
            this.Data = Path.GetFullPath(Path.Join(str));
        }

        public Location(Location baseLocation, params string[] str)
            : this(baseLocation.Data, Path.Join(str)) { }

        public Location(Item baseLocation, params string[] str)
            : this(baseLocation.Location.Data, Path.Join(str)) { }

        public Location(Inventory baseLocation, params string[] str)
            : this(baseLocation.Location.Data, Path.Join(str)) { }

        public bool IsExists()
        {
            return Path.Exists(this.Data);
        }

        public bool IsItem()
        {
            if (!IsExists()) return false;

            return !IsDirectory();
        }

        public bool IsDirectory()
        {
            if (!IsExists()) return false;

            return File.GetAttributes(this.Data).HasFlag(FileAttributes.Directory);
        }

        public string? GetName()
        {
            if (!IsExists()) return null;

            if (IsDirectory())
            {
                return Path.GetDirectoryName(this.Data);
            }

            return Path.GetFileName(this.Data);
        }

        string? GetParentPath()
        {
            if (!IsExists()) return null;

            if (IsItem())
            {
                return Path.GetDirectoryName(this.Data);
            }

            DirectoryInfo? parent = Directory.GetParent(this.Data);

            if (parent == null)
            {
                return null;
            }
            else
            {
                return parent.FullName;
            }
        }

        public Location? GetParent()
        {
            string? path = GetParentPath();

            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            else
            {
                return new(path);
            }
        }

        public string GetRoot()
        {
            return Path.GetPathRoot(this.Data) ?? string.Empty;
        }

        public Item ParseItem()
        {
            return new(this);
        }

        public Inventory ParseInventory()
        {
            return new(this);
        }
    }
}
