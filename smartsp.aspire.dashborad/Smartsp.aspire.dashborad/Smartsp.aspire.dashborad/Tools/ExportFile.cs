using System;


namespace Smartsp.aspire.dashborad.Tools
{
    public class ExportFile
    {
        public static string GetNameFileExport(string basicName)
        {
            return string.Format("{0}_{1}.xls", basicName, DateTime.Now.ToString("dd.MM.yyyy HH_mm"));
        }
    }
}