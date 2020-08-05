using LogStudio.Data;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace LogStudio.DataTest
{
    [TestFixture]
    public class PagedStreamReaderTests
    {
        [Test]
        public void FetchPageWithHalfSize_Test()
        {
            using (var stream = new MemoryStream(new byte[4096 + 1024]))
            {
                var reader = new PagedStreamReader(stream, 4096);

                var page = reader.FetchPage(0);

                Assert.AreEqual(0, page.PageIndex);
                Assert.AreEqual(4096, page.Data.Length);

                page = reader.FetchPage(1);

                Assert.AreEqual(1, page.PageIndex);
                Assert.AreEqual(1024, page.Data.Length);
            }
        }

        [Test]
        public void ReadPageAndCompareContentWithSource_PageSize_4096_Test()
        {
            var source = Enumerable.Range(0, 500).Select(p => (byte)(p % 256)).ToArray();
            using (var stream = new MemoryStream(source))
            {
                var reader = new PagedStreamReader(stream, 4096);

                var buffer = new byte[137];
                var read = reader.ReadPage(buffer, 0, 0, buffer.Length);

                Assert.AreEqual(buffer.Length, read);


                read = reader.ReadPage(buffer, 0, 480, buffer.Length);

                Assert.AreEqual(20, read);

                Assert.That(buffer.Compare(0, source, 480, read));
            }
        }

        [Test]
        public void ReadPageAndCompareContentWithSource_PageSize_300_Test()
        {
            var source = Enumerable.Range(0, 500).Select(p => (byte)(p % 256)).ToArray();
            using (var stream = new MemoryStream(source))
            {
                var reader = new PagedStreamReader(stream, 300);

                var buffer = new byte[137];
                var read = reader.Read(buffer, 0, buffer.Length);

                Assert.AreEqual(buffer.Length, read);
                Assert.That(buffer.Compare(0, source, 0, buffer.Length));

                read = reader.Read(buffer, 480, buffer.Length);

                Assert.AreEqual(20, read);

                Assert.That(buffer.Compare(0, source, 480, read));
            }
        }


        [Test]
        public void IncreasingStreamLength_PageSize_300_Test()
        {
            var source = Enumerable.Range(0, 500).Select(p => (byte)(p % 256)).ToArray();
            using (var stream = new MemoryStream())
            {
                stream.Write(source, 0, source.Length);

                var reader = new PagedStreamReader(stream, 300);

                byte[] buffer = new byte[500];

                int read = reader.Read(buffer, 0, buffer.Length);

                var source2 = Enumerable.Range(0, 37).Select(p => (byte)(p % 256)).ToArray();
                stream.Write(source2, 0, source2.Length);

                read = reader.Read(buffer, read, buffer.Length);
                Assert.AreEqual(37, read);
                Assert.That(buffer.Compare(0, source2, 0, read));
            }
        }
    }
}
