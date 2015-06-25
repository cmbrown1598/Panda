using System.Data;

namespace Panda
{
    public interface ITableStructuredDataSource : IDataSource
    {
        string[] Columns { get; }
        int? RowCount { get; }

        DataTable Data { get; }
    }
}