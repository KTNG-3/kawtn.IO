namespace kawtn.IO.Konfig
{
    public static class KonfigSerializer
    {
        public static string Serialize<T>(T data)
        {
            Token[] tokens = Parser.Unparse(data);

            string content = Lexer.Untokenization(tokens);

            return content;
        }

        public static T Deserialize<T>(string content)
        {
            Token[] tokens = new Lexer().Tokenization(content);

            T data = Parser.Parse<T>(tokens);

            return data;
        }
    }
}
