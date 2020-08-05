using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LogStudio.Reader.Parser
{
    public class ExpressionParser
    {
        public static ParseResult ParseExpression(string data)
        {
            int index = data.LastIndexOf(']');

            if (index == -1)
                return null;

            string expressionString = data.Substring(1, index - 1);

            data = data.Remove(0, expressionString.Length + 2 + 1);

            var indexUser = data.LastIndexOf(' ');
            var module = data.Substring(indexUser + 2, data.Length - (indexUser + 3));
            module = module.Trim('(', ')');

            data = data.Remove(indexUser);

            var indexMethod = data.LastIndexOf('.');
            var methodName = data.Substring(indexMethod + 1);
            data = data.Remove(indexMethod);

            var indexBlock = data.LastIndexOf('.');
            var blockName = data.Substring(indexBlock + 1);
            return new ParseResult(expressionString, module, methodName, blockName);
        }

        public class ParseResult
        {
            public string Expression { get; }
            public string Module { get; }

            public ParseResult(string expression, string module, string methodName, string blockName)
            {
                Expression = expression;
                Module = module;
                MethodName = methodName;
                BlockName = blockName;
            }

            public string MethodName { get; private set; }
            public string BlockName { get; private set; }
        }

        public static IEnumerable<string> FindItems(ParseResult result, Predicate<string> checkItemExist)
        {
            string validTokens = @"[^a-zA-Z0-9_\.\[\]]+";


            int indexStartBracket = result.Expression.IndexOf('(');

            int indexLastBracket = result.Expression.LastIndexOf(')');
            var expressionString = result.Expression.Substring(indexStartBracket + 1, indexLastBracket - 1 - indexStartBracket);

            var cleaned = Regex.Replace(expressionString, validTokens, " ");


            var parts = cleaned.Split(' ');

            foreach (var part in parts.Where(p => !string.IsNullOrEmpty(p)))
            {
                var item = part;
                if (checkItemExist(item))
                    yield return item;
                else
                {
                    if (!item.StartsWith(result.BlockName + "."))
                        item = $"{result.BlockName}.{item}";

                    if (!item.StartsWith("Process."))
                        item = $"Process.{item}";

                    if (checkItemExist(item))
                        yield return item;
                }
            }
        }
    }
}
