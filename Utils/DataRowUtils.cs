using System;
using System.Data;
using Kudos.Coring.Constants;
using Kudos.Coring.Utils;

namespace Kudos.Dating.Utils
{
    public static class DataRowUtils
	{
        #region public static Object? GetValue(...)

        public static T? GetValue<T>(DataRow? dr, String? s) { return ObjectUtils.Parse<T>(GetValue(dr, s)); }
        public static Object? GetValue(DataRow? dr, String? s)
        {
            return
                dr != null
                && DataColumnCollectionUtils.HasColumn(dr.Table.Columns, s)
                    ? NormalizeValue(dr[s])
                    : null;
        }

        public static T? GetValue<T>(DataRow? dr, Int32 i) { return ObjectUtils.Parse<T>(GetValue(dr, i)); }
        public static Object? GetValue(DataRow? dr, Int32 i)
        {
            return
                dr != null
                && DataColumnCollectionUtils.IsValidColumnIndex(dr.Table.Columns, i)
                    ? NormalizeValue(dr.ItemArray[i])
                    : null;
        }

        #endregion

        #region public static Object? NormalizeValue(...)

        public static Object? NormalizeValue(Object? o) { DBNull? dbn = o as DBNull; return dbn == null ? o : null; }

        #endregion
    }
}