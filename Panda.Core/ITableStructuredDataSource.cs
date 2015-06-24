namespace Panda
{
    public interface ITableStructuredDataSource : IDataSource
    {
        string[] Columns { get; }
    }
}