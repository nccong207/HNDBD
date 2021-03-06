using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CDTDatabase;
using CDTLib;
using Plugins;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace CheckNgayGH
{
    public class CheckNgayGH : ICData 
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
            Database db = Database.NewDataDatabase();
            bool khoanhNo = false;
            string dtIdHoVay = "";
            if (data.CurMasterIndex < 0)
                return;
            drMaster = data.DsData.Tables[0].Rows[data.CurMasterIndex];
            DataTable dt = new DataTable();
            if (drMaster.RowState == DataRowState.Deleted)
            {
                // kiểm tra tính hợp lệ của ngày gia hạn
                dtIdHoVay = drMaster["Duyet", DataRowVersion.Original].ToString();
                if (Boolean.Parse(drMaster["Duyet",DataRowVersion.Original].ToString()))
                {
                    XtraMessageBox.Show("Đơn đã được duyệt,bạn không được chỉnh sửa dữ liệu", Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    info.Result = false;
                }
            }
            else if (drMaster.RowState == DataRowState.Modified)
            {
                if (Boolean.Parse(drMaster["Duyet", DataRowVersion.Original].ToString()))
                {
                    XtraMessageBox.Show("Đơn đã được duyệt,bạn không được chỉnh sửa dữ liệu", Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    info.Result = false;
                }
                else
                {
                    // kiểm tra tính hợp lệ của ngày gia hạn
                    dt = db.GetDataTable(string.Format("SELECT NgayTN FROM DTHoVay WHERE DTID = '{0}'", drMaster["HoVay"].ToString()));
                    if (!string.IsNullOrEmpty(drMaster["NgayGH"].ToString()) && (DateTime.Compare(DateTime.Parse(drMaster["NgayXinGH"].ToString()), DateTime.Parse(dt.Rows[0]["NgayTN"].ToString())) < 0
                    || DateTime.Compare(DateTime.Parse(drMaster["NgayGH"].ToString()), DateTime.Parse(dt.Rows[0]["NgayTN"].ToString())) < 0))
                    {
                        XtraMessageBox.Show("Ngày đề nghị và ngày gia hạn phải lớn hơn ngày nợ hiện tại", Config.GetValue("PackageName").ToString(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        info.Result = false;
                    }
                    else if (Boolean.Parse(drMaster["Duyet", DataRowVersion.Original].ToString()) == false)
                    {
                        if (!string.IsNullOrEmpty(drMaster["NgayGH"].ToString()))
                        {
                            string sql = string.Format(" UPDATE DTHoVay SET NgayTN = '{0}' WHERE DTID = '{1}' ", DateTime.Parse(drMaster["NgayGH"].ToString()), drMaster["HoVay"].ToString());
                            info.Result = data.DbData.UpdateByNonQuery(sql);
                        }
                        string laisuat = drMaster["LaiSuatG"].ToString();
                        if (!string.IsNullOrEmpty(laisuat))
                        {
                            string sql = string.Format("UPDATE DTHoVay SET LaiSuat = '{0}' WHERE DTID = '{1}' ", laisuat, drMaster["HoVay"].ToString());
                            info.Result = data.DbData.UpdateByNonQuery(sql);
                        }
                        khoanhNo = (bool)drMaster["KhoanhNo"];
                        string sql1 = string.Format("UPDATE DTHoVay SET KhoanhNo = '{0}' FROM DTHoVay WHERE DTID = '{1}'", khoanhNo, drMaster["HoVay"].ToString());
                        info.Result = data.DbData.UpdateByNonQuery(sql1);
                    }
                    else
                        info.Result = true;
                }
            }
            else if(drMaster.RowState == DataRowState.Added)
            {
                khoanhNo = (bool)drMaster["KhoanhNo"];
                if (Boolean.Parse(drMaster["Duyet"].ToString()))
                {
                    if (!string.IsNullOrEmpty(drMaster["NgayGH"].ToString()))
                    {
                        string sql = string.Format(" UPDATE DTHoVay SET NgayTN = '{0}' WHERE DTID = '{1}' ", DateTime.Parse(drMaster["NgayGH"].ToString()), drMaster["HoVay"].ToString());
                        info.Result = data.DbData.UpdateByNonQuery(sql);
                    }
                    string laisuat = drMaster["LaiSuatG"].ToString();
                    if (!string.IsNullOrEmpty(laisuat))
                    {
                        string sql = string.Format("UPDATE DTHoVay SET LaiSuat = '{0}' WHERE DTID = '{1}' ", laisuat, drMaster["HoVay"].ToString());
                         info.Result = data.DbData.UpdateByNonQuery(sql);
                    }

                    khoanhNo = (bool)drMaster["KhoanhNo"];
                    string sql1 = string.Format("UPDATE DTHoVay SET KhoanhNo = '{0}' FROM DTHoVay WHERE DTID = '{1}'", khoanhNo, drMaster["HoVay"].ToString());
                    info.Result = data.DbData.UpdateByNonQuery(sql1);
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
