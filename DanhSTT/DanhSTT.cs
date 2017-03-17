using CDTDatabase;
using Plugins;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DanhSTT
{
    public class DanhSTT:ICData
    {
        private DataCustomData _data;
        private InfoCustomData _info = new InfoCustomData(IDataType.MasterDetailDt);
        public DataCustomData Data
        {
            set { _data = value; }
        }

        public void ExecuteAfter()
        {
            return;
        }

        public void ExecuteBefore()
        {
            DataRow drMaster = _data.DsData.Tables[0].Rows[_data.CurMasterIndex];
            if (drMaster.RowState == DataRowState.Deleted)
                return;

            var currentCQQD = drMaster["CQQD"].ToString();
            var currentNgayNN = drMaster["NgayNN"].ToString();
            //Lấy maximum STT trong DB

            //Lấy maximum STT trong DB
            string sqlQuery = "select year(b.NgayNN) as year, CASE WHEN b.CQQD LIKE '%[0-9]%' THEN LEFT(b.CQQD, LEN(b.CQQD)-2) ELSE b.CQQD END as CQQD, max(Stt) as max" +
                " FROM DTNhanNo a join MTNhanNo b on a.MTID = b.MTID where b.CQQD like  '%{0}%' and year(b.NgayNN) = year('{1}') group by year(b.NgayNN), " +
                "CASE WHEN b.CQQD LIKE '%[0-9]%' THEN LEFT(b.CQQD, LEN(b.CQQD)-2) ELSE b.CQQD END";

            int maxStt = Convert.ToInt32(_data.DbData.GetDataTable(string.Format(sqlQuery, currentCQQD, currentNgayNN)).Rows[0]["max"]);
            DataView dv = new DataView(_data.DsData.Tables[1]);
            dv.RowStateFilter = DataViewRowState.Added;

            foreach (DataRowView drv in dv)
            {
                maxStt++;
                drv["Stt"] = maxStt;
            }
        }

        public InfoCustomData Info
        {
            get { return _info; }
        }
    }
}
