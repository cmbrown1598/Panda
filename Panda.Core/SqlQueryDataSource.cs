using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Security.Principal;
using NodaTime;

namespace Panda
{
    public class SqlQueryDataSource : ITableStructuredDataSource
    {
        private string _name;
        private LoadLog _log = new LoadLog();

        public SqlQueryDataSource()
        {
            DataSourceIdentifier = Guid.NewGuid();
            
            var windowsIdentity = WindowsIdentity.GetCurrent();
            if (windowsIdentity != null) CreatedBy = windowsIdentity.Name;

            State = LoadState.NotLoaded;
        }

        public string ConnectionString { get; set; }
        public string SqlCommandText { get; set; }
        public CommandType CommandType { get; set; }
        public Guid DataSourceIdentifier { get; set; }

        public string Name
        {
            get { return _name ?? (_name = SqlCommandText); }
            set { _name = value; }
        }

        public bool LoadData()
        {
            try
            {
                _log.Info("Attempting to connect to database with connection string: {0}", ConnectionString);
                using (var sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                
                    _log.Info("Connection open");    

                    using (var scCommand = new SqlCommand(SqlCommandText, sqlConnection))
                    {
                        scCommand.CommandType = CommandType;
                        scCommand.Prepare();
                        
                        _log.Info("Command prepared. {0}", SqlCommandText);    

                        Data = new DataTable(Name);

                        using (var reader = scCommand.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            _log.Info("Command executed.");    

                            var fieldCount = reader.FieldCount;
                            var columnNames = new List<string>();
                            _log.Info("Reading columns.");    
                            for (var i = 0; i < fieldCount; i++)
                            {
                                var columnName = reader.GetName(i);
                                var dataType = reader.GetFieldType(i);
                                columnNames.Add(columnName);

                                Data.Columns.Add(dataType != null
                                    ? new DataColumn(columnName, dataType)
                                    : new DataColumn(columnName));

                            }
                            _log.Info("Loading data.");   
                            while(reader.Read())
                            {
                                var dataRow = Data.NewRow();

                                foreach (var columnName in columnNames)
                                {
                                    dataRow[columnName] = reader[columnName];
                                }
                                Data.Rows.Add(dataRow);
                            }

                            _log.Info("Data load complete. {0} rows loaded.", RowCount);   

                            Columns = columnNames.ToArray();
                        }
                    }
                    _log.Info("Closing connection.");   
                    sqlConnection.Close();
                }
                
                LastLoadDate = TimeHelpers.NowInLocalTime();
                State = LoadState.Loaded;
                _log.Info("Load completed");   
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

        public string CreatedBy { get; set; }
        public ZonedDateTime? LastLoadDate { get; set; }
        public LoadState State { get; private set; }
        
        public IEnumerable<LoadLogItem> GetLoadingLog()
        {
            return _log;
        }

        public string[] Columns { get; set; }
        public int? RowCount {
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

        public DataTable Data { get; private set; }
    }
}
