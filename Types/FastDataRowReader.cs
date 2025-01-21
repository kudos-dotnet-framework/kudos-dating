using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Kudos.Coring.Utils.Collections;
using Kudos.Dating.Utils;
using Kudos.Threading.Utils;

namespace Kudos.Dating.Types
{
    public sealed class FastDataRowReader : IDisposable
    {
        #region ... static ...

        private static readonly SemaphoreSlim __ss;
        private static readonly Dictionary<DataRow, FastDataRowReader> __d;
        static FastDataRowReader()
        {
            __ss = new SemaphoreSlim(1, 1);
            __d = new Dictionary<DataRow, FastDataRowReader>();
        }

        #endregion

        public readonly FastDataTableReader TableReader;
        public readonly DataRow Row;

        public object? this[int i] { get { return TableReader.ColumnsReader.IsValidIndex(i) ? DataRowUtils.NormalizeValue(Row.ItemArray[i]) : null; } }
        public object? this[String? s] { get { Int32? i = TableReader.ColumnsReader.GetIndex(s); return i != null ? DataRowUtils.NormalizeValue(Row.ItemArray[i.Value]) : null; } }
        public object? this[DataColumn? dc] { get { Int32? i = TableReader.ColumnsReader.GetIndex(dc); return i != null ? DataRowUtils.NormalizeValue(Row.ItemArray[i.Value]) : null; } }

        private FastDataRowReader(ref DataRow dr)
        {
            TableReader = FastDataTableReader.New(dr.Table);
            Row = dr;
        }

        public static FastDataRowReader? New(DataRow? dr)
        {
            if (dr == null)
                return null;

            SemaphoreUtils.Wait(__ss);

            FastDataRowReader? fdrr;
            __d.TryGetValue(dr, out fdrr);

            if (fdrr == null)
            {
                fdrr = new FastDataRowReader(ref dr);
                __d[dr] = fdrr;
            }

            SemaphoreUtils.Release(__ss);

            return fdrr;
        }

        public void Dispose()
        {
            SemaphoreUtils.Wait(__ss);
            DictionaryUtils.Remove(__d, Row);
            SemaphoreUtils.Release(__ss);
        }
    }
}