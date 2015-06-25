using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Panda
{
    public interface ITableStructuredDataSource : IDataSource
    {
        string[] Columns { get; }
        int? RowCount { get; }

        DataTable Data { get; }
    }
}