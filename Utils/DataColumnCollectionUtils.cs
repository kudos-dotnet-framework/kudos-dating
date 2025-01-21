using System;
using System.Data;
using Kudos.Coring.Utils.Collections;

namespace Kudos.Dating.Utils
{
    public static class DataColumnCollectionUtils
    {
        #region public static Boolean? HasColumn(...)

        public static Boolean HasColumn(DataColumnCollection? dcc, String? s) { return s != null && dcc != null && dcc.Contains(s); }

        #endregion

        #region public static Boolean IsValidColumnIndex(...)

        public static Boolean IsValidColumnIndex(DataColumnCollection? dcc, Int32 i) { return CollectionUtils.IsValidIndex(dcc, i); }

        #endregion

        #region public static DataColumn? Get...(...)

        public static DataColumn? GetFirstColumn(DataColumnCollection? dcc) { return GetColumn(dcc, 0); }
        public static DataColumn? GetLastColumn(DataColumnCollection? dcc) { return dcc != null ? GetColumn(dcc, dcc.Count - 1) : null; }
        public static DataColumn? GetColumn(DataColumnCollection? dcc, String? s) { return HasColumn(dcc, s) ? dcc[s] : null; }
        public static DataColumn? GetColumn(DataColumnCollection? dcc, int i) { return IsValidColumnIndex(dcc, i) ? dcc[i] : null; }

        #endregion
    }
}

