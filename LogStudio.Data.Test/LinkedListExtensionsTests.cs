using LogStudio.Data;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Collections.Generic;
using System.Linq;

namespace LogStudio.Data.Test
{
    [TestFixture]
    public class LinkedListExtensionsTests
    {
        [Test]
        public void MergeLists_SourceStartsWithLowerValue()
        {
            int[] shouldBe = new[] { 1, 2, 4, 6, 7, 8, 9, 10, 12 };

            LinkedList<int> target = new LinkedList<int>(new[] { 1, 4, 7, 8 });
            int[] source = new[] { 2, 6, 9, 10, 12 };

            target.AddSorted(source);

            var res = target.ToArray();

            for (int i = 0; i < shouldBe.Length; i++)
            {
                Assert.AreEqual(shouldBe[i], res[i]);
            }
        }

        [Test]
        public void MergeLists_SourceStartsWithHigherValue()
        {
            int[] shouldBe = new[] { 1, 2, 3, 4, 6, 7, 8, 9, 10, 12 };

            LinkedList<int> target = new LinkedList<int>(new[] { 3, 4, 7, 8 });
            int[] source = new[] { 1, 2, 6, 9, 10, 12 };

            target.AddSorted(source);

            var res = target.ToArray();

            for (int i = 0; i < shouldBe.Length; i++)
            {
                Assert.AreEqual(shouldBe[i], res[i]);
            }
        }
    }
}
