using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using CDTDatabase;
using CDTLib;
using Plugins;

using DevExpress;
using DevExpress.XtraEditors;
using DevExpress.Utils;

namespace CQNQD
{
    public class CQNQD:ICControl
    {
        DataCustomFormControl data;
        InfoCustomControl info = new InfoCustomControl(IDataType.MasterDetailDt);

        DataRow drMTNhanNo;

        public DataCustomFormControl Data
        {
            set { data = value; }
        }
        public InfoCustomControl Info
        {
            get { return info; }
        }
        public void AddEvent()
        {
            GridLookUpEdit glueSoQD = data.FrmMain.Controls.Find("SoQD", true)[0] as GridLookUpEdit;
            if (glueSoQD.EditValue.ToString() != "" || !string.IsNullOrEmpty(glueSoQD.EditValue.ToString()))
            {
                glueSoQD.Popup += new EventHandler(glueSoQD_Popup);
            }

          
        }

        void glueSoQD_Popup(object sender, EventArgs e)
        {
            GridLookUpEdit glueCQNQD = data.FrmMain.Controls.Find("CQQD",true)[0] as GridLookUpEdit;
            GridLookUpEdit glueSoQD = sender as GridLookUpEdit;
            int index = glueSoQD.Properties.View.FocusedRowHandle;
            DataRow row = glueSoQD.Properties.View.GetDataRow(index);
            Type type = row.GetType();
            string MaBP = type.GetProperty("MaBP").GetValue(row, null).ToString();
            //if(MaBP == "TP" || MaBP == "TW")
            //    glueCQNQD.Properties.View.se

        }
       



    }
}
