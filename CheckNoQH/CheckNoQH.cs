using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraEditors;
using Plugins;
using CDTDatabase;
using System.Data;
using CDTLib;
using System.Windows.Forms;


namespace CheckNoQH
{
    public class CheckNoQH:ICData
    {
        private InfoCustomData info = new InfoCustomData(IDataType.Single);
        private DataCustomData data;
        DataRow drMaster;

        #region ICData Members

        public DataCustomData Data
        {
            set { data = value; }
        }

        public void ExecuteAfter()
        {

        }

        public void ExecuteBefore()
        {
            CheckTime();
        }
        void CheckTime()
        {
            Database db = Database.NewDataDatabase();
            if (data.CurMasterIndex < 0)
                return;
            drMaster = data.DsData.Tables[0].Rows[data.CurMasterIndex];
            DataTable dt = new DataTable();
            if (drMaster.RowState == DataRowState.Deleted)
            {
                if (Boolean.Parse(drMaster["Duyet", DataRowVersion.Original].ToString()))
                {
                    XtraMessageBox.Show("Đơn đã được duyệt, không được chỉnh sửa dữ liệu", Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    info.Result = false;
                }
                else
                    return;
            }
            else if (drMaster.RowState == DataRowState.Modified)
            {

                if (Boolean.Parse(drMaster["Duyet", DataRowVersion.Original].ToString()))
                {
                    XtraMessageBox.Show("Đơn đã được duyệt, không được chỉnh sửa dữ liệu", Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    info.Result = false;
                }
                else
                {
                    dt = db.GetDataTable(string.Format("SELECT NgayTN FROM DTHoVay WHERE DTID = '{0}'", drMaster["HoVay"].ToString()));
                    if (DateTime.Compare(DateTime.Parse(drMaster["NgayNoQH"].ToString()), DateTime.Parse(dt.Rows[0]["NgayTN"].ToString())) < 0)
                    {
                        XtraMessageBox.Show("Ngày quá hạn phải lớn hơn hoặc bằng ngày hạn cuối trả nợ hiện tại", Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        info.Result = false;
                    }
                    else if (Boolean.Parse(drMaster["Duyet", DataRowVersion.Original].ToString()) == false)
                    {
                        string sql = string.Format(" UPDATE DTHoVay SET IsQuaHan = 1 WHERE DTID = '{0}' ", drMaster["HoVay"].ToString());
                        data.DbData.UpdateByNonQuery(sql);
                        info.Result = true;
                    }
                    else
                        info.Result = true;
                }
            }
            else if (drMaster.RowState == DataRowState.Added)
            {
                if (Boolean.Parse(drMaster["Duyet"].ToString()))
                {
                    string sql = string.Format(" UPDATE DTHoVay SET IsQuaHan = 1 WHERE DTID = '{0}' ", drMaster["HoVay"].ToString());
                    data.DbData.UpdateByNonQuery(sql);
                    info.Result = true;
                }
                else
                    info.Result = true;

            }
        }

        public InfoCustomData Info
        {
            get { return info; }
        }

        #endregion
    }
}
