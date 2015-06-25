using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using NUnit.Framework;

namespace Panda.Core.Tests
{
    [TestFixture]
    public class SqlQueryDataSourceTests
    {
        [Test]
        public void TestDataSourceLoadsColumnsHappyPath()
        {
            var dataFileName = System.IO.Path.GetFullPath(@"Data\PandaSample.mdf");
            var columns = new []{"Id", "Name", "AddressLine1", "AddressLine2", "City", "StateCode", "PostalCode", "CreateDate", "LastUpdateDate"};
            var systemUnderTest = new SqlQueryDataSource
            {
                CommandType = CommandType.Text,
                SqlCommandText = "select * from dbo.PetOwner",
                ConnectionString = string.Format(@"Data Source=(LocalDB)\v11.0;AttachDbFilename={0};Database=PandaSample;Trusted_Connection=Yes;", dataFileName)
            };



            Assert.That(systemUnderTest.State, Is.EqualTo(LoadState.NotLoaded)); 

            var result = systemUnderTest.LoadData();

            Assert.That(result, Is.True, systemUnderTest.GetLoadingLog().ToString());

            Assert.That(systemUnderTest.DataSourceIdentifier, Is.Not.EqualTo(Guid.Empty));
            Assert.That(systemUnderTest.LastLoadDate.Value.ToDateTimeUnspecified(), Is.GreaterThanOrEqualTo(DateTime.Now.Subtract(new TimeSpan(0, 0, 10, 0))));
            Assert.That(systemUnderTest.CreatedBy, Is.Not.Empty);

            Assert.That(systemUnderTest.State, Is.EqualTo(LoadState.Loaded));

            Assert.That(systemUnderTest.Columns.Length, Is.EqualTo(9));
            for(var i = 0; i < columns.Length; i++)
            {
                Assert.That(systemUnderTest.Columns[i], Is.EqualTo(columns[i]));
            }
            Assert.That(systemUnderTest.RowCount, Is.EqualTo(3));

            var loadinglog = systemUnderTest.GetLoadingLog();
            Assert.That(loadinglog, Is.Not.Empty);

        }
    }
}
