using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace Geomatics.Windows.Extensions.Tests
{
    [TestFixture]
    public class UnitTest1
    {
        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }


        [Test]
        public void TestCombine()
        {
            byte[] a = new byte [] {1, 2};

            byte[] b = new byte[] {3, 4};

            byte[] c = new byte[] { 1, 2, 3, 4};

            byte[] result = Geomatics.Windows.Extensions.System.ByteArray.Utils.Combine(a, b);

            Assert.IsTrue(result.SequenceEqual(c));
        }
    }
}
