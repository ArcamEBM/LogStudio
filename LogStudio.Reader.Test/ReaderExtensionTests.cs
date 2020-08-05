using LogStudio.Data;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LogStudio.Reader.Test
{
    [TestFixture]
    public class ReaderExtensionTests
    {
        [Test]
        [Ignore("Need fix")]
        public void FindClosestRowKeyTest()
        {
            var itemDatabase = Substitute.For<IItemDatabase>();

            var triggerIds = new[]
            {
                new KeyValuePair<string, long[]>("Key1", new long[] {1, 3, 6}),
                new KeyValuePair<string, long[]>("Key2", new long[] {2, 4, 7})
            };

            itemDatabase.GetLogRowUniqueID(triggerIds[0].Key).Returns(triggerIds[0].Value);
            itemDatabase.GetLogRowUniqueID(triggerIds[1].Key).Returns(triggerIds[1].Value);
            itemDatabase.GetItemRow(triggerIds[0].Key, 1).Returns(callInfo =>
                new LogRowData(DateTime.UtcNow, triggerIds[0].Key, "", callInfo.ArgAt<long>(0) * 10, String.Empty));
            itemDatabase.GetItemRow(triggerIds[0].Key, 3).Returns(callInfo =>
                new LogRowData(DateTime.UtcNow, triggerIds[0].Key, "", callInfo.ArgAt<long>(0) * 10, String.Empty));
            itemDatabase.GetItemRow(triggerIds[0].Key, 6).Returns(callInfo =>
                new LogRowData(DateTime.UtcNow, triggerIds[0].Key, "", callInfo.ArgAt<long>(0) * 10, String.Empty));

            LogRowKey key = itemDatabase.FindClosestRowKey(5, 50, triggerIds[0].Key, triggerIds[0].Value);

            key = itemDatabase.FindClosestRowKey(5, 60, triggerIds[0].Key, triggerIds[0].Value);

            Assert.AreEqual(3, key.UniqueId);
        }

        [Test]
        [Ignore("Need fix")]
        public void FindClosestRowKey_Exception()
        {
            var itemDatabase = Substitute.For<IItemDatabase>();

            var triggerIds = new[]
            {
                new KeyValuePair<string, long[]>("Key1", new long[] {1, 3, 6}),
                new KeyValuePair<string, long[]>("Key2", new long[] {2})
            };

            itemDatabase.GetLogRowUniqueID(triggerIds[0].Key).Returns(triggerIds[0].Value);
            itemDatabase.GetLogRowUniqueID(triggerIds[1].Key).Returns(triggerIds[1].Value);
            itemDatabase.GetItemRow(triggerIds[0].Key, 1).Returns(callInfo =>
                new LogRowData(DateTime.UtcNow, triggerIds[0].Key, "", callInfo.ArgAt<long>(0) * 10, String.Empty));
            itemDatabase.GetItemRow(triggerIds[0].Key, 3).Returns(callInfo =>
                new LogRowData(DateTime.UtcNow, triggerIds[0].Key, "", callInfo.ArgAt<long>(0) * 10, String.Empty));
            itemDatabase.GetItemRow(triggerIds[0].Key, 6).Returns(callInfo =>
                new LogRowData(DateTime.UtcNow, triggerIds[0].Key, "", callInfo.ArgAt<long>(0) * 10, String.Empty));

            LogRowKey key = itemDatabase.FindClosestRowKey(4, 20, triggerIds[0].Key, triggerIds[0].Value);

            Assert.AreEqual(3, key.UniqueId);
        }
    }
}