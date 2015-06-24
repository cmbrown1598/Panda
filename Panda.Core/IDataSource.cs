using System;

namespace Panda
{
    public interface IDataSource
    {
        Guid DataSourceIdentifier { get; }
        string Name { get; }
        bool LoadData();
        string CreatedBy { get; }
        DateTime LastLoadDate { get; }
    }
}