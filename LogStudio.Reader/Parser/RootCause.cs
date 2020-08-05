using LogStudio.Data;
using System.Collections.Generic;
using System.Linq;

namespace LogStudio.Reader.Parser
{
    public static class ChangeFactory
    {
        public static Change CreateFrom(IItemDatabase database, LogRowKey key, int maxDepth)
        {
            Change result = ParseChange(database, key, 0, maxDepth);
            return result;
        }

        private static Change ParseChange(IItemDatabase database, LogRowKey key, int depth, int maxDepth)
        {
            if (depth == maxDepth)
                return null;

            (IEnumerable<LogRowKey>, ExpressionParser.ParseResult) triggers = ParseExpression(database, key);

            return new Change(key, triggers.Item2, triggers.Item1.Where(p => p != null).Select(p => ParseChange(database, p, depth + 1, maxDepth)));
        }

        private static (IEnumerable<LogRowKey>, ExpressionParser.ParseResult) ParseExpression(IItemDatabase database, LogRowKey source)
        {
            ExpressionParser.ParseResult parseResult = ExpressionParser.ParseExpression(source.Data.User);
            if (parseResult != null)
            {
                IEnumerable<string> triggerItems = ExpressionParser.FindItems(parseResult, database.Exists).Distinct();

                return (database.FindClosestTriggerKeys(source.UniqueId, source.Data.CycleIndex, triggerItems), parseResult);
            }

            return (new LogRowKey[0], parseResult);
        }
    }

    public class Change
    {
        public LogRowKey Key { get; }
        public IEnumerable<Change> States { get; }

        public ExpressionParser.ParseResult Result { get; }

        public Change(LogRowKey key, ExpressionParser.ParseResult result, IEnumerable<Change> states)
        {
            Key = key;
            Result = result;
            States = states;
        }

        public IEnumerable<(Change, bool)> GetStatesAndCause()
        {
            return States.Select(p => (p, p.Key.Data.CycleIndex == Key.Data.CycleIndex - 1 || !p.Key.Data.HasExpression() && p.Key.Data.CycleIndex == Key.Data.CycleIndex));
        }
    }
}
