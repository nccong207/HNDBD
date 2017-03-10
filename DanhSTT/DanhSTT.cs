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
            if (drMaster.RowState != DataRowState.Added)
                return;

            var currentMTID = drMaster["MTID"].ToString();
            //Lấy maximum STT trong DB
            int maxStt = Convert.ToInt32(_data.DbData.GetDataTable("select MAX(Stt) as max from DTNhanNo").Rows[0]["max"]);
            DataView dv = new DataView(_data.DsData.Tables[1]);
            dv.RowStateFilter = DataViewRowState.Added | DataViewRowState.Deleted | DataViewRowState.ModifiedCurrent;

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
