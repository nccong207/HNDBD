using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CDTLib;
using CDTDatabase;
namespace GetToTrinh
{
    public partial class FrmDSTT : DevExpress.XtraEditors.XtraForm
    {
        Database db = Database.NewDataDatabase();
        public DataTable dt = new DataTable();
        public string SoTT = "";
        public string MaBP, CQQD;
        public int THVay;

        public FrmDSTT()
        {
            InitializeComponent();
           
        }

        private void FrmDSTT_Load(object sender, EventArgs e)
        {
            // Tờ trình do Huyện lập
            string sql = string.Format(@"
                SELECT	CAST(0 as BIT) ColChon,t.* 
                FROM	MTTTrinh t 
                WHERE	isTrinhDuyet = 1 AND t.THVay = '{0}' AND BoPhan = '{2}'
		                AND t.MTTTID NOT IN ( SELECT MTTTID FROM DTQD WHERE MTTTID IS NOT NULL)
		                AND (t.NguonVon = '{1}' OR t.NguonVon LIKE 'TP%' OR t.NguonVon = 'TW' 
                                OR t.NguonVon IN (SELECT ID
								                    FROM	DMXaPhuong 
								                    WHERE	BoPhan = '{2}' ))"
                                        , THVay, Config.GetValue("MaCN").ToString(), MaBP);
            /* hvkhoi hiện tại không dùng theo cách này -- Đối với dự án có nguồn vốn là Trung ương
             * thì tờ trình sẽ do Tỉnh nhập. Quyết định này được Tỉnh lập, cơ quan nhận quyết định
             * là Huyện, Cơ quan ra quyết định là Trung ương
             */
            if (Config.GetValue("MaCN").ToString() == "TP" && CQQD == "TW")
            {
                sql = string.Format(@"
                SELECT CAST(0 AS BIT) ColChon, t.*
                FROM MTTTrinh t
                WHERE isTrinhDuyet = 1 AND THVay = '{0}' AND BoPhan = 'TP'
	                AND t.MTTTID  NOT IN (SELECT MTTTID FROM DTQD WHERE MTTTID IS NOT NULL)
	                AND (t.NguonVon = '{1}' OR ('{1}' = 'TP' AND t.NguonVon LIKE 'TP%'))
                ", THVay, CQQD);
            }
            
            // End hvkhoi
            gcTT.DataSource = db.GetDataTable(sql);
            //gcTT.DataSource = dt;
            gvTT.BestFitColumns();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string MTTTID = "";
            using (DataTable dt1 = gcTT.DataSource as DataTable)
            {
                using (DataView dv = new DataView(dt1))
                {
                    dv.RowFilter = "ColChon = 1";
                    
                    for (int i = 0; i < dv.Count; i++)
                    {
                        DataRow dr = dv[i].Row;
                        if ((bool)dr["Colchon"])
                        {
                            MTTTID += string.Format("'{0}',", dr["MTTTID"]);
                            SoTT += dr["SoTTrinh"].ToString()+ ";";
                        }
                    }
                    
                    if (dv.Count == 0)
                    {
                        XtraMessageBox.Show("Vui lòng chọn tờ trình", Config.GetValue("PackageName").ToString());
                        return;
                    }
                }
            }
            MTTTID = MTTTID.Length > 0 ? MTTTID.Substring(0, MTTTID.Length - 1) : "";
            if (!string.IsNullOrEmpty(MTTTID))
            {
                /* Hvkhoi -- Do một tờ trình có thể được Huyện lập hoặc Tỉnh lập
                 * Nếu Tỉnh lập thì nguồn vốn là TW. Trong đó có thể có nhiều dự án
                 * của nhiều Huyện khác nhau
                 * cho nên câu SQL chỉ lấy dự án có MaBP trùng với MaBP(CQNQD) trong form này
                 */
                string _sql = string.Format(@"
                    SELECT * 
                    FROM DTTTrinh tt INNER JOIN DMXaPhuong xp ON tt.PhuongXa = xp.ID
                    WHERE MTTTID IN ({0})
	                    AND xp.BoPhan = '{1}'", MTTTID, MaBP);
                dt = db.GetDataTable(_sql);
            }
            this.DialogResult = DialogResult.OK;
        }
    }
}