using System;
using System.Data;
using NUnit.Framework;

namespace Panda.Core.Tests
{
    [TestFixture]
    public class ExcelFileDataSourceTests
    {
        [Test]
        public void TestDataSourceLoadsColumnsHappyPath()
        {
            var dataFileName = System.IO.Path.GetFullPath(@"Data\Beers.xlsx");
            var columns = new[] { "Beer ID", "Name" };
            var systemUnderTest = new ExcelFileDataSource
            {
                FileName = dataFileName,
                Worksheet = "Beers",
                ColumnNamesInHeader = true
            };

            Assert.That(systemUnderTest.State, Is.EqualTo(LoadState.NotLoaded));

            var result = systemUnderTest.LoadData();

            Assert.That(result, Is.True, systemUnderTest.GetLoadingLog().ToString());
            Assert.That(systemUnderTest.LastLoadDate.Value.ToDateTimeUnspecified(), Is.GreaterThanOrEqualTo(DateTime.Now.Subtract(new TimeSpan(0, 0, 10, 0))));
            Assert.That(systemUnderTest.CreatedBy, Is.Not.Empty);

            Assert.That(systemUnderTest.State, Is.EqualTo(LoadState.Loaded));

            Assert.That(systemUnderTest.Columns.Length, Is.EqualTo(9));
            for (var i = 0; i < columns.Length; i++)
            {
                Assert.That(systemUnderTest.Columns[i], Is.EqualTo(columns[i]));
            }
            Assert.That(systemUnderTest.RowCount, Is.EqualTo(3));

            var loadinglog = systemUnderTest.GetLoadingLog();
            Assert.That(loadinglog, Is.Not.Empty);

            systemUnderTest.ApproximateSizeInBytes();

            Console.Write(loadinglog);

        }
    }
}