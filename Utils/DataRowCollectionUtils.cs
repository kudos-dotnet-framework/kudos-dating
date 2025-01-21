using System;
using System.Data;
using Kudos.Coring.Utils.Collections;

namespace Kudos.Dating.Utils
{
	public static class DataRowCollectionUtils
    {
        #region public static DataRow? Get(...)

        public static DataRow? GetFirstRow(DataRowCollection? drc) { return GetRow(drc, 0); }
        public static DataRow? GetLastRow(DataRowCollection? drc) { return drc != null ? GetRow(drc, drc.Count - 1) : null; }
        public static DataRow? GetRow(DataRowCollection? drc, int i) { return IsValidRowIndex(drc, i) ? drc[i] : null; }

        #endregion

        #region public static Boolean IsValidRowIndex(...)

        public static Boolean IsValidRowIndex(DataRowCollection? drc, Int32 i) { return CollectionUtils.IsValidIndex(drc, i); }

        #endregion
    }
}

