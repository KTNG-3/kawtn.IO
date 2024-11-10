using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kawtn.IO
{
    public class FileManager
    {
        protected readonly string path;

        public FileManager(string path)
        {
            this.path = Path.GetFullPath(path);

            FileHelper.Create(this.path);
        }

        public bool Exists()
        {
            return File.Exists(this.path);
        }

        public void Write(byte[] data)
        {
            FileHelper.UpdateBytes(this.path, data);
        }

        public byte[] Read()
        {
            return FileHelper.ReadBytes(this.path);
        }

        public void Clone(string destinationPath)
        {
            FileHelper.Clone(this.path, destinationPath);
        }

        public void Delete()
        {
            FileHelper.Delete(this.path);
        }

        public void Move(string destinationPath)
        {
            Clone(destinationPath);
            Delete();
        }
    }
}
