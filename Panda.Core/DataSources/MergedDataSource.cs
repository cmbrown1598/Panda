using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Panda.DataSources
{
    public class MergedDataSource : TableStructuredDataSource
    {
        public ITableStructuredDataSource Left { get; set; }
        public ITableStructuredDataSource Right { get; set; }
        public string JoinOn { get; set; }

        public override string Name
        {
            get { return base.Name ?? (base.Name = string.Format("{0} join {1} on {2}", Left.Name, Right.Name, JoinOn)); }
            set { base.Name = value; }
        }

        public override bool SettingsAreValid()
        {
            var retValue = (Left != null) && Left.SettingsAreValid();
            retValue &= (Right != null) && Right.SettingsAreValid();

            retValue &= (!string.IsNullOrEmpty(JoinOn));

            return retValue;
        }

        protected override bool Preview()
        {
            Left.LoadPreview();
            Right.LoadPreview();

            JoinData(Left.Data, Right.Data, JoinOn);
            
            return true;
        }

        protected override bool Load()
        {
            Left.LoadData();
            Right.LoadData();

            JoinData(Left.Data, Right.Data, JoinOn);

            return true;
        }

        private void JoinData(DataTable leftDataTable, DataTable rightDataTable, string joinOn)
        {
            var dictionary = new Dictionary<string, string>();

            Data = new DataTable(Name);
            for (var i = 0; i < leftDataTable.Columns.Count; i++)
            {
                var columnName = AddColumn(Data, leftDataTable.Columns[i].ColumnName, leftDataTable.Columns[i].DataType);
                dictionary.Add(columnName, "LEFT_" + leftDataTable.Columns[i].ColumnName);
            }

            for (var i = 0; i < rightDataTable.Columns.Count; i++)
            {
                var columnName = AddColumn(Data, rightDataTable.Columns[i].ColumnName, rightDataTable.Columns[i].DataType);
                dictionary.Add(columnName, "RIGHT_" + rightDataTable.Columns[i].ColumnName);
            }




            var idxItems = CreateIndex(CreateIndex(leftDataTable), CreateIndex(rightDataTable));

            foreach (var indexMatch in idxItems)
            {
                var leftRows = leftDataTable.Select(string.Format("[{0}] = '{1}'", joinOn, indexMatch));
                var rightRows = rightDataTable.Select(string.Format("[{0}] = '{1}'", joinOn, indexMatch));

                foreach (var row in leftRows)
                {
                    foreach (var rRow in rightRows)
                    {
                        var newRow = Data.NewRow();

                        foreach (var item in dictionary.Keys)
                        {
                            var colName = dictionary[item];
                            var isRight = colName.StartsWith("RIGHT_");
                            colName = colName.Replace("RIGHT_", "LEFT_").Replace("LEFT_", string.Empty);
                            
                            if (!isRight)
                            {
                                newRow[item] = row[colName];
                            }
                            else
                            {
                                newRow[item] = rRow[colName];
                            }
                        }

                        Data.Rows.Add(newRow);
                    }
                }
            }
        }

        private static HashSet<string> CreateIndex(HashSet<string> left, HashSet<string> right)
        {
            var idx = new HashSet<string>();

            foreach (var item in left.Where(right.Contains))
                idx.Add(item);

            return idx;
        }

        private HashSet<string> CreateIndex(DataTable dataTable)
        {
            var idx = new HashSet<string>();
        
            for(var i =0;i < dataTable.Rows.Count; i++)
            {
                var value = dataTable.Rows[i][JoinOn].ToString();
                if(!idx.Contains(value))
                    idx.Add(value);
            }
            return idx;
        }


        private string AddColumn(DataTable table, string columnName, Type columnType)
        {
            var counter = 0;
            var cName = columnName;
            while (table.Columns[cName] != null)
            {
                counter++;
                cName = columnName + counter;
            }
            if (columnName != JoinOn)
            {
                table.Columns.Add(new DataColumn(cName, columnType));
            }
            return cName;
        }
    }
}