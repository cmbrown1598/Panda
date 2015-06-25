using System;
using NodaTime;

namespace Panda
{
    public interface IDataSource
    {
        Guid DataSourceIdentifier { get; }
        string Name { get; }

        bool LoadPreview();
        bool LoadData();

        string CreatedBy { get; }
        ZonedDateTime? LastLoadDate { get; }
        LoadState State { get; }
        ILoadLog GetLoadingLog();

        long ApproximateSizeInBytes();
    }
}