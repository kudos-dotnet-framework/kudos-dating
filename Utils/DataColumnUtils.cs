using System;
using System.Data;
using Kudos.Coring.Constants;

namespace Kudos.Dating.Utils
{
	public static class DataColumnUtils
	{
        public static Exception? Dispose(DataColumn? dc)
        {
            if (dc == null) return CException.ArgumentNullException;
            try { dc.Dispose(); return null; }
            catch (Exception exc) { return exc; }
        }
    }
}