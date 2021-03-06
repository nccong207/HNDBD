using System;
using System.Collections.Generic;
using System.Text;
using CDTDatabase;
using CDTLib;
using Plugins;
using DevExpress;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using System.Data;

namespace TinhTrangDA
{
    public class TinhTrangDA : ICData 
    {
        Database db = Database.NewDataDatabase();
        private InfoCustomData info = new InfoCustomData(IDataType.MasterDetailDt);
        private DataCustomData data;
        DataRow drMaster;

        int vaOld;
        int vaNew;
        //int TongSoHo = 0;
        //int TongSoLD = 0;
        //int TongSoLDTH = 0;
        //int TongSoNN = 0;
        //decimal TongVonCo = 0;
        //decimal TongVonVay = 0;
        //decimal TongVonDA = 0;
        //int isHoTro = 0;
        //decimal TongVonDN;

        #region ICData Members

        public DataCustomData Data
        {
            set { data = value ; }
        }

        public void ExecuteAfter()
        {
        }

        public void ExecuteBefore()
        {
            if (data.CurMasterIndex < 0) return;
            drMaster = data.DsData.Tables[0].Rows[data.CurMasterIndex];
            if ((drMaster.RowState == DataRowState.Deleted || drMaster.RowState == DataRowState.Modified) 
                && (bool)drMaster["GiaiNgan", DataRowVersion.Original])
            {
                XtraMessageBox.Show("Bạn không được sửa/xóa dữ liệu khi dự án đã giải ngân", Config.GetValue("PackageName").ToString());
                info.Result = false;
                return;
            }
            if (drMaster.RowState == DataRowState.Deleted)
                return;
            //ChangeTT();

            CapNhatSoTong();
            //Kiem tra so tien vay cua cac ho vay
            string nguonId = drMaster["NguonID"].ToString();
            string nguonSql = "SELECT TienVayMin,TienVayMax FROM DMNguon WHERE ID = {0}";
            DataTable minmaxTienVay = db.GetDataTable(string.Format(nguonSql, nguonId));
            
            decimal min = 0, max = 0;
            if (minmaxTienVay.Rows.Count > 0)
            {
                if (minmaxTienVay.Rows[0]["TienVayMin"] != DBNull.Value)
                    min = Convert.ToDecimal(minmaxTienVay.Rows[0]["TienVayMin"].ToString());

                if (minmaxTienVay.Rows[0]["TienVayMax"] != DBNull.Value)
                    max = Convert.ToDecimal(minmaxTienVay.Rows[0]["TienVayMax"].ToString());
            }

            var rows = data.DsData.Tables[1].Select("MTID = '" + drMaster["MTID"].ToString() + "'");
            List<string> hovayvipham = new List<string>();
            foreach (var row in rows)
            {
                decimal tienvay = Convert.ToDecimal(row["CanVay"].ToString());
                if ((tienvay <  min && min > 0)||( tienvay > max && max > 0))
                {
                    string tenhovay = row["HoTen"].ToString();
                    hovayvipham.Add(tenhovay);
                }
            }

            if (hovayvipham.Count > 0)
            {
                string ds = string.Join(", ", hovayvipham.ToArray());
                string mess = string.Format("Hộ dân {0} đã vay với số tiền không nằm trong khung quy định từ {1} đến {2}", ds, string.Format("{0:n0}", min), string.Format("{0:n0}", max));
                XtraMessageBox.Show(mess, Config.GetValue("PackageName").ToString());
                info.Result = false;
                return;
            }
        }

        private void CapNhatSoTong()
        {
            int TrongTrot = 0;
            int VatNuoi = 0;
            int CayTrong = 0;
            int Khac = 0;
            int DichVu = 0;
            int ThuySan = 0;
            int TieuThuCong = 0;

            DataView dvDT = new DataView(data.DsData.Tables[1]);
            dvDT.RowFilter = "MTID = '" + drMaster["MTID"].ToString() + "'";
            if (dvDT.Count == 0)
                return;
            // Tính lại phần tổng hợp
            for (int i = 0; i < dvDT.Count; i++)
            {
                if (dvDT[i].Row["NNChinh"].ToString() == "1. Chăn nuôi")
                    VatNuoi += 1;

                if (dvDT[i].Row["NNChinh"].ToString() == "2. Trồng trọt")
                    TrongTrot += 1;

                if (dvDT[i].Row["NNChinh"].ToString() == "3. Rau màu")
                    CayTrong += 1;

                if (dvDT[i].Row["NNChinh"].ToString() == "4. Thủy sản")
                    ThuySan += 1;

                if (dvDT[i].Row["NNChinh"].ToString() == "5. Tiểu thủ công")
                    TieuThuCong += 1;

                if (dvDT[i].Row["NNChinh"].ToString() == "6. Dịch vụ")
                    DichVu += 1;

                if (dvDT[i].Row["NNChinh"].ToString() == "7. Khác")
                    Khac += 1;
            }

            drMaster["VatNuoi"] = VatNuoi;
            drMaster["TrongTrot"] = TrongTrot;
            drMaster["CayTrong"] = CayTrong;
            drMaster["ThuySan"] = ThuySan;
            drMaster["TCNghiep"] = TieuThuCong;
            drMaster["DichVu"] = DichVu;
            drMaster["Khac"] = Khac;
            //tong so ho
            drMaster["TSoHo"] = dvDT.Count;
            dvDT.RowFilter += " and IsHoTro = 1";
            drMaster["TSoHDN"] = dvDT.Count;
        }

        public InfoCustomData Info
        {
            get {return info; }
        }

        #endregion
    }
}
