using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading;
using Kudos.Coring.Utils.Collections;
using Kudos.Threading.Utils;

namespace Kudos.Dating.Types
{
    public sealed class FastDataColumnCollectionReader : IDisposable
	{
        #region ... static ...

        private static readonly SemaphoreSlim __ss;
        private static readonly Dictionary<DataColumnCollection, FastDataColumnCollectionReader> __d;

        static FastDataColumnCollectionReader()
        {
            __ss = new SemaphoreSlim(1,1);
            __d = new Dictionary<DataColumnCollection, FastDataColumnCollectionReader>();
        }

        #endregion

        //public Int32 Count { get { return _dcc.Count; } }
        //public Boolean IsReadOnly { get { return _dcc.IsReadOnly; } }
        //public Boolean IsSynchronized { get { return _dcc.IsSynchronized; } }
        //public Object SyncRoot { get { return _dcc.SyncRoot; } }

        private readonly SemaphoreSlim _ss;
        public readonly DataColumnCollection Columns;
        private readonly Dictionary<String, Int32?> _ddcn2dci;
        private readonly Dictionary<Int32, DataColumn?> _ddci2dc;

        public DataColumn? this[Int32 i]
        {
            get
            {
                DataColumn? dc;
                _ddci2dc.TryGetValue(i, out dc);
                return dc;
            }
        }

        public DataColumn? this[String? s]
        {
            get
            {
                Int32? i = GetIndex(s);
                return i != null ? this[i.Value] : null;
            }
        }

        public DataColumn? GetColumn(Int32 i) { return this[i]; }
        public DataColumn? GetColumn(String? s) { return this[s]; }

        public Int32? GetIndex(DataColumn? dc)
        {
            return
                dc != null
                    ? GetIndex(dc.ColumnName)
                    : null;
        }

        public Int32? GetIndex(String? s)
        {
            if (s == null) return null;
            Int32? i; _ddcn2dci.TryGetValue(s, out i); return i;
        }

        public Boolean IsValidIndex(Int32 i)
        {
            return this[i] != null;
        }

        internal FastDataColumnCollectionReader(ref DataColumnCollection dcc)
        {
            _ss = new SemaphoreSlim(1,1);

            Columns = dcc;

            _ddcn2dci = new Dictionary<string, int?>(Columns.Count);
            _ddci2dc = new Dictionary<int, DataColumn?>(Columns.Count);

            try { Columns.CollectionChanged += _Columns_OnCollectionChanged; } catch { }
            _Columns_OnCollectionChanged(null, null);
        }

        private void _Columns_OnCollectionChanged(object? sender, CollectionChangeEventArgs e)
        {
            SemaphoreUtils.Wait(_ss);

            _ddci2dc.Clear();
            _ddcn2dci.Clear();

            try
            {
                for (int i = 0; i < Columns.Count; i++)
                {
                    _ddci2dc[i] = Columns[i];
                    _ddcn2dci[Columns[i].ColumnName] = i;
                }
            }
            catch
            {
            }

            SemaphoreUtils.Release(_ss);
        }

        public void Dispose()
        {
            try { Columns.CollectionChanged -= _Columns_OnCollectionChanged; } catch { }
            SemaphoreUtils.Dispose(_ss);
            SemaphoreUtils.Wait(__ss);
            DictionaryUtils.Remove(__d, Columns);
            SemaphoreUtils.Release(__ss);
        }

        public static FastDataColumnCollectionReader? New(DataColumnCollection? dcc)
        {
            if (dcc == null)
                return null;

            SemaphoreUtils.Wait(__ss);

            FastDataColumnCollectionReader? fdccr;
            __d.TryGetValue(dcc, out fdccr);

            if (fdccr == null)
            {
                fdccr = new FastDataColumnCollectionReader(ref dcc);
                __d[dcc] = fdccr;
            }

            SemaphoreUtils.Release(__ss);

            return fdccr;
        }
    }
}

