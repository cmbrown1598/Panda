using System;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using NodaTime;
using Panda.Logging;

namespace Panda.DataSources
{
    public class TableStructuredDataSource : ITableStructuredDataSource
    {
        private readonly LoadLog _log = new LoadLog();
        private DataTable _data;

        protected ILoadLog Log {
            get { return _log; }
        }

        public TableStructuredDataSource()
        {
            var windowsIdentity = WindowsIdentity.GetCurrent();
            if (windowsIdentity != null) CreatedBy = windowsIdentity.Name;

            State = LoadState.NotLoaded;
        }



        public Guid DataSourceIdentifier { get; set; }
        public virtual string Name { get; set; }


        protected void LoadFromDataAdapter(IDataAdapter dataAdapter)
        {
            var dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            Data = dataSet.Tables[0];

            Log.Info("Data loaded.");

            for (var i = 0; i < Data.Columns.Count; i++)
            {
                var columnName = Data.Columns[i].ColumnName;
                var dataType = Data.Columns[i].DataType;
                Log.Info("Loaded column '{0}', data type '{1}'.", columnName, dataType != null ? dataType.Name : "unknown");

            }

            Log.Info("Data load complete. {0} rows loaded.", Data.Rows.Count);

        }


        public virtual bool SettingsAreValid()
        {
            Log.Error("Base implementation does not contain settings, and should be overriden. Bad programmer, no biscuit.");
            return false;
        }

        protected virtual bool Load()
        {
            return true;
        }

        protected virtual bool Preview()
        {
            return true;
        }

        private bool DoDataLoad(Action loadAction)
        {
            try
            {
                if (!SettingsAreValid())
                {
                    _log.Error("Cannot load. Settings are not valid.");
                    return false;
                }
                
                loadAction();
                LastLoadDate = TimeHelpers.NowInLocalTime();
                return true;
            }
            catch (Exception exception)
            {
                _log.Error("Exception occured!! Stack trace of error follows:");
                _log.Error(exception);
                State = LoadState.Errored;
                return false;
            }
        }


        public bool LoadPreview()
        {
            return DoDataLoad(() =>
            {
                Preview();
                State = LoadState.PreviewLoaded;
                _log.Info("Preview load completed");
            });
        }

        public bool LoadData()
        {
            return DoDataLoad(() =>
            {
                Load();
                State = LoadState.Loaded;
                _log.Info("Load completed");
            });
        }

        public string CreatedBy { get; private set; }
        public ZonedDateTime? LastLoadDate { get; private set; }
        public LoadState State { get; private set; }
        public ILoadLog GetLoadingLog()
        {
            return _log;
        }

        public long ApproximateSizeInBytes()
        {

            using (var stream = new MemoryStream())
            {
                var serializer = new BinaryFormatter();
                serializer.Serialize(stream, Data);
                return stream.Length;
            }
        }

        public string[] Columns {
            get
            {
                switch (State)
                {
                    case LoadState.Loaded:
                    case LoadState.PreviewLoaded:
                        var l = new string[Data.Columns.Count];
                        for(var c = 0; c < Data.Columns.Count; c++)
                        {
                            l[c] = Data.Columns[c].ColumnName;
                        }
                        return l;
                    default:
                        return null;
                }

            }
        }
        public int? RowCount
        {
            get
            {
                switch (State)
                {
                    case LoadState.Loaded:
                    case LoadState.PreviewLoaded:
                        return Data.Rows.Count;
                    default:
                        return null;
                }

            }
        }
        public DataTable Data {
            get { return _data; }
            protected set
            {
                var prop = value ?? new DataTable(Name ?? string.Empty);
                _data = prop;
            } 
        }
    }
}