using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Globalization;
using System.IO;

namespace Panda
{
    public class ExcelFileDataSource : TableStructuredDataSource
    {
        public string FileName { get; set; }
        public string Worksheet { get; set; }
        public bool ColumnNamesInHeader { get; set; }

        private string ConnectionString {
            get
            {
                var extension = Path.GetExtension(FileName).ToUpper(CultureInfo.InvariantCulture);
                return string.Format(MapOfFileExtensionToConnectionStrings[extension], FileName, ColumnNamesInHeader);
            }
        }


        private static readonly Dictionary<string, string> MapOfFileExtensionToConnectionStrings = new Dictionary<string, string>{
            {".XLSX", @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR={1}""" },
            {".XLSB", @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0;HDR={1}""" },
            {".XLSM", @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Macro;HDR={1}""" },
            {".XLS", @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 8.0;HDR={1}""" },
        };

        private string SqlCommandText {
            get { return string.Format("select * from [{0}]", Worksheet); }
        }

        public override bool SettingsAreValid()
        {
            if (string.IsNullOrEmpty(FileName))
            {
                Log.Error("File name not specified.");
                return false;
            }

            if (string.IsNullOrEmpty(Worksheet))
            {
                Log.Error("Worksheet not specified.");
                return false;
            }

            if (!File.Exists(FileName))
            {
                Log.Error("File {0} not found!");
                return false;
            }


            var extension = Path.GetExtension(FileName).ToUpper(CultureInfo.InvariantCulture);

            if (MapOfFileExtensionToConnectionStrings.ContainsKey(extension)) return true;
            
            Log.Error("File may not be a valid Excel File.");
            return false;
        }

        public override string Name
        {
            get { return base.Name ?? (base.Name = Worksheet); }
            set { base.Name = value; }
        }

        private bool DoDataLoad(string sqlCommand)
        {
            Log.Info("Attempting to connect to Excel File: {0}", FileName);

            using (var connection = new OleDbConnection(ConnectionString))
            {
                connection.Open();

                Log.Info("Connection open");

                var adapter = new OleDbDataAdapter(sqlCommand, connection);
                LoadFromDataAdapter(adapter);
                Log.Info("Command prepared. {0}", SqlCommandText);

                Log.Info("Closing connection.");
                connection.Close();
                Log.Info("Connection closed.");
            }
            return true;
        }

        protected override bool Preview()
        {
            var sqlText = string.Format("SELECT TOP 2 alpha.* FROM ({0}) as alpha", SqlCommandText);

            return DoDataLoad(sqlText);
        }

        protected override bool Load()
        {
            return DoDataLoad(SqlCommandText);
        }
    }
}