using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Kudos.Coring.Utils.Collections;
using Kudos.Dating.Utils;
using Kudos.Threading.Utils;

namespace Kudos.Dating.Types
{
    public sealed class FastDataTableReader : IDisposable
    {
        #region ... static ...

        private static readonly SemaphoreSlim __ss;
        private static readonly Dictionary<DataTable, FastDataTableReader> __d;

        static FastDataTableReader()
        {
            __ss = new SemaphoreSlim(1,1);
            __d = new Dictionary<DataTable, FastDataTableReader>();
        }

        #endregion

        public readonly DataTable Table;
        public readonly FastDataColumnCollectionReader ColumnsReader;
        public FastDataRowCollectionReader RowsReader { get; private set; }

        //public Boolean CaseSensitive { get { return DataTable.CaseSensitive; } }
        //public DataRelationCollection ChildRelations { get { return DataTable.ChildRelations; } }
        //public ConstraintCollection Constraints { get { return DataTable.Constraints; } }
        //public IContainer? Container { get { return DataTable.Container; } }
        //public DataSet? DataSet { get { return DataTable.DataSet; } }
        //public DataView DefaultView { get { return DataTable.DefaultView; } }
        //public Boolean DesignMode { get { return DataTable.DesignMode; } }
        //public String DisplayExpression { get { return DataTable.DisplayExpression; } }
        //public PropertyCollection ExtendedProperties { get { return DataTable.ExtendedProperties; } }
        //public Boolean HasErrors { get { return DataTable.HasErrors; } }
        //public Boolean IsInitialized { get { return DataTable.IsInitialized; } }
        //public CultureInfo Locale { get { return DataTable.Locale; } }
        //public Int32 MinimumCapacity { get { return DataTable.MinimumCapacity; } }
        //public String Namespace { get { return DataTable.Namespace; } }
        //public DataRelationCollection ParentRelations { get { return DataTable.ParentRelations; } }
        //public String Prefix { get { return DataTable.Prefix; } }
        //public DataColumn[] PrimaryKey { get { return DataTable.PrimaryKey; } }
        //public SerializationFormat RemotingFormat { get { return DataTable.RemotingFormat; } }
        //public ISite? Site { get { return DataTable.Site; } }
        //public String TableName { get { return DataTable.TableName; } }

        private FastDataTableReader(ref DataTable dt)
        {
            Table = dt;
            try { Table.TableNewRow += _Table_OnRowRecalculating; } catch { }
            _Table_OnRowRecalculating(null, null);
            ColumnsReader = FastDataColumnCollectionReader.New(dt.Columns);
        }

        private void _Table_OnRowRecalculating(object sender, DataTableNewRowEventArgs e)
        {
            if (RowsReader != null) RowsReader.Dispose();
            RowsReader = FastDataRowCollectionReader.New(Table.Rows);
        }

        public void Dispose()
        {
            SemaphoreUtils.Wait(__ss);
            try { Table.TableNewRow -= _Table_OnRowRecalculating; } catch { }
            DictionaryUtils.Remove(__d, Table);
            DataTableUtils.Dispose(Table);
            SemaphoreUtils.Release(__ss);
        }

        public static FastDataTableReader? New(DataTable? dt)
        {
            if (dt == null)
                return null;

            SemaphoreUtils.Wait(__ss);

            FastDataTableReader? fdtr;
            __d.TryGetValue(dt, out fdtr);

            if (fdtr == null)
            {
                fdtr = new FastDataTableReader(ref dt);
                __d[dt] = fdtr;
            }

            SemaphoreUtils.Release(__ss);

            return fdtr;
        }
    }
}