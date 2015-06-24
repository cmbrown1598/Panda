using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Panda.Core.Tests
{
    [TestFixture]
    public class SqlQueryDataSourceTests
    {
        [Test]
        public void TestDataSourceHappyPath()
        {
            var columns = new []{"Id", "Name", "AddressLine1", "AddressLine2", "City", "StateCode", "PostalCode", "CreateDate", "LastUpdateDate"};
            var systemUnderTest = new SqlQueryDataSource
            {
                CommandType = CommandType.Text,
                SqlCommandText = "select * from dbo.PetOwners",
                ConnectionString = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=Data\PandaSample.mdf;Integrated Security=True"
            };


            systemUnderTest.LoadData();

            Assert.That(systemUnderTest.CreatedBy, Is.Empty);
            Assert.That(systemUnderTest.Columns.Length, Is.EqualTo(9));
            for(var i = 0; i < columns.Length; i++)
            {
                Assert.That(systemUnderTest.Columns[i], Is.EqualTo(columns[i]));
            }


        }
    }
}
