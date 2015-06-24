using System;
using System.Data;

namespace Panda
{
    public class SqlQueryDataSource : ITableStructuredDataSource
    {
        public string ConnectionString { get; set; }
        public string SqlCommandText { get; set; }
        public CommandType CommandType { get; set; }


        public Guid DataSourceIdentifier { get; private set; }
        public string Name { get; private set; }
        public bool LoadData()
        {
            throw new NotImplementedException();
        }

        public string CreatedBy { get; private set; }
        public DateTime LastLoadDate { get; private set; }
        public string[] Columns { get; private set; }
    }
}
