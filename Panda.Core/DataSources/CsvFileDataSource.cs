using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Panda.DataSources
{
    public class CsvFileDataSource : TableStructuredDataSource
    {
        public string FileName { get; set; }
        public bool FirstRowAsColumnNames { get; set; }

        public override string Name {
            get { return base.Name ?? (base.Name = FileName); }
            set { base.Name = value; }
        }

        public override bool SettingsAreValid()
        {
            if (string.IsNullOrEmpty(FileName))
                return false;
            return File.Exists(FileName);
        }

        private bool GetFileData(int lineCount = 0)
        {
            Log.Info("Reading contents of file {0}", FileName);

            var lines = lineCount == 0 ? File.ReadAllLines(FileName) : File.ReadLines(FileName).Take(lineCount).ToArray();

            Log.Info("Processing file contents.");

            Data = new DataTable(Name);

            var counter = 1;

            foreach (var columnName in ParseLine(lines[0]).Select(column => FirstRowAsColumnNames ? column : string.Format("Column {0}", counter)))
            {
                Data.Columns.Add(new DataColumn(columnName));
                counter ++;

                Log.Info("Loading column {0}.", columnName);
            }

            var data = FirstRowAsColumnNames ? lines.Skip(1).ToArray() : lines;

            foreach (var line in data)
            {
                var dataRow = Data.NewRow();
                counter = 0;
                foreach (var columnData in ParseLine(line))
                {
                    dataRow[counter++] = columnData;
                }
                Data.Rows.Add(dataRow);
            }

            Log.Info("Loaded {0} rows.", data.Length);
            
            return true;
        }

        protected override bool Load()
        {
            return GetFileData();
        }

        protected override bool Preview()
        {
            return GetFileData(5);
        }


        private IEnumerable<string> ParseLine(string line)
        {
            var matches = new Regex("((?<=\")[^\"]*(?=\"(,|$)+)|(?<=,|^)[^,\"]*(?=,|$))").Matches(line);
            foreach (var match in matches)
                yield return match.ToString();
        }
        
    }
}