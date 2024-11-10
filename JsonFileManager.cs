using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kawtn.IO
{
    public class JsonFileManager<T> : FileManager
    {
        public JsonFileManager(string path) : base(path)
        {
            
        }

        public void Write(T data)
        {
            FileHelper.UpdateJSON(this.path, data);
        }

        public new T Read()
        {
            T? data = FileHelper.ReadJSON<T>(this.path);
            if (data == null) throw new NullReferenceException(this.path);

            return data;
        }

        public void Edit(Func<T, T> editor)
        {
            T read = Read();
            T data = editor.Invoke(read);

            Write(data);
        }
    }
}
