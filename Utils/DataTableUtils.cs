using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Kudos.Coring.Constants;
using Kudos.Coring.Types;

namespace Kudos.Dating.Utils
{
	public static class DataTableUtils
	{
        #region Column

        public static Boolean HasColumn(DataTable? dt, String? s) { return dt != null && DataColumnCollectionUtils.HasColumn(dt.Columns, s); }
        public static Boolean IsValidColumnIndex(DataTable? dt, Int32 i) { return dt != null && DataColumnCollectionUtils.IsValidColumnIndex(dt.Columns, i); }
        public static DataColumn? GetColumn(DataTable? dt, String? s) { return dt != null ? DataColumnCollectionUtils.GetColumn(dt.Columns, s) : null; }
        public static DataColumn? GetColumn(DataTable? dt, int i) { return dt != null ? DataColumnCollectionUtils.GetColumn(dt.Columns, i) : null; }
        public static DataColumn? GetFirstColumn(DataTable? dt) { return dt != null ? DataColumnCollectionUtils.GetFirstColumn(dt.Columns) : null; }
        public static DataColumn? GetLastColumn(DataTable? dt) { return dt != null ? DataColumnCollectionUtils.GetLastColumn(dt.Columns) : null; }

        #endregion

        #region Row

        public static Boolean IsValidRowIndex(DataTable? dt, Int32 i) { return dt != null && DataRowCollectionUtils.IsValidRowIndex(dt.Rows, i); }
        public static DataRow? GetRow(DataTable? dt, int i) { return dt != null ? DataRowCollectionUtils.GetRow(dt.Rows, i) : null; }
        public static DataRow? GetFirstRow(DataTable? dt) { return dt != null ? DataRowCollectionUtils.GetFirstRow(dt.Rows) : null; }
        public static DataRow? GetLastRow(DataTable? dt) { return dt != null ? DataRowCollectionUtils.GetLastRow(dt.Rows) : null; }

        #endregion

        public static Exception? Dispose(DataTable? dt)
        {
            if (dt == null) return CException.ArgumentNullException;
            try { dt.Dispose(); return null; }
            catch(Exception exc) { return exc; }
        }

        public static async Task<SmartResult<DataTable?>> NewAsync(DbDataReader? dbdr, Int32? iMinimumCapacity = null)
        {
            if (dbdr == null) return SmartResult<DataTable?>.ArgumentNullException;
            else if (!dbdr.HasRows) return SmartResult<DataTable?>.ArgumentOutOfRangeException;
            DataTable dt = new DataTable();
            if (iMinimumCapacity != null && iMinimumCapacity > 0)
                dt.MinimumCapacity = iMinimumCapacity.Value;

            SmartResult<DataTable?> sr;
            try
            {
                dt.Load(dbdr);
                sr = new SmartResult<DataTable?>(dt);
            }
            catch (Exception exc)
            {
                sr = new SmartResult<DataTable?>(exc);
            }

            try { await dbdr.DisposeAsync(); } catch { }

            return sr;
        }
    }
}