using System.Collections.Generic;
using System.Text;

namespace kawtn.IO.Konfig
{
    enum TokenType
    {
        String,

        Table,
        List,

        Space,
        NewLine, // \n
        Backslash, // \
        QuotationMark, // "
        Equal, // =
        Comment, // #
        LeftAngleBracket, // <
        RightAngleBracket, // >
        LeftSquareBracket, // [
        RightSquareBracket, // ]

        End,
        EndOfFile
    }

    class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }

        public Token(TokenType type, string? value = null)
        {
            this.Type = type;
            this.Value = value ?? string.Empty;
        }
    }

    class Lexer
    {
        int index = -1;
        readonly List<Token> list = new();

        void Read(char[] content)
        {
            list.Clear();

            Dictionary<char, TokenType> specific = new()
            {
                { '\n', TokenType.NewLine },
                { '\\', TokenType.Backslash },
                { '"', TokenType.QuotationMark },
                { '=', TokenType.Equal },
                { '#', TokenType.Comment },
                { '<', TokenType.LeftAngleBracket },
                { '>', TokenType.RightAngleBracket },
                { '[', TokenType.LeftSquareBracket },
                { ']', TokenType.RightSquareBracket }
            };

            StringBuilder strBuilder = new();

            foreach (char c in content)
            {
                if (specific.TryGetValue(c, out TokenType type))
                {
                    string value = strBuilder.ToString().Trim();

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        list.Add(new Token(TokenType.String, value));
                        strBuilder.Clear();
                    }

                    list.Add(new Token(type, c.ToString()));
                }
                else
                {
                    strBuilder.Append(c);
                }
            }

            list.Add(new Token(TokenType.End));
            list.Add(new Token(TokenType.EndOfFile));
        }

        bool EOF(Token token)
        {
            return token.Type == TokenType.EndOfFile;
        }

        bool Comment(Token token)
        {
            bool isComment =
                token.Type == TokenType.Comment;

            if (!isComment)
            {
                return false;
            }

            while (token.Type != TokenType.NewLine)
            {
                list.RemoveAt(index);

                token = list[index];
            }

            return true;
        }

        bool Table(Token token)
        {
            bool isTable =
                token.Type == TokenType.LeftAngleBracket &&
                list[index + 2].Type == TokenType.RightAngleBracket;

            if (!isTable)
            {
                return false;
            }

            list.Insert(index, new Token(TokenType.End)); index++;

            string name = list[index + 1].Value;

            list.RemoveRange(index, 3);
            list.Insert(index, new Token(TokenType.Table, name));

            return true;
        }

        bool List(Token token)
        {
            bool isList =
                token.Type == TokenType.LeftSquareBracket &&
                list[index + 2].Type == TokenType.RightSquareBracket;

            if (!isList)
            {
                return false;
            }

            list.Insert(index, new Token(TokenType.End));
            index++;

            string name = list[index + 1].Value;

            list.RemoveRange(index, 3);
            list.Insert(index, new Token(TokenType.List, name));

            return true;
        }

        bool quote = false;
        int quoteIndex = -1;
        readonly StringBuilder quoteBuilder = new();

        void Quote(Token token)
        {
            bool isQuoteMark =
                token.Type == TokenType.QuotationMark &&
                list[index - 1].Type != TokenType.Backslash;

            if (!isQuoteMark)
            {
                quoteBuilder.Append(token.Value);

                return;
            }

            quote = !quote;

            if (quote)
            {
                quoteIndex = index;
                quoteBuilder.Clear();

                return;
            }

            list.RemoveRange(quoteIndex, index - quoteIndex + 1);
            list.Insert(quoteIndex, new Token(TokenType.String, quoteBuilder.ToString()));

            index = quoteIndex + 1;
        }

        Token NextToken()
        {
            index++;
            return list[index];
        }

        void Token()
        {
            while (true)
            {
                Token token = this.NextToken();

                if (this.EOF(token)) break;

                if (this.Comment(token)) continue;
                if (this.Table(token)) continue;
                if (this.List(token)) continue;

                this.Quote(token);
            }
        }

        public Token[] Tokenization(string content)
        {
            content += "\n";
            this.Read(content.ToCharArray());

            this.Token();

            return list.ToArray();
        }

        public static string Untokenization(Token[] tokens)
        {
            StringBuilder builder = new();

            foreach (Token token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.String:
                        {
                            builder.Append(token.Value);
                            break;
                        }
                    case TokenType.Equal:
                        {
                            builder.Append(" = ");
                            break;
                        }
                    case TokenType.Table:
                        {
                            builder.Append($"<{token.Value}>");
                            break;
                        }
                    case TokenType.List:
                        {
                            builder.Append($"[{token.Value}]");
                            break;
                        }
                    case TokenType.NewLine:
                        {
                            builder.Append("\n");
                            break;
                        }
                }
            }

            return builder.ToString();
        }
    }
}
