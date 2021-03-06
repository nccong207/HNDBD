using System;
using System.Collections.Generic;
using System.Text;
using Plugins;
using System.Data;

namespace TMBCTC
{
    public class TMBCTC : IC
    {
        private List<InfoCustom> _lstInfo = new List<InfoCustom>();

        public void Execute(DataRow drMenu)
        {
            int num1 = int.Parse(drMenu["MenuPluginID"].ToString());
            int num2 = int.Parse(drMenu["sysMenuID"].ToString());
            if (this._lstInfo[0].CType != ICType.Custom || this._lstInfo[0].MenuID != num1)
                return;

            FrmFilter frm = new FrmFilter();
            frm.isQTTC = (num2 != 9609);
            frm.ShowDialog();
        }

        public List<InfoCustom> LstInfo
        {
            get
            {
                return this._lstInfo;
            }
        }

        public TMBCTC()
        {
            this._lstInfo.Add(new InfoCustom(1006, "Thuyết minh báo cáo tài chính", "Tổng hợp"));
        }
    }
}
