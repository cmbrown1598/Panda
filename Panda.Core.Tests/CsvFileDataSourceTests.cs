using System;
using NUnit.Framework;
using Panda.DataSources;

namespace Panda.Core.Tests
{
    [TestFixture]
    public class CsvFileDataSourceTests
    {
        [Test]
        public void TestDataSourceLoadsColumnsHappyPath()
        {
            var dataFileName = System.IO.Path.GetFullPath(@"Data\Beers.csv");
            var columns = new[] { "Beer ID", "Name" };
            var systemUnderTest = new CsvFileDataSource
            {
                FileName = dataFileName,
                FirstRowAsColumnNames = true
            };

            Assert.That(systemUnderTest.State, Is.EqualTo(LoadState.NotLoaded));

            var result = systemUnderTest.LoadData();

            Assert.That(result, Is.True, systemUnderTest.GetLoadingLog().ToString());
            Assert.That(systemUnderTest.LastLoadDate.Value.ToDateTimeUnspecified(), Is.GreaterThanOrEqualTo(DateTime.Now.Subtract(new TimeSpan(0, 0, 10, 0))));
            Assert.That(systemUnderTest.CreatedBy, Is.Not.Empty);

            Assert.That(systemUnderTest.State, Is.EqualTo(LoadState.Loaded));

            Assert.That(systemUnderTest.Columns.Length, Is.EqualTo(2));
            for (var i = 0; i < columns.Length; i++)
            {
                Assert.That(systemUnderTest.Columns[i], Is.EqualTo(columns[i]));
            }
            Assert.That(systemUnderTest.RowCount, Is.EqualTo(4));

            var loadinglog = systemUnderTest.GetLoadingLog();
            Assert.That(loadinglog, Is.Not.Empty);

            Console.Write("Approximate size: {0}", systemUnderTest.ApproximateSizeInBytes());

            Console.Write(loadinglog);
        }

        [Test]
        public void TestDataSourceLoadsNoColumnHeaderColumnsHappyPath()
        {
            var dataFileName = System.IO.Path.GetFullPath(@"Data\Beers_NoHeader.csv");
            var columns = new[] { "Column 1", "Column 2" };
            var systemUnderTest = new CsvFileDataSource
            {
                FileName = dataFileName,
                FirstRowAsColumnNames = false
            };

            Assert.That(systemUnderTest.State, Is.EqualTo(LoadState.NotLoaded));

            var result = systemUnderTest.LoadData();

            Assert.That(result, Is.True, systemUnderTest.GetLoadingLog().ToString());
            Assert.That(systemUnderTest.LastLoadDate.Value.ToDateTimeUnspecified(), Is.GreaterThanOrEqualTo(DateTime.Now.Subtract(new TimeSpan(0, 0, 10, 0))));
            Assert.That(systemUnderTest.CreatedBy, Is.Not.Empty);

            Assert.That(systemUnderTest.State, Is.EqualTo(LoadState.Loaded));

            Assert.That(systemUnderTest.Columns.Length, Is.EqualTo(2));
            for (var i = 0; i < columns.Length; i++)
            {
                Assert.That(systemUnderTest.Columns[i], Is.EqualTo(columns[i]));
            }
            Assert.That(systemUnderTest.RowCount, Is.EqualTo(4));

            var loadinglog = systemUnderTest.GetLoadingLog();
            Assert.That(loadinglog, Is.Not.Empty);

            Console.Write("Approximate size: {0}", systemUnderTest.ApproximateSizeInBytes());

            Console.Write(loadinglog);
        }
    }
}