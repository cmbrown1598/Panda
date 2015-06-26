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
            if (string.IsNullOrEmpty(ConnectionString))
            {
                Log.Error("Connection String was null or empty.");
                return false;
            }

            if (!string.IsNullOrEmpty(SqlCommandText)) return true;
            
            Log.Error("Command text was null or empty.");
            return false;
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

                var adapter = new SqlDataAdapter(sqlCommand, sqlConnection);

                Log.Info("Command prepared. {0}", SqlCommandText);
                
                LoadFromDataAdapter(adapter);

                Log.Info("Data load complete. {0} rows loaded.", Data.Rows.Count);

                Log.Info("Closing connection.");
                sqlConnection.Close();
                Log.Info("Connection closed.");
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
