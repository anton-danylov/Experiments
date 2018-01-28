using System;
using StringComparer;
using NUnit.Framework;
using System.Collections.Generic;

namespace LargeFileSorter.Tests
{
    [TestFixture]
    public class StringComparerTests
    {
        IComparer<string> _comparer = null;

        [OneTimeSetUp]
        public void SetUp()
        {
            _comparer = new UnsafeStringComparer2();

        }

        [TestCase("415.Apple", "1.Apple", 1)]
        [TestCase("1.Apple", "415.Apple", -1)]
        [TestCase("415.Apple", "415.Apple", 0)]
        [TestCase("30432.Something something something", "32.Cherry is the best", 1)]
        [TestCase("2.Banana is yellow", "32.Cherry is the best", -1)]
        [TestCase("22222.ZZ", "32.Cherry is the best", 1)]
        [TestCase("22222.A", "45.Cherry is the best", -1)]
        [TestCase("9.Apple", "-9.Apple", 1)]
        [TestCase("-1005009.Zapple", "100500.apple", -1)]
        public void TestNormalCase(string first, string second, int expected)
        {
            // Act
            int actual = _comparer.Compare(first, second);
            actual = (actual < 0) ? -1 : (actual > 0) ? +1 : 0;

            // Assert
            Assert.AreEqual(actual, expected);
        }
    }
}
