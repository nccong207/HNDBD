using System;
using System.Collections.Generic;
using System.Text;
using DevExpress;
using CDTDatabase;
using CDTLib;
using Plugins;
using System.Data;
using DevExpress.XtraEditors;

namespace TaoSoCT
{
    public class TaoSoCT:ICData
    {
        //bang mtgiacong khong co trong binh duong
        List<string> lstTable = new List<string>(new string[] {"MT11","MT12","MT15","MT16",
                "MT21","MT22","MT23","MT24","MT25","MT31","MT32","MT33","MT41","MT42","MT43","MT44","MT45","MTGiaCong","MT51"});
       
        private InfoCustomData _info;
        private DataCustomData _data;
        Database db = Database.NewDataDatabase();
        Database dbCDT = Database.NewStructDatabase();

        #region ICData Members
 
        public TaoSoCT()
        {
            _info = new InfoCustomData(IDataType.MasterDetailDt);
        }

        public DataCustomData Data
        {
            set { _data = value; }
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }

        public void ExecuteAfter()
        {
            
        }

        public void ExecuteBefore()
        {
            if (!lstTable.Contains(_data.DrTableMaster["TableName"].ToString()))
                return;
            CreateCT();
        }

        private bool KTSuaNgay(DataRow drMaster)
        {
            DateTime dt1 = DateTime.Parse(drMaster["NgayCT", DataRowVersion.Current].ToString());
            DateTime dt2 = DateTime.Parse(drMaster["NgayCT", DataRowVersion.Original].ToString());
            return (dt1.Year != dt2.Year);
        }

        void CreateCT()
        {
            if (_data.CurMasterIndex < 0)
                return;
            DataRow drMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];

            if (!drMaster.Table.Columns.Contains("SoCT") || !drMaster.Table.Columns.Contains("NgayCT") || !drMaster.Table.Columns.Contains("MaCT"))
                return;
            if (drMaster.RowState == DataRowState.Added
                || (drMaster.RowState == DataRowState.Modified && KTSuaNgay(drMaster)))
            {
                if (_data.DrTable["MaCT"].ToString() == "")
                    return;

                string pk = _data.DrTableMaster["Pk"].ToString();
                DataRow[] drDetail = _data.DsData.Tables[1].Select(string.Format("{0} = '{1}'", pk, drMaster[pk]));

                string sql = "", soctNew = "", mact = "", maCN = "", prefix = "", suffix = "", Thang = "", Nam = "";
                mact = _data.DrTable["MaCT"].ToString();
                DateTime NgayCT = (DateTime)drMaster["NgayCT"];
                // Tháng: 2 chữ số
                // Năm: 2 số cuối của năm
                Thang = NgayCT.Month.ToString("D2");
                Nam = NgayCT.Year.ToString();

                Nam = Nam.Substring(2, 2);

                if (Config.GetValue("MaCN") != null)
                    maCN = Config.GetValue("MaCN").ToString();
                // 3. Số phiếu thu tự nhảy. Qua mỗi năm số phiếu nhảy lại 001
                // vd: PT001/13
                // Số CT = [Mã CT] + [Số thứ tự] + "/" + [Năm] + [MaCN]
                prefix = mact;

                //suffix = "/" + Nam + maCN;

                string maVV = GetMaVV(drDetail);

                if (!string.IsNullOrEmpty(maVV) && maVV.Equals("ERROR"))
                {
                    XtraMessageBox.Show("Chọn nhiều xã phường trong cùng 1 chứng từ sẽ không xác định được số chứng từ, vui lòng kiểm tra lại!", Config.GetValue("PackageName").ToString());
                    _info.Result = false;
                    return;
                } else if (!string.IsNullOrEmpty(maVV))
                {
                    suffix = "/" + Nam + maVV;
                } else
                {
                    suffix = "/" + Nam + maCN;
                }

                sql = string.Format(@"  SELECT	TOP 1 CAST(REPLACE(REPLACE(SoCT,'{1}',''),'{2}','') AS INT) [SoCT]
                                        FROM	{0}
                                        WHERE	SoCT LIKE '{1}%{2}' 
		                                        AND ISNUMERIC(REPLACE(REPLACE(SoCT,'{1}',''),'{2}','')) = 1
                                        ORDER BY CAST(REPLACE(REPLACE(SoCT,'{1}',''),'{2}','') AS INT) DESC"
                                    , _data.DrTableMaster["TableName"].ToString(), prefix, suffix);

                using (DataTable dt = db.GetDataTable(sql))
                {
                    if (dt.Rows.Count > 0)
                    {
                        int i = (int)dt.Rows[0]["SoCT"] + 1;
                        soctNew = prefix + i.ToString("D3") + suffix;
                    }
                    else
                    {
                        soctNew = prefix + "001" + suffix;
                    }
                }
                if (soctNew != "")
                    drMaster["SoCT"] = soctNew;
            }
        }

        private string GetMaVV(DataRow[] drDetail)
        {
            if (drDetail.Length == 0) return string.Empty;

            try
            {
                string mavv = drDetail[0]["MaVV"].ToString();
                foreach (DataRow item in drDetail)
                {
                    if (!item["MaVV"].ToString().Equals(mavv)) return "ERROR";
                }

                return mavv;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private string GetNewValue(string OldValue)
        {
            try
            {
                int i = OldValue.Length - 1;
                for (; i > 0; i--)
                    if (!Char.IsNumber(OldValue, i))
                        break;
                if (i == OldValue.Length - 1)
                {
                    int NewValue = Int32.Parse(OldValue) + 1;
                    return NewValue.ToString();
                }
                string PreValue = OldValue.Substring(0, i + 1);
                string SufValue = OldValue.Substring(i + 1);
                int intNewSuff = Int32.Parse(SufValue) + 1;
                string NewSuff = intNewSuff.ToString().PadLeft(SufValue.Length, '0');
                return (PreValue + NewSuff);
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion
    }
}
