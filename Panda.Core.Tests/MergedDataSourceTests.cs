using System;
using System.Linq;
using NUnit.Framework;
using Panda.DataSources;

namespace Panda.Core.Tests
{
    [TestFixture]
    public class MergedDataSourceTests
    {
        [Test]
        public void TestDataSourceLoadsColumnsHappyPath()
        {
            var dataFileName = System.IO.Path.GetFullPath(@"Data\Beers.csv");
            var secondDataFile = System.IO.Path.GetFullPath(@"Data\BeersDrinkingHistory.csv");

            var leftFile = new CsvFileDataSource
            {
                FileName = dataFileName,
                FirstRowAsColumnNames = true
            };
            var rightFile = new CsvFileDataSource
            {
                FileName = secondDataFile,
                FirstRowAsColumnNames = true
            };

            
            var systemUnderTest = new MergedDataSource
            {
                Left = leftFile,
                Right = rightFile,
                JoinOn = "Beer ID"
            };

            systemUnderTest.LoadData();

            Assert.That(systemUnderTest.RowCount, Is.EqualTo(4));
            Assert.That(systemUnderTest.Data.Select("[Beer ID] = 1").Count(), Is.EqualTo(0));
            Assert.That(systemUnderTest.Data.Select("[Beer ID] = 3").Count(), Is.EqualTo(2));
        }
    }

}