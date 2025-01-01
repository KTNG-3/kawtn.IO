using kawtn.IO.Serializable;

namespace kawtn.IO.Konfig
{
    public class KonfigSerializer<T> : Serializer<T>
    {
        public override string Serialize(T data)
        {
            Token[] tokens = Parser.Unparse(data);

            string content = Lexer.Untokenization(tokens);

            return content;
        }

        public override T? Deserialize(string content)
        {
            Token[] tokens = new Lexer().Tokenization(content);

            T data = Parser.Parse<T>(tokens);

            return data;
        }
    }
}
