using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTDatabase;
using CDTLib;
using System.Globalization;
namespace GLKC
{
    public partial class GLChooseTrans : DevExpress.XtraEditors.XtraForm
    {   
        public List<DataRow> TransList;
        private DataTable dmglck;
        public Database _dbData = Database.NewDataDatabase();
        private string _phanLoai;
        private string namLv;

        public GLChooseTrans(string phanLoai)
        {
            InitializeComponent();
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GLChooseTrans_Keyup);
            _phanLoai = phanLoai;
            namLv = Config.GetValue("NamLamViec").ToString();
            spinEdit1.EditValue = Config.GetValue("KyKeToan") == null ? DateTime.Today.Month : Int32.Parse(Config.GetValue("KyKeToan").ToString());
            spinEdit2.EditValue = Config.GetValue("KyKeToan") == null ? DateTime.Today.Month : Int32.Parse(Config.GetValue("KyKeToan").ToString());
        }

        private void GLChooseTrans_Load(object sender, EventArgs e)
        {
            string sql = "select *,0.0 as Ps from dmKetchuyen";
            if (_phanLoai != "")
                sql += " where PhanLoai = N'" + _phanLoai + "'";
            sql += " order by ttkc";
            dmglck = _dbData.GetDataTable(sql);
            this.gridControl1.DataSource = dmglck.DefaultView;
            gridView1.BestFitColumns();
            if (Config.GetValue("Language").ToString() == "1")
                FormFactory.DevLocalizer.Translate(this);
        }
        private void GLChooseTrans_Keyup(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.F2)
            {
                simpleButton1_Click(simpleButton1, new EventArgs());
            }
            else if (e.KeyCode == Keys.F4)
            {
                simpleButton2_Click(simpleButton2, new EventArgs());
            }
        }

        private List<DataRow> GetTransList()
        {
            List<DataRow> TransList = new List<DataRow>();
            TransList.Clear();
            if (rgOption.SelectedIndex == 1)
                foreach (int i in gridView1.GetSelectedRows())
                {
                    TransList.Add(gridView1.GetDataRow(i));
                }
            else
                for (int i = 0; i < dmglck.Rows.Count; i++)
                {
                    TransList.Add(dmglck.Rows[i]);
                }
            return TransList;
        }

        private bool KhoaSo(int newMonth)
        {
            if (Config.GetValue("NgayKhoaSo") == null)
                return false;
            if (Config.GetValue("NamLamViec") == null)
                return false;
            string tmp = Config.GetValue("NgayKhoaSo").ToString();
            int nam = Int32.Parse(Config.GetValue("NamLamViec").ToString());
            DateTime ngayKhoa;
            DateTimeFormatInfo dtInfo = new DateTimeFormatInfo();
            dtInfo.ShortDatePattern = "dd/MM/yyyy";
            if (DateTime.TryParse(tmp, dtInfo, DateTimeStyles.None, out ngayKhoa))
            {
                if (nam == ngayKhoa.Year && newMonth <= ngayKhoa.Month)
                {
                    string msg = "Kỳ kế toán đã khóa! Không thể chỉnh sửa số liệu!";
                    if (Config.GetValue("Language").ToString() == "1")
                        msg = UIDictionary.Translate(msg);
                    XtraMessageBox.Show(msg);
                    return true;
                }
                else
                    return false;
            }
            return false;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (spinEdit1.Value > spinEdit2.Value)
            {
                return;
            }
            if (KhoaSo(Int32.Parse(spinEdit2.Value.ToString())))
                return;
            GlKcCal CK;
            CultureInfo ci = Application.CurrentCulture;
            List<DataRow> TransList = GetTransList();
            for (int i = int.Parse(spinEdit1.Value.ToString()); i <= spinEdit2.Value; i++)
            {
                try
                {
                    foreach (DataRow dr in TransList)
                    {
                        decimal ps = 0;
                        CK = new GlKcCal(i, namLv, dr);
                        if (!CK.Createbt(ref ps))
                        {
                            string msg = "Không tạo được bút toán kết chuyển";
                            if (Config.GetValue("Language").ToString() == "1")
                                UIDictionary.Translate(msg);
                            XtraMessageBox.Show(msg);
                            return;
                        }
                        else
                        {
                            dr["Ps"] = ps;
                        }
                    }
                    string msg1 = "Tạo thành công kết chuyển tháng ";
                    if (Config.GetValue("Language").ToString() == "1")
                        UIDictionary.Translate(msg1);
                    XtraMessageBox.Show(msg1 + i.ToString() + "!");
                }

                catch (Exception ex)
                {
                    Application.CurrentCulture = ci;
                    XtraMessageBox.Show(ex.Message);
                    return;
                }

            }
            Application.CurrentCulture = ci;
            gridView1.BestFitColumns();

        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (spinEdit1.Value > spinEdit2.Value)
            {
                return;
            }
            if (KhoaSo(Int32.Parse(spinEdit2.Value.ToString())))
                return;
            GlKcCal CK;
            List<DataRow> TransList = GetTransList();
            for (int i = int.Parse(spinEdit1.Value.ToString()); i <= spinEdit2.Value; i++)
            {
                try
                {
                    foreach (DataRow dr in TransList)
                    {
                        CK = new GlKcCal(i, namLv, dr);
                        CK.DeleteBt();
                    }
                    string msg = "Xóa thành công kết chuyển tháng ";
                    if (Config.GetValue("Language").ToString() == "1")
                        UIDictionary.Translate(msg);
                    XtraMessageBox.Show(msg + i.ToString() + "!");
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message);
                    return;
                }
            }
        }
    }
}