using System.Data;
using System.Data.SqlClient;

namespace Panda
{

    public class SqlQueryDataSource : TableStructuredDataSource
    {
        public string ConnectionString { get; set; }
        public string SqlCommandText { get; set; }
        public CommandType CommandType { get; set; }


        public override bool SettingsAreValid()
        {
            // not the most secure, but workable for now.

            return !(string.IsNullOrEmpty(ConnectionString) | string.IsNullOrEmpty(SqlCommandText));
        }

        public override string Name
        {
            get { return base.Name ?? (base.Name = SqlCommandText); }
            set { base.Name = value; }
        }

        private bool DoDataLoad(string sqlCommand)
        {
            Log.Info("Attempting to connect to database with connection string: {0}", ConnectionString);
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                Log.Info("Connection open");

                using (var scCommand = new SqlCommand(sqlCommand, sqlConnection))
                {
                    scCommand.CommandType = CommandType;
                    scCommand.Prepare();

                    Log.Info("Command prepared. {0}", SqlCommandText);

                    Data = new DataTable(Name);

                    using (var reader = scCommand.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        Log.Info("Command executed.");

                        var fieldCount = reader.FieldCount;

                        Log.Info("Loading columns.");

                        for (var i = 0; i < fieldCount; i++)
                        {
                            var columnName = reader.GetName(i);
                            var dataType = reader.GetFieldType(i);
                            Log.Info("Loading column '{0}', data type '{1}'.", columnName, dataType != null ? dataType.Name : "unknown");
                            Data.Columns.Add(dataType != null
                                ? new DataColumn(columnName, dataType)
                                : new DataColumn(columnName));

                        }
                        Log.Info("Loading data.");

                        while (reader.Read())
                        {
                            var dataRow = Data.NewRow();

                            for (var i = 0; i < fieldCount; i++)
                            {
                                var columnName = Data.Columns[i].ColumnName;
                                dataRow[columnName] = reader[columnName];
                            }
                            Data.Rows.Add(dataRow);
                        }

                        Log.Info("Data load complete. {0} rows loaded.", Data.Rows.Count);
                    }
                }
                Log.Info("Closing connection.");
                sqlConnection.Close();
            }
            return true;
        }

        protected override bool Preview()
        {
            var sqlText = string.Format("SELECT TOP 0.01 PERCENT alpha.* FROM ({0}) as alpha", SqlCommandText);

            return DoDataLoad(sqlText);
        }

        protected override bool Load()
        {
            return DoDataLoad(SqlCommandText);
        }

    }
}
