using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTLib;
using System.IO;
using System.Diagnostics;

namespace TMBCTC
{
    public partial class FrmFilter : DevExpress.XtraEditors.XtraForm
    {
        private string _file = Application.StartupPath + "\\Reports\\" + Config.GetValue("Package").ToString() + "\\TMBCTCtmp.xls";
        private string _file2 = Application.StartupPath + "\\Reports\\" + Config.GetValue("Package").ToString() + "\\TMBCTCtmp02.xls";
        public bool isQTTC = true;

        public FrmFilter()
        {
            InitializeComponent();
            deFromDate.EditValue = DateTime.Parse("01/01/" + Config.GetValue("NamLamViec").ToString());
            deToDate.EditValue = DateTime.Parse("12/31/" + Config.GetValue("NamLamViec").ToString());
            if (Config.GetValue("Language").ToString() == "1")
                FormFactory.DevLocalizer.Translate(this);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void btnSuaMau_Click(object sender, EventArgs e)
        {
            if (File.Exists(this._file) && this.isQTTC)
                Process.Start(this._file);
            else if (File.Exists(this._file2) && !this.isQTTC)
            {
                Process.Start(this._file2);
            }
            else
            {
                string str = "Không tìm thấy file mẫu!";
                if (Config.GetValue("Language").ToString() == "1")
                    str = UIDictionary.Translate(str);
                int num = (int)XtraMessageBox.Show(str);
            }
        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel file (*.xls)|*.xls";
            saveFileDialog.AddExtension = true;
            int num1 = (int)saveFileDialog.ShowDialog();
            string fileName = saveFileDialog.FileName;
            if (!(fileName != string.Empty))
                return;
            ExcelReport excelReport = !this.isQTTC ? new ExcelReport(this._file2, fileName, this.deFromDate.DateTime, this.deToDate.DateTime) : new ExcelReport(this._file, fileName, this.deFromDate.DateTime, this.deToDate.DateTime);
            excelReport.FillData();
            string str = "Chưa hoàn thành lập thuyết minh báo cáo tài chính! \nVui lòng kiểm tra lại số liệu trên file đã xuất ra!";
            if (Config.GetValue("Language").ToString() == "1")
                str = UIDictionary.Translate(str);
            if (excelReport.IsError)
            {
                int num2 = (int)XtraMessageBox.Show(str);
            }
            Process.Start(fileName);
            this.Close();
        }
    }
}