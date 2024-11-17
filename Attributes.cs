using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kawtn.IO
{
    public class Attributes
    {
        readonly Location Location;

        public Attributes(Location location)
        {
            this.Location = location;
        }

        public FileAttributes Get()
        {
            return File.GetAttributes(this.Location.Data);
        }

        public void Set(FileAttributes attributes)
        {
            File.SetAttributes(this.Location.Data, attributes);
        }

        public void Add(params FileAttributes[] attributes)
        {
            FileAttributes read = Get();

            foreach (FileAttributes write in attributes)
            {
                read |= write;
            }

            Set(read);
        }

        public void Remove(params FileAttributes[] attributes)
        {
            FileAttributes read = Get();

            foreach (FileAttributes write in attributes)
            {
                read &= ~write;
            }

            Set(read);
        }

        public bool Has(FileAttributes attributes)
        {
            FileAttributes read = Get();

            return read.HasFlag(attributes);
        }
    }
}
