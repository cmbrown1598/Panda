using System.Data;

namespace Panda.DataSources
{
    public interface ITableStructuredDataSource : IDataSource
    {
        bool SettingsAreValid();
        string[] Columns { get; }
        int? RowCount { get; }

        DataTable Data { get; }
    }
}