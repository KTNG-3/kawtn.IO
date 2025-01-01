using System.IO;

namespace kawtn.IO
{
    public class StringItem : Item
    {
        public StringItem(string location)
            : base(location) { }

        public StringItem(Location location)
            : this(location.Data) { }

        public void Write(string data)
        {
            this.Create();

            File.WriteAllText(this.Location.Data, data);
        }

        public void WriteString(string data)
        {
            this.Write(data);
        }

        public new string Read()
        {
            if (File.Exists(this.Location.Data))
            {
                return File.ReadAllText(this.Location.Data);
            }

            return string.Empty;
        }

        public string ReadString()
        {
            return this.Read();
        }
    }
}
