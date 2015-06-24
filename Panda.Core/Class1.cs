using System;

namespace Panda
{
    public interface IDataSource
    {
        string Name { get; }
        bool LoadData();
        string CreatedBy { get; }
        DateTime LastLoadDate { get; }
    }

    public interface ITableStructuredDataSource : IDataSource
    {

    }

    public interface ICustomStructureDataSource : IDataSource
    {

    }

    public class Mapping
    {
    }


}
