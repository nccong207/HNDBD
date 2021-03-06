using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using CDTLib;

namespace CDT
{
    public partial class FrmStartup : DevExpress.XtraEditors.XtraForm
    {
        private bool _vi = Config.GetValue("Language").ToString() == "0";
        private DataTable dtVideo = new DataTable();
        public FrmStartup()
        {
            InitializeComponent();
            axWebBrowser1.Navigate("http://hoatieuvietnam.com/project/phan-mem-ke-toan/");
        }

        private void FrmStartup_FormClosed(object sender, FormClosedEventArgs e)
        {
            string value = "1";
            string msg = "Tự động xuất hiện màn hình 'Bắt đầu sử dụng' trong những lần sau?";
            msg = _vi ? msg : UIDictionary.Translate(msg);
            if (XtraMessageBox.Show(msg, Config.GetValue("PackageName").ToString(), MessageBoxButtons.YesNo) == DialogResult.No)
            {
                value = "0";
            }
            AppCon ac = new AppCon();
            ac.SetValue("Beginner", value);
        }
    }
}