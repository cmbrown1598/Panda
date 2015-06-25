using System;
using System.Collections.Generic;
using NodaTime;

namespace Panda
{
    public interface IDataSource
    {
        Guid DataSourceIdentifier { get; }
        string Name { get; }
        bool LoadData();
        string CreatedBy { get; }
        ZonedDateTime? LastLoadDate { get; }
        LoadState State { get; }

        IEnumerable<LoadLogItem> GetLoadingLog();
    }
}