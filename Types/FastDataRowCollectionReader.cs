using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Kudos.Coring.Utils.Collections;
using Kudos.Dating.Utils;
using Kudos.Threading.Utils;

namespace Kudos.Dating.Types
{
    public sealed class FastDataRowCollectionReader : IDisposable
	{
        #region ... static ...

        private static readonly SemaphoreSlim __ss;
        private static readonly Dictionary<DataRowCollection, FastDataRowCollectionReader> __d;

        static FastDataRowCollectionReader()
        {
            __ss = new SemaphoreSlim(1, 1);
            __d = new Dictionary<DataRowCollection, FastDataRowCollectionReader>();
        }

        #endregion

        public readonly DataRowCollection Rows;

        public FastDataRowReader? this[Int32 i] { get { return FastDataRowReader.New(DataRowCollectionUtils.GetRow(Rows, i)); } }
        public FastDataRowReader? GetFirstRow() { return FastDataRowReader.New(DataRowCollectionUtils.GetFirstRow(Rows)); }
        public FastDataRowReader? GetLastRow() { return FastDataRowReader.New(DataRowCollectionUtils.GetLastRow(Rows)); }
        public FastDataRowReader? GetRow(Int32 i) { return this[i]; }

        internal FastDataRowCollectionReader(ref DataRowCollection drc)
        {
            Rows = drc;
        }

        public void Dispose()
        {
            SemaphoreUtils.Wait(__ss);
            DictionaryUtils.Remove(__d, Rows);
            SemaphoreUtils.Release(__ss);
        }

        public static FastDataRowCollectionReader? New(DataRowCollection? drc)
        {
            if (drc == null)
                return null;

            SemaphoreUtils.Wait(__ss);

            FastDataRowCollectionReader? fdrcr;
            __d.TryGetValue(drc, out fdrcr);

            if (fdrcr == null)
            {
                fdrcr = new FastDataRowCollectionReader(ref drc);
                __d[drc] = fdrcr;
            }

            SemaphoreUtils.Release(__ss);

            return fdrcr;
        }
    }
}

