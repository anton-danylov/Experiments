using System;
using StringComparer;
using NUnit.Framework;
using System.Collections.Generic;

namespace LargeFileSorter.Tests
{
    [TestFixture]
    public class ChunkSorterTests
    {
        [TestCase(@"d:\large_file.txt", 0, "large_file.0")]
        [TestCase(@"c:\temp\file__.txt", 5, "file__.5")]
        public void TestGetTempFileName(string filePath, int chunk, string expected)
        {

            // Act
            var actual = Utilities.GetTempFileName(filePath, chunk);

            // Assert
            Assert.AreEqual(actual, expected);
        }

        [TestCase(@"d:\large_file.txt", 0, @"d:\large_file.0")]
        [TestCase(@"c:\temp\file__.txt", 5, @"c:\temp\file__.5")]
        public void TestGetTempFilePath(string filePath, int chunk, string expected)
        {
            // Act
            var actual = Utilities.GetTempFilePath(filePath, chunk);

            // Assert
            Assert.AreEqual(actual, expected);
        }
    }
}
