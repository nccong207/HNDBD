using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CDTDatabase;
using CDTLib;
using Plugins;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraLayout;
using DevExpress.XtraGrid.Views.Grid;
using System.Windows.Forms;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraEditors.Controls;

namespace ChonCQQDICC
{
    public class ChonCQQDICC:ICControl
    {
        private InfoCustomControl info = new InfoCustomControl(IDataType.MasterDetailDt);
        private DataCustomFormControl data;
        DataRow drMaster;
        ComboBoxEdit cboChonCQQD;
        GridLookUpEdit glueCQQD;
        LayoutControl lcMain;
        bool isVisible = false;

        #region ICControl Members

        public void AddEvent()
        {
            try
            {
                /* Nếu đơn vị đăng nhập là TP thì chọn cơ quan ra quyết định là TP hoặc TW
                 * Nếu đơn vị đăng nhập không phải là TP thì ẩn cột chọn loại quyết định
                 */ 
                drMaster = (data.BsMain.Current as DataRowView).Row;
                if (drMaster == null || drMaster.RowState == DataRowState.Deleted)
                    return;
                cboChonCQQD = data.FrmMain.Controls.Find("ChonCQQD", true)[0] as ComboBoxEdit;
                if (cboChonCQQD == null)
                    return;
                data.FrmMain.Shown += new EventHandler(FrmMain_Shown);

                /* Do datasource change ko chạy được khi thay đổi giá trị của ComboboxEdit nên 
                 * dùng cách của ComboboxEdit
                 */
                cboChonCQQD.SelectedValueChanged += new EventHandler(cboChonCQQD_SelectedValueChanged);
                //cboChonCQQD.EditValueChanged += new EventHandler(cboChonCQQD_EditValueChanged);
               
                
                /* Do sau khi thay đổi giá trị của ComboboxEdit vẫn không thay đổi Text của ComboboxEdit 
                 * nên dùng thêm xự kiện Datasourcechanged
                 * Chưa hiểu nguyên nhân tại sao...
                 */
                data.BsMain.DataSourceChanged += new EventHandler(BsMain_DataSourceChanged);
                BsMain_DataSourceChanged(data.BsMain, new EventArgs());
               

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message.ToString(), Config.GetValue("PackageName").ToString(),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        void cboChonCQQD_SelectedValueChanged(object sender, EventArgs e)
        {
            drMaster = (data.BsMain.Current as DataRowView).Row;
            if (Config.GetValue("MaCN").ToString() != "TP" || drMaster == null) return;
            if (cboChonCQQD.EditValue.ToString().ToUpper() == "QĐ CỦA TỈNH" && !isChange)
            {
                isChange = true;
                drMaster["BoPhan"] = "TP";
                isChange = false;
            }
            else if (cboChonCQQD.EditValue.ToString().ToUpper() == "QĐ CỦA TRUNG ƯƠNG" && !isChange)
            {
                isChange = true;
                drMaster["BoPhan"] = "TW";
                isChange = false;
            }
        }

        void FrmMain_Shown(object sender, EventArgs e)
        {
            lcMain = data.FrmMain.Controls.Find("lcMain", true)[0] as LayoutControl;
            foreach (BaseLayoutItem item in lcMain.Items)
            {
                // Không sử dụng trường này nữa nên ẩn đi hvkhoi
                if (item.Text.ToUpper() == "LOẠI QĐ" || item.Text == "item1")
                {
                    item.Visibility = LayoutVisibility.Never;
                }

                if (item.Text.ToUpper() == "SỐ QĐ (TỈNH)" || item.Text.ToUpper() == "Ngày QĐ (Tỉnh)")
                {
                    if (Config.GetValue("MaCN").ToString() == "TP")
                        item.Visibility = LayoutVisibility.Always;
                    else
                        item.Visibility = LayoutVisibility.Never;
                }
            }  
        }
        

        void BsMain_DataSourceChanged(object sender, EventArgs e)
        {
            DataSet ds = data.BsMain.DataSource as DataSet;
            if (ds == null) return;
            if (data.BsMain.Current != null)
                drMaster = (data.BsMain.Current as DataRowView).Row;

            ds.Tables[0].ColumnChanged += new DataColumnChangeEventHandler(ChonCQQDICC_ColumnChanged);
        }

        void ChonCQQDICC_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnName == "BoPhan" && drMaster["BoPhan"].ToString() == "TW")
            {
                drMaster["ChonCQQD"] = "QĐ của Trung ương";
            }
            if (e.Column.ColumnName == "BoPhan" && drMaster["BoPhan"].ToString() == "TP")
            {
                drMaster["ChonCQQD"] = "QĐ của Tỉnh";
            }
        }

        bool isChange = false;
        void cboChonCQQD_EditValueChanged(object sender, EventArgs e)
        {
            drMaster = (data.BsMain.Current as DataRowView).Row;
            if (drMaster == null) return;
            if (cboChonCQQD.EditValue.ToString().ToUpper() == "QĐ CỦA TỈNH" && !isChange )
            {
                isChange = true;
                drMaster["BoPhan"] = "TP";
                drMaster.AcceptChanges();
                isChange = false; 
            }
            else if( cboChonCQQD.EditValue.ToString().ToUpper() == "QĐ CỦA TRUNG ƯƠNG" && !isChange )
            {
                isChange = true;
                drMaster["BoPhan"] = "TW";
                drMaster.AcceptChanges();
                isChange = false;
                
            }
        } 

        public DataCustomFormControl Data
        {
            set { data = value; }
        }

        public InfoCustomControl Info
        {
            get { return info; }
        }

        #endregion
    }
}
