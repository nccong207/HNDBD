using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.Data;

namespace CDTControl
{
    public class ExportExcel
    {
        private string _tmpFile = string.Empty;
        private string _fileName = string.Empty;
        private System.Data.DataTable _dtData;
        private bool _isError = false;

        public ExportExcel(string tmpFile, string fileName, System.Data.DataTable dtData)
        {
            _fileName = fileName;
            _tmpFile = tmpFile;
            _dtData = dtData;
        }
        public bool Export()
        {
            Application app = new ApplicationClass();
            Workbook wb = app.Workbooks.Open(_tmpFile, Type.Missing, false, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            try
            {
                Worksheet ws = (Worksheet)wb.Sheets[1];
                for (int i = 0; i < _dtData.Rows.Count; i++)
                {
                    ws.Cells[i + 2, 1] = i + 1;
                    for (int j = 0; j < _dtData.Columns.Count; j++)
                    {
                        ws.Cells[i + 2, j + 2] = _dtData.Rows[i][j];
                    }
                }
            }
            catch
            {
                _isError = true;
            }
            finally
            {
                try
                {
                    wb.SaveAs(_fileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    wb.Close(false, _tmpFile, false);
                    app.Quit();
                }
                catch { }
                finally
                {
                    app.Quit();
                }
            }
            return true;
        }
    }
}
